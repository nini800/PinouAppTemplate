#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using Pinou.EntitySystem;
using Mirror;
using System.Collections.Generic;
using System;

namespace Pinou.Networking
{
    public class EntityAbilitiesNetData : EntityAbilitiesData
    {
        #region Vars, Fields, Getters
        #endregion

        #region Utilities
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityAbilitiesNet, EntityAbilitiesNetData>(master, references, this);
        }
        #endregion

        #region Editor
        #endregion

        public class EntityAbilitiesNet : EntityAbilities, INetworkEntityData
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityAbilitiesNetData)base.Data;
            }

            private EntityAbilitiesNetData _data = null;
            protected new EntityAbilitiesNetData Data => _data;
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
            private List<AbilityCastData> _nextAbilitiesCast = new List<AbilityCastData>();
            private List<AbilityCastData> _nextHitboxDestroyed = new List<AbilityCastData>();
            #endregion

            #region Behaviour
            public override void SlaveStart()
			{
				base.SlaveStart();
                PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(master.gameObject, SyncableVariable.EntityAbilityCast, SyncFrequency.Instant, RegisterCastAbility, SyncCastAbility, GetSizeCastAbility, false);
                PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(master.gameObject, SyncableVariable.EntityAbilityHitboxDestroyed, SyncFrequency.Instant, RegisterDestroyAbilityHitbox, SyncDestroyAbilityHitbox, GetSizeDestroyAbilityHitbox, false);
			}



			private void RegisterCastAbility(SyncableVariable var, NetworkWriter writer)
            {
                writer.WriteInt32(_nextAbilitiesCast.Count);
                for (int i = 0; i < _nextAbilitiesCast.Count; i++)
                {
                    writer.WriteAbilityCastData(_nextAbilitiesCast[i]);
                }

                _nextAbilitiesCast.Clear();
            }
            private void SyncCastAbility(SyncableVariable var, NetworkReader reader)
			{
                int hitboxCount = reader.ReadInt32();
                for (int i = 0; i < hitboxCount; i++)
                {
                    AbilityCastData castData = reader.ReadAbilityCastData();
                    if (castData.AbilityCast.Hitbox.UnlimitedLifeSpan == true || castData.AbilityCast.Hitbox.LifeSpan > 0)
                    {
                        AbilityHitbox hitBox = AbilityPerformer.HandleSpawnAbilityHitbox(castData);
                        hitBox.ActivateVisualMode();
                    }

                    OnPerformAbility.Invoke(master, castData);
                    if (References.NetworkIdentity.isServer && References.NetworkIdentity.hasAuthority == false)
					{
                        _nextAbilitiesCast.Add(castData);
                        PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityAbilityCast);
                    }
                }
            }
            private int GetSizeCastAbility(NetworkWriter dummyWriter)
            {
                dummyWriter.WriteInt32(_nextAbilitiesCast.Count);
                for (int i = 0; i < _nextAbilitiesCast.Count; i++)
                {
                    dummyWriter.WriteAbilityCastData(_nextAbilitiesCast[i]);
                }
                return dummyWriter.Length;
            }

            private void RegisterDestroyAbilityHitbox(SyncableVariable var, NetworkWriter writer)
			{
                writer.WriteInt32(_nextHitboxDestroyed.Count);
				for (int i = 0; i < _nextHitboxDestroyed.Count; i++)
				{
                    writer.WriteAbilityCastData(_nextHitboxDestroyed[i]);
				}

                _nextHitboxDestroyed.Clear();
			}
            private void SyncDestroyAbilityHitbox(SyncableVariable var, NetworkReader reader)
            {
                int hitboxCount = reader.ReadInt32();
				for (int i = 0; i < hitboxCount; i++)
				{
                    AbilityCastData castData = reader.ReadAbilityCastData();
                    AbilityHitbox.DestroyFromCastData(castData);
                    if (References.NetworkIdentity.isServer && References.NetworkIdentity.hasAuthority == false)
                    {
                        _nextHitboxDestroyed.Add(castData);
				        PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityAbilityHitboxDestroyed);
                    }
                }
            }
            private int GetSizeDestroyAbilityHitbox(NetworkWriter dummyWriter)
            {
                dummyWriter.WriteInt32(_nextHitboxDestroyed.Count);
				for (int i = 0; i < _nextHitboxDestroyed.Count; i++)
				{
                    dummyWriter.WriteAbilityCastData(_nextHitboxDestroyed[i]);
                }
                return dummyWriter.Length;
            }

            public override void SlaveEnabled()
			{
                base.OnDestroyAbilityHitbox.SafeSubscribe(OnDestroyAbilityHitbox);
            }
			public override void SlaveDisabled()
			{
                base.OnDestroyAbilityHitbox.Unsubscribe(OnDestroyAbilityHitbox);
            }
			#endregion

			#region Utilities
			protected override void PerformAbility(AbilityCastData castData)
			{
				base.PerformAbility(castData);
                if (References.NetworkIdentity.hasAuthority == true)
                {
                    _nextAbilitiesCast.Add(castData);
                    PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityAbilityCast);
                }
            }
            #endregion

            #region Events
            private new void OnDestroyAbilityHitbox(Entity ent, AbilityHitbox hitbox)
            {
                if (References.NetworkIdentity.hasAuthority == true)
                {
                    _nextHitboxDestroyed.Add(hitbox.CastData);
				    PinouNetworkManager.MainBehaviour.SetDirty(master.gameObject, SyncableVariable.EntityAbilityHitboxDestroyed);
                }
            }
            #endregion
        }
    }
}