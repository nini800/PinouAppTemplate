using Pinou.EntitySystem;

namespace Pinou.Networking
{
	public interface INetworkEntityData
	{
		EntityNet Master { get; }
		EntityReferencesNet References { get; }

		IControllerNet Controller { get; }
		EntityStatsNetData.EntityStatsNet Stats { get; }
		EntityEquipmentNetData.EntityEquipmentNet Equipment { get; }
		EntityBeingNetData.EntityBeingNet Being { get; }
		EntityAbilitiesNetData.EntityAbilitiesNet Abilities { get; }
		EntityInteractionsNetData.EntityInteractionsNet Interactions { get; }
		EntityMovementsNetData.EntityMovementsNet Movements { get; }
		EntityAnimationsNetData.EntityAnimationsNet Animations { get; }
		EntityVisualNetData.EntityVisualNet Visual { get; }
		EntityLootNetData.EntityLootNet Loot { get; }
	}
}
