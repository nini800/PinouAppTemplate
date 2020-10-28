#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.ItemSystem;

namespace Pinou.EntitySystem
{
    public class EntityAbilitiesData : EntityComponentData
    {
		#region Serialized Data Classes
		[System.Serializable]
        public class OnLevelUpAbility
		{
            [SerializeField] private EntityStatsLevelType _levelTypeToLink;
            [SerializeField] private AbilityData _ability;

            public EntityStatsLevelType LevelTypeToLink => _levelTypeToLink;
            public AbilityData Ability => _ability;
		}

        [System.Serializable]
        public class EntityEquipedAbility
        {
            [SerializeField, ReadOnly] private int _ID = -1;
            [SerializeField] private string _IDName;
            [SerializeField] private AbilityData _abilityData;

            public int ID => _ID;
            public string IDName => _IDName;
            public AbilityData AbilityData => _abilityData;

            #if UNITY_EDITOR
            public void E_UpdateID(int id)
            {
                _ID = id;
                _IDName = "Ability_" + id;
            }
            #endif
        }
		#endregion

		#region Vars, Fields, Getters
		[SerializeField] protected List<EntityEquipedAbility> equipedAbilities;
        [SerializeField] protected OnLevelUpAbility[] levelUpAbilities;

        public OnLevelUpAbility[] LevelUpAbilities => levelUpAbilities;
        #endregion

        #region Utilities
        public EntityEquipedAbility GetAbility(int abilityID)
        {
            if (abilityID < 0 | abilityID >= equipedAbilities.Count) { return null; }
            return equipedAbilities[abilityID];
        }
        public EntityEquipedAbility GetAbility(string abilityIDName)
        {
            for (int i = 0; i < equipedAbilities.Count; i++)
            {
                if (equipedAbilities[i].IDName == abilityIDName)
                {
                    return equipedAbilities[i];
                }
            }
            return null;
        }
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityAbilities, EntityAbilitiesData>(master, references, this);
        }
        #endregion

        #region Editor
        #if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < equipedAbilities.Count; i++)
            {
                if (equipedAbilities[i].ID != i)
                {
                    equipedAbilities[i].E_UpdateID(i);
                }
            }
        }
        #endif
        #endregion

        public class EntityAbilities : EntityComponent
        {
			#region Instance Data Classes
            public class AbilityCastInfos
			{
                public AbilityCastInfos(IAbilityContainer container)
				{
                    if (container is EntityEquipable eq)
					{
                        _equipable = eq;
                    }

                    _ability = container.Ability;
                    _lastCastTime = Time.time;
				}

                private EntityEquipable _equipable;
                private AbilityData _ability;
                private float _lastCastTime;

                public EntityEquipable Equipable => _equipable;
                public AbilityData Ability => _ability;
                public float LastCastTime => _lastCastTime;

                public void Cast()
				{
                    _lastCastTime = Time.time;
				}
			}
			#endregion
			#region OnConstruct
			/// <summary>
			/// Need base
			/// </summary>
			protected override void OnConstruct()
            {
                _data = (EntityAbilitiesData)((EntityComponent)this).Data;
            }

            private EntityAbilitiesData _data = null;
            protected new EntityAbilitiesData Data => _data;
            #endregion

            public override AbilityState AbilityState => _currentAbilityState;
            #region Vars, Getters
            private AbilityState _currentAbilityState = AbilityState.None;
            private Coroutine _abilityCastTimingsCoroutine = null;
            private AbilityCastData _activeCastData = null;

            private Dictionary<IAbilityContainer, AbilityCastInfos> _abilityCastInfos = new Dictionary<IAbilityContainer, AbilityCastInfos>();

            public float GetAbilityCooldownProgress(IAbilityContainer ability)
			{
                if (_abilityCastInfos.ContainsKey(ability))
				{
                    return Mathf.Clamp(Time.time - _abilityCastInfos[ability].LastCastTime / ability.AbilityCooldown, 0f, 1f);
				}
                else
				{
                    return 1f;
				}
			}
            #endregion

            #region Behaviour
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveStart()
			{
                HandleLevelUpAbilitySubscribe();
                HandleSubscribeDestroyHitbox();
            }
            private void HandleLevelUpAbilitySubscribe()
			{
                if (HasStats == false) { return; }
				foreach (OnLevelUpAbility onLevelUpAbility in _data.levelUpAbilities)
				{
                    Stats.GetLevelExperienceData(onLevelUpAbility.LevelTypeToLink)?.OnLevelUp.SafeSubscribe(OnLevelUp, new GenericDataHolder(onLevelUpAbility.Ability));
				}
			}
            private void HandleSubscribeDestroyHitbox()
			{
                OnBuildAbilityHitbox.Subscribe(
                    delegate (Entity ent, AbilityHitbox hitboxBuilt)
                    {
                        hitboxBuilt.OnHitboxDestroyed.SafeSubscribe(
                            delegate (AbilityHitbox hitboxDestroyed)
                            {
                                OnDestroyAbilityHitbox.Invoke(master, hitboxDestroyed);
                            });
                    });
            }
			private void OnLevelUp(EntityStatsData.LevelExperienceData levelData, int newLevel, GenericDataHolder dataHolder)
			{
                AbilityData ability = (AbilityData)dataHolder.GenericData;
                CastAbility(ability);
			}

			#endregion

			#region Utilities
			#region Ability Casting
            protected void TryTriggerAbility(IAbilityContainer container)
			{
                if (container.ContainsAbility == false) { return; }
                if (_abilityCastInfos.ContainsKey(container))
				{
                    if (Time.time >= _abilityCastInfos[container].LastCastTime + container.AbilityCooldown)
					{
                        _abilityCastInfos[container].Cast();
                        TriggerAbility(container);
					}
				}
                else
				{
                    _abilityCastInfos.Add(container, new AbilityCastInfos(container));
                    TriggerAbility(container);
				}
			}
            protected void TriggerAbility(IAbilityContainer container)
			{
                if (container.ContainsAbility == false) { return; }
				switch (container.AbilityTriggerData.TriggerMethod)
				{
					case AbilityTriggerMethod.Single:
					case AbilityTriggerMethod.Automatic:
                        TriggerAbility_Single(container);
                        break;
                    case AbilityTriggerMethod.Burst:
                        master.StartCoroutine(TriggerBurstAbilityCoroutine(container));
						break;
				}

			}
            protected void TriggerAbility_Single(IAbilityContainer container)
			{
                if (container.ContainsAbility == false) { return; }
                if (container.AbilityTriggerData.MultiCastCount > 1)
                {
                    for (int i = 0; i < container.AbilityTriggerData.MultiCastCount; i++)
                    {
                        CastAbility(container, i);
                    }
                }
                else
				{
                    CastAbility(container);
				}

            }

            protected void CastAbility(IAbilityContainer container, int multiCastID) => CastAbility(container, default, multiCastID);
            protected void CastAbility(IAbilityContainer container, Vector3 castDirection) => CastAbility(container, castDirection, default);
            protected void CastAbility(IAbilityContainer container) => CastAbility(container, default, default);
            protected void CastAbility(IAbilityContainer container, Vector3 castDirection, int multiCastID)
            {
                if (container.ContainsAbility == false) { return; }
                AbilityData ability = container.Ability;
                AbilityCastData castData = new AbilityCastData(master, container, -1, multiCastID);

                _activeCastData = castData;
                castData.FillBase(
                    ComputeResourcesImpacts(container),
                    ability.Main.BaseKnockback * (HasStats ? Stats.EvaluateAbilitiesStat(EntityAbilitiesStat.KnockbackInflictedFactor, 1f) : 1f),
                    ability.Main.BaseRecoil / container.AbilityTriggerData.MultiCastCount);

                if (castDirection.sqrMagnitude <= Mathf.Epsilon)
                {
                    castData.FillCastDirection(DetermineCastDirection(castData.AbilityCast, container.AbilityTriggerData));
                }
                else
				{
                    castData.FillCastDirection(castDirection);
                }
                if (castData.AbilityCast.Methods.CastOriginTiming == AbilityCastOriginTiming.CastEntrance)
                {
                    castData.FillOrigin(DetermineCastOrigin(castData.AbilityCast));
                }

                if (castData.AbilityCast.Timing.Instant == true)
                {
                    PerformAbility(castData);
                    _activeCastData = null;
                    _abilityCastTimingsCoroutine = null;
                }
                else
                {
                    master.RestartCoroutine(CastAbilityTimingCoroutine(castData), ref _abilityCastTimingsCoroutine);
                }
            }

            protected virtual AbilityResourceImpactData[] ComputeResourcesImpacts(IAbilityContainer container)
            {
                if (HasStats == false) { return container.Ability.Main.BaseResourcesImpacts; }

                AbilityResourceImpactData[] impacts = new AbilityResourceImpactData[container.Ability.Main.BaseResourcesImpacts.Length];

                for (int i = 0; i < impacts.Length; i++)
                {
                    impacts[i] = new AbilityResourceImpactData(
                        container.Ability.Main.BaseResourcesImpacts[i].ResourceType,
                        Stats.EvaluateAbilitiesResourcesStat(
                            container.Ability.Main.BaseResourcesImpacts[i].ResourceType,
                            container.Ability.Main.BaseResourcesImpacts[i].ResourceChange));
                }

                if (container is EntityEquipable eq)
                {
                    eq.EquippedAbility.ApplyResourcesInfluencesOnImpacts(impacts);
                }

                return impacts;
            }
            protected virtual Vector3 DetermineCastOrigin(AbilityData ability)
            {
                switch (ability.Methods.CastOrigin)
                {
                    case AbilityCastOrigin.Caster:
                        return Position;
                    case AbilityCastOrigin.AimAbsolute:
                        if (HasController == true)
						{
                            return Controller.AimTarget;
                        }
                        else
						{
                            return Position;
						}
                }

                throw new System.Exception("Ability Cast Origin" + ability.Methods.CastOrigin + " not found.");
            }
            protected virtual Vector3 DetermineCastDirection(AbilityData ability, AbilityTriggerData triggerData)
            {
                Vector3 dir;
                switch (ability.Methods.DirectionMethod)
                {
                    case DirectionMethod.AimBased:
                        if (HasController == false)
						{
                            dir = PinouApp.Entity.Mode2D ? Forward2D.ToV3() : Forward;
                        }
                        else
						{
                            dir = Controller.AimDirection;
                        }
                        break;
                    case DirectionMethod.CharacterForward:
                        dir = PinouApp.Entity.Mode2D ? Forward2D.ToV3() : Forward;
                        break;
                    default:
                        throw new System.Exception("Direction Method " + ability.Methods.DirectionMethod + " not found.");
                }

                float spreadAngle = (1f - triggerData.Precision) * Mathf.PI;
                if (PinouApp.Entity.Mode2D)
				{
                    dir = dir.SetZ(0f).normalized;
                    float curAngle = dir.y > 0 ? Mathf.Acos(dir.x) : -Mathf.Acos(dir.x);
                    curAngle += Random.Range(-spreadAngle, spreadAngle);

                    return new Vector3(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
				}
                else
				{
                    throw new System.Exception("To Implement: Spherical Direction Projection");
				}
			}

            protected virtual void PerformAbility(AbilityCastData castData)
            {
                if (castData.AbilityCast.Methods.CastOriginTiming == AbilityCastOriginTiming.PerformEntrance)
                {
                    castData.FillOrigin(DetermineCastOrigin(castData.AbilityCast));
                }

                OnPerformAbility.Invoke(master, castData);

                castData.OnResultEmitted.Subscribe(OnCastDataEmitResult);
                AbilityPerformer.PerformAbility(castData);

                OnAbilityPerformed.Invoke(master, castData);
            }
            #endregion
            #endregion

            #region Coroutines
            protected IEnumerator TriggerBurstAbilityCoroutine(IAbilityContainer container)
			{
                int triggerCount = 0;
                float counter = container.AbilityTriggerData.BurstPeriod;
                while (triggerCount < container.AbilityTriggerData.BurstCount)
				{
                    counter += Time.deltaTime;

                    while (counter > container.AbilityTriggerData.BurstPeriod)
					{
                        TriggerAbility_Single(container);
                        counter -= container.AbilityTriggerData.BurstPeriod;
                        triggerCount++;
                    }
                    yield return null;
				}
			}

            private readonly AbilityState[] _abilityCastStatesArray = new AbilityState[]{
                    AbilityState.Performing,
                    AbilityState.HardRecovering,
                    AbilityState.SoftRecovering,
                    AbilityState.None
                };
            protected IEnumerator CastAbilityTimingCoroutine(AbilityCastData castData)
            {
                _currentAbilityState = AbilityState.Casting;

                int arrayIndex = 0;
                float attackSpeedFactor = HasStats ? Stats.EvaluateAbilitiesStat(EntityAbilitiesStat.AttackSpeed, 1f) : 1f;
                float[] durationArray =
                {
                    (castData.AbilityCast.Timing.CastDuration) * attackSpeedFactor,
                    (castData.AbilityCast.Timing.CastDuration + castData.AbilityCast.Timing.PerformDuration) * attackSpeedFactor,
                    (castData.AbilityCast.Timing.CastDuration + castData.AbilityCast.Timing.PerformDuration + castData.AbilityCast.Timing.HardRecoverDuration) * attackSpeedFactor,
                    (castData.AbilityCast.Timing.CastDuration + castData.AbilityCast.Timing.PerformDuration + castData.AbilityCast.Timing.HardRecoverDuration + castData.AbilityCast.Timing.SoftRecoverDuration) * attackSpeedFactor
                };
               

                float timeAtStart = Time.time;
                float lastDifference = 0f;

                while (arrayIndex < durationArray.Length)
                {
                    float timeDifference = Time.time - timeAtStart;

                    if (timeDifference > durationArray[arrayIndex])
                    {
                        if (lastDifference <= durationArray[arrayIndex])
                        {
                            _currentAbilityState = _abilityCastStatesArray[arrayIndex];
                            if (_abilityCastStatesArray[arrayIndex] == AbilityState.Performing)
                            {
                                PerformAbility(castData);
                            }

                            arrayIndex++;
                        }
                    }

                    lastDifference = timeDifference;
                    yield return new WaitForFixedUpdate();
                }

                _activeCastData = null;
                _abilityCastTimingsCoroutine = null;
            }
            #endregion

            #region Events
            public PinouUtils.Delegate.Action<Entity, AbilityHitbox> OnBuildAbilityHitbox { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityHitbox>();
            public PinouUtils.Delegate.Action<Entity, AbilityHitbox> OnDestroyAbilityHitbox { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityHitbox>();
            public PinouUtils.Delegate.Action<Entity, AbilityCastData> OnPerformAbility { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastData>();
            public PinouUtils.Delegate.Action<Entity, AbilityCastData> OnAbilityPerformed { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastData>();
            public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnAbilityHitResultEmitted { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();

			private void OnCastDataEmitResult(AbilityCastData castData, AbilityCastResult result)
            {
                OnAbilityHitResultEmitted.Invoke(master, result);
            }
            #endregion
        }
    }
}