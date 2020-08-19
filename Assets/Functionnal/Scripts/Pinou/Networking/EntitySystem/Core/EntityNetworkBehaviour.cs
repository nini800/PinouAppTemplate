#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using Pinou.EntitySystem;
using UnityEngine;
using Mirror;

namespace Pinou.Networking
{
	public class EntityNetworkBehaviour : NetworkBehaviour
	{
		[SerializeField] private EntityNet _entity;
		public EntityNet Entity => _entity;

		#region EntityFunctions
		[Command]
		public void CmdSendAbilityHit(GameObject target, AbilityCastResult result)
		{
			target.GetComponent<EntityNet>().ReceiveAbilityHit(result);
		}
		[ClientRpc]
		public void RpcOnAbilityHitResult(AbilityCastResult result)
		{
			_entity.OnAbilityHitResult.Invoke(_entity, result);
		}
		#endregion

		#region BeingFunctions
		[ClientRpc]
		public void RpcOnDeath(AbilityCastResult result)
		{
			_entity.Being.OnDeath.Invoke(_entity, result);
		}

		[ClientRpc]
		public void RpcSyncBeingCurrentResource(int enumIndex, float curResource)
		{
			if (isServer) { return; }
			_entity.Being.SetCurrentResource((EntityBeingResourcesType)enumIndex, curResource);
		}
		#endregion
	}
}