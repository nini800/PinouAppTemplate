using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    public class EntityAnimationsData : EntityComponentData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityAnimations, EntityAnimationsData>(master, references, this);
        }
        #endregion

        public class EntityAnimations : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityAnimationsData)((EntityComponent)this).Data;
            }

            private EntityAnimationsData _data = null;
            protected new EntityAnimationsData Data => _data;
            #endregion
        }
    }
}