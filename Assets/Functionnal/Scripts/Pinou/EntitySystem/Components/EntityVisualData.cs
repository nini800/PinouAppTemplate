#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityVisualData : EntityComponentData
	{
        #region Fields, Getters
        [Header("Parameters")]
        [Space]
        [SerializeField] protected float rotationSpeed = 600f;
        [Header("FXs")]
        [Space]
        [SerializeField] private AbilityVisualData.AbilityVisualFX _deathFX;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityVisual, EntityVisualData>(master, references, this);
        }
        #endregion

        public class EntityVisual : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityVisualData)((EntityComponent)this).Data;
            }

            private EntityVisualData _data = null;
            public new EntityVisualData Data => _data;
            #endregion

            /// <summary>
            /// Need base.
            /// </summary>
            public override void SlaveUpdate()
            {
                HandleVisualStickToBody();
            }
            private void HandleVisualStickToBody()
            {
                references.VisualBody.position = Position;
            }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveEnabled()
            {
                master.OnAbilityHitResult.Subscribe(OnAbilityHitResult);
                if (HasBeing)
                {
                    Being.OnDeath.Subscribe(OnDeath);
                }
                if (HasAbilities)
                {
                    Abilities.OnPerformAbility.Subscribe(OnPerformAbility);
                }
            }
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveDisabled()
            {
                master.OnAbilityHitResult.Unsubscribe(OnAbilityHitResult);
                if (HasBeing)
                {
                    Being.OnDeath.Unsubscribe(OnDeath);
                }
                if (HasAbilities)
                {
                    Abilities.OnPerformAbility.Unsubscribe(OnPerformAbility);
                }
            }


            #region Utilities
            protected virtual void RotateTowardsCastDirection(AbilityCastData castData)
            {

            }
            #endregion
            
            #region Events
            private void OnPerformAbility(Entity ent, AbilityCastData castData)
            {
                AbilityPerformer.HandleAbilityCastDataVisual(castData);
                if (castData.AbilityCast.Main.RotateOnCast == true)
                {
                    RotateTowardsCastDirection(castData);
                }
            }
            private void OnAbilityHitResult(Entity ent, AbilityCastResult result)
            {
                AbilityPerformer.HandleAbilityCastResultVisual(result);
            }
            private void OnDeath(Entity ent, AbilityCastResult killingResult)
            {
                AbilityPerformer.HandleAbilityCastResultFX(killingResult, Data._deathFX);
            }
            #endregion
        }
    }
}