using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou;
using Pinou.EntitySystem;
using Mirror;

namespace Pinou.Networking
{
	[CreateAssetMenu(fileName = "EntityNetData", menuName = "Pinou/Network/EntityNetData", order = 1000)]
	public class PinouEntityManagerNetData : PinouEntityManagerData
	{
		public override PinouManager BuildManagerInstance()
		{
			return new PinouEntityManagerNet(this);
		}

		public class PinouEntityManagerNet : PinouEntityManager
		{
			#region OnConstruct
			public PinouEntityManagerNet(PinouEntityManagerNetData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouEntityManagerNetData Data { get; private set; }
			#endregion

			public override void SlaveStart()
			{
				base.SlaveStart();
				PinouNetworkManager.OnBeforeServerDisconnect.Subscribe(OnBeforeServerDisconnect);
				PinouNetworkManager.OnBeforeStopClient.Subscribe(OnBeforeStopClient);
				PinouNetworkManager.OnTargetAddPlayer.Subscribe(OnTargetAddPlayer);
			}

			private void OnTargetAddPlayer(NetworkConnection conn, GameObject playerPrefab)
			{
				RegisterEntity(playerPrefab.GetComponent<Entity>());
			}

			private void OnBeforeServerDisconnect(NetworkConnection conn)
			{
				foreach (NetworkIdentity clientOwnedObject in conn.clientOwnedObjects)
				{
					Entity ent;
					if (ent = clientOwnedObject.GetComponent<Entity>())
					{
						PinouApp.Entity.DestroyEntity(ent);
					}
				}
			}

			private void OnBeforeStopClient()
			{
				ClearEntities();
			}

			public override Entity CreateEntity(GameObject entityModel, Vector3 pos, float startRotation)
			{
				if (PinouNetworkManager.IsServer == false) { return null; }

				Entity ent = base.CreateEntity(entityModel, pos, startRotation);
				NetworkServer.Spawn(ent.gameObject);
				if (ent.Team != EntityTeam.Players)
				{
					ent.GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkServer.localConnection);
				}

				PinouNetworkManager.EntityBehaviour.RpcCreateEntity(ent.gameObject);

				return ent;
			}
		}
	}
}
