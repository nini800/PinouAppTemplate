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
		[Header("Parameters")]
		[Space]
		[SerializeField] private EntityEquipableType _builtType;

		public EntityEquipableType BuiltType => _builtType;
	}
}