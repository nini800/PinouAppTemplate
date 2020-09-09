#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{

    [CreateAssetMenu(fileName = "AbilityData", menuName = "Pinou/Entity/AbilityData", order = 1000)]
	public class AbilityData : SerializedScriptableObject, IAbilityContainer, IDatabaseItem
	{
		private const string mpName = "Identity Parameters";
		[FoldoutGroup(mpName), SerializeField, ReadOnly] private int _abilityID = -1;
		[FoldoutGroup(mpName), SerializeField] private string _name = "New Ability";
		[FoldoutGroup(mpName), SerializeField] private AbilityType _type = AbilityType.Instant;

		[TitleGroup("Main"), SerializeField] private AbilityMainData _main;
		[TitleGroup("Trigger"), SerializeField] private AbilityTriggerData _trigger;
		[TitleGroup("Timings"), SerializeField] private AbilityTimingData _timing;
		[TitleGroup("Hitbox"), SerializeField] private AbilityHitboxData _hitbox;
		[TitleGroup("Methods"), SerializeField] private AbilityMethodsData _methods;
		[TitleGroup("Velocity Overrides"), SerializeField] private VelocityOverrideChainData _velocityOverrideChain;
		[TitleGroup("Visual"), SerializeField] private AbilityVisualData _visual;

		#region IAbilityContainer
		public bool ContainsAbility => true;
		public AbilityData Ability => this;
		public AbilityTriggerData AbilityTriggerData => _trigger;
		public float AbilityCooldown => _timing.Cooldown;
		#endregion

		//Identity Parameters
		public int AbilityID => _abilityID;
		public string Name => _name;
		public AbilityType Type => _type;

		//Parameters
		public AbilityMainData Main => _main;
		public AbilityTriggerData Trigger => _trigger;
		public AbilityTimingData Timing => _timing;
		public AbilityHitboxData Hitbox => _hitbox;
		public AbilityMethodsData Methods => _methods;

		public VelocityOverrideChainData VelocityOverrideChain => _velocityOverrideChain;
		public bool HasVelocityOverrides => _velocityOverrideChain.OverrideChain != null && _velocityOverrideChain.OverrideChain.Length > 0;

		public AbilityVisualData Visual => _visual;

		public int DatabaseID => _abilityID;
		public DatabaseType DatabaseType => DatabaseType.Ability;
		public void SetDatabaseID(int id)
		{
			_abilityID = id;
		}

#if UNITY_EDITOR
		[Button("Call OnValidate")]
		private void OnValidate()
		{
			_velocityOverrideChain.E_UpdateAverageCurvesValue();

			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == true) { return; }

			if (PinouApp.Resources == null) { return; }
			if (PinouApp.Resources.Data.Databases.GetDatabase(DatabaseType.Ability).GetItem(_abilityID) != this) { _abilityID = -1; }

			if (_abilityID == -1)
			{
				PinouApp.Resources.Data.Databases.AddItem(DatabaseType.Ability, this);
			}

			_methods.GetType().GetField("_abilityData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_methods, this);
        }


#endif
	}
}