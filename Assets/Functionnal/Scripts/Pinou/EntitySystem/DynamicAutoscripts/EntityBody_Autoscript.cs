using UnityEngine;
using Pinou;

namespace Pinou.EntitySystem
{		
	public partial class EntityBody : PinouBehaviour
	{
		[Header("Sockets References")]
		[Space]
		[SerializeField] protected Transform bodySocket;
		public bool HasBodySocket => bodySocket != null;
		public Transform BodySocket;

		[SerializeField] protected Transform weaponSocket;
		public bool HasWeaponSocket => weaponSocket != null;
		public Transform WeaponSocket;

		[SerializeField] protected Transform auraSocket;
		public bool HasAuraSocket => auraSocket != null;
		public Transform AuraSocket;


		public Transform GetSocket(EntityBodySocket socket)
		{
			switch(socket)
			{
				case EntityBodySocket.Body:
					return bodySocket;
				case EntityBodySocket.Weapon:
					return weaponSocket;
				case EntityBodySocket.Aura:
					return auraSocket;
			}
			
			throw new System.Exception("No Socket " + socket + " found.");
		}
	}
}