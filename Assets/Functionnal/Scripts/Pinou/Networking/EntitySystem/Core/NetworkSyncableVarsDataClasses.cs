using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.Networking
{
	public class NetworkSyncableVarsDirtyDataChunk
	{
		public int expectedSize = 0;
		public List<NetworkSyncableVarsDirtyData> Datas = new List<NetworkSyncableVarsDirtyData>();
	}
	public class NetworkSyncableVarsDirtyData
	{
		public NetworkSyncableVarsDirtyData(GameObject go)
		{
			_gameObject = go;
			_dirtyVars = 0;
		}

		private GameObject _gameObject;
		private SyncableVariable _dirtyVars;

		public GameObject GameObject => _gameObject;
		public SyncableVariable DirtyVars => _dirtyVars;

		public void AddDirty(SyncableVariable var)
		{
			_dirtyVars |= var;
		}

		public bool IsDirty(SyncableVariable syncableVariable)
		{
			return (_dirtyVars & syncableVariable) > 0;
		}
	}

	public class NetworkSyncableVarMethods
	{
		public NetworkSyncableVarMethods(SyncableVariable var, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod)
		{
			_syncVar = var;
			_registerMethod = registerMethod;
			_syncMethod = syncMethod;
		}

		private SyncableVariable _syncVar;
		private Action<SyncableVariable, NetworkWriter> _registerMethod;
		private Action<SyncableVariable, NetworkReader> _syncMethod;

		public void InvokeRegisterMethod(NetworkWriter writer)
		{
			_registerMethod.Invoke(_syncVar, writer);
		}
		public void InvokeSyncMethod(NetworkReader reader)
		{
			_syncMethod.Invoke(_syncVar, reader);
		}
	}

	public class NetworkSyncableGameobjectInfos
	{
		public NetworkSyncableGameobjectInfos(GameObject go)
		{
			_gameObject = go;
			_frequenciesMasks = new SyncableVariable[Enum.GetValues(typeof(SyncFrequency)).Length];
			_varIgnoresAuthority = new Dictionary<SyncableVariable, bool>();
			_varSizes = new int[Enum.GetValues(typeof(SyncableVariable)).Length];
			_methods = new Dictionary<SyncableVariable, NetworkSyncableVarMethods>();
			_varSizesMethods = new Dictionary<SyncableVariable, Func<NetworkWriter, int>>();
		}

		private SyncableVariable[] _frequenciesMasks;
		private SyncableVariable _dirtyVars;
		private GameObject _gameObject;
		private Dictionary<SyncableVariable, NetworkSyncableVarMethods> _methods;
		private Dictionary<SyncableVariable, bool> _varIgnoresAuthority;
		private int[] _varSizes;
		private Dictionary<SyncableVariable, Func<NetworkWriter, int>> _varSizesMethods;
		NetworkWriter dummyWriter = new NetworkWriter();

		public SyncableVariable[] FrequenciesMasks => _frequenciesMasks;
		public GameObject GameObject => _gameObject;

		public void RegisterVar(SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod) => RegisterVar(var, frequency, registerMethod, syncMethod, null, false);
		public void RegisterVar(SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod, bool ignoreAuthority) => RegisterVar(var, frequency, registerMethod, syncMethod, null, ignoreAuthority);
		public void RegisterVar(SyncableVariable var, SyncFrequency frequency, Action<SyncableVariable, NetworkWriter> registerMethod, Action<SyncableVariable, NetworkReader> syncMethod, Func<NetworkWriter, int> byteSizeMethod, bool ignoreAuthority)
		{
			if (_varIgnoresAuthority.ContainsKey(var)) { _varIgnoresAuthority.Remove(var); }
			if (_methods.ContainsKey(var)) { _methods.Remove(var); }
			if (_varSizesMethods.ContainsKey(var)) { _varSizesMethods.Remove(var); }

			_varIgnoresAuthority[var] = ignoreAuthority;
			_methods.Add(var, new NetworkSyncableVarMethods(var, registerMethod, syncMethod));

			_frequenciesMasks[(int)frequency] |= var;

			if (byteSizeMethod == null)
			{
				BakeVarSize(var, registerMethod);
			}
			else
			{
				_varSizesMethods.Add(var, byteSizeMethod);
			}
		}
		private void BakeVarSize(SyncableVariable var, Action<SyncableVariable, NetworkWriter> registerMethod)
		{
			dummyWriter.Reset();
			registerMethod.Invoke(var, dummyWriter);

			_varSizes[var.GetIndex()] = dummyWriter.Length;
		}

		public int GetSyncVarByteSize(SyncableVariable var)
		{
			if (_varSizesMethods.ContainsKey(var))
			{
				dummyWriter.Reset();
				return _varSizesMethods[var].Invoke(dummyWriter);
			}
			else
			{
				return _varSizes[var.GetIndex()];
			}
		}

		public void InvokeRegisterMethod(SyncableVariable var, NetworkWriter writer)
		{
			_methods[var].InvokeRegisterMethod(writer);
		}
		public void InvokeSyncMethod(SyncableVariable var, NetworkReader reader)
		{
			_methods[var].InvokeSyncMethod(reader);
		}

		public void SetDirty(SyncableVariable var)
		{
			_dirtyVars |= var;
		}
		public bool IsDirty(SyncableVariable var)
		{
			return (var & _dirtyVars) > 0;
		}
		public void ClearDirty(SyncableVariable var)
		{
			_dirtyVars &= ~var;
		}
		public bool IgnoreAuthority(SyncableVariable var)
		{
			if (_varIgnoresAuthority.ContainsKey(var) == false) { return false; }
			return _varIgnoresAuthority[var];
		}

	}
}
