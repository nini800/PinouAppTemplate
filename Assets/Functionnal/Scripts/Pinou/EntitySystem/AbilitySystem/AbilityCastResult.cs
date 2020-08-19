#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class AbilityCastResult
	{
		public AbilityCastResult(AbilityCastData castData)
        {
			_castData = castData;
        }
		private AbilityCastResultType _resultType;
		private AbilityCastData _castData;

		private Entity _victim;
		private Vector3 _impact;
		private Vector3 _knockbackApplied;

		private List<AbilityResourceImpactData> _resourcesChanges = new List<AbilityResourceImpactData>();

		public AbilityCastResultType ResultType => _resultType;
		public AbilityCastData CastData => _castData;

		public Entity Victim => _victim;
		public Vector3 Impact => _impact;
		public Vector3 KnockbackApplied => _knockbackApplied;

		public AbilityResourceImpactData[] ResourcesChanges => _resourcesChanges.ToArray();
		public float GetResourceChange(EntityBeingResourcesType type)
		{
			for (int i = 0; i < _resourcesChanges.Count; i++)
			{
				if (_resourcesChanges[i].ResourceType == type) { return _resourcesChanges[i].ResourceChange; }
			}
			return 0f;
		}

		public void FillVictim(Entity victim)
        {
			_victim = victim;
        }
		public void FillImpact(Vector3 impact)
		{
			_impact = impact;
		}
		public void FillKnockbackApplied(Vector3 knockback)
		{
			_knockbackApplied = knockback;
		}
		public void FillResourceChange(EntityBeingResourcesType resource, float change)
        {
			AbilityResourceImpactData data = null;
			for (int i = 0; i < _resourcesChanges.Count; i++)
			{
				if (_resourcesChanges[i].ResourceType == resource)
				{
					data = _resourcesChanges[i];
				}
			}
			if (data == null)
			{
				_resourcesChanges.Add(new AbilityResourceImpactData(resource, change));
			}
			else
			{
				data.SetResourceChange(change);
			}
        }
		public void SetResourceChanges(AbilityResourceImpactData[] resourceChanges)
		{
			_resourcesChanges = new List<AbilityResourceImpactData>(resourceChanges);
		}
    }
}