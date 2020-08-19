#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pinou.EntitySystem
{
	[System.Serializable]
	public class AbilityResourceImpactData
	{
		public AbilityResourceImpactData(EntityBeingResourcesType resourceType, float change)
		{
			_resourceType = resourceType;
			_resourceChange = change;
		}

		[SerializeField] private EntityBeingResourcesType _resourceType;
		[SerializeField] private float _resourceChange = 0f;

		public EntityBeingResourcesType ResourceType => _resourceType;
		public float ResourceChange => _resourceChange;

		public void SetResourceChange(float change)
		{
			_resourceChange = change;
		}
	}

	[System.Serializable]
	public class AbilityMainData
	{
		[SerializeField] private AbilityResourceImpactData[] _baseResourcesImpacts = new AbilityResourceImpactData[] { };
		[Space]
		[SerializeField] private Vector3 _baseKnockback = Vector3.zero;
		[SerializeField] private Vector3 _baseRecoil = Vector3.zero;
		[Space]
		[SerializeField] private bool _rotateOnCast = true;
		[SerializeField] private bool _canRotateDuringCast = false;

		public AbilityResourceImpactData[] BaseResourcesImpacts => _baseResourcesImpacts;
		public float GetResourceImpact(EntityBeingResourcesType type)
		{
			for (int i = 0; i < _baseResourcesImpacts.Length; i++)
			{
				if (_baseResourcesImpacts[i].ResourceType == type) { return _baseResourcesImpacts[i].ResourceChange; }
			}
			return 0f;
		}

		public Vector3 BaseKnockback => _baseKnockback;
		public Vector3 BaseRecoil => _baseRecoil;
		public bool RotateOnCast => _rotateOnCast;
		public bool CanRotateDuringCast => _canRotateDuringCast;
	}


	[System.Serializable]
	public class AbilityTimingData
	{
		[SerializeField] private bool _instant = false;
		[SerializeField, ShowIf("@_instant == false"), Min(0f)] private float _castDuration = 0f;
		[SerializeField, ShowIf("@_instant == false"), Min(0f)] private float _performDuration = 0f;
		[SerializeField, ShowIf("@_instant == false"), Min(0f)] private float _hardRecoverDuration = 0f;
		[SerializeField, ShowIf("@_instant == false"), Min(0f)] private float _softRecoverDuration = 0f;
		[Space]
		[SerializeField, Min(0f)] private float _cooldown = 0f;

		public bool Instant => _instant;
		public float CastDuration => _castDuration;
		public float PerformDuration => _performDuration;
		public float HardRecoverDuration => _hardRecoverDuration;
		public float SoftRecoverDuration => _softRecoverDuration;
	}

	[System.Serializable]
	public class AbilityHitboxData
	{
		[SerializeField] private HitboxType _type;

		[SerializeField, ShowIf("_type", HitboxType.Sphere), Min(0.0001f)] private float _radius;
		[SerializeField, ShowIf("_type", HitboxType.Box)] private Vector3 _size;
		[SerializeField, ShowIf("_type", HitboxType.Box)] private Vector3 _orientation;
		[SerializeField] private Vector3 _offset;
		[Space]
		[SerializeField] private bool _unlimitedLifeSpan = false;
		[SerializeField, ShowIf("@_unlimitedLifeSpan == false"), Min(0f)] private float _lifeSpan = 0f;
		[Space]
		[SerializeField, EnumToggleButtons] private AbilityTargets _possibleTargets = AbilityTargets.Enemies;
		[SerializeField, Min(1)] private int _maxTargetHit = 1;

		public HitboxType Type => _type;
		public float Radius => _radius;
		public Vector3 Size => _size;
		public Vector3 Orientation => _orientation;
		public Vector3 Offset => _offset;

		public bool UnlimitedLifeSpan => _unlimitedLifeSpan;
		public float LifeSpan => _lifeSpan;

		public AbilityTargets PossibleTargets => _possibleTargets;
		public bool CanHitSelf => (_possibleTargets & AbilityTargets.Self) != 0;
		public bool CanHitAllies => (_possibleTargets & AbilityTargets.Allies) != 0;
		public bool CanHitEnemies => (_possibleTargets & AbilityTargets.Enemies) != 0;
		public int MaxTargetHit => _maxTargetHit;
	}

	[System.Serializable]
	public class AbilityVisualData
    {
		public enum AbilityVisualFXTimingMethod
		{
			Cast,
			Result
		}
		public enum AbilityVisualFXPlacingMethod
        {
			Impact,
			Caster,
			Victim,
			Script
        }
		public enum AbilityVisualFXDestroyMethod
		{
			Timed,
			Script
		}
		public enum AbilityResultDirectionMethod
        {
			CastDirection,
			KnockbackDirection
        }
		[System.Serializable]
		public class AbilityVisualFX
        {
			[SerializeField] private GameObject _model;
			[SerializeField] private AbilityVisualFXTimingMethod _timingMethod;
			[SerializeField] private AbilityVisualFXPlacingMethod _placingMethod;
			[SerializeField, ShowIf("@_placingMethod != AbilityVisualFXPlacingMethod.Script")] private AbilityResultDirectionMethod _directionMethod;
			[SerializeField] private AbilityVisualFXDestroyMethod _destroyMethod;
			[SerializeField, ShowIf("_destroyMethod", AbilityVisualFXDestroyMethod.Timed)] private float _destroyTime;

			public GameObject Model => _model;
			public AbilityVisualFXTimingMethod TimingMethod => _timingMethod;
			public AbilityVisualFXPlacingMethod PlacingMethod => _placingMethod;
			public AbilityResultDirectionMethod DirectionMethod => _directionMethod;
			public AbilityVisualFXDestroyMethod DestroyMethod => _destroyMethod;
			public float DestroyTime => _destroyTime;
        }
		[SerializeField] private AbilityVisualFX[] _FXs;

		public bool HasFX => _FXs.Length > 0;
		public AbilityVisualFX[] FXs => _FXs;
    }

	[System.Serializable]
	public class AbilityMethodsData
	{
		[SerializeField] private DirectionMethod _directionMethod = DirectionMethod.AimBased;
		[SerializeField] private RecoilCondition _recoilCondition = RecoilCondition.OnCast;
		[SerializeField] private KnockbackMethod _knockbackMethod = KnockbackMethod.AttackBased;
		[SerializeField] private AbilityCastOrigin _castOrigin = AbilityCastOrigin.Caster;
		[SerializeField] private AbilityCastOriginTiming _castOriginTiming = AbilityCastOriginTiming.CastEntrance;
		[SerializeField] private AbilityImpactMethod _impactMethod = AbilityImpactMethod.VictimCenter;

		public DirectionMethod DirectionMethod => _directionMethod;
		public RecoilCondition RecoilCondition => _recoilCondition;
		public KnockbackMethod KnockbackMethod => _knockbackMethod;
		public AbilityCastOrigin CastOrigin => _castOrigin;
		public AbilityCastOriginTiming CastOriginTiming => _castOriginTiming;
		public AbilityImpactMethod ImpactMethod => _impactMethod;
	}
}