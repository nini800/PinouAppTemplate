#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    [CreateAssetMenu(fileName = "EquipableBuilder", menuName = "Pinou/Entity/EquipableBuilder", order = 1000)]
	public class EntityEquipableBuilder : SerializedScriptableObject
	{
		[Title("Identity Parameters")]
		[SerializeField] private EntityEquipableType _builtType;
		[SerializeField] private string _baseName;

		[Title("Stats Influences")]
		[SerializeField] private StatsInfluenceData[] _statsInfluencesDatas;
		[Title("Equipped Ability")]
		[SerializeField] private bool _hasEquippedAbility;
		[SerializeField, ShowIf("_hasEquippedAbility")] private EntityEquipableAbilityBuilder _equippedAbility;
		[Title("Visual")]
		[SerializeField] private bool _hasVisual;
		[SerializeField, ShowIf("_hasVisual")] private EntityEquipableVisual _visual;

		public EntityEquipableType BuiltType => _builtType;
		public string BaseName => _baseName;

		public EntityEquipable BuildEquipable(int equipableLevel)
		{
			StatsInfluenceData.StatsInfluence[] statsInfluences = new StatsInfluenceData.StatsInfluence[_statsInfluencesDatas.Length];
			for (int i = 0; i < _statsInfluencesDatas.Length; i++)
			{
				statsInfluences[i] = new StatsInfluenceData.StatsInfluence(_statsInfluencesDatas[i], equipableLevel);
			}

			return new EntityEquipable(_baseName, equipableLevel, _builtType,
				statsInfluences.Length > 0 ? statsInfluences : null,
				_hasEquippedAbility ? _equippedAbility.Build(equipableLevel) : null,
				_hasVisual ? _visual : null);
		}
	} 
}