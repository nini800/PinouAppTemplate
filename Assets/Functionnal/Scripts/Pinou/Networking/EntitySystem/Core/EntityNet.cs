#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public class EntityNet : Entity, INetworkEntityData
	{
		#region INetworkEntityData
		public new EntityNet Master => this as EntityNet;
		public new EntityReferencesNet References => base.References as EntityReferencesNet;
		public new EntityControllerNetData.EntityControllerNet Controller => base.Controller as EntityControllerNetData.EntityControllerNet;
		public new EntityBeingNetData.EntityBeingNet Being => base.Being as EntityBeingNetData.EntityBeingNet;
		public new EntityAbilitiesNetData.EntityAbilitiesNet Abilities => base.Abilities as EntityAbilitiesNetData.EntityAbilitiesNet;
		public new EntityMovementsNetData.EntityMovementsNet Movements => base.Movements as EntityMovementsNetData.EntityMovementsNet;
		public new EntityVisualNetData.EntityVisualNet Visual => base.Visual as EntityVisualNetData.EntityVisualNet;
		#endregion

		public override void ReceiveAbilityHit(AbilityCastResult result)
		{
			if (References.NetBehaviour.isServer)
			{
				OnReceiveAbilityHit.Invoke(this, result);
				result.CastData.FillResult(result);

				References.NetBehaviour.RpcOnAbilityHitResult(result);
			}
			else
			{
				((EntityNet)result.CastData.Caster).References.NetBehaviour.CmdSendAbilityHit(gameObject, result);
			}
		}
	}
}