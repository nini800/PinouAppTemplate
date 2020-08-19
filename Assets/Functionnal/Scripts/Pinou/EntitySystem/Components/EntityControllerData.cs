﻿using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityControllerData : EntityComponentData
	{
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityController, EntityControllerData>(master, references, this);
        }
        #endregion

        public class EntityController : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityControllerData)((EntityComponent)this).Data;
            }

            private EntityControllerData _data = null;
            protected new EntityControllerData Data => _data;
            #endregion

            public override ControllerState ControllerState => ControllerState.None;
        }
    }
}