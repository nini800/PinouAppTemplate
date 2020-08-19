using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class AbilityPerformer
	{
        #region Commands
        public static void PerformAbility(AbilityCastData castData)
        {
            if (castData.AbilityCast.Hitbox.UnlimitedLifeSpan == true || castData.AbilityCast.Hitbox.LifeSpan > 0)
            {
                HandleSpawnAbilityHitbox(castData);
            }
            else
            {
                ApplyAbilityPerformedOnHitEntities(castData,
                    ComputeHitboxHitEntities(castData));
            }
        }
        #endregion

        #region Utilities
        private static void HandleSpawnAbilityHitbox(AbilityCastData castData)
        {

        }
        private static Entity[] ComputeHitboxHitEntities(AbilityCastData castData, ICollection<Entity> toIgnore = null)
        {
            AbilityHitboxData hParams = castData.AbilityCast.Hitbox;
            List<Entity> hitEntities = new List<Entity>();
            List<float> hitEntitiesDst = new List<float>();

            #region Detect all entities hit
            Collider[] entitiesHit = null;
            if (hParams.Type == HitboxType.Sphere)
            {
                entitiesHit = Physics.OverlapSphere(castData.Origin, hParams.Radius, PinouApp.Resources.Data.Layers.Entities);
            }
            else if (hParams.Type == HitboxType.Box)
            {
                entitiesHit = Physics.OverlapBox(castData.Origin, hParams.Size * 0.5f, Quaternion.Euler(hParams.Orientation), PinouApp.Resources.Data.Layers.Entities);
            }
            #endregion

            #region Sort entities
            //Sort allowed entities and store them
            foreach (Collider coll in entitiesHit)
            {
                Entity collMaster = coll.GetComponentInParent<Entity>();
                if (hitEntities.Contains(collMaster) == false &&
                    (toIgnore == null || toIgnore.Contains(collMaster) == false) &&
                    CanPerformedAbilityHitEntity(castData, collMaster) == true)
                {
                    float dst = Vector3.SqrMagnitude(collMaster.Position - castData.Origin);
                    if (hitEntities.Count == 0)
                    {
                        hitEntities.Add(collMaster);
                        hitEntitiesDst.Add(dst);
                    }
                    else
                    {
                        //Order the hit entity directly by distance
                        for (int i = hitEntities.Count - 1; i >= 0; i--)
                        {
                            if (dst > (hitEntitiesDst[i]))
                            {
                                hitEntities.Insert(i + 1, collMaster);
                                hitEntitiesDst.Insert(i + 1, dst);
                                break;
                            }
                            else if (i == 0)
                            {
                                hitEntities.Insert(0, collMaster);
                                hitEntitiesDst.Insert(0, dst);
                                break;
                            }
                        }
                    }
                }
            }

            //Keep only nearest entities and right count
            if (hitEntities.Count > hParams.MaxTargetHit)
            {
                hitEntities.RemoveRange(hParams.MaxTargetHit, hitEntities.Count - hParams.MaxTargetHit);
            }
            #endregion

            return hitEntities.ToArray();
        }
        private static void ApplyAbilityPerformedOnHitEntities(AbilityCastData castData, ICollection<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                AbilityCastResult result = new AbilityCastResult(castData);
                result.FillVictim(entity);
                result.FillImpact(ComputePerformedAbilityImpact(castData, entity));

                entity.ReceiveAbilityHit(result);
            }
        }
        private static Vector3 ComputePerformedAbilityImpact(AbilityCastData castData, Entity entity)
        {
            switch (castData.AbilityCast.Methods.ImpactMethod)
            {
                case AbilityImpactMethod.VictimCenter:
                    return entity.Position;
                case AbilityImpactMethod.CastOrigin:
                    return castData.Origin;
                case AbilityImpactMethod.OriginVictimCenterAverage:
                    return (entity.Position + castData.Origin) * 0.5f;
                case AbilityImpactMethod.RayCastTowardVictim:
                    throw new System.NotImplementedException();
            }

            throw new System.Exception("Enum AbilityImapctMethod changed. Require update here.");
        }
        public static bool CanPerformedAbilityHitEntity(AbilityCastData castData, Entity entity)
        {
            if (castData.AbilityCast.Hitbox.CanHitSelf == false && entity == castData.Caster ||
                castData.AbilityCast.Hitbox.CanHitAllies == false && 
                    ((entity.Team == EntityTeam.Players && entity != castData.Caster) ||
                    entity.Team == EntityTeam.Allies) ||
                castData.AbilityCast.Hitbox.CanHitEnemies == false && entity.Team == EntityTeam.Enemies)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Ability Visual Handling
        private static void HandleAbilityPlacingMethod(GameObject fxGo, AbilityCastData castData, AbilityVisualData.AbilityVisualFX castFx)
		{
            switch (castFx.PlacingMethod)
            {
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Caster:
                    fxGo.transform.position = castData.Caster.Position;
                    break;
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Script:
                    fxGo.GetComponent<FX_Base>().StartFX(castData);
                    break;
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Impact:
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Victim:
                    Debug.LogError("Wrong parameter: " + castFx.PlacingMethod + " for a cast FX");
                    break;

            }
        }
        private static void HandleAbilityDirectionMethod(GameObject fxGo, AbilityCastData castData, AbilityVisualData.AbilityVisualFX castFx)
        {
            if (castFx.PlacingMethod != AbilityVisualData.AbilityVisualFXPlacingMethod.Script)
            {
                switch (castFx.DirectionMethod)
                {
                    case AbilityVisualData.AbilityResultDirectionMethod.CastDirection:
                        fxGo.transform.rotation = Quaternion.LookRotation(castData.CastDirection);
                        break;
                    case AbilityVisualData.AbilityResultDirectionMethod.KnockbackDirection:
                        Debug.LogError("Wrong parameter: " + castFx.DirectionMethod + " for a cast FX");
                        break;
                }
            }
        }
        private static void HandleAbilityDestroyMethod(GameObject fxGo, AbilityCastData castData, AbilityVisualData.AbilityVisualFX castFx)
        {
            if (castFx.DestroyMethod == AbilityVisualData.AbilityVisualFXDestroyMethod.Timed)
            {
                Object.Destroy(fxGo, castFx.DestroyTime);
            }
        }
        private static void HandleAbilityPlacingMethod(GameObject fxGo, AbilityCastResult result, AbilityVisualData.AbilityVisualFX resultFx)
        {
            switch (resultFx.PlacingMethod)
            {
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Caster:
                    HandleAbilityPlacingMethod(fxGo, result.CastData, resultFx);
                    break;
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Impact:
                    fxGo.transform.position = result.Impact;
                    break;
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Victim:
                    fxGo.transform.position = result.Victim.Position;
                    break;
                case AbilityVisualData.AbilityVisualFXPlacingMethod.Script:
                    fxGo.GetComponent<FX_Base>().StartFX(result);
                    break;
            }
        }
        private static void HandleAbilityDirectionMethod(GameObject fxGo, AbilityCastResult result, AbilityVisualData.AbilityVisualFX resultFx)
        {
            if (resultFx.PlacingMethod != AbilityVisualData.AbilityVisualFXPlacingMethod.Script)
            {
                switch (resultFx.DirectionMethod)
                {
                    case AbilityVisualData.AbilityResultDirectionMethod.CastDirection:
                        HandleAbilityDirectionMethod(fxGo, result.CastData, resultFx);
                        break;
                    case AbilityVisualData.AbilityResultDirectionMethod.KnockbackDirection:
                        fxGo.transform.rotation = Quaternion.LookRotation(result.KnockbackApplied.normalized);
                        break;
                }
            }
        }
        public static void HandleAbilityCastDataVisual(AbilityCastData castData)
        {
            if (castData.AbilityCast.Visual.HasFX == false)
            {
                return;
            }
            for (int i = 0; i < castData.AbilityCast.Visual.FXs.Length; i++)
            {
                if (castData.AbilityCast.Visual.FXs[i].TimingMethod == AbilityVisualData.AbilityVisualFXTimingMethod.Cast)
                {
                    HandleAbilityCastDataFX(castData, castData.AbilityCast.Visual.FXs[i]);
                }
            }
        }
        public static void HandleAbilityCastDataFX(AbilityCastData castData, AbilityVisualData.AbilityVisualFX fx)
        {
            GameObject fxGo = Object.Instantiate(fx.Model, PinouApp.Scene.ActiveSceneInfos.FXsHolder);
            HandleAbilityPlacingMethod(fxGo, castData, fx);
            HandleAbilityDirectionMethod(fxGo, castData, fx);
            HandleAbilityDestroyMethod(fxGo, castData, fx);
        }

        public static void HandleAbilityCastResultVisual(AbilityCastResult result)
        {
            if (result.CastData.AbilityCast.Visual.HasFX == false)
            {
                return;
            }
            for (int i = 0; i < result.CastData.AbilityCast.Visual.FXs.Length; i++)
            {
                if (result.CastData.AbilityCast.Visual.FXs[i].TimingMethod == AbilityVisualData.AbilityVisualFXTimingMethod.Result)
				{
                    HandleAbilityCastResultFX(result, result.CastData.AbilityCast.Visual.FXs[i]);
                }
            }
        }
        public static void HandleAbilityCastResultFX(AbilityCastResult result, AbilityVisualData.AbilityVisualFX fx)
        {
            GameObject fxGo = Object.Instantiate(fx.Model, PinouApp.Scene.ActiveSceneInfos.FXsHolder);
            HandleAbilityPlacingMethod(fxGo, result, fx);
            HandleAbilityDirectionMethod(fxGo, result, fx);
            HandleAbilityDestroyMethod(fxGo, result.CastData, fx);
        }
		#endregion

        #endregion
    }
}