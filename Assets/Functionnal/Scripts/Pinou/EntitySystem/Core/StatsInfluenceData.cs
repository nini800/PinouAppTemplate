#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Pinou.EntitySystem
{
    [Serializable]
    public class StatsInfluenceData
    {
        [Serializable]
        public class StatsLevelInfluenceData
        {
            [SerializeField] private EntityStatsLevelType levelInfluenced;
            [SerializeField] private EntityStatsLevelStat statInfluenced;

            public EntityStatsLevelType LevelInfluenced => levelInfluenced;
            public EntityStatsLevelStat StatInfluenced => statInfluenced;
        }
        [Serializable]
        public class BeingResourcesInfluenceData
        {
            [SerializeField] private EntityBeingResourceType resourceInfluenced;
            [SerializeField] private EntityBeingResourceStat statInfluenced;

            public EntityBeingResourceType ResourceInfluenced => resourceInfluenced;
            public EntityBeingResourceStat StatInfluenced => statInfluenced;
        }
        [Serializable]
        public class AbilitiesResourcesInfluenceData
        {
            [SerializeField] private EntityBeingResourceType resourceInfluenced;
            [SerializeField] private EntityAbilityResourceStat statInfluenced;

            public EntityBeingResourceType ResourceInfluenced => resourceInfluenced;
            public EntityAbilityResourceStat StatInfluenced => statInfluenced;
        }
        [Header("Simple Stats")]
        [Space]
        [SerializeField] protected EntityStatsStat statsStatsInfluenced;
        [SerializeField] protected EntityBeingStat beingStatsInfluenced;
        [SerializeField] protected EntityMovementsStat movementsStatsInfluenced;
        [SerializeField] protected EntityAbilitiesStat abilitiesStatsInfluenced;
        [SerializeField] protected EntityVisualStat visualStatsInfluenced;

        [Header("Composed Stats")]
        [Space]
        [SerializeField] protected StatsLevelInfluenceData[] statsLevelsInfluenced;
        [SerializeField] protected BeingResourcesInfluenceData[] beingResourcesInfluenced;
        [SerializeField] protected AbilitiesResourcesInfluenceData[] abilitiesResourcesInfluenced;

        [Header("Formulas")]
        [Space]
        [SerializeField] protected bool hasFlatFormula;
        [SerializeField, ShowIf("hasFlatFormula")] protected PinouUtils.Maths.Formula flatFormula;

        [SerializeField] protected bool hasFactorFormula;
        [SerializeField, ShowIf("hasFactorFormula")] protected PinouUtils.Maths.Formula factorFormula;

        public EntityStatsStat StatsStatsInfluenced => statsStatsInfluenced;
        public EntityBeingStat BeingStatsInfluenced => beingStatsInfluenced;
        public EntityMovementsStat MovementsStatsInfluenced => movementsStatsInfluenced;
        public EntityAbilitiesStat AbilitiesStatsInfluenced => abilitiesStatsInfluenced;
        public EntityVisualStat VisualStatsInfluenced => visualStatsInfluenced;
        public StatsLevelInfluenceData[] StatsLevelsInfluenced => statsLevelsInfluenced;
        public BeingResourcesInfluenceData[] BeingResourcesInfluenced => beingResourcesInfluenced;
        public AbilitiesResourcesInfluenceData[] AbilitiesResourcesInfluenced => abilitiesResourcesInfluenced;

        public bool HasFlatFormula => hasFlatFormula;
        public bool HasFactorFormula => hasFactorFormula;
        public PinouUtils.Maths.Formula FactorFormula => factorFormula;
        public PinouUtils.Maths.Formula FlatFormula => flatFormula;

        public class StatsInfluence
		{
            public StatsInfluence(StatsInfluenceData data, float evaluateAmount)
			{
                _data = data;
                Reevaluate(evaluateAmount);
            }
            private StatsInfluenceData _data;
            public StatsInfluenceData Data => _data;

            private float _flatAmount;
            private float _factorAmount;
            public float FlatAmount => _flatAmount;
            public float FactorAmount => _factorAmount;

			public void Reevaluate(float amount)
			{
                _flatAmount = _data.flatFormula.Evaluate(amount);
                _factorAmount = _data.factorFormula.Evaluate(amount);
            }
		}
    }


}