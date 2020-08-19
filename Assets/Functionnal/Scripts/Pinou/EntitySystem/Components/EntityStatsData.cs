using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    public class EntityStatsData : EntityComponentData
    {
        #region DataClasses
        [System.Serializable]
        public class LevelData
        {
            [SerializeField] protected string levelName;
            [SerializeField] protected string experienceName;
            [SerializeField] protected int maxLevel;
            [SerializeField] protected Vector2Int startLevelRange;
            [SerializeField] protected AnimationCurve startLevelRepartitionCurve;
            [SerializeField] protected PinouUtils.Maths.Formula experienceFormula;
            [SerializeField] protected LevelStatsInfluenceData[] statsInfluences;

            public string LevelName => levelName;
            public string ExperienceName => experienceName;
            public int MaxLevel => maxLevel;
            public int ComputeStartLevel() => Mathf.FloorToInt(Mathf.Lerp(startLevelRange.x, startLevelRange.y + 1, startLevelRepartitionCurve.Evaluate(Random.value)));

            public PinouUtils.Maths.Formula ExperienceFormula => experienceFormula;
            public LevelStatsInfluenceData[] StatsInfluences => statsInfluences;
        }
        [System.Serializable]
        public class LevelStatsInfluenceData
        {
            [SerializeField] protected EntityBeingStats beingStatsInfluenced;
            [SerializeField] protected EntityMovementsStats movementsStatsInfluenced;
            [SerializeField] protected EntityAbilitiesStats abilitiesStatsInfluenced;
            [SerializeField] protected EntityVisualStats visualStatsInfluenced;

            [SerializeField] protected bool hasFlatFormula;
            [SerializeField, ShowIf("hasFlatFormula")] protected PinouUtils.Maths.Formula flatFormula;

            [SerializeField] protected bool hasFactorFormula;
            [SerializeField, ShowIf("hasFactorFormula")] protected PinouUtils.Maths.Formula factorFormula;


            public PinouUtils.Maths.Formula FactorFormula => factorFormula;
            public PinouUtils.Maths.Formula FlatFormula => flatFormula;
        }

        #endregion
        #region Fields, Getters
        [Header("Levels")]
        [Space]
        [SerializeField] protected LevelData[] levels;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityStats, EntityStatsData>(master, references, this);
        }
        #endregion

        #region Editor
        #endregion

        public class EntityStats : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityStatsData)((EntityComponent)this).Data;
            }

            private EntityStatsData _data = null;
            public new EntityStatsData Data => _data;
            #endregion

            #region Utilities
            #endregion

            #region Events
            #endregion
        }
    }
}