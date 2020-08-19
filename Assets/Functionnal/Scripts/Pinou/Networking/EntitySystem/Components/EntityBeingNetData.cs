#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public partial class EntityBeingNetData : EntityBeingData
    {
        #region Fields, Getters
        #endregion

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
            public new EntityControllerNetData.EntityControllerNet Controller => base.Controller as EntityControllerNetData.EntityControllerNet;
            public new EntityBeingNetData.EntityBeingNet Being => base.Being as EntityBeingNetData.EntityBeingNet;
            public new EntityAbilitiesNetData.EntityAbilitiesNet Abilities => base.Abilities as EntityAbilitiesNetData.EntityAbilitiesNet;
            public new EntityMovementsNetData.EntityMovementsNet Movements => base.Movements as EntityMovementsNetData.EntityMovementsNet;
            public new EntityVisualNetData.EntityVisualNet Visual => base.Visual as EntityVisualNetData.EntityVisualNet;
            #endregion

            #region Vars, Getters
            protected int[] oldResourcesEnumIndices;
            protected float[] oldCurrentResources;
            #endregion

            #region Behaviour
            public override void SlaveAwake()
			{
				base.SlaveAwake();

                System.Array enumResourcesValues = System.Enum.GetValues(typeof(EntityBeingResourcesType));
                int resourcesCount = enumResourcesValues.Length;
                oldResourcesEnumIndices = new int[resourcesCount];
                oldCurrentResources = new float[resourcesCount];
                for (int i = 0; i < resourcesCount; i++)
                {
                    oldResourcesEnumIndices[i] = (int)enumResourcesValues.GetValue(i);
                }
            }

			public override void SlaveLateUpdate()
			{
                if (References.NetBehaviour.isServer == false) { return; }

				base.SlaveLateUpdate();
                HandleStoreCurrentResources();
			}
            public void HandleStoreCurrentResources()
            {
                for (int i = 0; i < oldCurrentResources.Length; i++)
                {
                    float curResource = GetCurrentResource((EntityBeingResourcesType)oldResourcesEnumIndices[i]);
                    if (oldCurrentResources[i] != curResource)
                    {
                        oldCurrentResources[i] = curResource;
                        References.NetBehaviour.RpcSyncBeingCurrentResource(oldResourcesEnumIndices[i], curResource);
                    }
                }
            }
            #endregion

            #region Utilities
            protected override void HandleDeath()
			{
                if (deathResult != null)
                {
                    References.NetBehaviour.RpcOnDeath(deathResult);
                    deathResult = null;
                }
            }
			#endregion

			#region Events
			#endregion
		}
    }
}