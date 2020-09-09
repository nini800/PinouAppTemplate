#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public class EntityVisualNetData : EntityVisualData
    {
        #region Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityVisualNet, EntityVisualNetData>(master, references, this);
        }
        #endregion

        public class EntityVisualNet : EntityVisual, INetworkEntityData
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityVisualNetData)base.Data;
            }

            private EntityVisualNetData _data = null;
            public new EntityVisualNetData Data => _data;
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

			#region Behaviour
			protected override void HandleRotateBody(RotationMethod method)
            {
                if (References.NetworkIdentity.hasAuthority == false)
				{
                    return;
				}
                else
				{
                    base.HandleRotateBody(method);
				}
			}
			#endregion

			#region Utilities
			#endregion

			#region Events
			#endregion
		}
	}
}