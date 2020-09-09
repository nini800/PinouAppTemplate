#pragma warning disable 0649, 0414
using Mirror;
using Pinou.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.Networking
{
    public static class AbilityDataSerializer
    {

        #region AbilityTriggerData
        public static void WriteAbilityTriggerData(this NetworkWriter writer, AbilityTriggerData triggerData)
        {
            writer.WriteInt32((int)triggerData.TriggerMethod);
            writer.WriteDouble(triggerData.EditorPrecision);
            writer.WriteInt32(triggerData.BurstCount);
            writer.WriteDouble(triggerData.BurstPeriod);
            writer.WriteInt32(triggerData.MultiCastCount);
        }
        public static AbilityTriggerData ReadAbilityTriggerData(this NetworkReader reader)
        {
            return new AbilityTriggerData(
                    (AbilityTriggerMethod)reader.ReadInt32(),
                    (float)reader.ReadDouble(),
                    reader.ReadInt32(),
                    (float)reader.ReadDouble(),
                    reader.ReadInt32());
        }
        #endregion
        #region EntityEquipable
        public static void WriteAbilityData(this NetworkWriter writer, AbilityData ability)
        {
            writer.WriteInt32(ability.AbilityID);
        }
        public static AbilityData ReadAbilityData(this NetworkReader reader)
        {
            return (AbilityData)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.Ability, reader.ReadInt32());
        }
        #endregion
    }
}