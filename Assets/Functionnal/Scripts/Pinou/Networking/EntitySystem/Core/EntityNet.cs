#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pinou.EntitySystem;
using System.ComponentModel.Design;

namespace Pinou.Networking
{
	public class EntityNet : Entity, INetworkEntityData
	{
		#region INetworkEntityData
		public new EntityNet Master => this as EntityNet;
		public new EntityReferencesNet References => base.References as EntityReferencesNet;
		public new IControllerNet Controller => base.Controller as IControllerNet;
		public new EntityStatsNetData.EntityStatsNet Stats => base.Stats as EntityStatsNetData.EntityStatsNet;
		public new EntityEquipmentNetData.EntityEquipmentNet Equipment => base.Equipment as EntityEquipmentNetData.EntityEquipmentNet;
		public new EntityBeingNetData.EntityBeingNet Being => base.Being as EntityBeingNetData.EntityBeingNet;
		public new EntityAbilitiesNetData.EntityAbilitiesNet Abilities => base.Abilities as EntityAbilitiesNetData.EntityAbilitiesNet;
		public new EntityInteractionsNetData.EntityInteractionsNet Interactions => base.Interactions as EntityInteractionsNetData.EntityInteractionsNet;
		public new EntityMovementsNetData.EntityMovementsNet Movements => base.Movements as EntityMovementsNetData.EntityMovementsNet;
		public new EntityAnimationsNetData.EntityAnimationsNet Animations => base.Animations as EntityAnimationsNetData.EntityAnimationsNet;
		public new EntityVisualNetData.EntityVisualNet Visual => base.Visual as EntityVisualNetData.EntityVisualNet;
		public new EntityLootNetData.EntityLootNet Loot => base.Loot as EntityLootNetData.EntityLootNet;
		#endregion

		private List<AbilityCastResult> _nextSyncResults = new List<AbilityCastResult>();
		protected override void OnStart()
		{
			PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(gameObject, SyncableVariable.EntityAbilityHit, SyncFrequency.Instant, RegisterAbilityHit, SyncAbilityHit, GetSizeAbilityHit, true);
		}

		protected override void OnDeath(Entity entity, AbilityCastResult killingResult)
		{
			if (team != EntityTeam.Players)
			{
				base.OnDeath(entity, killingResult);
			}
			else
			{
				//Prevent pooling to prevent authority issues in multiplayer
				gameObject.SetActive(false);
				gameObject.Destroy(1f);
			}
		}

		private void RegisterAbilityHit(SyncableVariable var, NetworkWriter writer)
		{
			writer.WriteInt32(_nextSyncResults.Count);
			for (int i = 0; i < _nextSyncResults.Count; i++)
			{
				writer.WriteAbilityCastResult(_nextSyncResults[i]);
			}
			_nextSyncResults.Clear();
		}
		private void SyncAbilityHit(SyncableVariable var, NetworkReader reader)
		{
			if (References.NetworkIdentity.isServer)
			{
				_nextSyncResults = new List<AbilityCastResult>();
			}
			int resultsCount = reader.ReadInt32();
			for (int i = 0; i < resultsCount; i++)
			{
				AbilityCastResult result = reader.ReadAbilityCastResult();

				if (References.NetworkIdentity.isServer)
				{
					OnReceiveAbilityHit.Invoke(this, result);
					result.CastData.FillResult(result);
					_nextSyncResults.Add(result);
					PinouNetworkManager.MainBehaviour.SetDirty(gameObject, SyncableVariable.EntityAbilityHit);
				}
				else if (References.NetworkIdentity.hasAuthority)
				{
					OnReceiveAbilityHit.Invoke(this, result);
					result.CastData.FillResult(result);
				}

				if (result.CastData.Caster == this) { return; }

				OnAbilityHitResult.Invoke(this, result);
			}
		}
		private int GetSizeAbilityHit(NetworkWriter dummyWriter)
		{
			dummyWriter.WriteInt32(_nextSyncResults.Count);
			for (int i = 0; i < _nextSyncResults.Count; i++)
			{
				dummyWriter.WriteAbilityCastResult(_nextSyncResults[i]);
			}
			return dummyWriter.Length;
		}

		public override void ReceiveAbilityHit(AbilityCastResult result)
		{
			OnReceiveAbilityHit.Invoke(this, result);
			result.CastData.FillResult(result);

			OnAbilityHitResult.Invoke(this, result);

			_nextSyncResults.Add(result);

			PinouNetworkManager.MainBehaviour.SetDirty(gameObject, SyncableVariable.EntityAbilityHit);
		}
	}
}