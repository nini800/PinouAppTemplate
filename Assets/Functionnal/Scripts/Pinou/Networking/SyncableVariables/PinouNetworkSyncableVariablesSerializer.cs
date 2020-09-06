using System.Collections.Generic;
using Mirror;
using System;
using UnityEngine;

namespace Pinou.Networking
{
    public static class PinouNetworkSyncableVariablesSerializer
    {
        public static void WriteNetworkSyncableVarsDirtyDataChunk(this NetworkWriter writer, NetworkSyncableVarsDirtyDataChunk dataChunk)
		{
            writer.WriteInt32(dataChunk.Datas.Count);
			for (int i = 0; i < dataChunk.Datas.Count; i++)
			{
                writer.WriteNetworkSyncableVarsDirtyData(dataChunk.Datas[i]);
            }
        }
        public static NetworkSyncableVarsDirtyDataChunk ReadNetworkSyncableVarsDirtyDataChunk(this NetworkReader reader)
		{
            int dataCount = reader.ReadInt32();
            NetworkSyncableVarsDirtyDataChunk chunk = new NetworkSyncableVarsDirtyDataChunk();
			for (int i = 0; i < dataCount; i++)
			{
                chunk.Datas.Add(reader.ReadNetworkSyncableVarsDirtyData());
			}
            return chunk;
		}
        public static void WriteNetworkSyncableVarsDirtyData(this NetworkWriter writer, NetworkSyncableVarsDirtyData data)
        {
            writer.WriteGameObject(data.GameObject);
            writer.WriteInt64((long)data.DirtyVars);

            PinouNetworkManager.MainBehaviour.InvokeRegisterMethods(data.GameObject, data.DirtyVars, writer);
        }
        public static NetworkSyncableVarsDirtyData ReadNetworkSyncableVarsDirtyData(this NetworkReader reader)
        {
            GameObject gameObject = reader.ReadGameObject();
            SyncableVariable dirtyVars = (SyncableVariable)reader.ReadInt64();

            if (gameObject != null)
            {
                PinouNetworkManager.MainBehaviour.InvokeSyncMethods(gameObject, dirtyVars, reader);
            }

            return new NetworkSyncableVarsDirtyData(gameObject);
        }
    }
}

