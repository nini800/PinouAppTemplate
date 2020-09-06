#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{

    [CreateAssetMenu(fileName = "AbilityData", menuName = "Pinou/Entity/AbilityData", order = 1000)]
	public class AbilityData : SerializedScriptableObject
	{
		private const string mpName = "Identity Parameters";
		[FoldoutGroup(mpName), SerializeField, ReadOnly] private int _abilityID = -1;
		[FoldoutGroup(mpName), SerializeField] private string _name = "New Ability";
		[FoldoutGroup(mpName), SerializeField] private AbilityType _type = AbilityType.Instant;

		[TitleGroup("Main"), SerializeField] private AbilityMainData _main;
		[TitleGroup("Timings"), SerializeField] private AbilityTimingData _timing;
		[TitleGroup("Hitbox"), SerializeField] private AbilityHitboxData _hitbox;
		[TitleGroup("Methods"), SerializeField] private AbilityMethodsData _methods;
		[TitleGroup("Velocity Overrides"), SerializeField] private VelocityOverrideChainData _velocityOverrideChain;
		[TitleGroup("Visual"), SerializeField] private AbilityVisualData _visual;

		//Identity Parameters
		public int AbilityID => _abilityID;
		public string Name => _name;
		public AbilityType Type => _type;

		//Parameters
		public AbilityMainData Main => _main;
		public AbilityTimingData Timing => _timing;
		public AbilityHitboxData Hitbox => _hitbox;
		public AbilityMethodsData Methods => _methods;

		public VelocityOverrideChainData VelocityOverrideChain => _velocityOverrideChain;
		public bool HasVelocityOverrides => _velocityOverrideChain.OverrideChain != null && _velocityOverrideChain.OverrideChain.Length > 0;

		public AbilityVisualData Visual => _visual;

#if UNITY_EDITOR
		private void OnValidate()
        {
			_velocityOverrideChain.E_UpdateAverageCurvesValue();

			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == true) { return; }

			if (PinouApp.Resources == null) { return; }
			if (PinouApp.Resources.Data.AbilityDatabase.GetAbilityByID(_abilityID) != this) { _abilityID = -1; }
			if (_abilityID == -1)
			{
				int id = PinouApp.Resources.Data.AbilityDatabase.E_AddAbility(this);
				E_SetAbilityID(id);
			}

			_methods.GetType().GetField("_abilityData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_methods, this);
        }
		public void E_SetAbilityID(int id)
		{
			_abilityID = id;
		}
#endif
    }
}