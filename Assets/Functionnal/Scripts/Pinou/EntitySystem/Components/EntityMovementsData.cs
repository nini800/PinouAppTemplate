#pragma warning disable 0649
using UnityEngine;


namespace Pinou.EntitySystem
{
	public class EntityMovementsData : EntityComponentData
	{
        #region Vars, Fields, Getters
        [SerializeField] private bool _brakeWhileMoving;
        [SerializeField] private float _baseAcceleration;
        [SerializeField] private float _baseMaxSpeed;
        [SerializeField] private float _baseBrake;

        public bool BrakeWhileMoving => _brakeWhileMoving;
        public float BaseAcceleration => _baseAcceleration;
        public float BaseMaxSpeed => _baseMaxSpeed;
        public float BaseBrake => _baseBrake;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityMovements, EntityMovementsData>(master, references, this);
        }
        #endregion

        public class EntityMovements : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityMovementsData)((EntityComponent)this).Data;
            }

            private EntityMovementsData _data = null;
            public new EntityMovementsData Data => _data;
            #endregion

            public override MovementState MovementState => voAgent == null ? MovementState.None : MovementState.Overriden;
            public override MovementDirection MovementDirection => MovementDirection.None;

            protected VelocityOverrideAgent voAgent = null;
            public VelocityOverrideAgent VoAgent { get => voAgent; }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveEnabled()
            {
                master.OnReceiveAbilityHit.Subscribe(OnReceiveAbilityHit);
                if (HasAbilities)
                {
                    Abilities.OnAbilityPerformed.Subscribe(OnAbilityPerformed);
                }
            }
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveDisabled()
            {
                master.OnReceiveAbilityHit.Unsubscribe(OnReceiveAbilityHit);
                if (HasAbilities)
                {
                    Abilities.OnAbilityPerformed.Unsubscribe(OnAbilityPerformed);
                }
            }
            #region Utilities
            protected Vector3 DetermineAbilityRecoil(AbilityCastData castData)
            {
                return PinouUtils.Transform.TransformVector(castData.BaseRecoil, Quaternion.LookRotation(castData.CastDirection));
            }
            protected Vector3 DetermineAbilityHitKnockback(AbilityCastResult result)
            {
                switch (result.CastData.AbilityCast.Methods.KnockbackMethod)
                {
                    case KnockbackMethod.AttackBased:
                        return PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(result.CastData.CastDirection));

                    case KnockbackMethod.AttackEntityAverage:
                        return
                            (PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(result.CastData.CastDirection)) +
                            references.VisualBody.TransformVector(result.CastData.BaseKnockback)) * 0.5f;

                    case KnockbackMethod.VictimEntityBased:
                        return references.VisualBody.TransformVector(result.CastData.BaseKnockback);

                    case KnockbackMethod.AttackImpactToEntity:
                        return PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(Position - result.Impact));
                }

                throw new System.Exception("KnockbackMethod enum updated, this should be too.");
            }
            protected virtual void StartVelocityOverrideFromAbilityCast(AbilityCastData castData)
            {
                voAgent = castData.AbilityCast.VelocityOverrideChain.StartChain(references.RigidBody, Is2D ? Forward2D.ToV3() : Forward);
                voAgent.OnAgentStop.Subscribe(OnVelocityOverrideStops);
            }
            #endregion

            #region Events
            protected virtual void OnAbilityPerformed(Entity ent, AbilityCastData castData)
            {
                if (castData.AbilityCast.Methods.RecoilCondition != RecoilCondition.MustHit || castData.Results.Length > 0)
                {
                    Vector3 recoil = DetermineAbilityRecoil(castData);
                    references.RigidBody.velocity += recoil;
                }

                if (castData.AbilityCast.HasVelocityOverrides == true)
                {
                    StartVelocityOverrideFromAbilityCast(castData);
                }
            }
            protected virtual void OnReceiveAbilityHit(Entity ent, AbilityCastResult result)
            {
                Vector3 knockback = DetermineAbilityHitKnockback(result);

                references.RigidBody.velocity += knockback;
                result.FillKnockbackApplied(knockback);
            }

            protected virtual void OnVelocityOverrideStops(VelocityOverrideAgent agent)
            {
                voAgent = null;
            }
            #endregion
        }
    }
}