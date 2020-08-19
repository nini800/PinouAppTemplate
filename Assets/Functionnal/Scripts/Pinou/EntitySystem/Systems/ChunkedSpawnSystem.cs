#pragma warning disable 1522, 0649
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class ChunkedSpawnSystem : SpawnSystemBase
	{
		public class EntitySpawnedInChunkData
		{
			public EntitySpawnedInChunkData(Entity ent, float cost, ChunkData chunk)
			{
				_entity = ent;
				_cost = cost;
				_chunkData = chunk;
			}
			private Entity _entity;
			private float _cost;
			private ChunkData _chunkData;

			public float Cost => _cost;
			public ChunkData ChunkData => _chunkData;
		}
		public class ChunkData
		{
			private DateTime _checkedTime;
			private float _chunkCredits;
			private float _maxCredits;

			public DateTime CheckedTime => _checkedTime;
			public float ChunkCredits => _chunkCredits;
			public float MaxCredits => _maxCredits;

			public void RefillCredits(float credits)
			{
				_chunkCredits += credits;
				_chunkCredits = Mathf.Clamp(_chunkCredits, 0f, _maxCredits);
			}
			public void SetMaxCredits(float max)
			{
				_maxCredits = max;
			}

			public float GrabAllCredits()
			{
				float temp = _chunkCredits;
				_chunkCredits = 0f;
				return temp;
			}

			public void Check()
			{
				_checkedTime = DateTime.Now;
			}
		}

		[Header("Chunk Parameters")]
		[Space]
		[SerializeField] private float _chunkSize;
		[SerializeField] private bool _checkYAxis = false;
		[SerializeField] private bool _checkZAxis = false;
		[SerializeField, Range(0.017f, 1f)] private float _checkTime;

		[Header("Spawn Parameters")]
		[Space]
		[SerializeField] private PinouUtils.Maths.Formula _spawnCreditPerDurationFormula;

		[Header("References")]
		[Space]
		[SerializeField] private PassiveSpawnSystem _spawnSystem;

		private float _counter = 0f;
		private Dictionary<Vector3, ChunkData> _chunkChecked = new Dictionary<Vector3, ChunkData>();
		private Dictionary<Entity, EntitySpawnedInChunkData> _entityChunks = new Dictionary<Entity, EntitySpawnedInChunkData>();

		private void Update()
		{
			_counter += Time.deltaTime;

			if (_counter > _checkTime)
			{
				_counter -= _checkTime;
				HandleCheckForSpawn();
			}
		}

		private void HandleCheckForSpawn()
		{
			foreach (Entity player in PinouApp.Entity.Players)
			{
				Vector3 center = player.Position;
				Vector3 chunkCenter = new Vector3(
						Mathf.Floor(center.x / _chunkSize),
						Mathf.Floor(center.y / _chunkSize),
						Mathf.Floor(center.z / _chunkSize));

				for (int x = -1; x <= 1; x++)
				{
					if (_checkZAxis == false)
					{
						if (_checkYAxis == false)
						{
							CheckChunk(player, new Vector3(chunkCenter.x + x, chunkCenter.y, chunkCenter.z));
						}
						else
						{
							for (int y = -1; y <= 1; y++)
							{
								CheckChunk(player, new Vector3(chunkCenter.x + x, chunkCenter.y + y, chunkCenter.z));
							}
						}
					}
					else
					{
						for (int z = -1; z <= 1; z++)
						{
							if (_checkYAxis == false)
							{
								CheckChunk(player, new Vector3(chunkCenter.x + x, chunkCenter.y, chunkCenter.z + z));
							}
							else
							{
								for (int y = -1; y <= 1; y++)
								{
									CheckChunk(player, new Vector3(chunkCenter.x + x, chunkCenter.y + y, chunkCenter.z + z));
								}
							}
						}
					}
				}
			}
		}

		private void CheckChunk(Entity player, Vector3 chunkPos)
		{
			float lastCheckDuration;
			if (_chunkChecked.ContainsKey(chunkPos) == false)
			{
				lastCheckDuration = 1 / 0.0f;
				_chunkChecked.Add(chunkPos, new ChunkData());
			}
			else
			{
				lastCheckDuration = (float)(DateTime.Now - _chunkChecked[chunkPos].CheckedTime).TotalSeconds;
				_chunkChecked[chunkPos].Check();
			}

			float credit = TotalCreditFactor(player) * _spawnCreditPerDurationFormula.Evaluate(lastCheckDuration) + _chunkChecked[chunkPos].GrabAllCredits();
			_chunkChecked[chunkPos].SetMaxCredits(TotalCreditFactor(player) * _spawnCreditPerDurationFormula.Evaluate(Mathf.Infinity));

			PinouResourcesEntities.SpawnGroup spawnGroup = PinouApp.Resources.Data.Entities.GetPool("LowLevel").GetRandomGroup();
			while (credit > spawnGroup.Cost)
			{
				credit -= spawnGroup.Cost;
				Entity[] entitiesCreated = PinouApp.Entity.CreateEntityGroup(spawnGroup, GetRandomPosInsideChunk(chunkPos));

				HandleRememberEntityChunks(entitiesCreated, _chunkChecked[chunkPos], spawnGroup.Cost / entitiesCreated.Length);

				spawnGroup = PinouApp.Resources.Data.Entities.GetPool("LowLevel").GetRandomGroup();
			}
		}

		private void HandleRememberEntityChunks(Entity[] entitiesCreated, ChunkData chunkData, float cost)
		{
			for (int i = 0; i < entitiesCreated.Length; i++)
			{
				_entityChunks.Add(entitiesCreated[i], new EntitySpawnedInChunkData(entitiesCreated[i], cost, chunkData));
				entitiesCreated[i].OnRemovedByManager.Subscribe(OnEntityRemovedByManager);
				entitiesCreated[i].Being.OnDeath.Subscribe(OnEntityDeath);
			}
		}

		private void OnEntityRemovedByManager(Entity ent)
		{
			if (_entityChunks.ContainsKey(ent))
			{
				_entityChunks[ent].ChunkData.RefillCredits(_entityChunks[ent].Cost);
				_entityChunks.Remove(ent);
			}
		}
		private void OnEntityDeath(Entity ent, AbilityCastResult killingResult)
		{
			if (_entityChunks.ContainsKey(ent))
			{
				_entityChunks.Remove(ent);
			}
		}

		private Vector3 GetRandomPosInsideChunk(Vector3 chunkPos)
		{
			if (_checkYAxis == true)
			{
				if (_checkZAxis == true)
				{
					chunkPos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
					chunkPos.y += UnityEngine.Random.Range(-0.5f, 0.5f);
					chunkPos.z += UnityEngine.Random.Range(-0.5f, 0.5f);
				}
				else
				{
					chunkPos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
					chunkPos.y += UnityEngine.Random.Range(-0.5f, 0.5f);
				}
			}
			else
			{
				if (_checkZAxis == true)
				{
					chunkPos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
					chunkPos.z += UnityEngine.Random.Range(-0.5f, 0.5f);
				}
				else
				{
					chunkPos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
				}
			}

			return chunkPos * _chunkSize;
		}

#if UNITY_EDITOR
		[Header("Editor")]
		[Space]
		[SerializeField] private bool E_DisplayChunk;
        [SerializeField, ShowIf("E_DisplayChunk")] private int E_DisplayChunkCount;
        [SerializeField, ShowIf("E_DisplayChunk")] private Color E_ChunkGridColor;
        [SerializeField, ShowIf("E_DisplayChunk")] private Color E_ChunkCenterColor;

		protected override void E_OnDrawGizmos()
		{
			E_HandleDrawChunkGrid();
		}
		private void E_HandleDrawChunkGrid()
		{
			if (E_DisplayChunk == false)
			{
				return;
			}
			Vector3 center = PinouApp.Entity.Player.Position;
			Gizmos.color = E_ChunkGridColor;
			for (int x = -E_DisplayChunkCount; x <= E_DisplayChunkCount; x++)
			{
				for (int y = -E_DisplayChunkCount; y <= E_DisplayChunkCount; y++)
				{
					Vector3 start = new Vector3(
						Mathf.Floor(center.x / _chunkSize + x),
						Mathf.Floor(center.y / _chunkSize + y),
						Mathf.Floor(center.z / _chunkSize)) * _chunkSize + Vector3.one * _chunkSize * 0.5f;

					Vector3 miOffset = Vector3.forward * (_chunkSize * (E_DisplayChunkCount));
					Gizmos.DrawLine(start - miOffset, start + miOffset);
				}
			}

			for (int x = -E_DisplayChunkCount; x <= E_DisplayChunkCount; x++)
			{
				for (int z = -E_DisplayChunkCount; z <= E_DisplayChunkCount; z++)
				{
					Vector3 start = new Vector3(
						Mathf.Floor(center.x / _chunkSize + x),
						Mathf.Floor(center.y / _chunkSize),
						Mathf.Floor(center.z / _chunkSize + z)) * _chunkSize + Vector3.one * _chunkSize * 0.5f;

					Vector3 miOffset = Vector3.up * (_chunkSize * (E_DisplayChunkCount));
					Gizmos.DrawLine(start - miOffset, start + miOffset);
				}
			}

			for (int y = -E_DisplayChunkCount; y <= E_DisplayChunkCount; y++)
			{
				for (int z = -E_DisplayChunkCount; z <= E_DisplayChunkCount; z++)
				{
					Vector3 start = new Vector3(
						Mathf.Floor(center.x / _chunkSize),
						Mathf.Floor(center.y / _chunkSize + y),
						Mathf.Floor(center.z / _chunkSize + z)) * _chunkSize + Vector3.one * _chunkSize * 0.5f;

					Vector3 miOffset = Vector3.right * (_chunkSize * (E_DisplayChunkCount));
					Gizmos.DrawLine(start - miOffset, start + miOffset);
				}
			}

			Gizmos.color = E_ChunkCenterColor;
			for (int x = -E_DisplayChunkCount; x <= E_DisplayChunkCount; x++)
			{
				for (int y = -E_DisplayChunkCount; y <= E_DisplayChunkCount; y++)
				{
					for (int z = -E_DisplayChunkCount; z <= E_DisplayChunkCount; z++)
					{
						Vector3 start = new Vector3(
						Mathf.Floor(center.x / _chunkSize + x),
						Mathf.Floor(center.y / _chunkSize + y),
						Mathf.Floor(center.z / _chunkSize + z)) * _chunkSize;
						Gizmos.DrawSphere(start, 0.2f);
					}
				}
			}
		}
#endif
	}
}

