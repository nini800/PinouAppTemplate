using System.Collections.Generic;
using Pinou.EntitySystem;
using Pinou.ItemSystem;
using Mirror;

namespace Pinou.Networking
{
    public class EntityEquipmentNetData : EntityEquipmentData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityEquipmentNet, EntityEquipmentNetData>(master, references, this);
        }
        #endregion

        public class EntityEquipmentNet : EntityEquipment, INetworkEntityData
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityEquipmentNetData)base.Data;
            }

            private EntityEquipmentNetData _data = null;
            public new EntityEquipmentNetData Data => _data;
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

            private List<bool> _equipableSyncMethod = new List<bool>();
            private List<EntityEquipable> _equipablesToSync = new List<EntityEquipable>();


            public override void SlaveAwake()
			{
                PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(master.gameObject, SyncableVariable.EntityEquip, SyncFrequency.Instant, RegisterEntityEquip, SyncEntityEquip, GetSizeEntityEquip, true);
                SubscribeToEquipEvents();
            }
            private void SubscribeToEquipEvents()
			{
                OnEquip.SafeSubscribe(OnEquipNet);
                OnUnequip.SafeSubscribe(OnUnequipNet);
            }
            private void UnsubscribeToEquipEvents()
			{
                OnEquip.Unsubscribe(OnEquipNet);
                OnUnequip.Unsubscribe(OnUnequipNet);
            }

            private void RegisterEntityEquip(SyncableVariable var, NetworkWriter writer)
			{
                writer.WriteInt32(_equipableSyncMethod.Count);
                for (int i = 0; i < _equipablesToSync.Count; i++)
                {
                    writer.WriteBoolean(_equipableSyncMethod[i]);
                    writer.WriteEntityEquipable(_equipablesToSync[i]);
                }

                _equipableSyncMethod.Clear();
                _equipablesToSync.Clear();
            }
            private void SyncEntityEquip(SyncableVariable var, NetworkReader reader)
            {
                int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
                    bool method = reader.ReadBoolean();
                    EntityEquipable eq = reader.ReadEntityEquipable();

                    UnsubscribeToEquipEvents();
                    if (method)
                    {
                        Equip(eq);
                    }
                    else
					{
                        Unequip(eq);
                    }
                    SubscribeToEquipEvents();

                    if (References.NetworkIdentity.isServer == true && References.NetworkIdentity.hasAuthority == false)
					{
                        _equipableSyncMethod.Add(method);
                        _equipablesToSync.Add(eq);
                        PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityEquip);
                    }
                }
            }
            private int GetSizeEntityEquip(NetworkWriter dummyWriter)
            {
                dummyWriter.WriteInt32(_equipableSyncMethod.Count);
				for (int i = 0; i < _equipablesToSync.Count; i++)
				{
                    dummyWriter.WriteBoolean(_equipableSyncMethod[i]);
                    dummyWriter.WriteEntityEquipable(_equipablesToSync[i]);
                }

                return dummyWriter.Length;
            }

            private void OnEquipNet(Entity master, EntityEquipable eq)
            {
                _equipableSyncMethod.Add(true);
                _equipablesToSync.Add(eq);

                PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityEquip);
            }
            private void OnUnequipNet(Entity master, EntityEquipable eq)
            {
                _equipableSyncMethod.Add(false);
                _equipablesToSync.Add(eq);

                PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityEquip);
            }
        }
    }
}