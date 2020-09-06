using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou;
using Pinou.EntitySystem;
using Mirror;
using System.Threading.Tasks;

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

			protected Entity localPlayer = null;

			public override Entity Player => localPlayer;
			public Entity LocalPlayer => localPlayer;
			public void SetLocalPlayer(EntityNet newPlayer)
			{
				localPlayer = newPlayer;
				OnLocalPlayerAssigned.Invoke(newPlayer);
			}

			#region Behaviour
			public override void SlaveStart()
			{
				base.SlaveStart();
				PinouNetworkManager.OnBeforeServerDisconnect.Subscribe(OnBeforeServerDisconnect);
				PinouNetworkManager.OnBeforeStopClient.Subscribe(OnBeforeStopClient);
				PinouNetworkManager.OnTargetAddPlayer.Subscribe(OnTargetAddPlayer);
			}
			#endregion

			#region Utilities
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

			protected async override void HandleRespawnPlayer(Entity ent)
			{
				if (PinouNetworkManager.IsServer == false) { return; }
				NetworkConnection conn = (ent as EntityNet).References.NetworkIdentity.connectionToClient;
				await Task.Delay(2000);
				Vector3 spawnPos = default;
				if (PinouApp.Entity.Players.Length > 0)
				{
					float randAngle = Random.Range(0f, Mathf.PI * 2f);
					Vector3 offset = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) * 3f;
					if (PinouApp.Entity.Mode2D == false) { offset = offset.SwapYZ(); }
					spawnPos = PinouApp.Entity.Players.Random().Position + offset;
				}
				EntityNet newPlayer = (EntityNet)CreateEntity(PinouApp.Resources.Data.Entities.PlayerModel, spawnPos, default);
				newPlayer.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
				if (conn == NetworkServer.localConnection) { SetLocalPlayer(newPlayer); }
				else
				{
					PinouNetworkManager.EntityBehaviour.TargetSetLocalPlayer(conn, newPlayer.gameObject);
				}
			}
			#endregion

			#region Events
			private void OnTargetAddPlayer(NetworkConnection conn, GameObject playerPrefab)
			{
				EntityNet ent = playerPrefab.GetComponent<EntityNet>();
				RegisterEntity(ent);
				SetLocalPlayer(ent);
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
			#endregion

			public PinouUtils.Delegate.Action<Entity> OnLocalPlayerAssigned { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
		}
	}
}
