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
		public float GetResourceChange(EntityBeingResourceType type)
		{
			for (int i = 0; i < _resourcesChanges.Count; i++)
			{
				if (_resourcesChanges[i].ResourceType == type) { return _resourcesChanges[i].ResourceChange; }
			}
			return 0f;
		}

		private bool _victimFilled, _impactFilled, _knockbackAppliedFilled, _resourceChangesFilled;
		public bool VictimFilled => _victimFilled;
		public bool ImpactFilled => _impactFilled;
		public bool KnockbackAppliedFilled => _knockbackAppliedFilled;
		public bool ResourceChangesFilled => _resourceChangesFilled;

		public void FillVictim(Entity victim)
        {
			_victim = victim;
			_victimFilled = true;
        }
		public void FillImpact(Vector3 impact)
		{
			_impact = impact;
			_impactFilled = true;
		}
		public void FillKnockbackApplied(Vector3 knockback)
		{
			_knockbackApplied = knockback;
			_knockbackAppliedFilled = true;
		}
		public void FillResourceChange(EntityBeingResourceType resource, float change)
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

			_resourceChangesFilled = true;
        }
		public void SetResourceChanges(AbilityResourceImpactData[] resourceChanges)
		{
			_resourcesChanges = new List<AbilityResourceImpactData>(resourceChanges);
		}
    }
}