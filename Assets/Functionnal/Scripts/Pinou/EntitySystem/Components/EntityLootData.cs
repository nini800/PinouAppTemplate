using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    public class EntityLootData : EntityComponentData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityLoot, EntityLootData>(master, references, this);
        }
        #endregion

        public class EntityLoot : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityLootData)((EntityComponent)this).Data;
            }

            private EntityLootData _data = null;
            protected new EntityLootData Data => _data;
            #endregion
        }
    }
}