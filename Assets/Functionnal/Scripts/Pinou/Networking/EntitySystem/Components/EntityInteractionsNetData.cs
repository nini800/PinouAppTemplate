using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
    public class EntityInteractionsNetData : EntityInteractionsData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityInteractionsNet, EntityInteractionsNetData>(master, references, this);
        }
        #endregion

        public class EntityInteractionsNet : EntityInteractions
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityInteractionsNetData)base.Data;
            }

            private EntityInteractionsNetData _data = null;
            public new EntityInteractionsNetData Data => _data;
            #endregion
        }
    }
}