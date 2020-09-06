#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityMovementsData : EntityComponentData
	{
        public enum MovementsMethod
        {
            Classic_3D,
            TopDown_2D,
            SideView_2D,
            Custom
        }

        #region Vars, Fields, Getters
        [Header("Main")]
        [Space]
        [SerializeField] private MovementsMethod _movementsType;

        [Header("Base Movements Parameters")]
        [Space]
        [SerializeField] private bool _brakeWhileMoving;
        [SerializeField] private float _baseAcceleration;
        [SerializeField] private float _baseMaxSpeed;
        [SerializeField] private float _baseBrake;

        [Header("Sprint Parameters")]
        [Space]
        [SerializeField] private bool _hasSprint;
        [ShowIf("_hasSprint"), Indent(1)]
        [SerializeField] private float _sprintSpeedFactor = 2f;
        [ShowIf("_hasSprint"), Indent(1)]
        [SerializeField] private float _sprintStaminaConsumption = 1f;

        [Header("Dash Parameters")]
        [Space]
        [SerializeField] private bool _hasDash;
        [ShowIf("_hasDash"), Indent(1)]
        [SerializeField] private VelocityOverrideData _dashVelocityOverride;
        [Space]
        [ShowIf("_hasDash"), Indent(1)]
        [SerializeField] private float _dashCooldown = 5f;
        [ShowIf("_hasDash"), Indent(1)]
        [SerializeField] private int _dashCharges = 2;
        [Space]
        [ShowIf("_hasDash"), Indent(1)]
        [SerializeField] private float _dashStaminaConsumption = 5f;

        [Header("Misc. Parameters")]
        [Space]
        [SerializeField] private float _baseRecoilFactor = 1f;
        [SerializeField] private float _baseKnockbackTakenFactor = 1f;
        [SerializeField] private bool _computeAverageVelocity = false;

        public MovementsMethod MovementsType => _movementsType;

        public bool BrakeWhileMoving => _brakeWhileMoving;
        public float BaseAcceleration => _baseAcceleration;
        public float BaseMaxSpeed => _baseMaxSpeed;
        public float BaseBrake => _baseBrake;

		public bool HasSprint => _hasSprint;
        public float SprintSpeedFactor => _sprintSpeedFactor;
        public float SprintStaminaConsumption => _sprintStaminaConsumption;

        public bool HasDash => _hasDash;
        public VelocityOverrideData DashVelocityOverride => _dashVelocityOverride;
        public float DashCooldown => _dashCooldown;
        public int DashCharges => _dashCharges;
        public float DashStaminaConsumption => _dashStaminaConsumption;

        public float BaseRecoilFactor => _baseRecoilFactor;
        public float BaseKnockbackTakenFactor => _baseKnockbackTakenFactor;
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
                moveAgent = new EntityMovementsAgent(this);
            }

            private EntityMovementsData _data = null;
            public new EntityMovementsData Data => _data;
			#endregion

			#region Vars, Getters
            public override MovementState MovementState
            {
                get
                {
                    if (voAgent != null) { return MovementState.Overriden; }

                    if (HasController == false)
                    {
                        return MovementState.None;
                    }

                    if (Controller.InputingMovement == true)
                    {
                        return MovementState.Moving;
                    }
                    else
                    {
                        return MovementState.Idle;
                    }
                }
            }
            public override MovementDirection MovementDirection => MovementDirection.None;

            private float _lastDashTime = -1 / 0f;

            private bool _shouldUpdateAverageVelocityThisFrame = false;
            private int _velocityAverageCount;
            private List<Vector3> _oldVelocities = new List<Vector3>();
            private Vector3 _averageVelocity;

            public Vector3 AverageVelocity => _data._computeAverageVelocity ? _averageVelocity : References.RigidBody.velocity;

            protected EntityMovementsAgent moveAgent = null;
            public EntityMovementsAgent MoveAgent => moveAgent;

            protected VelocityOverrideAgent voAgent = null;
            public VelocityOverrideAgent VoAgent { get => voAgent; }

            public float Acceleration => HasStats ? Stats.EvaluateMovementsStat(EntityMovementsStat.Acceleration, _data._baseAcceleration) : _data._baseAcceleration;
            public float MaxSpeed => HasStats ? Stats.EvaluateMovementsStat(EntityMovementsStat.MaxSpeed, _data._baseMaxSpeed) : _data._baseMaxSpeed;
            public float BrakeForce => HasStats ? Stats.EvaluateMovementsStat(EntityMovementsStat.BrakeForce, _data._baseBrake) : _data._baseBrake;
            public float RecoilFactor => HasStats ? Stats.EvaluateMovementsStat(EntityMovementsStat.RecoilFactor, _data._baseRecoilFactor) : _data._baseRecoilFactor;
            public float KnockbackTakenFactor => HasStats ? Stats.EvaluateMovementsStat(EntityMovementsStat.KnockbackTakenFactor, _data._baseKnockbackTakenFactor) : _data._baseKnockbackTakenFactor;
            public int DashMaxCharges => HasStats ? Mathf.FloorToInt(Stats.EvaluateMovementsStat(EntityMovementsStat.DashCharges, _data._dashCharges)) : _data._dashCharges;
            public float LastDashTime => _lastDashTime;
            public float LastDashMaxDuration => DashMaxCharges * DashCooldown;
            public float LastDashClampedDuration => Mathf.Clamp(Time.time - _lastDashTime, 0f, LastDashMaxDuration);
            public int CurrentDashCharges => Mathf.FloorToInt(LastDashClampedDuration / DashCooldown);
            public float DashCooldown => HasStats ? Mathf.FloorToInt(Stats.EvaluateMovementsStat(EntityMovementsStat.DashCooldown, _data._dashCooldown)) : _data._dashCooldown;
			#endregion

			#region Behaviour
            /// <summary>
            /// Need base.
            /// </summary>
			public override void SlaveAwake()
			{
                _velocityAverageCount = Mathf.FloorToInt(1 / Time.fixedDeltaTime);
			}
			/// <summary>
			/// Need base
			/// </summary>
			public override void SlaveFixedUpdate()
			{
                _shouldUpdateAverageVelocityThisFrame = _data._computeAverageVelocity ? true : false;
                moveAgent.FixedUpdate.Invoke();
			}


            /// <summary>
            /// Need base.
            /// </summary>
			public override void SlaveUpdate()
			{
                HandleCheckDash();
                if (_shouldUpdateAverageVelocityThisFrame == true)
				{
                    HandleCalculateAverageVelocity();
				}
			}
            private void HandleCheckDash()
			{
                if (_data._hasDash == false) { return; }
                if (CurrentDashCharges <= 0) { return; }

                if (references.InputReceiver.Game_Dash)
				{
                    PerformDash();
				}
			}
            protected void HandleCalculateAverageVelocity()
            {
                _shouldUpdateAverageVelocityThisFrame = false;
                Vector3 currentVelocity = References.RigidBody.velocity;

                if (_oldVelocities.Count < _velocityAverageCount)
                {
                    _oldVelocities.Add(currentVelocity);
                    _averageVelocity = References.RigidBody.velocity;
                }
                else if (_oldVelocities.Count == _velocityAverageCount - 1)
                {
                    _oldVelocities.Add(currentVelocity);

                    _averageVelocity = Vector3.zero;
                    for (int i = 0; i < _velocityAverageCount; i++)
					{
                        _averageVelocity += _oldVelocities[i];
					}

                    _averageVelocity /= _velocityAverageCount;
                }
                else
				{
                    _oldVelocities.Add(currentVelocity);
                    _averageVelocity += currentVelocity / _velocityAverageCount;
                    _averageVelocity -= _oldVelocities[0] / _velocityAverageCount;
                    _oldVelocities.RemoveAt(0);
                }
            }

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
            #endregion

            #region Utilities
            private void PerformDash()
            {
                Vector3 forward = Controller.InputingMovement ? Controller.MoveVector.normalized : (PinouApp.Entity.Mode2D == false ? Forward : Forward2D.ToV3());
                voAgent = _data._dashVelocityOverride.StartVelictyOverride(references.RigidBody, forward);
                if (HasStats == true)
				{
                    voAgent.SetStatsSpeedFactor(Stats.EvaluateMovementsStat(EntityMovementsStat.Acceleration, 1f));
                }
                voAgent.OnAgentStop.Subscribe(OnVelocityOverrideStops);

                _lastDashTime = Time.time - LastDashClampedDuration + DashCooldown;
                OnDash.Invoke(master, forward);
            }

            protected Vector3 DetermineAbilityRecoil(AbilityCastData castData)
            {
                return PinouUtils.Transform.TransformVector(castData.BaseRecoil, PinouUtils.Quaternion.SafeLookRotation(castData.CastDirection, -Forward)) * RecoilFactor;
            }
            protected Vector3 DetermineAbilityHitKnockback(AbilityCastResult result)
            {
                switch (result.CastData.AbilityCast.Methods.KnockbackMethod)
                {
                    case KnockbackMethod.AttackBased:
                        return PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(result.CastData.CastDirection)) * KnockbackTakenFactor;

                    case KnockbackMethod.AttackEntityAverage:
                        return
                            (PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(result.CastData.CastDirection)) +
                            references.VisualBody.TransformVector(result.CastData.BaseKnockback)) * 0.5f * KnockbackTakenFactor;

                    case KnockbackMethod.VictimEntityBased:
                        return references.VisualBody.TransformVector(result.CastData.BaseKnockback) * KnockbackTakenFactor;

                    case KnockbackMethod.AttackImpactToEntity:
                        return PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, PinouUtils.Quaternion.SafeLookRotation(Position - result.Impact, result.CastData.CastDirection)) * KnockbackTakenFactor;
                    case KnockbackMethod.CasterToVictim:
                        return PinouUtils.Transform.TransformVector(result.CastData.BaseKnockback, Quaternion.LookRotation(result.Victim.Position - result.CastData.Caster.Position)) * KnockbackTakenFactor;
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
                if (agent == voAgent)
				{
                    voAgent = null;
                }
            }
            public PinouUtils.Delegate.Action<Entity, Vector3> OnDash { get; private set; } = new PinouUtils.Delegate.Action<Entity, Vector3>();
            #endregion
        }
    }
}