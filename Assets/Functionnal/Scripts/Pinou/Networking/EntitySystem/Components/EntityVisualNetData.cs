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

        public class EntityVisualNet : EntityVisual
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

            #region Utilities
            #endregion
            
            #region Events

            #endregion
        }
    }
}