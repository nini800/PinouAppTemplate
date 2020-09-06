#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Pinou;

namespace Pinou.EntitySystem
{
	public enum EntityEffectType
	{
		ModifyInfoValue,
	}

	[System.Serializable]
	public class EntityEffect
	{
		[SerializeField] private EntityEffectType _effectType;

		//ModifyInfoValue
		[ShowIf(nameof(_effectType), EntityEffectType.ModifyInfoValue), ValueDropdown("ComputePossibleLinkedInfoType")]
		[SerializeField] private EntityLinkedInfoType _infoType;

		[ShowIf(nameof(_effectType), EntityEffectType.ModifyInfoValue), ShowIf(nameof(_infoType), EntityLinkedInfoType.CurrentResource)]
		[SerializeField] private EntityBeingResourceType _resourceType;

		[ShowIf(nameof(_effectType), EntityEffectType.ModifyInfoValue), ShowIf(nameof(_infoType), EntityLinkedInfoType.Experience)]
		[SerializeField] private EntityStatsLevelType _levelType;

		[ShowIf(nameof(_effectType), EntityEffectType.ModifyInfoValue)]
		[SerializeField] private float _modifyAmount;


		private IEnumerable<EntityLinkedInfoType> ComputePossibleLinkedInfoType()
		{
			return new EntityLinkedInfoType[] { EntityLinkedInfoType.CurrentResource, EntityLinkedInfoType.Experience };
		}

		public void Apply(Entity target)
		{
			switch (_effectType)
			{
				case EntityEffectType.ModifyInfoValue:
					switch (_infoType)
					{
						case EntityLinkedInfoType.CurrentResource:
							if (target.HasBeing == false) { return; }
							target.Being.ModifyCurrentResource(_resourceType, _modifyAmount);
							break;
						case EntityLinkedInfoType.Experience:
							if (target.HasStats == false) { return; }
							target.Stats.ModifyLevelExperience(_levelType, _modifyAmount);
							break;
					}
					break;
			}
		}
	}

	[System.Serializable]
	public class EntityEffectBundle
	{
		[SerializeField] private EntityEffect[] _effects;

		public EntityEffect[] Effects => _effects;

		public void ApplyEffects(Entity target)
		{
			for (int i = 0; i < _effects.Length; i++)
			{
				_effects[i].Apply(target);
			}
		}
		public void ApplyRandomEffect(Entity target)
		{
			_effects.Random().Apply(target);
		}
	}
}
