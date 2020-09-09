using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem	
{
	public partial class EntityEquipmentData : EntityComponentData
	{
		[Header("Default Equipment")]
		[Space]
		[SerializeField, ValidateInput("ValidateShellBuilder", "Builder must have Shell built type"), InlineEditor] protected EntityEquipableBuilder defaultShellBuilder;
		private bool ValidateShellBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Shell; }
		public bool HasDefaultShell => defaultShellBuilder != null;
		public EntityEquipableBuilder DefaultShellBuilder => defaultShellBuilder;

		[SerializeField, ValidateInput("ValidateWeaponBuilder", "Builder must have Weapon built type"), InlineEditor] protected EntityEquipableBuilder defaultWeaponBuilder;
		private bool ValidateWeaponBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Weapon; }
		public bool HasDefaultWeapon => defaultWeaponBuilder != null;
		public EntityEquipableBuilder DefaultWeaponBuilder => defaultWeaponBuilder;

		[SerializeField, ValidateInput("ValidateAuraBuilder", "Builder must have Aura built type"), InlineEditor] protected EntityEquipableBuilder defaultAuraBuilder;
		private bool ValidateAuraBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Aura; }
		public bool HasDefaultAura => defaultAuraBuilder != null;
		public EntityEquipableBuilder DefaultAuraBuilder => defaultAuraBuilder;

		[SerializeField, ValidateInput("ValidateReactorBuilder", "Builder must have Reactor built type"), InlineEditor] protected EntityEquipableBuilder defaultReactorBuilder;
		private bool ValidateReactorBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Reactor; }
		public bool HasDefaultReactor => defaultReactorBuilder != null;
		public EntityEquipableBuilder DefaultReactorBuilder => defaultReactorBuilder;


		public EntityEquipableBuilder GetDefaultEquipableBuilder(EntityEquipableType equipmentType)
		{
			switch(equipmentType)
			{
				case EntityEquipableType.Shell:
					return defaultShellBuilder;
				case EntityEquipableType.Weapon:
					return defaultWeaponBuilder;
				case EntityEquipableType.Aura:
					return defaultAuraBuilder;
				case EntityEquipableType.Reactor:
					return defaultReactorBuilder;
			}
			
			throw new System.Exception("No Equipable " + equipmentType + " found.");
		}
	
        public partial class EntityEquipment : EntityComponent
        {
			protected EntityEquipable equippedShell;
			public bool HasShellEquipped => equippedShell != null;
			public EntityEquipable EquippedShell => equippedShell;

			protected EntityEquipable equippedWeapon;
			public bool HasWeaponEquipped => equippedWeapon != null;
			public EntityEquipable EquippedWeapon => equippedWeapon;

			protected EntityEquipable equippedAura;
			public bool HasAuraEquipped => equippedAura != null;
			public EntityEquipable EquippedAura => equippedAura;

			protected EntityEquipable equippedReactor;
			public bool HasReactorEquipped => equippedReactor != null;
			public EntityEquipable EquippedReactor => equippedReactor;


			public bool GetHasEquipped(EntityEquipableType equipmentType)
			{
				switch(equipmentType)
				{
					case EntityEquipableType.Shell:
						return HasShellEquipped;
					case EntityEquipableType.Weapon:
						return HasWeaponEquipped;
					case EntityEquipableType.Aura:
						return HasAuraEquipped;
					case EntityEquipableType.Reactor:
						return HasReactorEquipped;
				}
				
				throw new System.Exception("No Equipable " + equipmentType + " found.");
			}
			public EntityEquipable GetEquipped(EntityEquipableType equipmentType)
			{
				switch(equipmentType)
				{
					case EntityEquipableType.Shell:
						return equippedShell;
					case EntityEquipableType.Weapon:
						return equippedWeapon;
					case EntityEquipableType.Aura:
						return equippedAura;
					case EntityEquipableType.Reactor:
						return equippedReactor;
				}
				
				throw new System.Exception("No Equipable " + equipmentType + " found.");
			}
			protected void SetEquipped(EntityEquipableType type, EntityEquipable equipable)
			{
				switch(type)
				{
					case EntityEquipableType.Shell:
						equippedShell = equipable;
						break;
					case EntityEquipableType.Weapon:
						equippedWeapon = equipable;
						break;
					case EntityEquipableType.Aura:
						equippedAura = equipable;
						break;
					case EntityEquipableType.Reactor:
						equippedReactor = equipable;
						break;
				}
			}
        }
    }
}