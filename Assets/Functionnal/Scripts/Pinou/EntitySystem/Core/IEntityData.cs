using UnityEngine;

namespace Pinou.EntitySystem
{
	public interface IOverridableEntityDataComponents
	{
		Entity Master { get; }
		EntityReferences References { get; }

		EntityControllerData.EntityController Controller { get; }
		EntityStatsData.EntityStats Stats { get; }
		EntityEquipmentData.EntityEquipment Equipment { get; }
		EntityBeingData.EntityBeing Being { get; }
		EntityAbilitiesData.EntityAbilities Abilities { get; }
		EntityInteractionsData.EntityInteractions Interactions { get; }
		EntityMovementsData.EntityMovements Movements { get; }
		EntityAnimationsData.EntityAnimations Animations { get; }
		EntityVisualData.EntityVisual Visual { get; }
		EntityLootData.EntityLoot Loot { get; }
	}
	public interface IEntityBaseData
	{
		string EntityName { get; }
		EntityTeam Team { get; }
		bool IsPlayer { get; }
		bool Is2D { get; }

		bool HasController { get; }
		bool HasStats { get; }
		bool HasEquipment { get; }
		bool HasBeing { get; }
		bool HasAbilities { get; }
		bool HasInteractions { get; }
		bool HasMovements { get; }
		bool HasAnimations { get; }
		bool HasVisual { get; }
		bool HasLoot { get; }

		ControllerState ControllerState { get; }
		BeingState BeingState { get; }
		AbilityState AbilityState { get; }
		InteractionState InteractionState { get; }
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

	public interface IController
	{
		bool InputingMovement { get; }
		Vector3 MoveVector { get; }

		Vector3 AimDirection { get; }
		Vector3 AimTarget { get; }
		bool Shoot { get; }
		bool ShootHeld { get; }
	}
}