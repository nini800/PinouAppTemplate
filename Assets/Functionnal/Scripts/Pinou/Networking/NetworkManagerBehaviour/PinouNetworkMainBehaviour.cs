﻿#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.Networking;
using Pinou.EntitySystem;
using Mirror;

namespace Pinou.Networking
{
	public class PinouNetworkMainBehaviour : NetworkBehaviour
	{
		[TargetRpc]
		public void TargetOnClientAddPlayer(NetworkConnection conn, GameObject playerEntity)
		{
			Debug.LogError("Register Player");
			PinouNetworkManager.OnTargetAddPlayer.Invoke(conn, playerEntity);
		}
	}
}