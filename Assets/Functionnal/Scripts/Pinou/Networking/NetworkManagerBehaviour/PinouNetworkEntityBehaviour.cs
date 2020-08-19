#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.Networking;
using Pinou.EntitySystem;
using Mirror;

namespace Pinou.Networking
{
	public class PinouNetworkEntityBehaviour : NetworkBehaviour
	{
		[ClientRpc]
		public void RpcCreateEntity(GameObject entity)
		{
			if (PinouNetworkManager.IsServer == true) { return; }

			PinouApp.Entity.RegisterEntity(entity.GetComponent<Entity>());
		}
	}
}