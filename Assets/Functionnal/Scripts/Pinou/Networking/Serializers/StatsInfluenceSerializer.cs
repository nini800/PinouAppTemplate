#pragma warning disable 0649, 0414
using Mirror;
using Pinou.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.Networking
{
    public static class StatInfluenceSerializer
    {
        #region AbilitiesResourcesInfluenceData
        public static void WriteAbilitiesResourcesInfluenceData(this NetworkWriter writer, StatsInfluenceData.AbilitiesResourcesInfluenceData resourcesInfluences)
        {
            writer.WriteInt32((int)resourcesInfluences.ResourceInfluenced);
            writer.WriteInt32((int)resourcesInfluences.StatInfluenced);
        }
        public static StatsInfluenceData.AbilitiesResourcesInfluenceData ReadAbilitiesResourcesInfluenceData(this NetworkReader reader)
        {
            return new StatsInfluenceData.AbilitiesResourcesInfluenceData(
                (EntityBeingResourceType)reader.ReadInt32(), (EntityAbilityResourceStat)reader.ReadInt32());
        }
        #endregion

        #region StatsInfluence
        public static void WriteStatsInfluence(this NetworkWriter writer, StatsInfluenceData.StatsInfluence statsInfluence)
        {
            writer.WriteDouble(statsInfluence.FlatAmount);
            writer.WriteDouble(statsInfluence.FactorAmount);
        }
        public static StatsInfluenceData.StatsInfluence ReadStatsInfluence(this NetworkReader reader)
        {
            StatsInfluenceData.StatsInfluence stat = new StatsInfluenceData.StatsInfluence(
                    null,
                    (float)reader.ReadDouble(),
                    (float)reader.ReadDouble()
                );

            return stat;
        }
        #endregion
    }
}