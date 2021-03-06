﻿#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pinou.EntitySystem
{
	[System.Serializable]
	public class AbilityResourceImpactData
	{
		public AbilityResourceImpactData(EntityBeingResourceType resourceType, float change)
		{
			_resourceType = resourceType;
			_resourceChange = change;
		}

		[SerializeField, EnumPaging, ValidateInput("ValidateResourceType")] private EntityBeingResourceType _resourceType;
		[SerializeField] private float _resourceChange = 0f;

		private bool ValidateResourceType(EntityBeingResourceType resourceType)
		{
			if (PinouUtils.Maths.Pow2toIndex((int)resourceType) == -1)
			{
				_resourceType = 0;
			}

			return true;
		}

		public EntityBeingResourceType ResourceType => _resourceType;
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
		public float GetResourceImpact(EntityBeingResourceType type)
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
	public class AbilityTriggerData
	{
		public AbilityTriggerData(AbilityTriggerMethod method, float editorPrecision, int burstCount, float burstPeriod, int multiCastCount)
		{
			_triggerMethod = method;
			_precision = editorPrecision;
			_burstCount = burstCount;
			_burstPeriod = burstPeriod;
			_multiCastCount = multiCastCount;
			ValidatePrecision(_precision);
		}
		[SerializeField] private AbilityTriggerMethod _triggerMethod = AbilityTriggerMethod.Single;
		[SerializeField, Range(0f, 1f), ValidateInput("ValidatePrecision")] private float _precision = 1f;
		[SerializeField, HideInInspector] private float _tweakedPrecision = 1f;
		[SerializeField, ReadOnly] private float _precisionAngle = 1f;
		[SerializeField, ShowIf("_triggerMethod", AbilityTriggerMethod.Burst), Min(1)] private int _burstCount;
		[SerializeField, ShowIf("_triggerMethod", AbilityTriggerMethod.Burst), Min(0.001f)] private float _burstPeriod = 0.05f;
		[SerializeField, Min(1)] private int _multiCastCount = 1;

		public AbilityTriggerMethod TriggerMethod => _triggerMethod;
		public float EditorPrecision => _precision;
		public float Precision => _tweakedPrecision;
		public int BurstCount => _burstCount;
		public float BurstPeriod => _burstPeriod;
		public int MultiCastCount => _multiCastCount;

		private bool ValidatePrecision(float p)
		{
			_tweakedPrecision = Mathf.Pow(_precision, 0.15f);
			_precisionAngle = (1 - _tweakedPrecision) * 360f;
			return true;
		}
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
		public float Cooldown => _cooldown;
	}

	[System.Serializable]
	public class AbilityHitboxData
	{
		[SerializeField] private HitboxType _type;

		[SerializeField, ShowIf("_type", HitboxType.Sphere), Min(0.0001f)] private float _radius;
		[SerializeField, ShowIf("_type", HitboxType.Box)] private Vector3 _size;
		[SerializeField, ShowIf("_type", HitboxType.Box)] private Vector3 _orientation;
		[SerializeField] private Vector3 _offset;
		[SerializeField] private bool _randomSpeed = false;
		[SerializeField, Min(0f)] private float _moveSpeed;
		[SerializeField, MinValue("@_moveSpeed"), ShowIf("_randomSpeed")] private float _maxMoveSpeed;
		[Space]
		[SerializeField] private bool _unlimitedLifeSpan = false;
		[SerializeField, ShowIf("@_unlimitedLifeSpan == false"), Min(0f)] private float _lifeSpan = 0f;
		[Space]
		[SerializeField, EnumToggleButtons] private AbilityTargets _possibleTargets = AbilityTargets.Enemies;
		[SerializeField, Min(0f)] private float _sameTargetHitPeriod = 0.5f;
		[SerializeField, Min(1)] private int _maxTargetHit = 1;

		public HitboxType Type => _type;
		public float Radius => _radius;
		public Vector3 Size => _size;
		public Vector3 Orientation => _orientation;
		public Vector3 Offset => _offset;

		public bool RandomSpeed => _randomSpeed;
		public float MoveSpeed => _moveSpeed;
		public float MaxMoveSpeed => _maxMoveSpeed;

		public bool UnlimitedLifeSpan => _unlimitedLifeSpan;
		public float LifeSpan => _lifeSpan;

		public AbilityTargets PossibleTargets => _possibleTargets;
		public bool CanHitSelf => (_possibleTargets & AbilityTargets.Self) != 0;
		public bool CanHitAllies => (_possibleTargets & AbilityTargets.Allies) != 0;
		public bool CanHitEnemies => (_possibleTargets & AbilityTargets.Enemies) != 0;
		public float SameTargetHitPeriod => _sameTargetHitPeriod;
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
			[SerializeField] private VisualManipulator.VisualParameters _fxVisualParameters;
			[SerializeField] private AbilityVisualFXTimingMethod _timingMethod;
			[SerializeField] private AbilityVisualFXPlacingMethod _placingMethod;
			[SerializeField, ShowIf("@_placingMethod != AbilityVisualFXPlacingMethod.Script")] private AbilityResultDirectionMethod _directionMethod;
			[SerializeField] private AbilityVisualFXDestroyMethod _destroyMethod;
			[SerializeField, ShowIf("_destroyMethod", AbilityVisualFXDestroyMethod.Timed)] private float _destroyTime;

			public GameObject Model => _model;
			public VisualManipulator.VisualParameters FXVisualParameters => _fxVisualParameters;
			public AbilityVisualFXTimingMethod TimingMethod => _timingMethod;
			public AbilityVisualFXPlacingMethod PlacingMethod => _placingMethod;
			public AbilityResultDirectionMethod DirectionMethod => _directionMethod;
			public AbilityVisualFXDestroyMethod DestroyMethod => _destroyMethod;
			public float DestroyTime => _destroyTime;
        }
		[SerializeField] private GameObject _hitboxVisualModel;
		[SerializeField] private VisualManipulator.VisualParameters _hitboxVisualParameters;
		[SerializeField, ShowIf("@_hitboxVisualModel != null")] private float _hitboxVisualModelRelativeLifeSpan = 0f;
		[SerializeField] private AbilityVisualFX[] _FXs;

		public bool HasFX => _FXs.Length > 0;
		public AbilityVisualFX[] FXs => _FXs;

		public GameObject HitboxVisualModel => _hitboxVisualModel;
		public VisualManipulator.VisualParameters HitboxVisualParameters => _hitboxVisualParameters;
		public float HitboxVisualLifeSpan => _hitboxVisualModelRelativeLifeSpan;
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
		[SerializeField, ShowIf("@_abilityData == null ? false : _abilityData._type == AbilityType.Projectile")] private ProjectileImpactMethod _projectileImpactMethod = ProjectileImpactMethod.Pierce;
		[SerializeField, HideInInspector] private AbilityData _abilityData;

		public DirectionMethod DirectionMethod => _directionMethod;
		public RecoilCondition RecoilCondition => _recoilCondition;
		public KnockbackMethod KnockbackMethod => _knockbackMethod;
		public AbilityCastOrigin CastOrigin => _castOrigin;
		public AbilityCastOriginTiming CastOriginTiming => _castOriginTiming;
		public AbilityImpactMethod ImpactMethod => _impactMethod;
		public ProjectileImpactMethod ProjectileImpactMethod => _projectileImpactMethod;
	}
}