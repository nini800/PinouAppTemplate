#pragma warning disable 1522, 0649
using Pinou.EntitySystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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

	public interface IDatabaseItem
	{
		void SetDatabaseID(int id);
		int DatabaseID { get; }
		DatabaseType DatabaseType { get; }
	}

	[Serializable]
	public class PinouResourcesDatabases
	{
		[Serializable]
		public class Database
		{
			public enum DatabaseMethod
			{
				IDatabaseItems,
				ImportFromFolder
			}
			public Database(DatabaseType type)
			{
				_type = type;
			}
			[Title("@_type.ToString() + \"DataBase\"")]
			[SerializeField] private DatabaseType _type;
			[SerializeField] private DatabaseMethod _method;
			[SerializeField, ShowIf("_method", DatabaseMethod.ImportFromFolder), FolderPath] private string _folder;
			[SerializeField] private List<UnityEngine.Object> _items = new List<UnityEngine.Object>();
			public DatabaseType Type => _type;
			public UnityEngine.Object[] Items => _items.ToArray();

			public int GetIndex(UnityEngine.Object item)
			{
				return _items.IndexOf(item);
			}
			public UnityEngine.Object GetItem(int id)
			{
				if (id < 0 || id >= _items.Count) { return null; }

				return _items[id];
			}

			[Button("Update IDs")]
			private void DatabaseSecurityCheck()
			{
				List<UnityEngine.Object> itemsEncountered = new List<UnityEngine.Object>();
				for (int i = _items.Count - 1; i >= 0; i--)
				{
					if (_items[i] == null || itemsEncountered.Contains(_items[i]))
					{
						_items.RemoveAt(i);
						continue;
					}
					else
					{
						itemsEncountered.Add(_items[i]);
					}
				}
				for (int i = 0; i < _items.Count; i++)
				{
					if (_items[i] is IDatabaseItem dbIt)
					{
						dbIt.SetDatabaseID(i);
					}
				}

				if (_method == DatabaseMethod.ImportFromFolder)
				{
#if UNITY_EDITOR
					PinouUtils.Editor.OnReloadScripts -= OnShouldCheckFolder_sc;
					PinouUtils.Editor.OnReloadScripts += OnShouldCheckFolder_sc;
					EditorApplication.playModeStateChanged -= OnShouldCheckFolder;
					EditorApplication.playModeStateChanged += OnShouldCheckFolder;
					OnShouldCheckFolder_sc();
#endif
				}
			}

#if UNITY_EDITOR
			private void OnShouldCheckFolder(PlayModeStateChange obj)
			{
				OnShouldCheckFolder_sc();
			}
			private void OnShouldCheckFolder_sc()
			{
				string path = _folder + "/";

				string[] objectsAtPath = Directory.GetFiles(path);
				_items = new List<UnityEngine.Object>();

				for (int i = 0; i < objectsAtPath.Length; i++)
				{
					UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(objectsAtPath[i]);
					if (o != null)
					{
						_items.Add(o);
					}
				}
			}
#endif


			public void AddItem(UnityEngine.Object item)
			{
				DatabaseSecurityCheck();
				if (_items.Contains(item) == false)
				{
					_items.Add(item);
					if (item is IDatabaseItem dbIt)
					{
						dbIt.SetDatabaseID(_items.Count - 1);
					}
				}
				else
				{
					if (item is IDatabaseItem dbIt)
					{
						dbIt.SetDatabaseID(_items.IndexOf(item));
					}
				}
			}
		}

		[SerializeField] private string _databaseAutoscriptFolderPath;
		[SerializeField, ValidateInput("ValidateDatabasesTypes")] private string[] _databasesTypes;
		[SerializeField, ValidateInput("ValidateDatabases")] private List<Database> _databases = new List<Database>();
		private bool ValidateDatabasesTypes(string[] databases)
		{
			PinouAutoscript.UpdatePinouResourcesDatabasesAutoScript(_databaseAutoscriptFolderPath, databases);
			return true;
		}
		private bool ValidateDatabases(List<Database> databases)
		{
			if (_databases.Count < _databasesTypes.Length)
			{
				for (int i = _databases.Count; i < _databasesTypes.Length; i++)
				{
					_databases.Add(new Database((DatabaseType)i));
				}
			}

			for (int i = 0; i < _databases.Count; i++)
			{
				DatabaseType t = ((DatabaseType)i);
				if (_databases[i].Type != t)
				{
					_databases[i].GetType().GetField("_type", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(_databases[i], t);
				}
			}

			if (_databases.Count > _databasesTypes.Length)
			{
				for (int i = _databasesTypes.Length; i > _databases.Count; i--)
				{
					_databases.RemoveAt(i);
				}
			}
			return true;
		}

		public Database GetDatabase(DatabaseType type)
		{
			int index = (int)type;
			if (index < 0 || index > _databases.Count) { return null; }

			return _databases[index];
		}

		public UnityEngine.Object GetItem(DatabaseType type, int index)
		{
			return GetDatabase(type).GetItem(index);
		}
		public void AddItem(DatabaseType type, UnityEngine.Object dbItem)
		{
			GetDatabase(type).AddItem(dbItem);
		}
		public int GetIndex(DatabaseType type, UnityEngine.Object dbItem)
		{
			return GetDatabase(type).GetIndex(dbItem);
		}
	}
}