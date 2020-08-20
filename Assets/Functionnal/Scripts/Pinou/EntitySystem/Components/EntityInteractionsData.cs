using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    public class EntityInteractionsData : EntityComponentData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityInteractions, EntityInteractionsData>(master, references, this);
        }
        #endregion

        public class EntityInteractions : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityInteractionsData)((EntityComponent)this).Data;
            }

            private EntityInteractionsData _data = null;
            protected new EntityInteractionsData Data => _data;
			#endregion

			public override InteractionState InteractionState => InteractionState.None;
		}
    }
}