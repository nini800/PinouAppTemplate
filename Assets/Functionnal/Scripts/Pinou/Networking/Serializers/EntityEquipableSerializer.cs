#pragma warning disable 0649, 0414
using Mirror;
using Pinou.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pinou.Networking
{

    public static class EntityEquipableSerializer
	{
        #region EntityEquipableVisual
        public static void WriteVisualPart(this NetworkWriter writer, EntityEquipableVisual.VisualPart visualPart)
        {
            writer.WriteInt32(PinouApp.Resources.Data.Databases.GetIndex(DatabaseType.ItemModel, visualPart.Model));
            writer.WriteInt32((int)visualPart.Socket);
        }
        public static EntityEquipableVisual.VisualPart ReadVisualPart(this NetworkReader reader)
        {
            GameObject model = (GameObject)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.ItemModel, reader.ReadInt32());
            EntityBodySocket socket = (EntityBodySocket)reader.ReadInt32();

            return new EntityEquipableVisual.VisualPart(model, socket);
        }

        public static void WriteEntityEquipableVisual(this NetworkWriter writer, EntityEquipableVisual equipableVisual)
        {
            writer.WriteInt32(PinouApp.Resources.Data.Databases.GetIndex(DatabaseType.Icon, equipableVisual.Icon));
            int visualPartsLength = equipableVisual.Parts == null ? 0 : equipableVisual.Parts.Length;
            writer.WriteInt32(visualPartsLength);
			for (int i = 0; i < visualPartsLength; i++)
			{
                writer.WriteVisualPart(equipableVisual.Parts[i]);
			}
        }
        public static EntityEquipableVisual ReadEntityEquipableVisual(this NetworkReader reader)
        {
            Sprite icon = (Sprite)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.Icon, reader.ReadInt32());
            int visualPartsLength = reader.ReadInt32();
            EntityEquipableVisual.VisualPart[] parts = new EntityEquipableVisual.VisualPart[visualPartsLength];
			for (int i = 0; i < visualPartsLength; i++)
			{
                parts[i] = reader.ReadVisualPart();
			}

            return new EntityEquipableVisual(icon, parts);
        }
        #endregion
        #region EntityEquipableAbility
        public static void EntityEquipableAbilityResourcesInfluences(this NetworkWriter writer, EntityEquipableAbilityResourcesInfluences eeari)
		{
            writer.WriteAbilitiesResourcesInfluenceData(eeari.AbilitiesResourcesInfluences);
            writer.WriteBoolean(eeari.UseFlat);
            writer.WriteDouble(eeari.Flat);
            writer.WriteBoolean(eeari.UseFactor);
            writer.WriteDouble(eeari.Factor);
        }
        public static EntityEquipableAbilityResourcesInfluences ReadEntityEquipableAbilityResourcesInfluences(this NetworkReader reader)
		{
            return new EntityEquipableAbilityResourcesInfluences(
                reader.ReadAbilitiesResourcesInfluenceData(),
                reader.ReadBoolean(),
                (float)reader.ReadDouble(),
                reader.ReadBoolean(),
                (float)reader.ReadDouble());
		}

        public static void WriteEntityEquipableAbility(this NetworkWriter writer, EntityEquipableAbility equipableAbility)
        {
            writer.WriteInt32(equipableAbility.Ability.AbilityID);
            writer.WriteBoolean(equipableAbility.OverrideTrigger);
            if (equipableAbility.OverrideTrigger)
			{
                writer.WriteAbilityTriggerData(equipableAbility.TriggerData);
			}
            writer.WriteBoolean(equipableAbility.OverrideCooldown);
            if (equipableAbility.OverrideCooldown)
			{
                writer.WriteDouble(equipableAbility.Cooldown);
			}

            int abilitiesResourcesInfluencesLength = equipableAbility.AbilitiesResourcesInfluences == null ? 0 : equipableAbility.AbilitiesResourcesInfluences.Length;
            writer.WriteInt32(abilitiesResourcesInfluencesLength);
			for (int i = 0; i < abilitiesResourcesInfluencesLength; i++)
			{
                writer.EntityEquipableAbilityResourcesInfluences(equipableAbility.AbilitiesResourcesInfluences[i]);
			}
        }
        public static EntityEquipableAbility ReadEntityEquipableAbility(this NetworkReader reader)
		{
            AbilityData ability = (AbilityData)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.Ability, reader.ReadInt32());
            bool overrideTrigger = reader.ReadBoolean();
            AbilityTriggerData triggerData = null;
            if (overrideTrigger)
			{
                triggerData = reader.ReadAbilityTriggerData();
			}
            bool overrideCooldown = reader.ReadBoolean();
            float cooldown = 0f;
            if (overrideCooldown)
			{
                cooldown = (float)reader.ReadDouble();
			}

            int abilitiesResourcesInfluencesLength = reader.ReadInt32();
            EntityEquipableAbilityResourcesInfluences[] abilitiesResourcesInfluences = new EntityEquipableAbilityResourcesInfluences[abilitiesResourcesInfluencesLength];
			for (int i = 0; i < abilitiesResourcesInfluencesLength; i++)
			{
                abilitiesResourcesInfluences[i] = reader.ReadEntityEquipableAbilityResourcesInfluences();
            }

            return new EntityEquipableAbility(
                ability,
                overrideTrigger,
                triggerData,
                overrideCooldown,
                cooldown,
                abilitiesResourcesInfluences);
		}
        #endregion

        #region EntityEquipable
        public static void WriteEntityEquipable(this NetworkWriter writer, EntityEquipable equipable)
        {
            writer.WriteString(equipable.Name);
            writer.WriteInt32(equipable.ItemLevel);
            writer.WriteInt32((int)equipable.Type);
            int statsInfluencesLength = equipable.StatsInfluences == null ? 0 : equipable.StatsInfluences.Length;
            writer.WriteInt32(statsInfluencesLength);
			for (int i = 0; i < statsInfluencesLength; i++)
			{
                writer.WriteStatsInfluence(equipable.StatsInfluences[i]);
			}
            writer.WriteBoolean(equipable.HasEquippedAbility);
            if (equipable.HasEquippedAbility)
			{
                writer.WriteEntityEquipableAbility(equipable.EquippedAbility);
			}

            writer.WriteBoolean(equipable.HasVisual);
            if (equipable.HasVisual)
			{
                writer.WriteEntityEquipableVisual(equipable.Visual);
			}
        }
        public static EntityEquipable ReadEntityEquipable(this NetworkReader reader)
        {
            string name = reader.ReadString();
            int level = reader.ReadInt32();
            EntityEquipableType type = (EntityEquipableType)reader.ReadInt32();
            int statsInfluencesLength = reader.ReadInt32();
            StatsInfluenceData.StatsInfluence[] statsInfluences = new StatsInfluenceData.StatsInfluence[statsInfluencesLength];
			for (int i = 0; i < statsInfluencesLength; i++)
			{
                statsInfluences[i] = reader.ReadStatsInfluence();
			}

            bool hasEquippedAbility = reader.ReadBoolean();
            EntityEquipableAbility equipableAbility = null;
            if (hasEquippedAbility)
			{
                equipableAbility = reader.ReadEntityEquipableAbility();
			}

            bool hasVisual = reader.ReadBoolean();
            EntityEquipableVisual visual = null;
            if (hasVisual)
			{
                visual = reader.ReadEntityEquipableVisual();
			}

            return new EntityEquipable(
                name,
                level,
                type,
                statsInfluences,
                equipableAbility,
                visual);
        }
        #endregion
    }
}