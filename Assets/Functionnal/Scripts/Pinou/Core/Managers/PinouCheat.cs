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
			TeleportEntity
		}

		public enum SpawnMethod
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

		[SerializeField, ShowIf("_cheatType", PinouCheatType.SpawnEntity)] private GameObject _entityModel;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.SpawnEntity)] private SpawnMethod _spawnMethod;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.SpawnEntity), ShowIf("_spawnMethod", SpawnMethod.PlayerForward)] private float _forwardOffset;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.ModifyEntityHealth)] private float _healthModifier;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.MakeEntityImmortal)] private TargetEntity _entitiesToImmortalize;
		[SerializeField, ShowIf("_cheatType", PinouCheatType.TeleportEntity)] private Vector3 _teleportOffset;

		public PinouCheatType CheatType => _cheatType;
		public KeyCode CheatKey => _cheatKey;

		public GameObject EntityModel => _entityModel;
		public float HealthModifier => _healthModifier;
		public TargetEntity EntitiesToImmortalize => _entitiesToImmortalize;
		public Vector3 TeleportOffset => _teleportOffset;

		public void CheckCheat()
        {
			if (Input.GetKey(_cheatKeyModifier) && Input.GetKeyDown(_cheatKey))
            {
                switch (_cheatType)
                {
                    case PinouCheatType.SpawnEntity:
						HandleSpawnEntity();
                        break;
                    case PinouCheatType.ModifyEntityHealth:
                        break;
                    case PinouCheatType.MakeEntityImmortal:
                        break;
                    case PinouCheatType.TeleportEntity:
                        break;
                }
            }
        }

		private void HandleSpawnEntity()
        {
            switch (_spawnMethod)
            {
                case SpawnMethod.PlayerForward:
					Entity player = PinouApp.Entity.Player;
					if (player != null)
                    {
						PinouApp.Entity.CreateEntity(_entityModel, player.Position + player.Forward * _forwardOffset, 0f);
                    }
                    break;
                case SpawnMethod.RayCastMouse:
					throw new System.Exception("To implement");
                    //break;
                case SpawnMethod.Mouse2d:
						PinouApp.Entity.CreateEntity(_entityModel, Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0f), 0f);
                    break;
			}
        }
		private void HandleModifyEntityHealth()
		{

		}
		private void HandleMakeEntityImmortal()
		{

		}
		private void HandleTeleportEntity()
		{

		}
	}
}