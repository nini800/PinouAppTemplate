#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.ItemSystem
{
	public class EntityEquipable : IAbilityContainer
	{
		public EntityEquipable(string name, int itemLevel, EntityEquipableType type) : this(name, itemLevel, type, null, null, null) { }
		public EntityEquipable(string name, int itemLevel, EntityEquipableType type, StatsInfluenceData.StatsInfluence[] statsInfluences) : this(name, itemLevel, type, statsInfluences, null, null) { }
		public EntityEquipable(string name, int itemLevel, EntityEquipableType type, EntityEquipableAbility equippedAbility) : this(name, itemLevel, type, null, equippedAbility, null) { }
		public EntityEquipable(string name, int itemLevel, EntityEquipableType type, EntityEquipableVisual visual) : this(name, itemLevel, type, null, null, visual) { }
		public EntityEquipable(string name, int itemLevel, EntityEquipableType type, StatsInfluenceData.StatsInfluence[] statsInfluences = null, EntityEquipableAbility equippedAbility = null, EntityEquipableVisual visual = null)
		{
			this.name = name;
			this.itemLevel = itemLevel;
			this.type = type;

			this.statsInfluences = statsInfluences;
			this.equippedAbility = equippedAbility;
			this.visual = visual;
		}

		protected string name;
		protected int itemLevel;
		protected EntityEquipableType type;

		protected StatsInfluenceData.StatsInfluence[] statsInfluences;
		protected EntityEquipableAbility equippedAbility;
		protected EntityEquipableVisual visual;

		#region IAbilityContainer
		public bool ContainsAbility => equippedAbility != null;
		public AbilityData Ability => equippedAbility.Ability;
		public AbilityTriggerData AbilityTriggerData => equippedAbility.TriggerData;
		public float AbilityCooldown => equippedAbility.Cooldown;
		#endregion

		public string Name => name;
		public int ItemLevel => itemLevel;
		public EntityEquipableType Type => type;

		public bool HasStatsInfluences => statsInfluences != null && statsInfluences.Length > 0;
		public StatsInfluenceData.StatsInfluence[] StatsInfluences => statsInfluences;

		public bool HasEquippedAbility => equippedAbility != null;
		public EntityEquipableAbility EquippedAbility => equippedAbility;

		public bool HasVisual => visual != null;
		public EntityEquipableVisual Visual => visual;
	}
}