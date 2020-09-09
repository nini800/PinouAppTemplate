namespace Pinou.EntitySystem
{
	public interface IAbilityContainer
	{
		bool ContainsAbility { get; }
		AbilityData Ability { get; }
		AbilityTriggerData AbilityTriggerData { get; }
		float AbilityCooldown { get; }
	}
}
