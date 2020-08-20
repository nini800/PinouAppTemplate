using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pinou.EntitySystem
{
    public partial class EntityStatsData : EntityComponentData
    {
        #region DataClasses
        [Serializable]
        public class LevelData
        {
            [SerializeField, ReadOnly] protected EntityStatsLevelType levelType;
            [SerializeField] protected int maxLevel;
            [SerializeField] protected Vector2Int startLevelRange;
            [SerializeField] protected AnimationCurve startLevelRepartitionCurve;
            [SerializeField] protected PinouUtils.Maths.Formula experienceFormula;
            [SerializeField] protected LevelStatsInfluenceData[] statsInfluences;

            public EntityStatsLevelType LevelType => levelType;
            public int MaxLevel => maxLevel;
            public int ComputeStartLevel() => Mathf.FloorToInt(Mathf.Lerp(startLevelRange.x, startLevelRange.y + 1, startLevelRepartitionCurve.Evaluate(UnityEngine.Random.value)));

            public PinouUtils.Maths.Formula ExperienceFormula => experienceFormula;
            public LevelStatsInfluenceData[] StatsInfluences => statsInfluences;

            public void SetLevelType(EntityStatsLevelType type)
			{
                levelType = type;
			}
        }
        [Serializable]
        public class LevelStatsInfluenceData
        {
            [SerializeField] protected EntityBeingStat beingStatsInfluenced;
            [SerializeField] protected EntityMovementsStat movementsStatsInfluenced;
            [SerializeField] protected EntityAbilitiesStat abilitiesStatsInfluenced;
            [SerializeField] protected EntityVisualStat visualStatsInfluenced;

            [SerializeField] protected bool hasFlatFormula;
            [SerializeField, ShowIf("hasFlatFormula")] protected PinouUtils.Maths.Formula flatFormula;

            [SerializeField] protected bool hasFactorFormula;
            [SerializeField, ShowIf("hasFactorFormula")] protected PinouUtils.Maths.Formula factorFormula;


            public PinouUtils.Maths.Formula FactorFormula => factorFormula;
            public PinouUtils.Maths.Formula FlatFormula => flatFormula;
        }
        public class LevelExperienceData
		{
            public LevelExperienceData(LevelData levelDataToLink) 
            { 
                linkedLevelData = levelDataToLink;
                levelType = linkedLevelData.LevelType;
                level = linkedLevelData.ComputeStartLevel();
                experience = 0f;
            }
            protected LevelData linkedLevelData;
            protected EntityStatsLevelType levelType;
            protected int level;
            protected float experience;

            public bool IsAtMaxLevel => level >= linkedLevelData.MaxLevel;
            public int Level => level;
            public int NextLevel => IsAtMaxLevel ? level : level + 1;
            public int MaxLevel => linkedLevelData.MaxLevel;
            public float TotalExperienceForNextLevel => linkedLevelData.ExperienceFormula.Evaluate(level);
            public float RemainingExperienceForNextLevel => TotalExperienceForNextLevel - experience;
            public float Experience => experience;
            public float ExperienceProgress => experience / TotalExperienceForNextLevel;

            private void CheckForLevelUp()
			{
                if (IsAtMaxLevel) { return; }

                while (experience > TotalExperienceForNextLevel)
				{
                    experience -= TotalExperienceForNextLevel;
                    level++;
                    OnLevelChange.Invoke(this, level);
                    OnLevelUp.Invoke(this, level);
				}
			}

            public void SetLevel(int level, bool resetExperience = true)
			{
                if (this.level != level)
				{
                    int oldLevel = this.level;
                    this.level = Mathf.Clamp(level, 1, MaxLevel);
                    if (resetExperience) { experience = 0f; }

                    OnLevelChange.Invoke(this, level);
                    if (oldLevel < level)
					{
                        OnLevelUp.Invoke(this, level);
                    }
                }
            }
            public void ModifyLevel(int levelDifference, bool resetExperience = true)
			{
                if (levelDifference != 0)
				{
                    int oldLevel = this.level;
                    level = Mathf.Clamp(level + levelDifference, 1, MaxLevel);
                    if (resetExperience) { experience = 0f; }

                    OnLevelChange.Invoke(this, level);
                    if (oldLevel < level)
                    {
                        OnLevelUp.Invoke(this, level);
                    }
                }
            }

            public void SetExperience(float amount)
			{
                experience = Mathf.Clamp(amount, 0f, Mathf.Infinity);
                CheckForLevelUp();
			}
            public void ModifyExperience(float amountDifference)
			{
                experience = Mathf.Clamp(experience + amountDifference, 0f, Mathf.Infinity);
                CheckForLevelUp();
            }
            public void SetExperiencePct(float pct)
			{
                pct = Mathf.Clamp(pct, 0f, Mathf.Infinity);
                experience = TotalExperienceForNextLevel * pct;
                CheckForLevelUp();
            }
            public void ModifyExperiencePct(float pctDifference)
			{
                experience += TotalExperienceForNextLevel * pctDifference;
                experience = Mathf.Clamp(experience, 0f, Mathf.Infinity);
                CheckForLevelUp();
            }

            public PinouUtils.Delegate.Action<LevelExperienceData, int> OnLevelChange { get; private set; } = new PinouUtils.Delegate.Action<LevelExperienceData, int>();
            public PinouUtils.Delegate.Action<LevelExperienceData, int> OnLevelUp { get; private set; } = new PinouUtils.Delegate.Action<LevelExperienceData, int>();
        }
        #endregion
        #region Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityStats, EntityStatsData>(master, references, this);
        }
		#endregion

		#region Editor
		private void OnValidate()
		{
            Array levelTypes = Enum.GetValues(typeof(EntityStatsLevelType));
			for (int i = 0; i < levelTypes.Length; i++)
			{
                EntityStatsLevelType levelType = (EntityStatsLevelType)levelTypes.GetValue(i);
                LevelData levelData = GetLevelData(levelType);
                levelData?.SetLevelType(levelType);
            }
		}
		#endregion

		public partial class EntityStats : EntityComponent
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

			#region Behaviour
			public override void SlaveAwake()
			{
                HandleCreateLevelExperienceDatas();
            }
            private void HandleCreateLevelExperienceDatas()
			{
                Array levelTypes = Enum.GetValues(typeof(EntityStatsLevelType));
                for (int i = 0; i < levelTypes.Length; i++)
                {
                    EntityStatsLevelType levelType = (EntityStatsLevelType)levelTypes.GetValue(i);
                    if (_data.GetHasLevelData(levelType))
					{
                        LevelExperienceData led = new LevelExperienceData(_data.GetLevelData(levelType));
                        SetLevelExperienceData(levelType, led);
                    }
                }
            }
			#endregion

			#region Utilities
			#endregion

			#region Events
			#endregion
		}
	}
}