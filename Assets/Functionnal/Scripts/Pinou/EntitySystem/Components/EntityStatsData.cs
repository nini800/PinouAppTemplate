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
            [SerializeField] protected StatsInfluenceData[] statsInfluencesData;

            public EntityStatsLevelType LevelType => levelType;
            public int MaxLevel => maxLevel;
            public int ComputeStartLevel() => Mathf.FloorToInt(Mathf.Lerp(startLevelRange.x, startLevelRange.y + 1, startLevelRepartitionCurve.Evaluate(UnityEngine.Random.value)));

            public PinouUtils.Maths.Formula ExperienceFormula => experienceFormula;
            public StatsInfluenceData[] StatsInfluencesData => statsInfluencesData;

            public void SetLevelType(EntityStatsLevelType type)
			{
                levelType = type;
			}
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
            protected StatsInfluenceData.StatsInfluence[] statsInfluences;

            public bool IsAtMaxLevel => level >= linkedLevelData.MaxLevel;
            public int Level => level;
            public int NextLevel => IsAtMaxLevel ? level : level + 1;
            public int MaxLevel => linkedLevelData.MaxLevel;
            public float TotalExperienceForNextLevel => linkedLevelData.ExperienceFormula.Evaluate(level);
            public float RemainingExperienceForNextLevel => TotalExperienceForNextLevel - experience;
            public float Experience => experience;
            public float ExperienceProgress => experience / TotalExperienceForNextLevel;
            public StatsInfluenceData.StatsInfluence[] StatsInfluences => statsInfluences;

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

            public StatsInfluenceData.StatsInfluence[] MakeStatsInfluences()
			{
                statsInfluences = new StatsInfluenceData.StatsInfluence[linkedLevelData.StatsInfluencesData.Length];
				for (int i = 0; i < statsInfluences.Length; i++)
				{
                    statsInfluences[i] = new StatsInfluenceData.StatsInfluence(linkedLevelData.StatsInfluencesData[i], level);
				}

                return statsInfluences;
            }
            public void ReevaluateStatsInfluences()
			{
				for (int i = 0; i < statsInfluences.Length; i++)
				{
                    statsInfluences[i].Reevaluate(level);
				}
			}
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

                InitializeStatsArray<EntityStatsStat>(ref _statsStatsFlats, ref _statsStatsFactors);
                InitializeDoubleStatsArray<EntityStatsLevelType, EntityStatsLevelStat>(ref _statsLevelStatsFlats, ref _statsLevelStatsFactors);

                InitializeStatsArray<EntityBeingStat>(ref _beingStatsFlats, ref _beingStatsFactors);
                InitializeDoubleStatsArray<EntityBeingResourceType, EntityBeingResourceStat>(ref _beingResourcesStatsFlats, ref _beingResourcesStatsFactors);

                InitializeStatsArray<EntityMovementsStat>(ref _movementsStatsFlats, ref _movementsStatsFactors);
                InitializeStatsArray<EntityAbilitiesStat>(ref _abilitiesStatsFlats, ref _abilitiesStatsFactors);
                InitializeDoubleStatsArray<EntityBeingResourceType, EntityAbilityResourceStat>(ref _abilitiesResourcesStatsFlats, ref _abilitiesResourcesStatsFactors);

                InitializeStatsArray<EntityVisualStat>(ref _visualStatsFlats, ref _visualStatsFactors);
            }
            private void InitializeStatsArray<T>(ref float[] statsFlats, ref float[] statsFactors) where T : Enum
			{
                int statCount = Enum.GetValues(typeof(T)).Length;
                statsFlats = new float[statCount - 1];
                statsFactors = new float[statCount - 1];
            }
			private void InitializeDoubleStatsArray<T, TT>(ref float[][] statsFlats, ref float[][] statsFactors) where T : Enum where TT : Enum
            {
                int typesCount = Enum.GetValues(typeof(T)).Length;
                int statsCount = Enum.GetValues(typeof(T)).Length;
                statsFlats = new float[typesCount][];
                statsFactors = new float[typesCount][];
				for (int i = 0; i < typesCount; i++)
				{
                    statsFlats[i] = new float[statsCount];
                    statsFactors[i] = new float[statsCount];
                }
            }

            private EntityStatsData _data = null;
            public new EntityStatsData Data => _data;
            #endregion

            #region Vars, Getters
            private float[] _statsStatsFlats;
            private float[] _statsStatsFactors;
            private float[][] _statsLevelStatsFlats;
            private float[][] _statsLevelStatsFactors;
            private float[] _beingStatsFlats;
            private float[] _beingStatsFactors;
            private float[][] _beingResourcesStatsFlats;
            private float[][] _beingResourcesStatsFactors;
            private float[] _movementsStatsFlats;
            private float[] _movementsStatsFactors;
            private float[] _abilitiesStatsFlats;
            private float[] _abilitiesStatsFactors;
            private float[][] _abilitiesResourcesStatsFlats;
            private float[][] _abilitiesResourcesStatsFactors;
            private float[] _visualStatsFlats;
            private float[] _visualStatsFactors;

            private List<StatsInfluenceData.StatsInfluence> _levelStatsInfluences = new List<StatsInfluenceData.StatsInfluence>();
            #endregion

            #region Behaviour
            public override void SlaveAwake()
			{
                HandleCreateLevelExperienceDatas();
                RefreshStatsEvaluationData();
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
                        StatsInfluenceData.StatsInfluence[] statsInfluences = led.MakeStatsInfluences();
                        _levelStatsInfluences.AddRange(statsInfluences);

                        led.OnLevelUp.SafeSubscribe(OnLevelUp, 10);
                    }
                }
            }
			#endregion

			#region Utilities
			#region Stats Evaluation
			public void RefreshStatsEvaluationData()
			{
                HandleSetAllArraysToDefault();
                HandleReevaluateLevelsStatsInfluences();
                HandleApplyStatsInfluences();
            }
            private void HandleSetAllArraysToDefault()
			{
                HandleSetArrayPairToDefault(ref _statsStatsFlats, ref _statsStatsFactors);
                HandleSetDoubleArrayPairToDefault(ref _statsLevelStatsFlats, ref _statsLevelStatsFactors);
                HandleSetArrayPairToDefault(ref _beingStatsFlats, ref _beingStatsFactors);
                HandleSetDoubleArrayPairToDefault(ref _beingResourcesStatsFlats, ref _beingResourcesStatsFactors);
                HandleSetArrayPairToDefault(ref _movementsStatsFlats, ref _movementsStatsFactors);
                HandleSetArrayPairToDefault(ref _abilitiesStatsFlats, ref _abilitiesStatsFactors);
                HandleSetDoubleArrayPairToDefault(ref _abilitiesResourcesStatsFlats, ref _abilitiesResourcesStatsFactors);
                HandleSetArrayPairToDefault(ref _visualStatsFlats, ref _visualStatsFactors);
            }
            private void HandleSetArrayPairToDefault(ref float[] flats, ref float[] factors)
			{
				for (int i = 0; i < flats.Length; i++)
				{
                    flats[i] = 0;
				}
				for (int i = 0; i < factors.Length; i++)
				{
                    factors[i] = 1;
				}
			}
            private void HandleSetDoubleArrayPairToDefault(ref float[][] flats, ref float[][] factors)
            {
                for (int i = 0; i < flats.Length; i++)
                {
					for (int j = 0; j < flats[i].Length; j++)
					{
                        flats[i][j] = 0;
                    }
                }
                for (int i = 0; i < factors.Length; i++)
                {
                    for (int j = 0; j < factors[i].Length; j++)
                    {
                        factors[i][j] = 1;
                    }
                }
            }

            private void HandleReevaluateLevelsStatsInfluences()
			{
                Array levelTypes = Enum.GetValues(typeof(EntityStatsLevelType));
                for (int i = 0; i < levelTypes.Length; i++)
                {
                    EntityStatsLevelType levelType = (EntityStatsLevelType)levelTypes.GetValue(i);
                    if (_data.GetHasLevelData(levelType))
                    {
                        GetLevelExperienceData(levelType).ReevaluateStatsInfluences();
                    }
                }
            }

            private void HandleApplyStatsInfluences()
			{
				for (int i = 0; i < _levelStatsInfluences.Count; i++)
				{
                    HandleApplyStatsInfluenceToArrays(_levelStatsInfluences[i], (int)_levelStatsInfluences[i].Data.StatsStatsInfluenced, ref _statsStatsFlats, ref _statsStatsFactors);
                    HandleApplyStatsInfluenceToArrays(_levelStatsInfluences[i], (int)_levelStatsInfluences[i].Data.BeingStatsInfluenced, ref _beingStatsFlats, ref _beingStatsFactors);
                    HandleApplyStatsInfluenceToArrays(_levelStatsInfluences[i], (int)_levelStatsInfluences[i].Data.MovementsStatsInfluenced, ref _movementsStatsFlats, ref _movementsStatsFactors);
                    HandleApplyStatsInfluenceToArrays(_levelStatsInfluences[i], (int)_levelStatsInfluences[i].Data.AbilitiesStatsInfluenced, ref _abilitiesStatsFlats, ref _abilitiesStatsFactors);
                    HandleApplyStatsInfluenceToArrays(_levelStatsInfluences[i], (int)_levelStatsInfluences[i].Data.VisualStatsInfluenced, ref _visualStatsFlats, ref _visualStatsFactors);

                    for (int j = 0; j < _levelStatsInfluences[i].Data.StatsLevelsInfluenced.Length; j++)
                    {
                        HandleApplyStatsInfluenceToDoubleArrays(
                            _levelStatsInfluences[i],
                            (int)_levelStatsInfluences[i].Data.StatsLevelsInfluenced[j].LevelInfluenced,
                            (int)_levelStatsInfluences[i].Data.StatsLevelsInfluenced[j].StatInfluenced,
                            ref _statsLevelStatsFlats,
                            ref _statsLevelStatsFactors);
                    }
                    for (int j = 0; j < _levelStatsInfluences[i].Data.BeingResourcesInfluenced.Length; j++)
                    {
                        HandleApplyStatsInfluenceToDoubleArrays(
                            _levelStatsInfluences[i],
                            (int)_levelStatsInfluences[i].Data.BeingResourcesInfluenced[j].ResourceInfluenced,
                            (int)_levelStatsInfluences[i].Data.BeingResourcesInfluenced[j].StatInfluenced,
                            ref _beingResourcesStatsFlats,
                            ref _beingResourcesStatsFactors);
                    }
                    for (int j = 0; j < _levelStatsInfluences[i].Data.AbilitiesResourcesInfluenced.Length; j++)
                    {
                        HandleApplyStatsInfluenceToDoubleArrays(
                            _levelStatsInfluences[i],
                            (int)_levelStatsInfluences[i].Data.AbilitiesResourcesInfluenced[j].ResourceInfluenced,
                            (int)_levelStatsInfluences[i].Data.AbilitiesResourcesInfluenced[j].StatInfluenced,
                            ref _abilitiesResourcesStatsFlats,
                            ref _abilitiesResourcesStatsFactors);
                    }
                }
			}
            private void HandleApplyStatsInfluenceToArrays(StatsInfluenceData.StatsInfluence statsInfluences, int bitmask, ref float[] flats, ref float[] factors)
			{
                if (bitmask == 0) { return; }
                int currentMask = 1;
				for (int i = 0; i < flats.Length; i++)
				{
                    if ((bitmask & (currentMask)) != 0)
					{
                        if (statsInfluences.Data.HasFlatFormula)
						{
                            flats[i] += statsInfluences.FlatAmount;
						}
                        if (statsInfluences.Data.HasFactorFormula)
						{
                            factors[i] *= statsInfluences.FactorAmount;
                        }
                    }
                    currentMask <<= 1;
				}
			}
            private void HandleApplyStatsInfluenceToDoubleArrays(StatsInfluenceData.StatsInfluence statsInfluences, int bitmaskOne, int bitmaskTwo, ref float[][] flats, ref float[][] factors)
            {
                if (bitmaskOne == 0 || bitmaskTwo == 0) { return; }
                int currentMaskOne = 1;
                for (int i = 0; i < flats.Length; i++)
                {
                    if ((bitmaskOne & currentMaskOne) != 0)
                    {
                        int currentMaskTwo = 1;
						for (int j = 0; j < flats[i].Length; j++)
						{
                            if ((bitmaskTwo & currentMaskTwo) != 0)
							{
                                if (statsInfluences.Data.HasFlatFormula)
                                {
                                    flats[i][j] += statsInfluences.FlatAmount;
                                }
                                if (statsInfluences.Data.HasFactorFormula)
                                {
                                    factors[i][j] *= statsInfluences.FactorAmount;
                                }
                            }
                            currentMaskTwo <<= 1;
                        }
                    }
                    currentMaskOne <<= 1;
                }
            }
            #endregion

            public float EvaluateStatsStat(EntityStatsStat stat, float baseAmount)
            {
                int index = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (index < 0) 
                { 
                    return baseAmount; 
                }
                else
				{
                    return (baseAmount * _statsStatsFactors[index]) + _statsStatsFlats[index];
                }
            }
            public float EvaluateStatsLevelStat(EntityStatsLevelType resource, EntityStatsLevelStat stat, float baseAmount)
            {
                int indexOne = PinouUtils.Maths.Pow2toIndex((int)resource);
                int indexTwo = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (indexOne < 0 || indexTwo < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _statsLevelStatsFactors[indexOne][indexTwo]) + _statsLevelStatsFlats[indexOne][indexTwo];
                }
            }
            public float EvaluateBeingStat(EntityBeingStat stat, float baseAmount)
			{
                int index = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (index < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _beingStatsFactors[index]) + _beingStatsFlats[index];
                }
            }
            public float EvaluateBeingResourceStat(EntityBeingResourceType resource, EntityBeingResourceStat stat, float baseAmount)
            {
                int indexOne = PinouUtils.Maths.Pow2toIndex((int)resource);
                int indexTwo = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (indexOne < 0 || indexTwo < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _beingResourcesStatsFactors[indexOne][indexTwo]) + _beingResourcesStatsFlats[indexOne][indexTwo];
                }
            }
            public float EvaluateMovementsStat(EntityMovementsStat stat, float baseAmount)
			{
                int index = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (index < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _movementsStatsFactors[index]) + _movementsStatsFlats[index];
                }
            }
            public float EvaluateAbilitiesStat(EntityAbilitiesStat stat, float baseAmount)
            {
                int index = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (index < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _abilitiesStatsFactors[index]) + _abilitiesStatsFlats[index];
                }
            }
            public float EvaluateAbilitiesResourcesStat(EntityBeingResourceType resourceType, float resourceChange)
            {
                int index = PinouUtils.Maths.Pow2toIndex((int)resourceType);
                if (index < 0)
                {
                    return resourceChange;
                }
                else
                {
                    bool sign = resourceChange < 0;
                    resourceChange *= _abilitiesResourcesStatsFactors[index][0];//0 => Index of EntityAbilityResourceStat.GlobalFactor
                    if (sign == true)
					{
                        resourceChange *= _abilitiesResourcesStatsFactors[index][1];//1 => Index of EntityAbilityResourceStat.NegativeFactor
                        resourceChange += _abilitiesResourcesStatsFlats[index][1];
                    }
                    else
					{
                        resourceChange *= _abilitiesResourcesStatsFactors[index][2];//2 => Index of EntityAbilityResourceStat.PositiveFactor
                        resourceChange += _abilitiesResourcesStatsFlats[index][2];
                    }
                    resourceChange += _abilitiesResourcesStatsFlats[index][0];
                    return resourceChange;
                }
            }
            public float EvaluateVisualStat(EntityVisualStat stat, float baseAmount)
            {
                int index = PinouUtils.Maths.Pow2toIndex((int)stat);
                if (index < 0)
                {
                    return baseAmount;
                }
                else
                {
                    return (baseAmount * _visualStatsFactors[index]) + _visualStatsFlats[index];
                }
            }
            #endregion

            #region Events
            private void OnLevelUp(LevelExperienceData expData, int level)
            {
                RefreshStatsEvaluationData();
            }
            #endregion
        }
	}
}