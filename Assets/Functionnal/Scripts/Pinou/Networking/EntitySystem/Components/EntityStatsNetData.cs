﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
    public class EntityStatsNetData : EntityStatsData
    {
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityStatsNet, EntityStatsNetData>(master, references, this);
        }
        #endregion

        public class EntityStatsNet : EntityStats
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                base.OnConstruct();
                _data = (EntityStatsNetData)base.Data;
            }

            private EntityStatsNetData _data = null;
            public new EntityStatsNetData Data => _data;
            #endregion
        }
    }
}