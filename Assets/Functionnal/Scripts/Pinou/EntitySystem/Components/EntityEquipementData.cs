#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pinou.EntitySystem
{
    public partial class EntityEquipmentData : EntityComponentData
    {
        #region Vars, Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityEquipment, EntityEquipmentData>(master, references, this);
        }
        #endregion

        public partial class EntityEquipment : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityEquipmentData)((EntityComponent)this).Data;
            }

            private EntityEquipmentData _data = null;
            protected new EntityEquipmentData Data => _data;
            #endregion

            #region Utilities
            #endregion

            #region Behaviour
            #endregion
        }
    }
}