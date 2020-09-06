#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.Networking;
using Pinou.EntitySystem;
using Mirror;
using System;
using Sirenix.Utilities;

namespace Pinou.Networking
{
	public interface IDestroyableNetworkSyncableGameObject
	{
		PinouUtils.Delegate.Action<GameObject> OnGameObjectDestroyed { get; }
	}

	public class PinouNetworkMainBehaviour : NetworkBehaviour
	{
		#region Syncable Network Variables
		private float _syncCheckCounter = 0f;
		private int _syncCheckCount = 0;
		private const int CHANNEL_COUNT = 3;
		[NonSerialized] private SyncableVariable[] _channelMasks;
		private Array _syncableVariablesEnumValues = Enum.GetValues(typeof(SyncableVariable));

		private Dictionary<GameObject, NetworkSyncableGameobjectInfos> _gameObjectSyncInfos = new Dictionary<GameObject, NetworkSyncableGameobjectInfos>();
		public void RegisterGameObjectSyncVar(GameObject go, SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod) => RegisterGameObjectSyncVar(go, var, frequency, registerMethod, syncMethod, null, false);
		public void RegisterGameObjectSyncVar(GameObject go, SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod, bool ignoreAuthority) => RegisterGameObjectSyncVar(go, var, frequency, registerMethod, syncMethod, null, ignoreAuthority);
		public void RegisterGameObjectSyncVar(GameObject go, SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod, Func<NetworkWriter, int> byteSizeMethod, bool ignoreAuthority)
		{
			IDestroyableNetworkSyncableGameObject destroyableGameObject = go.GetComponent<IDestroyableNetworkSyncableGameObject>();
			if (destroyableGameObject == null) { Debug.LogError("GameObject must have a IDestroyableNetworkSyncableGameObject component."); return; }

			if (_gameObjectSyncInfos.ContainsKey(go) == false) { _gameObjectSyncInfos.Add(go, new NetworkSyncableGameobjectInfos(go)); }

			_gameObjectSyncInfos[go].RegisterVar(var, frequency, registerMethod, syncMethod, byteSizeMethod, ignoreAuthority);
			destroyableGameObject.OnGameObjectDestroyed.SafeSubscribe(OnSyncableGameObjectDestroyed);
		}

		private int ComputeExpectedSize(NetworkSyncableVarsDirtyData dirtyData)
		{
			NetworkSyncableGameobjectInfos infos = _gameObjectSyncInfos[dirtyData.GameObject];
			int totalSize = 9;
			for (int i = 0; i < _syncableVariablesEnumValues.Length; i++)
			{
				SyncableVariable var = (SyncableVariable)_syncableVariablesEnumValues.GetValue(i);
				if (dirtyData.IsDirty(var))
				{
					totalSize += infos.GetSyncVarByteSize(var);
				}
			}

			return totalSize;
		}

		private void OnSyncableGameObjectDestroyed(GameObject go)
		{
			_gameObjectSyncInfos.Remove(go);
		}

		public void InvokeRegisterMethods(GameObject go, SyncableVariable varFlags, NetworkWriter writer)
		{
			for (int i = 0; i < _syncableVariablesEnumValues.Length; i++)
			{
				SyncableVariable curVar = (SyncableVariable)_syncableVariablesEnumValues.GetValue(i);
				if ((curVar & varFlags) > 0)
				{
					_gameObjectSyncInfos[go].InvokeRegisterMethod(curVar, writer);
				}
			}
		}
		public void InvokeSyncMethods(GameObject go, SyncableVariable varFlags, NetworkReader reader)
		{
			for (int i = 0; i < _syncableVariablesEnumValues.Length; i++)
			{
				SyncableVariable curVar = (SyncableVariable)_syncableVariablesEnumValues.GetValue(i);
				if ((curVar & varFlags) > 0)
				{
					_gameObjectSyncInfos[go].InvokeSyncMethod(curVar, reader);
				}
			}
		}
		public void SetDirty(GameObject go, SyncableVariable var)
		{
			_gameObjectSyncInfos[go].SetDirty(var);
		}

		private void Awake()
		{
			ComputeChannelMasks();
		}
		private void ComputeChannelMasks()
		{
			Array values = Enum.GetValues(typeof(SyncableVariable));
			_channelMasks = new SyncableVariable[CHANNEL_COUNT];
			for (int i = 0; i < values.Length; i++)
			{
				SyncableVariable var = (SyncableVariable)values.GetValue(i);
				_channelMasks[var.GetChannel()] |= var;
			}
		}

		private void LateUpdate()
		{
			HandleCheckSync();
		}
		private void HandleCheckSync()
		{
			if (isServer && NetworkServer.connections.Count <= 1)
			{
				return;
			}

			_syncCheckCounter += Time.deltaTime;
			bool[] syncFreq = new bool[]
			{
				true,//Instant
				false,//VeryShort
				false,//Shorty
				false,//Medium
				false,//Long
				false,//VeryLong
			};
			if (_syncCheckCounter > SyncFrequency.VeryShort.GetFrequencyPeriod())
			{
				_syncCheckCounter -= SyncFrequency.VeryShort.GetFrequencyPeriod();
				_syncCheckCount++;

				syncFreq[1] = true;
				for (int i = 2; i < (int)SyncFrequency.Count; i++)
				{
					if (_syncCheckCount % ((SyncFrequency)i).GetFrequencyModulo() == 0)
					{
						syncFreq[i] = true;
					}
				}
			}

			PerformSync(syncFreq);
		}
		private void PerformSync(bool[] syncFreqs)
		{
			for (int c = 0; c < CHANNEL_COUNT; c++)
			{
				List<NetworkSyncableVarsDirtyData> dirtyDatas = new List<NetworkSyncableVarsDirtyData>();
				SyncableVariable channelMask = _channelMasks[c];
				for (int f = 0; f < syncFreqs.Length; f++)
				{
					if (syncFreqs[f] == false) { continue; }

					foreach (GameObject go in _gameObjectSyncInfos.Keys)
					{
						bool hasAuthority = go.GetComponent<NetworkIdentity>().hasAuthority;

						NetworkSyncableVarsDirtyData currentDirtyData = null;
						bool goRequireSync = false;
						SyncableVariable frequencyMask = _gameObjectSyncInfos[go].FrequenciesMasks[f];
						for (int v = 0; v < _syncableVariablesEnumValues.Length; v++)
						{
							SyncableVariable var = (SyncableVariable)_syncableVariablesEnumValues.GetValue(v);
							if ((var & channelMask & frequencyMask) > 0)
							{
								if (_gameObjectSyncInfos[go].IgnoreAuthority(var) == false && hasAuthority == false)
								{
									continue;
								}
								if (_gameObjectSyncInfos[go].IsDirty(var))
								{
									if (goRequireSync == false)
									{
										goRequireSync = true;
										currentDirtyData = new NetworkSyncableVarsDirtyData(go);
										dirtyDatas.Add(currentDirtyData);
									}

									currentDirtyData.AddDirty(var);
									_gameObjectSyncInfos[go].ClearDirty(var);
								}
							}
						}
					}
				}
				if (dirtyDatas.Count > 0)
				{
					List<NetworkSyncableVarsDirtyDataChunk> dirtyDataChunks = ComputeDirtyDataChunks(dirtyDatas);
					for (int i = 0; i < dirtyDataChunks.Count; i++)
					{
						if (isServer == true)
						{
							PerformTargetSync(c, dirtyDataChunks[i]);
						}
						else
						{
							GetCmdPerformSyncMethod(c).Invoke(dirtyDataChunks[i]);
						}
					}
				}
			}
		}

		private List<NetworkSyncableVarsDirtyDataChunk> ComputeDirtyDataChunks(List<NetworkSyncableVarsDirtyData> networkSyncableVarsDirtyDatas)
		{
			List<NetworkSyncableVarsDirtyDataChunk> chunks = new List<NetworkSyncableVarsDirtyDataChunk>();
			chunks.Add(new NetworkSyncableVarsDirtyDataChunk());
			int currentByteCounter = 13;
			for (int i = 0; i < networkSyncableVarsDirtyDatas.Count; i++)
			{
				int expectedSize = ComputeExpectedSize(networkSyncableVarsDirtyDatas[i]);
				if (currentByteCounter + expectedSize > 1200)
				{
					chunks.Last().expectedSize = currentByteCounter - 13;
					chunks.Add(new NetworkSyncableVarsDirtyDataChunk());
					currentByteCounter = 13;
				}

				chunks.Last().Datas.Add(networkSyncableVarsDirtyDatas[i]);
				currentByteCounter += expectedSize;
			}

			chunks.Last().expectedSize = currentByteCounter - 13;
			return chunks;
		}



		private void PerformTargetSync(int channel, NetworkSyncableVarsDirtyDataChunk dirtyVarsToSync)
		{
			Action<NetworkConnection, NetworkSyncableVarsDirtyDataChunk> targetMethod = GetTargetPerformSyncMethod(channel);
			foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
			{
				if (conn != NetworkServer.localConnection)
				{
					targetMethod.Invoke(conn, dirtyVarsToSync);
				}
			}
		}

		private Action<NetworkSyncableVarsDirtyDataChunk> GetCmdPerformSyncMethod(int channel)
		{
			switch (channel)
			{
				case 0:
					return CmdPerformSync_Reliable;
				case 1:
					return CmdPerformSync_Unreliable;
				case 2:
					return CmdPerformSync_UnreliableNoDelay;
			}
			return null;
		}
		private Action<NetworkConnection, NetworkSyncableVarsDirtyDataChunk> GetTargetPerformSyncMethod(int channel)
		{
			switch (channel)
			{
				case 0:
					return TargetPerformSync_Reliable;
				case 1:
					return TargetPerformSync_Unreliable;
				case 2:
					return TargetPerformSync_UnreliableNoDelay;
			}
			return null;
		}
		[Command(ignoreAuthority = true, channel = 0)]
		private void CmdPerformSync_Reliable(NetworkSyncableVarsDirtyDataChunk finalVarToSync)
		{
			PerformTargetSync(0, finalVarToSync);
		}
		[Command(ignoreAuthority = true, channel = 1)]
		private void CmdPerformSync_Unreliable(NetworkSyncableVarsDirtyDataChunk finalVarToSync)
		{
			PerformTargetSync(1, finalVarToSync);
		}
		[Command(ignoreAuthority = true, channel = 2)]
		private void CmdPerformSync_UnreliableNoDelay(NetworkSyncableVarsDirtyDataChunk finalVarToSync)
		{
			PerformTargetSync(2, finalVarToSync);
		}
		[TargetRpc(channel = 0)]
		private void TargetPerformSync_Reliable(NetworkConnection conn, NetworkSyncableVarsDirtyDataChunk varsToSync)
		{

		}
		[TargetRpc(channel = 1)]
		private void TargetPerformSync_Unreliable(NetworkConnection conn, NetworkSyncableVarsDirtyDataChunk varsToSync)
		{

		}
		[TargetRpc(channel = 2)]
		private void TargetPerformSync_UnreliableNoDelay(NetworkConnection conn, NetworkSyncableVarsDirtyDataChunk varsToSync)
		{
		}

		#endregion


		[TargetRpc]
		public void TargetOnClientAddPlayer(NetworkConnection conn, GameObject playerEntity)
		{
			Debug.LogError("Register Player");
			PinouNetworkManager.OnTargetAddPlayer.Invoke(conn, playerEntity);
		}



	}
}