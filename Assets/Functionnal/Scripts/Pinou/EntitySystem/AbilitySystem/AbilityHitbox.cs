using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class AbilityHitbox : PinouBehaviour
	{
		private static Dictionary<AbilityCastData, AbilityHitbox> s_abilityHitboxes = new Dictionary<AbilityCastData, AbilityHitbox>();
		public static void DestroyFromCastData(AbilityCastData castData)
		{
			if (s_abilityHitboxes.ContainsKey(castData))
			{
				Destroy(s_abilityHitboxes[castData]);
				s_abilityHitboxes.Remove(castData);
			}
		}

		public void BuildHitbox(AbilityCastData castData)
		{
			_castData = castData;
			_abilityData = _castData.AbilityCast;
			_hitboxData = _abilityData.Hitbox;
			_hitboxLife = _hitboxData.MaxTargetHit;

			if (_abilityData.Visual.HitboxVisualModel != null)
			{
				_visualModel = PinouApp.Pooler.Retrieve(_abilityData.Visual.HitboxVisualModel, transform.position, transform.rotation, transform);
				_visualModelStoreCoroutine = 
					PinouUtils.Coroutine.Invoke(
						PinouApp.Pooler.Store,
						castData.AbilityCast.Visual.HitboxVisualLifeSpan + castData.AbilityCast.Hitbox.LifeSpan,
						_visualModel, true);
			}
			Destroy(gameObject, castData.AbilityCast.Hitbox.LifeSpan);

			s_abilityHitboxes.Add(_castData, this);

			_abilityData.Visual.HitboxVisualParameters.Apply(gameObject);
		}

		private Coroutine _visualModelStoreCoroutine = null;
		private AbilityCastData _castData = null;
		private AbilityData _abilityData = null;
		private AbilityHitboxData _hitboxData = null;

		private GameObject _visualModel = null;

		private Dictionary<Entity, float> _hitEntitiesDic = new Dictionary<Entity, float>();

		private int _hitboxLife;
		private bool _visualMode = false;

		public AbilityCastData CastData => _castData;

		public void ActivateVisualMode()
		{
			_visualMode = true;
		}

		private void FixedUpdate()
		{
			if (_visualMode == false)
			{
				if (_hitboxData.MoveSpeed <= 0f)
				{
					HandleStaticHitEntities();
				}
				else
				{
					HandleMovingHitEntities();
				}
			}

			HandleMovements();
		}
		private void HandleStaticHitEntities()
		{
			Entity[] hitEntities;
			if (_hitboxData.SameTargetHitPeriod > 0f)
			{
				hitEntities = AbilityPerformer.ComputeStaticHitboxHitEntities(_castData, transform.position);
				for (int i = 0; i < hitEntities.Length; i++)
				{
					if (_hitEntitiesDic.ContainsKey(hitEntities[i]))
					{
						if (Time.time - _hitEntitiesDic[hitEntities[i]] < _hitboxData.SameTargetHitPeriod)
						{
							continue;
						}
						else
						{
							_hitEntitiesDic[hitEntities[i]] = Time.time;
						}
					}
					else
					{
						_hitEntitiesDic.Add(hitEntities[i], Time.time);
					}

					ApplyHitOnEntity(hitEntities[i], null);
				}
			}
			else
			{
				hitEntities = AbilityPerformer.ComputeStaticHitboxHitEntities(_castData, transform.position, _hitEntitiesDic.Keys);
				for (int i = 0; i < hitEntities.Length; i++)
				{
					if (_hitEntitiesDic.ContainsKey(hitEntities[i])) { continue; }

					_hitEntitiesDic.Add(hitEntities[i], Time.time);
					ApplyHitOnEntity(hitEntities[i], null);
				}
			}
		}
		private void HandleMovingHitEntities()
		{
			Entity[] hitEntities;
			AbilityPerformer.AdditionalHitInfos[] hitEntitiesInfos;
			var everything = AbilityPerformer.ComputeMovingHitboxHitEntities(_castData, transform.position);
			hitEntities = everything.Item1;
			hitEntitiesInfos = everything.Item2;

			if (_hitboxData.SameTargetHitPeriod > 0f)
			{
				for (int i = 0; i < hitEntities.Length; i++)
				{
					if (_hitEntitiesDic.ContainsKey(hitEntities[i]))
					{
						if (Time.time - _hitEntitiesDic[hitEntities[i]] < _hitboxData.SameTargetHitPeriod)
						{
							continue;
						}
						else
						{
							_hitEntitiesDic[hitEntities[i]] = Time.time;
						}
					}
					else
					{
						_hitEntitiesDic.Add(hitEntities[i], Time.time);
					}

					ApplyHitOnEntity(hitEntities[i], hitEntitiesInfos[i]);
				}
			}
			else
			{
				hitEntities = AbilityPerformer.ComputeStaticHitboxHitEntities(_castData, transform.position, _hitEntitiesDic.Keys);
				for (int i = 0; i < hitEntities.Length; i++)
				{
					if (_hitEntitiesDic.ContainsKey(hitEntities[i])) { continue; }

					_hitEntitiesDic.Add(hitEntities[i], Time.time);
					ApplyHitOnEntity(hitEntities[i], hitEntitiesInfos[i]);
				}
			}
		}
		private void ApplyHitOnEntity(Entity ent, AbilityPerformer.AdditionalHitInfos infos)
		{
			AbilityCastResult result = null;
			if (infos != null)
			{
				result = new AbilityCastResult(_castData);
				Vector3 impact = infos.Hit.point;
				if (impact.sqrMagnitude <= Mathf.Epsilon) { impact = infos.RayOrigin; }
				result.FillImpact(impact);
			}
			AbilityPerformer.ApplyAbilityPerformedOnHitEntity(_castData, ent, result);
			HandleCheckHitboxLife();
		}
		private bool HandleCheckHitboxLife()
		{
			_hitboxLife--;
			if (_hitboxLife <= 0)
			{
				Destroy(gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}
		private void HandleMovements()
		{
			transform.position += transform.forward * _hitboxData.MoveSpeed * Time.fixedDeltaTime;
		}

		protected override void OnDestroyed()
		{
			if (_visualModel != null && _visualModel.activeSelf == true)
			{
				if (_abilityData.Visual.HitboxVisualLifeSpan > 0f)
				{
					float estimatedRemainingTime = _abilityData.Visual.HitboxVisualLifeSpan - _abilityData.Hitbox.LifeSpan;
					if (estimatedRemainingTime < 0f && _visualModel != null) 
					{
						PinouUtils.Coroutine.StopCoroutine(ref _visualModelStoreCoroutine);
						PinouApp.Pooler.Store(_visualModel);
					}
					else
					{
						PinouUtils.Coroutine.StopCoroutine(ref _visualModelStoreCoroutine);
						_visualModel.transform.SetParent(PinouApp.Scene.ActiveSceneInfos.FXsHolder);
						PinouUtils.Coroutine.Invoke(
							PinouApp.Pooler.Store,
							estimatedRemainingTime,
							_visualModel, true);
					}
				}
				else
				{
					PinouUtils.Coroutine.StopCoroutine(ref _visualModelStoreCoroutine);
					PinouApp.Pooler.Store(_visualModel);
				}
			}
			OnHitboxDestroyed.Invoke(this);
		}


		public PinouUtils.Delegate.Action<AbilityHitbox> OnHitboxDestroyed { get; private set; } = new PinouUtils.Delegate.Action<AbilityHitbox>();
	}
}