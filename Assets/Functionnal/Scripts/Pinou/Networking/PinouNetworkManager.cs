#pragma warning disable 0649, 0414
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

namespace Pinou.Networking
{
	public class PinouNetworkManager : NetworkManager
	{
		[Header("Custom")]
		[Space]
		[SerializeField] private Transform _transportChild;
		[SerializeField] private PinouNetworkMainBehaviour _mainBehaviour;
		[SerializeField] private PinouNetworkEntityBehaviour _entityBehaviour;

		public static PinouNetworkManager Instance => ((PinouNetworkManager)singleton);

		public static PinouNetworkMainBehaviour MainBehaviour => Instance._mainBehaviour;
		public static PinouNetworkEntityBehaviour EntityBehaviour => Instance._entityBehaviour;

		public static bool IsServer => Instance._mainBehaviour.isServer;
		public static bool IsClient => Instance._mainBehaviour.isClient;

		public bool UsingSteamworks { get; private set; }
		public FizzySteamworks FizzyTransport => transport as FizzySteamworks;

		#region Events
		public override void Start()
		{
			base.Start();

			if (transport is FizzySteamworks)
			{
				HandleFizzySteamworks();
			}
		}
		private void HandleFizzySteamworks()
		{
			Debug.Log("Fizzy Steamworks Initialized.");
			UsingSteamworks = true;
		}

		public override void OnStartServer()
		{
			Debug.Log("Server Started.");
			if (UsingSteamworks == true)
			{
			}
		}

		public override void OnStopServer()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.U))
			{
				SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(FizzyTransport.SteamUserID));
			}
		}
		public override void OnServerAddPlayer(NetworkConnection conn)
		{
			GameObject go = PinouApp.Entity.CreateEntity(playerPrefab, Vector3.zero, 0f).gameObject;
			if (conn == NetworkServer.localConnection)
			{
				PinouApp.Entity.SetLocalPlayer(go.GetComponent<EntityNet>());
			}

			NetworkServer.AddPlayerForConnection(conn, go);
			if (conn != NetworkServer.localConnection)
			{
				_mainBehaviour.TargetOnClientAddPlayer(conn, go);
			}
		}

		public override void OnServerDisconnect(NetworkConnection conn)
		{
			Debug.Log("Server Disconnect");
			OnBeforeServerDisconnect.Invoke(conn);

			base.OnServerDisconnect(conn);
		}
		public override void OnStopClient()
		{
			Debug.Log("Stop Client");
			OnBeforeStopClient.Invoke();
		}
		#endregion

		#region Delegates
		public static PinouUtils.Delegate.Action<NetworkConnection> OnBeforeServerDisconnect { get; private set; } = new PinouUtils.Delegate.Action<NetworkConnection>();
		public static PinouUtils.Delegate.Action OnBeforeStopClient { get; private set; } = new PinouUtils.Delegate.Action();
		public static PinouUtils.Delegate.Action<NetworkConnection, GameObject> OnTargetAddPlayer { get; private set; } = new PinouUtils.Delegate.Action<NetworkConnection, GameObject>();
		#endregion

		#region Editor
		public override void OnValidate()
		{
			base.OnValidate();
			if (_mainBehaviour == null) { _mainBehaviour = GetComponentInChildren<PinouNetworkMainBehaviour>(); }
			if (_entityBehaviour == null) { _entityBehaviour = GetComponentInChildren<PinouNetworkEntityBehaviour>(); }
		}
		#endregion
	}
}