using Pinou.EntitySystem;
using TAP;

namespace Pinou.Networking
{
	public interface INetworkEntityData
	{
		EntityNet Master { get; }
		EntityReferencesNet References { get; }

		EntityControllerNetData.EntityControllerNet Controller { get; }
		EntityBeingNetData.EntityBeingNet Being { get; }
		EntityAbilitiesNetData.EntityAbilitiesNet Abilities { get; }
		EntityMovementsNetData.EntityMovementsNet Movements { get; }
		EntityVisualNetData.EntityVisualNet Visual { get; }
	}
}
