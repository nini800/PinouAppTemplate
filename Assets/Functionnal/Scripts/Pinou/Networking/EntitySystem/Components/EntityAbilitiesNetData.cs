#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
    public class EntityAbilitiesNetData : EntityAbilitiesData
    {
        #region Vars, Fields, Getters
        #endregion

        #region Utilities
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityAbilitiesNet, EntityAbilitiesNetData>(master, references, this);
        }
        #endregion

        #region Editor
        #endregion

        public class EntityAbilitiesNet : EntityAbilities
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityAbilitiesNetData)base.Data;
            }

            private EntityAbilitiesNetData _data = null;
            protected new EntityAbilitiesNetData Data => _data;
            #endregion

            #region Vars, Getters
            #endregion

            #region Utilities
            #endregion

            #region Coroutines
            #endregion

            #region Events
            #endregion
        }
    }
}