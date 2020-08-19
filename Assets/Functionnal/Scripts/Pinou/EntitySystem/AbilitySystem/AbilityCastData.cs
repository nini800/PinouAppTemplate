using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class AbilityCastData
	{
		#region Constructors
        public AbilityCastData(Entity caster, AbilityData ability, float castTime = -1f, int multiCastID = 0)
        {
			_caster = caster;
			_abilityCast = ability;
			_castTime = castTime < 0f ? Time.time : castTime;
			_multiCastID = multiCastID;
        }
		#endregion

		#region Vars, Getters
		private Entity _caster;
		private float _castTime;
		private int _multiCastID;
		private List<AbilityCastResult> _results = new List<AbilityCastResult>();

		private AbilityData _abilityCast;

		private AbilityResourceImpactData[] _baseResourcesImpacts = new AbilityResourceImpactData[] { };
		private Vector3 _baseKnockback;
		private Vector3 _baseRecoil;

		private Vector3 _castDirection;
		private Vector3 _origin;

		public Entity Caster => _caster;
		public float CastTime => _castTime;
		public int MultiCastID => _multiCastID;
		public AbilityCastResult[] Results => _results.ToArray();

		public AbilityData AbilityCast => _abilityCast;

		public AbilityResourceImpactData[] BaseResourcesImpacts => _baseResourcesImpacts;
		public float GetResourceImpact(EntityBeingResourcesType type)
		{
			for (int i = 0; i < _baseResourcesImpacts.Length; i++)
			{
				if (_baseResourcesImpacts[i].ResourceType == type) { return _baseResourcesImpacts[i].ResourceChange; }
			}
			return 0f;
		}
		public Vector3 BaseKnockback => _baseKnockback;
		public Vector3 BaseRecoil => _baseRecoil;

		public Vector3 CastDirection => _castDirection;
		public Vector3 Origin => _origin;
		#endregion

		#region Utilities
		public void FillBase(AbilityResourceImpactData[] baseResourcesImpacts, Vector3 baseKnockback, Vector3 baseRecoil)
        {
			_baseResourcesImpacts = new AbilityResourceImpactData[baseResourcesImpacts.Length];
			System.Array.Copy(baseResourcesImpacts, _baseResourcesImpacts, baseResourcesImpacts.Length);

			_baseKnockback = baseKnockback;
			_baseRecoil = baseRecoil;
        }
		public void FillOrigin(Vector3 origin)
        {
			_origin = origin;
        }
		public void FillCastDirection(Vector3 direction)
        {
			_castDirection = direction;
        }

		public void FillResult(AbilityCastResult result)
        {
			_results.Add(result);
			OnResultEmitted.Invoke(this, result);
		}
		#endregion

		#region Events
		public PinouUtils.Delegate.Action<AbilityCastData, AbilityCastResult> OnResultEmitted { get; private set; } = new PinouUtils.Delegate.Action<AbilityCastData, AbilityCastResult>();
        #endregion
    }
}