using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class AbilityPerformer
	{
		#region Data Class
        public class AdditionalHitInfos
		{
            public AdditionalHitInfos(Vector3 rayOrigin, Vector3 rayDirection, float distance, RaycastHit hit)
			{
                _rayOrigin = rayOrigin;
                _rayDirection = rayDirection;
                _distance = distance;
                _hit = hit;
			}
            private Vector3 _rayOrigin;
            private Vector3 _rayDirection;
            private float _distance;
            private RaycastHit _hit;

            public Vector3 RayOrigin => _rayOrigin;
            public Vector3 RayDirection => _rayDirection;
            public float Distance => _distance;
            public RaycastHit Hit => _hit;

            public AdditionalHitInfos CopyWithHit(RaycastHit hit)
			{
                return new AdditionalHitInfos(_rayOrigin, _rayDirection, _distance, hit);
			}
        }
		#endregion
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
                    ComputeStaticHitboxHitEntities(castData, castData.Origin + castData.AbilityCast.Hitbox.Offset));
            }
        }
        #endregion

        #region Utilities
        public static AbilityHitbox HandleSpawnAbilityHitbox(AbilityCastData castData)
        {
            GameObject hitBoxGo = new GameObject();
            hitBoxGo.name = castData.Caster.EntityName + "'s Ability: " + castData.AbilityCast.Name;
            hitBoxGo.transform.SetParent(PinouApp.Scene.ActiveSceneInfos.AbilitiesHolder);
            hitBoxGo.transform.position = castData.Origin + castData.AbilityCast.Hitbox.Offset;
            hitBoxGo.transform.rotation = PinouUtils.Quaternion.SafeLookRotation(castData.CastDirection, Vector3.right);
            AbilityHitbox hitBox = hitBoxGo.AddComponent<AbilityHitbox>();
            hitBox.BuildHitbox(castData);
            castData.Caster.Abilities.OnBuildAbilityHitbox.Invoke(castData.Caster, hitBox);
            return hitBox;
        }
        public static Entity[] ComputeStaticHitboxHitEntities(AbilityCastData castData, Vector3 hitBoxOrigin, ICollection<Entity> toIgnore = null)
        {
            AbilityHitboxData hParams = castData.AbilityCast.Hitbox;
            List<Entity> hitEntities = new List<Entity>();
            List<float> hitEntitiesDst = new List<float>();

            #region Detect all entities hit
            Collider[] entitiesHit = null;
            if (hParams.Type == HitboxType.Sphere)
            {
                entitiesHit = Physics.OverlapSphere(hitBoxOrigin, hParams.Radius, PinouApp.Resources.Data.Layers.Entities);
            }
            else if (hParams.Type == HitboxType.Box)
            {
                entitiesHit = Physics.OverlapBox(hitBoxOrigin, hParams.Size * 0.5f, Quaternion.Euler(hParams.Orientation), PinouApp.Resources.Data.Layers.Entities);
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
                    float dst = Vector3.SqrMagnitude(collMaster.Position - hitBoxOrigin);
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
        public static (Entity[], AdditionalHitInfos[]) ComputeMovingHitboxHitEntities(AbilityCastData castData, Vector3 hitBoxOrigin, ICollection<Entity> toIgnore = null)
        {
            AbilityHitboxData hParams = castData.AbilityCast.Hitbox;
            List<Entity> hitEntities = new List<Entity>();
            List<float> hitEntitiesDst = new List<float>();
            List<AdditionalHitInfos> hitEntitiesInfos = new List<AdditionalHitInfos>();
            Dictionary<Collider, AdditionalHitInfos> colliderHitInfos = new Dictionary<Collider, AdditionalHitInfos>();

            #region Detect all entities hit
            Collider[] entitiesHit = null;
            AdditionalHitInfos baseHitInfos = new AdditionalHitInfos(hitBoxOrigin, castData.CastDirection, hParams.MoveSpeed * Time.fixedDeltaTime, default);
            #region Static Hitboxes
            #endregion
            #region Moving Hitboxes
            if (hParams.Type == HitboxType.Sphere)
            {
                RaycastHit[] hits = Physics.SphereCastAll(
                    baseHitInfos.RayOrigin,
                    hParams.Radius,
                    baseHitInfos.RayDirection,
                    baseHitInfos.Distance,
                    PinouApp.Resources.Data.Layers.Entities);
                entitiesHit = new Collider[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                {
                    entitiesHit[i] = hits[i].collider;
                    colliderHitInfos.Add(hits[i].collider, baseHitInfos.CopyWithHit(hits[i]));
                }
            }
            else if (hParams.Type == HitboxType.Box)
            {
                RaycastHit[] hits = Physics.BoxCastAll(
                    baseHitInfos.RayOrigin,
                    hParams.Size * 0.5f,
                    baseHitInfos.RayDirection,
                    Quaternion.Euler(hParams.Orientation),
                    baseHitInfos.Distance,
                    PinouApp.Resources.Data.Layers.Entities);

                entitiesHit = new Collider[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                {
                    entitiesHit[i] = hits[i].collider;
                    colliderHitInfos.Add(hits[i].collider, baseHitInfos.CopyWithHit(hits[i]));
                }
            }
            #endregion
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
                    float dst = Vector3.SqrMagnitude(collMaster.Position - hitBoxOrigin);
                    if (hitEntities.Count == 0)
                    {
                        hitEntities.Add(collMaster);
                        hitEntitiesDst.Add(dst);
                        hitEntitiesInfos.Add(colliderHitInfos[coll]);
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
                                hitEntitiesInfos.Add(colliderHitInfos[coll]);
                                break;
                            }
                            else if (i == 0)
                            {
                                hitEntities.Insert(0, collMaster);
                                hitEntitiesDst.Insert(0, dst);
                                hitEntitiesInfos.Add(colliderHitInfos[coll]);
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

            return (hitEntities.ToArray(), hitEntitiesInfos.ToArray());
        }
        public static void ApplyAbilityPerformedOnHitEntities(AbilityCastData castData, ICollection<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                ApplyAbilityPerformedOnHitEntity(castData, entity);
            }
        }
        public static void ApplyAbilityPerformedOnHitEntity(AbilityCastData castData, Entity entity, AbilityCastResult existingResult = null)
		{
            AbilityCastResult result = existingResult == null ? new AbilityCastResult(castData) : existingResult;
            result.FillVictim(entity);
            if (result.ImpactFilled == false)
            {
                result.FillImpact(ComputePerformedAbilityImpact(castData, entity));
            }

            entity.ReceiveAbilityHit(result);
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
                case AbilityImpactMethod.ProjectileImpact:
                    throw new System.Exception("This line can not be called, projectiles' impacts are calculated earlier.");
            }

            throw new System.Exception("Enum AbilityImapctMethod changed. Require update here.");
        }
        public static bool CanPerformedAbilityHitEntity(AbilityCastData castData, Entity entity)
        {
            if (castData.AbilityCast.Hitbox.CanHitSelf == false && entity == castData.Caster)
            {
                return false;
            }
            else
            {
                if (castData.Caster.Team == EntityTeam.Players || castData.Caster.Team == EntityTeam.Allies)
				{
                    if (castData.AbilityCast.Hitbox.CanHitAllies == false && (entity.Team == EntityTeam.Allies || entity.Team == EntityTeam.Players))
					{
                        return false;
					}
                    if (castData.AbilityCast.Hitbox.CanHitEnemies == false && entity.Team == EntityTeam.Enemies)
					{
                        return false;
					}
				}
                else
				{
                    if (castData.AbilityCast.Hitbox.CanHitAllies == false && entity.Team == EntityTeam.Enemies)
                    {
                        return false;
                    }
                    if (castData.AbilityCast.Hitbox.CanHitEnemies == false && (entity.Team == EntityTeam.Players || entity.Team == EntityTeam.Allies))
                    {
                        return false;
                    }
                }
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
                        fxGo.transform.rotation = PinouUtils.Quaternion.SafeLookRotation(castData.CastDirection, Vector3.right);
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
                PinouUtils.Coroutine.Invoke(PinouApp.Pooler.Store, castFx.DestroyTime, fxGo, true);
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
            GameObject fxGo = PinouApp.Pooler.Retrieve(fx.Model, PinouApp.Scene.ActiveSceneInfos.FXsHolder);
            HandleAbilityPlacingMethod(fxGo, castData, fx);
            HandleAbilityDirectionMethod(fxGo, castData, fx);
            HandleAbilityDestroyMethod(fxGo, castData, fx);
            fx.FXVisualParameters.Apply(fxGo);
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
            GameObject fxGo = PinouApp.Pooler.Retrieve(fx.Model, PinouApp.Scene.ActiveSceneInfos.FXsHolder);
            HandleAbilityPlacingMethod(fxGo, result, fx);
            HandleAbilityDirectionMethod(fxGo, result, fx);
            HandleAbilityDestroyMethod(fxGo, result.CastData, fx);
            fx.FXVisualParameters.Apply(fxGo);
        }
        #endregion

        #endregion
    }
}