using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public class EntityControllerNetData : EntityControllerData
	{
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityControllerNet, EntityControllerNetData>(master, references, this);
        }
        #endregion

        public class EntityControllerNet : EntityController, IControllerNet
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityControllerNetData)base.Data;
            }

            private EntityControllerNetData _data = null;
            protected new EntityControllerNetData Data => _data;
            #endregion
        }
    }
}