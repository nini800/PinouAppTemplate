using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
    public class EntityLootNetData : EntityLootData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityLootNet, EntityLootNetData>(master, references, this);
        }
        #endregion

        public class EntityLootNet : EntityLoot
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityLootNetData)base.Data;
            }

            private EntityLootNetData _data = null;
            public new EntityLootNetData Data => _data;
            #endregion
        }
    }
}