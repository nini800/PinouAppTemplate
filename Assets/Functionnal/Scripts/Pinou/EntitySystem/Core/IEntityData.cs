using UnityEngine;

namespace Pinou.EntitySystem
{
	public interface IOverridableEntityDataComponents
	{
		Entity Master { get; }
		EntityReferences References { get; }

		EntityControllerData.EntityController Controller { get; }
		EntityBeingData.EntityBeing Being { get; }
		EntityAbilitiesData.EntityAbilities Abilities { get; }
		EntityMovementsData.EntityMovements Movements { get; }
		EntityVisualData.EntityVisual Visual { get; }
	}
	public interface IEntityBaseData
	{
		string EntityName { get; }
		EntityTeam Team { get; }
		bool IsPlayer { get; }
		bool Is2D { get; }

		bool HasController { get; }
		bool HasBeing { get; }
		bool HasAbilities { get; }
		bool HasMovements { get; }
		bool HasVisual { get; }

		ControllerState ControllerState { get; }
		BeingState BeingState { get; }
		AbilityState AbilityState { get; }
		MovementState MovementState { get; }
		MovementDirection MovementDirection { get; }

		Vector3 Position { get; set; }
		Quaternion Rotation { get; set; }
		Vector3 Forward { get; }

		Vector2 Position2D { get; set; }
		float Rotation2D { get; set; }
		Vector2 Forward2D { get; }
	}
	public interface IEntityData : IEntityBaseData, IOverridableEntityDataComponents
	{

	}
}