#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class PassiveSpawnSystem : SpawnSystemBase
	{
		[Header("Parameters")]
		[Space]
		[SerializeField, Range(0.017f, 1f)] private float _spawnTryPeriod = 0.1f;
		[SerializeField] private float _creditsPerSecond = 1f;
		[SerializeField, ReadOnly] private float _creditsStep = 1 / 60f;

		private float _spawnTryCounter = 0f;
		[ShowInInspector] private Dictionary<Entity, float> _spawnCredits = new Dictionary<Entity, float>();
		private Dictionary<Entity, PinouResourcesEntities.SpawnGroup> _choosenNextSpawns = new Dictionary<Entity, PinouResourcesEntities.SpawnGroup>();

		#region Behaviour
		protected override void OnAwake()
		{
		}
		protected override void OnSafeStart()
		{
			#region Prevent EditorCall
#if UNITY_EDITOR
			if (EditorApplication.isPlaying == false)
			{
				return;
			}
#endif
			#endregion

			foreach (Entity player in PinouApp.Entity.Players)
			{
				_spawnCredits.Add(player, 0f);
				_choosenNextSpawns.Add(player, null);
				_choosenNextSpawns[player] = PinouApp.Resources.Data.Entities.GetPool("LowLevel").GetRandomGroup();
			}
		}

		private void FixedUpdate()
		{
			HandlePassiveCreditGain();
		}
		private void HandlePassiveCreditGain()
		{
			foreach(Entity player in PinouApp.Entity.Players)
			{
				GiveCredits(player, _creditsStep);
			}
		}

		private void Update()
		{
			HandleTrySpawn();
		}
		private void HandleTrySpawn()
		{
			_spawnTryCounter += Time.deltaTime;
			if (_spawnTryCounter > _spawnTryPeriod)
			{
				_spawnTryCounter -= _spawnTryPeriod;
				TrySpawn();
			}
		}


		#endregion

		#region Utilities
		private void TrySpawn()
		{
			foreach (Entity player in PinouApp.Entity.Players)
			{
				if (_spawnCredits.ContainsKey(player) == false) { _spawnCredits.Add(player, 0f); }
				if (_choosenNextSpawns.ContainsKey(player) == false) { _choosenNextSpawns.Add(player, PinouApp.Resources.Data.Entities.GetPool("LowLevel").GetRandomGroup()); }
				if (_spawnCredits[player] > _choosenNextSpawns[player].Cost)
				{
					Spawn(player);
				}
			}
		}
		private void Spawn(Entity player)
		{
			_spawnCredits[player] -= _choosenNextSpawns[player].Cost;
			PinouApp.Entity.CreateEntityGroupAroundPlayer(_choosenNextSpawns[player], player);
			_choosenNextSpawns[player] = PinouApp.Resources.Data.Entities.GetPool("LowLevel").GetRandomGroup();
		}

		public void GiveCredits(Entity player, float credits)
		{
			if (_spawnCredits.ContainsKey(player) == false) { _spawnCredits.Add(player, 0f); }
			_spawnCredits[player] += TotalCreditFactor(player) * credits;
		}
		#endregion

		#region Editor
#if UNITY_EDITOR
		protected override void E_OnValidate()
		{
			_creditsStep = _creditsPerSecond / 60f;
		}
#endif
		#endregion
	}
}