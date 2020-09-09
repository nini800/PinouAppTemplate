#pragma warning disable 0649, 0414
using Mirror;
using Pinou.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pinou.Networking
{
    public struct AbilityCastIdentifier
    {
        private Entity _caster;
        private float _castTime;
        private int _abilityID;
        private int _mutliCastID;

        public Entity Caster => _caster;
        public float CastTime => _castTime;
        public int AbilityID => _abilityID;
        public int MultiCastID => _mutliCastID;

        public AbilityCastIdentifier(Entity caster, float castTime, int abilityID, int multiCastID = 0)
        {
            _caster = caster;
            _castTime = castTime;
            _abilityID = abilityID;
            _mutliCastID = multiCastID;
        }
    }

    public static class AbilityCastDataSerializer
	{
        private static Dictionary<AbilityCastIdentifier, AbilityCastData> _storedAbilityCastData = new Dictionary<AbilityCastIdentifier, AbilityCastData>();

        #region AbilityResourceImpactData
        public static void WriteAbilityResourceImpactData(this NetworkWriter writer, AbilityResourceImpactData impactData)
        {
            writer.WriteInt32((int)impactData.ResourceType);
            writer.WriteDouble(impactData.ResourceChange);
        }
        public static AbilityResourceImpactData ReadAbilityResourceImpactData(this NetworkReader reader)
        {
            AbilityResourceImpactData impactData = new AbilityResourceImpactData(
                (EntityBeingResourceType)reader.ReadInt32(),
                (float)reader.ReadDouble());

            return impactData;
        }
        #endregion

        #region AbilityCastIdentifier
        public static void WriteAbilityCastIdentifier(this NetworkWriter writer, AbilityCastIdentifier identifier)
        {
            //In case of the caster's death
            if (identifier.Caster == null) { writer.WriteGameObject(null); }
			else { writer.WriteGameObject(identifier.Caster.gameObject); }
            writer.WriteDouble(identifier.CastTime);
            writer.WriteInt32(identifier.AbilityID);
            writer.WriteInt32(identifier.MultiCastID);
        }
        public static AbilityCastIdentifier ReadAbilityCastIdentifier(this NetworkReader reader)
        {
            Entity ent;
            GameObject go = reader.ReadGameObject();
            if (go == null) { ent = null; }
			else { ent = go.GetComponent<Entity>(); }
            return new AbilityCastIdentifier(
                ent,
                (float)reader.ReadDouble(),
                reader.ReadInt32(),
                reader.ReadInt32());
        }
        #endregion

        #region AbilityCastData
        public static void WriteAbilityCastData(this NetworkWriter writer, AbilityCastData castData)
        {
            AbilityCastIdentifier identifier = new AbilityCastIdentifier(
                castData.Caster,
                castData.CastTime,
                castData.AbilityCast.AbilityID,
                castData.MultiCastID);
            //Identifier
            writer.WriteAbilityCastIdentifier(identifier);

            //Resources Impacts
            writer.WriteInt32(castData.BaseResourcesImpacts.Length);
            for (int i = 0; i < castData.BaseResourcesImpacts.Length; i++)
            {
                writer.WriteAbilityResourceImpactData(castData.BaseResourcesImpacts[i]);
            }

            //Vec3
            writer.WriteVector3(castData.BaseKnockback);
            writer.WriteVector3(castData.BaseRecoil);
            writer.WriteVector3(castData.CastDirection);
            writer.WriteVector3(castData.Origin);

            if (_storedAbilityCastData.ContainsKey(identifier))
            {
                _storedAbilityCastData[identifier] = castData;
            }
            else
            {
                _storedAbilityCastData.Add(identifier, castData);
            }
        }
        private static void ReadAbilityCastDataBody(NetworkReader reader, AbilityCastData castDataToFill)
        {
            //Resources Impacts
            int baseResourcesImpactLength = reader.ReadInt32();
            AbilityResourceImpactData[] impactsData = new AbilityResourceImpactData[baseResourcesImpactLength];
            for (int i = 0; i < impactsData.Length; i++)
            {
                impactsData[i] = reader.ReadAbilityResourceImpactData();
            }

            castDataToFill.FillBase(
                impactsData,
                //Vec3
                reader.ReadVector3(),
                reader.ReadVector3());
            castDataToFill.FillCastDirection(reader.ReadVector3());
            castDataToFill.FillOrigin(reader.ReadVector3());
        }
        public static AbilityCastData ReadAbilityCastData(this NetworkReader reader)
        {
            AbilityCastIdentifier identifier = ReadAbilityCastIdentifier(reader);
            if (_storedAbilityCastData.ContainsKey(identifier))
            {
                ReadAbilityCastDataBody(reader, _storedAbilityCastData[identifier]);
                return _storedAbilityCastData[identifier];
            }
            else
            {
                AbilityCastData castData = new AbilityCastData(
                    identifier.Caster,
                    (AbilityData)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.Ability, identifier.AbilityID),
                    identifier.CastTime,
                    identifier.MultiCastID);

                ReadAbilityCastDataBody(reader, castData);

                _storedAbilityCastData.Add(identifier, castData);
                return castData;
            }
        }
        #endregion

        #region AbilityCastResult
        public static void WriteAbilityCastResult(this NetworkWriter writer, AbilityCastResult result)
        {
            //CastData
            writer.WriteAbilityCastData(result.CastData);

            //Main
            writer.WriteInt32((int)result.ResultType);
            writer.WriteGameObject(result.Victim.gameObject);
            writer.WriteVector3(result.Impact);
            writer.WriteVector3(result.KnockbackApplied);

            //Resources Changes
            AbilityResourceImpactData[] resourceChanges = result.ResourcesChanges;
            writer.WriteInt32(resourceChanges.Length);
            for (int i = 0; i < resourceChanges.Length; i++)
            {
                writer.WriteAbilityResourceImpactData(resourceChanges[i]);
            }
        }
        public static AbilityCastResult ReadAbilityCastResult(this NetworkReader reader)
        {
            //CastData
            AbilityCastData castData;
            AbilityCastIdentifier identifier = ReadAbilityCastIdentifier(reader);
            if (_storedAbilityCastData.ContainsKey(identifier))
            {
                castData = _storedAbilityCastData[identifier];
            }
            else
            {
                castData = new AbilityCastData(
                    identifier.Caster,
                    (AbilityData)PinouApp.Resources.Data.Databases.GetItem(DatabaseType.Ability, identifier.AbilityID),
                    identifier.CastTime,
                    identifier.MultiCastID);
                _storedAbilityCastData.Add(identifier, castData);
            }

            ReadAbilityCastDataBody(reader, castData);

            AbilityCastResult result = new AbilityCastResult(castData);

            //Main
            _ = reader.ReadInt32();//Discards ResultType
            result.FillVictim(reader.ReadGameObject().GetComponent<Entity>());
            result.FillImpact(reader.ReadVector3());
            result.FillKnockbackApplied(reader.ReadVector3());

            //Resources Changes
            int resourceChangesLength = reader.ReadInt32();
            AbilityResourceImpactData[] resourceChanges = new AbilityResourceImpactData[resourceChangesLength];
            for (int i = 0; i < resourceChangesLength; i++)
            {
                resourceChanges[i] = reader.ReadAbilityResourceImpactData();
            }
            result.SetResourceChanges(resourceChanges);

            return result;
        }
        #endregion
    }
}