using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Pinou/PlayerData", order = 1000)]
	public class PinouPlayerManagerData : PinouManagerData
	{
		public override PinouManager BuildManagerInstance()
		{
			return new PinouPlayerManager(this);
		}

		public class PinouPlayerManager : PinouManager
		{
			public PinouPlayerManager(PinouPlayerManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouPlayerManagerData Data { get; private set; }

			#region Attributes, Accessors & Mutators
			private List<PinouPlayer> _players = new List<PinouPlayer>();
			public PinouPlayer[] Players => _players.ToArray();

			public PinouPlayer GetPlayerByID(int ID)
			{
				for (int i = 0; i < _players.Count; i++)
				{
					if (_players[i].PlayerID == ID)
					{
						return _players[i];
					}
				}

				return null;
			}
			public PinouPlayer GetPlayerByController(int controllerId)
			{
				for (int i = 0; i < _players.Count; i++)
				{
					if (_players[i].ControllerId == controllerId)
					{
						return _players[i];
					}
				}

				return null;
			}
			public PinouPlayer GetPlayerByName(string playerName)
			{
				for (int i = 0; i < _players.Count; i++)
				{
					if (_players[i].PlayerName.Equals(playerName))
					{
						return _players[i];
					}
				}

				return null;
			}
			#endregion

			#region Utilities
			private int GetFirstPlayerIDAvailable()
			{
				int ID = 0;
				while (GetPlayerByID(ID) != null)
				{
					ID++;
				}

				return ID;
			}
			public PinouPlayer CreatePlayer(string playerName, int controllerId)
			{
				return CreatePlayer<PinouPlayer>(playerName, controllerId);
			}
			public T CreatePlayer<T>(string playerName, int controllerId) where T : PinouPlayer, new()
			{
				T newPlayer = new T();
				newPlayer.Construct(GetFirstPlayerIDAvailable(), playerName, controllerId);

				_players.Add(newPlayer);
				OnPlayerCreated.Invoke(newPlayer);
				return newPlayer;
			}

			public bool RemovePlayerByID(int ID) => RemovePlayer(GetPlayerByID(ID));
			public bool RemovePlayerByName(string playerName) => RemovePlayer(GetPlayerByName(playerName));
			public bool RemovePlayerByController(int controllerId) => RemovePlayer(GetPlayerByController(controllerId));

			public bool RemovePlayer(PinouPlayer player)
			{
				if (player != null)
				{
					player.Destroy();
					_players.Remove(player);
					OnPlayerRemoved.Invoke(player);
					return true;
				}
				return false;
			}
			#endregion

			#region Events
			public PinouUtils.Delegate.Action<PinouPlayer> OnPlayerCreated { get; private set; } = new PinouUtils.Delegate.Action<PinouPlayer>();
			public PinouUtils.Delegate.Action<PinouPlayer> OnPlayerRemoved { get; private set; } = new PinouUtils.Delegate.Action<PinouPlayer>();
			#endregion
		}
	}
	
}