#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou
{
	[System.Serializable]
	public class PinouCheat
	{
		public enum PinouCheatType
		{
			SpawnEntity,
			ModifyEntityHealth,
			MakeEntityImmortal,
			TeleportEntity,
			SpawnLoot,
			EquipSomething
		}

		public enum LocationMethod
		{
			PlayerForward,
			RayCastMouse,
			Mouse2d
		}

		public enum TargetEntity
        {
			Player,
			Allies,
			Enemies
        }

		[Header("@_cheatType.ToString() + \" cheat\"")]
		[SerializeField] private PinouCheatType _cheatType;
		[SerializeField] private KeyCode _cheatKeyModifier;
		[SerializeField] private KeyCode _cheatKey;
		[SerializeField] private bool _canBeHeld;
		[SerializeField, ShowIf("_canBeHeld")] private float _holdTriggerPeriod;

		[ShowIf("@_cheatType == PinouCheatType.SpawnEntity || _cheatType == PinouCheatType.TeleportEntity || _cheatType == PinouCheatType.SpawnLoot")]
		[SerializeField] private LocationMethod _locationMethod;
		[ShowIf("@(_cheatType == PinouCheatType.SpawnEntity || _cheatType == PinouCheatType.TeleportEntity || _cheatType == PinouCheatType.SpawnLoot) && _locationMethod== LocationMethod.PlayerForward")]
		[SerializeField] private float _forwardOffset;

		[ShowIf("@_cheatType == PinouCheatType.SpawnEntity")]
		[SerializeField] private GameObject _model;
		[ShowIf("@_cheatType == PinouCheatType.SpawnLoot")]
		[SerializeField] private EntityDropData _dropData;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.ModifyEntityHealth)] private float _healthModifier;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.MakeEntityImmortal)] private TargetEntity _entitiesToImmortalize;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.TeleportEntity)] private Vector3 _teleportOffset;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.EquipSomething)] private EntityEquipableBuilder _equipableBuilder;

		public PinouCheatType CheatType => _cheatType;
		public KeyCode CheatKey => _cheatKey;

		public GameObject Model => _model;
		public EntityDropData DropData => _dropData;
		public float HealthModifier => _healthModifier;
		public TargetEntity EntitiesToImmortalize => _entitiesToImmortalize;
		public Vector3 TeleportOffset => _teleportOffset;

		private float _lastTriggerTime = -1 / 0f;

		public void CheckCheat()
        {
			if (_canBeHeld == true)
			{
				if ((_cheatKeyModifier == KeyCode.None || Input.GetKey(_cheatKeyModifier)) && Input.GetKey(_cheatKey))
				{
					if (Input.GetKeyDown(_cheatKey))
					{
						_lastTriggerTime = -1 / 0f;
					}

					if (Time.time - _lastTriggerTime > _holdTriggerPeriod)
					{
						TriggerCheat();
					}
				}
			}
			else
			{
				if ((_cheatKeyModifier == KeyCode.None || Input.GetKey(_cheatKeyModifier)) && Input.GetKeyDown(_cheatKey))
				{
					TriggerCheat();
				}
			}

        }

		private void TriggerCheat()
		{
			switch (_cheatType)
			{
				case PinouCheatType.SpawnEntity:
					PinouApp.Entity.CreateEntity(_model, GetLocation(_locationMethod), 0f);
					break;
				case PinouCheatType.ModifyEntityHealth:
					break;
				case PinouCheatType.MakeEntityImmortal:
					break;
				case PinouCheatType.TeleportEntity:
					PinouApp.Entity.Player.Position = GetLocation(_locationMethod);
					break;
				case PinouCheatType.SpawnLoot:
					PinouApp.Loot.HandleSpawnDrops(new EntityDropData[] { _dropData }, GetLocation(_locationMethod));
					break;
				case PinouCheatType.EquipSomething:
					PinouApp.Entity.Player.Equipment.Equip(_equipableBuilder.BuildEquipable(PinouApp.Entity.Player.Stats.MainExperience.Level));
					break;
			}
			_lastTriggerTime = Time.time;
		}

		private Vector3 GetLocation(LocationMethod method)
		{
			switch (method)
			{
				case LocationMethod.PlayerForward:
					Entity player = PinouApp.Entity.Player;
					if (player != null)
					{
						return player.Position + player.Forward * _forwardOffset;
					}
					break;
				case LocationMethod.RayCastMouse:
					throw new System.Exception("To implement");
				case LocationMethod.Mouse2d:
					return Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0f);
			}

			return Vector3.zero;
		}
	}
}