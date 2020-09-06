#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
                public AbilityCastInfos(AbilityData ability)
				{
                    _ability = ability;
                    _lastCastTime = Time.time;
				}
                private AbilityData _ability;
                private float _lastCastTime;
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

            private Dictionary<AbilityData, AbilityCastInfos> _abilityCastInfos = new Dictionary<AbilityData, AbilityCastInfos>();
            public float GetAbilityCooldownProgress(AbilityData ability)
			{
                if (_abilityCastInfos.ContainsKey(ability))
				{
                    return Mathf.Clamp(Time.time - _abilityCastInfos[ability].LastCastTime / ability.Timing.Cooldown, 0f, 1f);
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
            protected void TryCastAbility(AbilityData ability)
			{
                if (_abilityCastInfos.ContainsKey(ability))
				{
                    if (Time.time >= _abilityCastInfos[ability].LastCastTime + ability.Timing.Cooldown)
					{
                        _abilityCastInfos[ability].Cast();
                        CastAbility(ability);
					}
				}
                else
				{
                    _abilityCastInfos.Add(ability, new AbilityCastInfos(ability));
                    CastAbility(ability);
				}
			}
			protected void CastAbility(AbilityData ability)
            {
                AbilityCastData castData = new AbilityCastData(master, ability);

                _activeCastData = castData;
                castData.FillBase(
                    ComputeResourcesImpacts(ability),
                    ability.Main.BaseKnockback * (HasStats ? Stats.EvaluateAbilitiesStat(EntityAbilitiesStat.KnockbackInflictedFactor, 1f) : 1f),
                    ability.Main.BaseRecoil);

                DetermineCastDirection(castData);
                if (castData.AbilityCast.Methods.CastOriginTiming == AbilityCastOriginTiming.CastEntrance)
                {
                    DetermineCastOrigin(castData);
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
            protected virtual AbilityResourceImpactData[] ComputeResourcesImpacts(AbilityData ability)
			{
                if (HasStats == false) { return ability.Main.BaseResourcesImpacts; }
                AbilityResourceImpactData[] impacts = new AbilityResourceImpactData[ability.Main.BaseResourcesImpacts.Length];
				for (int i = 0; i < impacts.Length; i++)
				{
                    impacts[i] = new AbilityResourceImpactData(
                        ability.Main.BaseResourcesImpacts[i].ResourceType,
                        Stats.EvaluateAbilitiesResourcesStat(
                            ability.Main.BaseResourcesImpacts[i].ResourceType,
                            ability.Main.BaseResourcesImpacts[i].ResourceChange));
				}
                return impacts;
			}
            protected virtual void DetermineCastOrigin(AbilityCastData castData)
            {
                switch (castData.AbilityCast.Methods.CastOrigin)
                {
                    case AbilityCastOrigin.Caster:
                        castData.FillOrigin(Position);
                        break;
                    case AbilityCastOrigin.AimAbsolute:
                        throw new System.Exception("Aim Absolute depends on the type of game you're making. Override DetermineCastOrigin method first.");
                }
            }
            protected virtual void DetermineCastDirection(AbilityCastData castData)
            {
                switch (castData.AbilityCast.Methods.DirectionMethod)
                {
                    case DirectionMethod.AimBased:
                        throw new System.Exception("AimBased depends on the type of game you're making. Override DetermineCastDirection method first.");
                    case DirectionMethod.CharacterForward:
                        castData.FillCastDirection(Forward);
                        break;
                }
            }

            protected virtual void PerformAbility(AbilityCastData castData)
            {
                if (castData.AbilityCast.Methods.CastOriginTiming == AbilityCastOriginTiming.PerformEntrance)
                {
                    DetermineCastOrigin(castData);
                }

                OnPerformAbility.Invoke(master, castData);

                castData.OnResultEmitted.Subscribe(OnCastDataEmitResult);
                AbilityPerformer.PerformAbility(castData);

                OnAbilityPerformed.Invoke(master, castData);
            }
            #endregion
            #endregion

            #region Coroutines
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