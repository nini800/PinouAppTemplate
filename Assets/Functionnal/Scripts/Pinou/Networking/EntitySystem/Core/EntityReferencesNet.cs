#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public class EntityReferencesNet : EntityReferences
	{
		[Header("Network")]
		[Space]
		[SerializeField] private EntityNetworkBehaviour _netBehaviour;

		public EntityNetworkBehaviour NetBehaviour => _netBehaviour;
	}
}