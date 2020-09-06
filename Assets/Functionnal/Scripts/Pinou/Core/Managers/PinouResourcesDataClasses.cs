#pragma warning disable 1522, 0649
using Pinou.EntitySystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	[Serializable]
	public class PinouResourcesLayers
	{
		[SerializeField] private LayerMask _statics;
		[SerializeField] private LayerMask _ground;
		[SerializeField] private LayerMask _solids;
		[SerializeField] private LayerMask _entities;
		[SerializeField] private LayerMask _entityAbilities;
		[SerializeField] private LayerMask _entityHitboxes;
		[SerializeField] private LayerMask _entitysProjectiles;
		[SerializeField] private LayerMask _entityDrops;
		[SerializeField] private LayerMask _interactable;

		public LayerMask Statics => _statics;
		public LayerMask Ground => _ground;
		public LayerMask Solids => _solids;
		public LayerMask Entities => _entities;
		public LayerMask EntityAbilities => _entityAbilities;
		public LayerMask EntityHitboxes => _entityHitboxes;
		public LayerMask EntityProjectiles => _entitysProjectiles;
		public LayerMask EntityDrops => _entityDrops;
		public LayerMask Interactable => _interactable;
	}

	[Serializable]
	public class PinouResourcesMaterials
	{
		[SerializeField] private Material _defaultMaterial;
		public Material DefaultMaterial => _defaultMaterial;
	}

	[Serializable]
	public class PinouResourcesEntities
	{
		#region Data Classes
		[Serializable]
		public class SpawnGroup
		{
			[SerializeField] private GameObject[] _entities;
			[SerializeField] private float _cost;
			[SerializeField] private float _frequency;
			[SerializeField] private float _groupRadius;

			public GameObject[] Entities => _entities;
			public float Cost => _cost;
			public float Frequency => _frequency;
			public float GroupRadius => _groupRadius;
		}
		[Serializable]
		public class SpawnPool
		{
			[SerializeField] private string _poolName;
			[ValidateInput("TotalFrequencyValidator", "Total Frequency not equal to pool groups total frequency.")]
			[SerializeField] private SpawnGroup[] _poolGroups;
			[SerializeField, ReadOnly] private float _totalFrequency;

			public string PoolName => _poolName;

			private bool TotalFrequencyValidator(SpawnGroup[] groups)
			{
				_totalFrequency = 0f;
				for (int i = 0; i < groups.Length; i++)
				{
					_totalFrequency += groups[i].Frequency;
				}
				return true;
			}

			public SpawnGroup GetRandomGroup()
			{
				float randFreq = UnityEngine.Random.Range(0f, _totalFrequency);
				float countFreq = 0f;
				for (int i = 0; i < _poolGroups.Length; i++)
				{
					countFreq += _poolGroups[i].Frequency;
					if (randFreq <= countFreq)
					{
						return _poolGroups[i];
					}
				}

				throw new Exception("This should never happen.");
			}
		}
		#endregion

		[Header("Main")]
		[Space]
		[SerializeField] private GameObject _playerModel;
		[Header("Spawn Pools")]
		[Space]
		[SerializeField] private SpawnPool[] _spawnPools;

		[Header("FXs")]
		[Space]
		[SerializeField] private GameObject _dashFX;


		public GameObject PlayerModel => _playerModel;
		public SpawnPool[] SpawnPools => _spawnPools;
		public GameObject DashFX => _dashFX;

		public SpawnPool GetPool(string name)
		{
			for (int i = 0; i < _spawnPools.Length; i++)
			{
				if (_spawnPools[i].PoolName == name)
				{
					return _spawnPools[i];
				}
			}
			return null;
		}
	}

	[Serializable]
	public class PinouResourcesAbilityDatabse
	{
		[SerializeField] private List<AbilityData> _abilities;

		public AbilityData[] Abilities => _abilities.ToArray();

		public AbilityData GetAbilityByID(int id)
		{
			if (id < 0 || id >= _abilities.Count) { return null; }
			return _abilities[id];
		}

		#if UNITY_EDITOR
		[Button("Update Abilities IDs")]
		private void E_AbilityDatabaseSecurityCheck()
		{
			List<AbilityData> abilitiesEncountered = new List<AbilityData>();
			for (int i = _abilities.Count - 1; i >= 0; i--)
			{
				if (_abilities[i] == null || abilitiesEncountered.Contains(_abilities[i]))
				{
					_abilities.RemoveAt(i);
					continue;
				}
				else
				{
					abilitiesEncountered.Add(_abilities[i]);
				}
			}
			for (int i = 0; i < _abilities.Count; i++)
			{
				_abilities[i].E_SetAbilityID(i);
			}
		}
		public int E_AddAbility(AbilityData ability)
		{
			E_AbilityDatabaseSecurityCheck();
			if (_abilities.Contains(ability) == false)
			{
				_abilities.Add(ability);
				return _abilities.Count - 1;
			}
			else
			{
				return _abilities.IndexOf(ability);
			}
		}
		#endif
	}
}