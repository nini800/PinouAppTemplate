#pragma warning disable 0649
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public class EntityMovementsNetData : EntityMovementsData
	{
        #region Vars, Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityMovementsNet, EntityMovementsNetData>(master, references, this);
        }
        #endregion

        public class EntityMovementsNet : EntityMovements
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityMovementsNetData)base.Data;
            }

            private EntityMovementsNetData _data = null;
            public new EntityMovementsNetData Data => _data;
            #endregion

            #region Utilities
            #endregion

            #region Events
            #endregion
        }
    }
}