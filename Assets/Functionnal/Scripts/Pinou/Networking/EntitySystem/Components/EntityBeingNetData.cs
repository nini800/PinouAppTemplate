#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;
using Mirror;

namespace Pinou.Networking
{
	public partial class EntityBeingNetData : EntityBeingData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityBeingNet, EntityBeingNetData>(master, references, this);
        }
        #endregion

        public partial class EntityBeingNet : EntityBeing, INetworkEntityData
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityBeingNetData)base.Data;
            }

            private EntityBeingNetData _data = null;
            protected new EntityBeingNetData Data => _data;
            #endregion
            #region INetworkEntityData
            public new EntityNet Master => master as EntityNet;
            public new EntityReferencesNet References => base.References as EntityReferencesNet;
            public new IControllerNet Controller => base.Controller as IControllerNet;
            public new EntityStatsNetData.EntityStatsNet Stats => base.Stats as EntityStatsNetData.EntityStatsNet;
            public new EntityEquipmentNetData.EntityEquipmentNet Equipment => base.Equipment as EntityEquipmentNetData.EntityEquipmentNet;
            public new EntityBeingNetData.EntityBeingNet Being => base.Being as EntityBeingNetData.EntityBeingNet;
            public new EntityAbilitiesNetData.EntityAbilitiesNet Abilities => base.Abilities as EntityAbilitiesNetData.EntityAbilitiesNet;
            public new EntityInteractionsNetData.EntityInteractionsNet Interactions => base.Interactions as EntityInteractionsNetData.EntityInteractionsNet;
            public new EntityMovementsNetData.EntityMovementsNet Movements => base.Movements as EntityMovementsNetData.EntityMovementsNet;
            public new EntityAnimationsNetData.EntityAnimationsNet Animations => base.Animations as EntityAnimationsNetData.EntityAnimationsNet;
            public new EntityVisualNetData.EntityVisualNet Visual => base.Visual as EntityVisualNetData.EntityVisualNet;
            public new EntityLootNetData.EntityLootNet Loot => base.Loot as EntityLootNetData.EntityLootNet;
            #endregion

            #region Vars, Getters
            protected int[] oldResourcesEnumIndices;
            protected float[] oldCurrentResources;
            #endregion

            #region Behaviour
            public override void SlaveAwake()
			{
				base.SlaveAwake();

                System.Array enumResourcesValues = System.Enum.GetValues(typeof(EntityBeingResourceType));
                int resourcesCount = enumResourcesValues.Length;
                oldResourcesEnumIndices = new int[resourcesCount];
                oldCurrentResources = new float[resourcesCount];
                for (int i = 0; i < resourcesCount; i++)
                {
                    oldResourcesEnumIndices[i] = (int)enumResourcesValues.GetValue(i);
                }
            }

            /// <summary>
            /// Need base
            /// </summary>
			public override void SlaveStart()
			{
                PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(master.gameObject, SyncableVariable.EntityHealth, SyncFrequency.Short, RegisterHealth, SyncHealth);
                PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(master.gameObject, SyncableVariable.EntityDeath, SyncFrequency.Instant, RegisterDeath, SyncDeath, GetSizeDeath, true);
            }

            private void RegisterHealth(SyncableVariable var, NetworkWriter writer)
            {
                writer.WriteDouble(currentHealth);
            }
            private void SyncHealth(SyncableVariable var, NetworkReader reader)
            {
                currentHealth = (float)reader.ReadDouble();
            }

            private void RegisterDeath(SyncableVariable var, NetworkWriter writer)
            {
                writer.WriteAbilityCastResult(deathResult);
                deathResult = null;
            }
            private void SyncDeath(SyncableVariable var, NetworkReader reader)
            {
                OnDeath.Invoke(master, reader.ReadAbilityCastResult());
            }
            private int GetSizeDeath(NetworkWriter dummyWriter)
			{
                dummyWriter.WriteAbilityCastResult(deathResult);
                return dummyWriter.Length;
            }

            public override void SlaveLateUpdate()
			{
                if (References.NetworkIdentity.isServer == false) { return; }

				base.SlaveLateUpdate();
                HandleStoreCurrentResources();
			}
            public void HandleStoreCurrentResources()
            {
                for (int i = 0; i < oldCurrentResources.Length; i++)
                {
                    float curResource = GetCurrentResource((EntityBeingResourceType)oldResourcesEnumIndices[i]);
                    if (oldCurrentResources[i] != curResource)
                    {
                        oldCurrentResources[i] = curResource;
                        if ((EntityBeingResourceType)oldResourcesEnumIndices[i] == EntityBeingResourceType.Health)
						{
                            PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityHealth);
                        }
                    }
                }
            }
            #endregion

            #region Utilities
            protected override void HandleDeath()
			{
                if (References.NetworkIdentity.isServer == false) { return; }

                if (deathResult != null)
                {
                    OnDeath.Invoke(master, deathResult);
                    PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityDeath);
                }
            }
			#endregion
		}
    }
}