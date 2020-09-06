using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem	
{
	public partial class EntityEquipmentData : EntityComponentData
	{
		[Header("Default Equipment")]
		[Space]
		[SerializeField, ValidateInput("ValidateChestBuilder", "Builder must have Chest built type"), InlineEditor] protected EntityEquipableBuilder defaultChestBuilder;
		private bool ValidateChestBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Chest; }
		public bool HasDefaultChest => defaultChestBuilder != null;
		public EntityEquipableBuilder DefaultChestBuilder => defaultChestBuilder;

		[SerializeField, ValidateInput("ValidatePantsBuilder", "Builder must have Pants built type"), InlineEditor] protected EntityEquipableBuilder defaultPantsBuilder;
		private bool ValidatePantsBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Pants; }
		public bool HasDefaultPants => defaultPantsBuilder != null;
		public EntityEquipableBuilder DefaultPantsBuilder => defaultPantsBuilder;

		[SerializeField, ValidateInput("ValidateBootsBuilder", "Builder must have Boots built type"), InlineEditor] protected EntityEquipableBuilder defaultBootsBuilder;
		private bool ValidateBootsBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Boots; }
		public bool HasDefaultBoots => defaultBootsBuilder != null;
		public EntityEquipableBuilder DefaultBootsBuilder => defaultBootsBuilder;

		[SerializeField, ValidateInput("ValidateGlovesBuilder", "Builder must have Gloves built type"), InlineEditor] protected EntityEquipableBuilder defaultGlovesBuilder;
		private bool ValidateGlovesBuilder(EntityEquipableBuilder builder) { return builder == null || builder.BuiltType == EntityEquipableType.Gloves; }
		public bool HasDefaultGloves => defaultGlovesBuilder != null;
		public EntityEquipableBuilder DefaultGlovesBuilder => defaultGlovesBuilder;


		public EntityEquipableBuilder GetDefaultEquipmentBuilder(EntityEquipableType equipmentType)
		{
			switch(equipmentType)
			{
				case EntityEquipableType.Chest:
					return defaultChestBuilder;
				case EntityEquipableType.Pants:
					return defaultPantsBuilder;
				case EntityEquipableType.Boots:
					return defaultBootsBuilder;
				case EntityEquipableType.Gloves:
					return defaultGlovesBuilder;
			}
			
			throw new System.Exception("No Equipable " + equipmentType + " found.");
		}
	
        public partial class EntityEquipment : EntityComponent
        {
			protected EntityEquipable equippedChest;
			public bool HasChestEquipped => equippedChest != null;
			public EntityEquipable EquippedChest => equippedChest;

			protected EntityEquipable equippedPants;
			public bool HasPantsEquipped => equippedPants != null;
			public EntityEquipable EquippedPants => equippedPants;

			protected EntityEquipable equippedBoots;
			public bool HasBootsEquipped => equippedBoots != null;
			public EntityEquipable EquippedBoots => equippedBoots;

			protected EntityEquipable equippedGloves;
			public bool HasGlovesEquipped => equippedGloves != null;
			public EntityEquipable EquippedGloves => equippedGloves;


			public bool GetHasEquipped(EntityEquipableType equipmentType)
			{
				switch(equipmentType)
				{
					case EntityEquipableType.Chest:
						return HasChestEquipped;
					case EntityEquipableType.Pants:
						return HasPantsEquipped;
					case EntityEquipableType.Boots:
						return HasBootsEquipped;
					case EntityEquipableType.Gloves:
						return HasGlovesEquipped;
				}
				
				throw new System.Exception("No Equipable " + equipmentType + " found.");
			}
			public EntityEquipable GetEquipped(EntityEquipableType equipmentType)
			{
				switch(equipmentType)
				{
					case EntityEquipableType.Chest:
						return equippedChest;
					case EntityEquipableType.Pants:
						return equippedPants;
					case EntityEquipableType.Boots:
						return equippedBoots;
					case EntityEquipableType.Gloves:
						return equippedGloves;
				}
				
				throw new System.Exception("No Equipable " + equipmentType + " found.");
			}
        }
    }
}