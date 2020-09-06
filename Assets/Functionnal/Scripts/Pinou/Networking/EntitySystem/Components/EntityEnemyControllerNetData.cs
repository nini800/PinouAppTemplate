#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pinou.EntitySystem;
using Mirror;

namespace Pinou.Networking
{
	public class EntityEnemyControllerNetData : EntityEnemyControllerData
	{
        #region Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityEnemyControllerNet, EntityEnemyControllerNetData>(master, references, this);
        }
        #endregion

        public class EntityEnemyControllerNet : EntityEnemyController, IControllerNet
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityEnemyControllerNetData)base.Data;
            }

            private EntityEnemyControllerNetData _data = null;
            protected new EntityEnemyControllerNetData Data => _data;
			#endregion

			#region Vars, Getters
			#endregion

			#region Behaviour
			public override void SlaveAwake()
			{
                if (PinouNetworkManager.IsServer)
				{
                    base.SlaveAwake();
				}
			}
			public override void SlaveUpdate()
			{
                if (PinouNetworkManager.IsServer)
                {
                    base.SlaveUpdate();
                }
            }
			#endregion

			#region Editor
			#endregion
		}
    }
}