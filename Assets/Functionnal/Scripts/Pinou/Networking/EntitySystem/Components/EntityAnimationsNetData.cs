using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
    public class EntityAnimationsNetData : EntityAnimationsData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityAnimationsNet, EntityAnimationsNetData>(master, references, this);
        }
        #endregion

        public class EntityAnimationsNet : EntityAnimations
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityAnimationsNetData)base.Data;
            }

            private EntityAnimationsNetData _data = null;
            public new EntityAnimationsNetData Data => _data;
            #endregion
        }
    }
}