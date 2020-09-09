using UnityEngine;
using Pinou;

namespace Pinou.EntitySystem
{		
	public partial class EntityBody : PinouBehaviour
	{
		[Header("Sockets References")]
		[Space]
		[SerializeField] protected Transform shellSocket;
		public bool HasShellSocket => shellSocket != null;
		public Transform ShellSocket => shellSocket;

		[SerializeField] protected Transform weaponSocket;
		public bool HasWeaponSocket => weaponSocket != null;
		public Transform WeaponSocket => weaponSocket;

		[SerializeField] protected Transform auraSocket;
		public bool HasAuraSocket => auraSocket != null;
		public Transform AuraSocket => auraSocket;

		[SerializeField] protected Transform reactorSocket;
		public bool HasReactorSocket => reactorSocket != null;
		public Transform ReactorSocket => reactorSocket;


		public Transform GetSocket(EntityBodySocket socket)
		{
			switch(socket)
			{
				case EntityBodySocket.Shell:
					return shellSocket;
				case EntityBodySocket.Weapon:
					return weaponSocket;
				case EntityBodySocket.Aura:
					return auraSocket;
				case EntityBodySocket.Reactor:
					return reactorSocket;
			}
			
			throw new System.Exception("No Socket " + socket + " found.");
		}
	}
}