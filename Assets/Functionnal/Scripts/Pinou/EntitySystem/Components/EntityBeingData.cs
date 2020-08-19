#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem	
{
	public partial class EntityBeingData : EntityComponentData
	{
        #region Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityBeing, EntityBeingData>(master, references, this);
        }
        #endregion

        public partial class EntityBeing : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityBeingData)((EntityComponent)this).Data;
            }

            private EntityBeingData _data = null;
            protected new EntityBeingData Data => _data;
            #endregion

            #region IEntityData
            public override BeingState BeingState
            {
                get
                {
                    if (currentHealth <= 0f)
                    {
                        return BeingState.Dead;
                    }
                    else
                    {
                        return BeingState.Alive;
                    }
                }
            }
            #endregion

            #region Vars, Getters
            protected AbilityCastResult deathResult = null;
            public AbilityCastResult DeathResult => deathResult;
            #endregion

            #region Behaviour
            /// <summary>
            /// Need base.
            /// </summary>
            public override void SlaveAwake()
            {
                HandleStartResources();
            }
            private void HandleStartResources()
            {
                System.Array values = System.Enum.GetValues(typeof(EntityBeingResourcesType));

                for (int i = 0; i < values.Length; i++)
				{
                    if (_data.GetResourceStartNotAtMax((EntityBeingResourcesType)values.GetValue(i)) == true)
					{
                        SetCurrentResource((EntityBeingResourcesType)values.GetValue(i), _data.GetResourceStartAmount((EntityBeingResourcesType)values.GetValue(i)));
					}
                    else
					{
                        SetCurrentResource((EntityBeingResourcesType)values.GetValue(i), _data.GetMaxResource((EntityBeingResourcesType)values.GetValue(i)));
                    }
                }
            }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveEnabled()
            {
                master.OnReceiveAbilityHit.Subscribe(OnReceiveAbilityHit);
            }
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveDisabled()
            {
                master.OnReceiveAbilityHit.Unsubscribe(OnReceiveAbilityHit);
            }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveLateUpdate()
            {
                HandleDeath();
            }
            protected virtual void HandleDeath()
            {
                if (deathResult != null)
                {
                    OnDeath.Invoke(master, deathResult);
                    deathResult = null;
                }
            }
            #endregion

            #region Utilities
            protected void TakeAbilityHit(AbilityCastResult result)
            {
                float oldHealth = currentHealth;
                ModifyHealth(result.CastData.GetResourceImpact(EntityBeingResourcesType.Health));

                result.FillResourceChange(EntityBeingResourcesType.Health, currentHealth - oldHealth);

                if (BeingState == BeingState.Dead)
                {
                    Death(result);
                }
            }
            protected void Death(AbilityCastResult killingResult)
            {
                deathResult = killingResult;
            }
            #endregion

            #region Events
            public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnDeath { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();
            private void OnReceiveAbilityHit(Entity master, AbilityCastResult result)
            {
                TakeAbilityHit(result);
            }
            #endregion
        }
    }
}