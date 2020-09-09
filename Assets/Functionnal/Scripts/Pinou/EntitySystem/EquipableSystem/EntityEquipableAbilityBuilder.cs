#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pinou.EntitySystem
{
	[Serializable]
	public class EntityEquipableAbilityResourcesInfluencesBase
	{
		public EntityEquipableAbilityResourcesInfluencesBase(StatsInfluenceData.AbilitiesResourcesInfluenceData resourcesInfluences)
		{
			_abilitiesResourcesInfluences = resourcesInfluences;
		}
		[SerializeField] private StatsInfluenceData.AbilitiesResourcesInfluenceData _abilitiesResourcesInfluences;

		public StatsInfluenceData.AbilitiesResourcesInfluenceData AbilitiesResourcesInfluences => _abilitiesResourcesInfluences;
	}
	[Serializable]
	public class EntityEquipableAbilityResourcesInfluencesBuilder : EntityEquipableAbilityResourcesInfluencesBase
	{
		public EntityEquipableAbilityResourcesInfluencesBuilder(StatsInfluenceData.AbilitiesResourcesInfluenceData resourcesInfluences) : base(resourcesInfluences) { }

		[SerializeField] private bool _useFlatFormula;
		[SerializeField, ShowIf("_useFlatFormula")] private PinouUtils.Maths.Formula _flatFormula;
		[SerializeField] private bool _useFactorFormula;
		[SerializeField, ShowIf("_useFactorFormula")] private PinouUtils.Maths.Formula _factorFormula;

		public bool UseFlatFormula => _useFlatFormula;
		public PinouUtils.Maths.Formula FlatFormula => _flatFormula;
		public bool UseFactorFormula => _useFactorFormula;
		public PinouUtils.Maths.Formula FactorFormula => _factorFormula;

		public EntityEquipableAbilityResourcesInfluences Build(int level)
		{
			float flat = _useFlatFormula ? _flatFormula.Evaluate(level) : 0f;
			float factor = _useFactorFormula ? _factorFormula.Evaluate(level) : 1f;
			EntityEquipableAbilityResourcesInfluences arInfluences = new EntityEquipableAbilityResourcesInfluences(AbilitiesResourcesInfluences, _useFlatFormula, flat, _useFactorFormula, factor);

			return arInfluences;
		}
	}
	public class EntityEquipableAbilityResourcesInfluences : EntityEquipableAbilityResourcesInfluencesBase
	{
		public EntityEquipableAbilityResourcesInfluences(StatsInfluenceData.AbilitiesResourcesInfluenceData resourcesInfluences, bool useFlat, float flat, bool useFactor, float factor) : base(resourcesInfluences) 
		{
			_useFlat = useFlat;
			_flat = flat;
			_useFactor = useFactor;
			_factor = factor;
		}

		private bool _useFlat;
		private float _flat;
		private bool _useFactor;
		private float _factor;

		public bool UseFlat => _useFlat;
		public float Flat => _flat;
		public bool UseFactor => _useFactor;
		public float Factor => _factor;
	}

	[Serializable]
	public class EntityEquipableAbilityBase
	{
		public EntityEquipableAbilityBase(AbilityData ability, bool overrideTrigger, AbilityTriggerData triggerData, bool overrideCooldown, float cooldown)
		{
			_ability = ability;
			_overrideTrigger = overrideTrigger;
			_triggerData = triggerData;
			_overrideCooldown = overrideCooldown;
			_cooldown = cooldown;
		}

		[Header("Ability Reference")]
		[Space]
		[SerializeField, InlineEditor] private AbilityData _ability;

		[Header("Ability Trigger Overrides")]
		[Space]
		[SerializeField] private bool _overrideTrigger;
		[SerializeField, ShowIf("_overrideTrigger")] private AbilityTriggerData _triggerData;

		[Header("Ability Stats Overrides")]
		[Space]
		[SerializeField] private bool _overrideCooldown;
		[SerializeField, ShowIf("_overrideCooldown"), Indent] private float _cooldown;

		public AbilityData Ability => _ability;

		public bool OverrideTrigger => _overrideTrigger;
		public AbilityTriggerData TriggerData => _overrideTrigger ? _triggerData : _ability.Trigger;

		public bool OverrideCooldown => _overrideCooldown;
		public float Cooldown => _overrideCooldown ? _cooldown : _ability.Timing.Cooldown;
	}

	[Serializable]
	public class EntityEquipableAbilityBuilder : EntityEquipableAbilityBase
	{
		public EntityEquipableAbilityBuilder(AbilityData ability, bool overrideTrigger, AbilityTriggerData triggerData, bool overrideCooldown, float cooldown) : base(ability, overrideTrigger, triggerData, overrideCooldown, cooldown) { }

		[Space]
		[SerializeField] private EntityEquipableAbilityResourcesInfluencesBuilder[] _abilitiesResourcesInfluencesBuilders;

		public EntityEquipableAbility Build(int level)
		{
			EntityEquipableAbilityResourcesInfluences[] eeari = new EntityEquipableAbilityResourcesInfluences[_abilitiesResourcesInfluencesBuilders.Length];
			for (int i = 0; i < eeari.Length; i++)
			{
				eeari[i] = _abilitiesResourcesInfluencesBuilders[i].Build(level);
			}

			return new EntityEquipableAbility(Ability, OverrideTrigger, TriggerData, OverrideCooldown, Cooldown, eeari);
		}
	}

	public class EntityEquipableAbility : EntityEquipableAbilityBase
	{
		public EntityEquipableAbility(AbilityData ability, bool overrideTrigger, AbilityTriggerData triggerData, bool overrideCooldown, float cooldown, EntityEquipableAbilityResourcesInfluences[] abilitiesResourcesInfluences) : base(ability, overrideTrigger, triggerData, overrideCooldown, cooldown) 
		{
			_abilitiesResourcesInfluences = abilitiesResourcesInfluences;
		}

		private EntityEquipableAbilityResourcesInfluences[] _abilitiesResourcesInfluences;

		public EntityEquipableAbilityResourcesInfluences[] AbilitiesResourcesInfluences => _abilitiesResourcesInfluences;

		public void ApplyResourcesInfluencesOnImpacts(AbilityResourceImpactData[] impacts)
		{
			foreach (var impact in impacts)
			{
				foreach (var influence in _abilitiesResourcesInfluences)
				{
					if ((impact.ResourceType & influence.AbilitiesResourcesInfluences.ResourceInfluenced) > 0)
					{
						ApplyInfluenceToImpact(impact, influence);
					}
				}
			}
		}
		private void ApplyInfluenceToImpact(AbilityResourceImpactData impact, EntityEquipableAbilityResourcesInfluences influence)
		{
			bool signPositive = impact.ResourceChange >= 0;
			if (influence.UseFactor)
			{
				if (influence.AbilitiesResourcesInfluences.InfluencesGlobal ||
					signPositive && influence.AbilitiesResourcesInfluences.InfluencesPositive ||
					signPositive == false && influence.AbilitiesResourcesInfluences.InfluencesNegative)
				{
					impact.SetResourceChange(impact.ResourceChange * influence.Factor);
				}
			}
			if (influence.UseFlat)
			{
				if (influence.AbilitiesResourcesInfluences.InfluencesGlobal ||
					signPositive && influence.AbilitiesResourcesInfluences.InfluencesPositive ||
					signPositive == false && influence.AbilitiesResourcesInfluences.InfluencesNegative)
				{
					impact.SetResourceChange(impact.ResourceChange + influence.Flat);
				}
			}
		}
	}
}
