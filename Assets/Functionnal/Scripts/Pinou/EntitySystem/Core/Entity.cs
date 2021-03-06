﻿using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.PostProcessing;

namespace Pinou.EntitySystem
{
	public class Entity : PinouBehaviour, IEntityData
	{
		#region Vars, Fields & Getters
		[Header("Identity")]
		[Space]
		[SerializeField] protected string entityName;
		[SerializeField] protected EntityTeam team;

		[Header("Components")]
		[Space]
		[SerializeField] protected EntityControllerData controllerData;
		[SerializeField] protected EntityStatsData statsData;
		[SerializeField] protected EntityEquipmentData equipmentData;
		[SerializeField] protected EntityBeingData beingData;
		[SerializeField] protected EntityAbilitiesData abilitiesData;
		[SerializeField] protected EntityInteractionsData interactionsData;
		[SerializeField] protected EntityMovementsData movementsData;
		[SerializeField] protected EntityAnimationsData animationsData;
		[SerializeField] protected EntityVisualData visualData;
		[SerializeField] protected EntityLootData lootData;

		[Header("References")]
		[Space]
		[SerializeField] protected EntityReferences references;

		protected List<EntityComponentData.EntityComponent> components = new List<EntityComponentData.EntityComponent>();
		protected EntityControllerData.EntityController controller = null;
		protected EntityStatsData.EntityStats stats = null;
		protected EntityEquipmentData.EntityEquipment equipment = null;
		protected EntityBeingData.EntityBeing being = null;
		protected EntityAbilitiesData.EntityAbilities abilities = null;
		protected EntityInteractionsData.EntityInteractions interactions = null;
		protected EntityMovementsData.EntityMovements movements = null;
		protected EntityAnimationsData.EntityAnimations animations = null;
		protected EntityVisualData.EntityVisual visual = null;
		protected EntityLootData.EntityLoot loot = null;
		#endregion

		#region Behaviour
		protected override void OnAwake()
        {
			CreateComponents();
            for (int i = 0; i < components.Count; i++)
            {
				components[i].SlaveAwake();
            }
        }
		#region Components Creation
		private void CreateComponents()
        {
			CreateComponent(controllerData,		ref controller);
			CreateComponent(statsData,			ref stats);
			CreateComponent(equipmentData,		ref equipment);
			CreateComponent(beingData,			ref being);
			CreateComponent(abilitiesData,		ref abilities);
			CreateComponent(interactionsData,	ref interactions);
			CreateComponent(movementsData,		ref movements);
			CreateComponent(animationsData,		ref animations);
			CreateComponent(visualData,			ref visual);
			CreateComponent(lootData,			ref loot);
		}
		private void CreateComponent<T, TT>(T componentData, ref TT component) where T : EntityComponentData where TT : EntityComponentData.EntityComponent
        {
			if (componentData == null)
            {
				return;
            }
			component = (TT)componentData.ConstructComponent(this, references);
			components.Add(component);
        }
        #endregion

        protected override void OnSafeStart()
        {
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveStart();
			}
		}

		private void FixedUpdate()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveFixedUpdate();
			}
		}
        protected override void OnAfterFixedUpdate()
        {
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveAfterFixedUpdate();
			}
		}
        private void Update()
        {
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveUpdate();
			}
        }

        private void LateUpdate()
        {
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveLateUpdate();
			}
		}
        protected override void OnEnabled()
        {
			if (components.Count <= 0)
			{
				OnAwake();
				OnSafeStart();
			}
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveEnabled();
			}

			if (HasBeing)
            {
				being.OnDeath.Subscribe(OnDeath, -100);
            }
		}
		protected override void OnDisabled()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveDisabled();
			}

			if (HasBeing)
            {
				being.OnDeath.Unsubscribe(OnDeath);
			}
		}
		protected override void OnDestroyed()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveDestroyed();
			}
		}
		#endregion

		#region IEntityData
		public string EntityName => entityName;
		public EntityTeam Team => team;
		public bool IsPlayer => team == EntityTeam.Players;
		public bool Is2D => PinouApp.Entity.Mode2D;

		public Entity Master => this;
		public EntityReferences References => references;
		public new Vector3 Position 
		{ 
			get 
			{ 
				if (HasMovements == false)
                {
					return references.PhysicsBody.position;
                }
				else
                {
					return references.PhysicsBody.position;
					//return _references.RigidBody.position;--This one causes sluttering
				}
			}
			set
			{
				if (HasMovements == false)
				{
					references.PhysicsBody.position = value;
				}
				else
				{
					references.RigidBody.MovePosition(value);
				}
			}
		}
		public new Quaternion Rotation { get => references.VisualBody.rotation; set => references.VisualBody.rotation = value; }
		public new Vector3 Forward { get => references.VisualBody.forward; }

		public Vector2 Position2D { get => Position.ToV2(); set => Position = value.ToV3(); }
		public float Rotation2D { get => references.VisualBody.eulerAngles.z; set => references.VisualBody.rotation = Quaternion.Euler(0f,0f,value); }
		public Vector2 Forward2D { get => references.VisualBody.right; }

		public bool HasController => controller != null;
        public bool HasStats => stats != null;
        public bool HasEquipment => equipment != null;
		public bool HasBeing => being != null;
		public bool HasAbilities => abilities != null;
        public bool HasInteractions => interactions != null;
		public bool HasMovements => movements != null;
        public bool HasAnimations => animations != null;
		public bool HasVisual => visual != null;
		public bool HasLoot => loot != null;

        public EntityControllerData.EntityController Controller => controller;
        public EntityStatsData.EntityStats Stats => stats;
        public EntityEquipmentData.EntityEquipment Equipment => equipment;
		public EntityBeingData.EntityBeing Being => being;
		public EntityAbilitiesData.EntityAbilities Abilities => abilities;
		public EntityInteractionsData.EntityInteractions Interactions => interactions;
		public EntityMovementsData.EntityMovements Movements => movements;
		public EntityAnimationsData.EntityAnimations Animations => animations;
		public EntityVisualData.EntityVisual Visual => visual;
		public EntityLootData.EntityLoot Loot => loot;

		[Title("Entity States")]
		[ShowInInspector] public ControllerState ControllerState => controller != null ? controller.ControllerState : ControllerState.None;
		[ShowInInspector] public BeingState BeingState => being != null ? being.BeingState : BeingState.None;
		[ShowInInspector] public AbilityState AbilityState => abilities != null ? abilities.AbilityState : AbilityState.None;
		[ShowInInspector] public InteractionState InteractionState => interactions != null ? interactions.InteractionState : InteractionState.None;
		[ShowInInspector] public MovementState MovementState => movements != null ? movements.MovementState : MovementState.None;
		[ShowInInspector] public MovementDirection MovementDirection => movements != null ? movements.MovementDirection : MovementDirection.None;
		#endregion

		#region Utilities
		private void ResetEntity()
		{
			components.Clear();
			transform.position = Vector3.zero;
			references.PhysicsBody.localPosition = Vector3.zero;
			references.VisualBody.localPosition = Vector3.zero;
		}
		public virtual void ReceiveAbilityHit(AbilityCastResult result)
		{
			OnReceiveAbilityHit.Invoke(this, result);

			result.CastData.FillResult(result);
			OnAbilityHitResult.Invoke(this, result);
		}
		#endregion

		#region Events

		/// <summary>
		/// Component event
		/// </summary>
		public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnReceiveAbilityHit { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();
		/// <summary>
		/// After component event
		/// </summary>
		public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnAbilityHitResult { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();

		public PinouUtils.Delegate.Action<Entity> OnRemovedByManager { get; private set; } = new PinouUtils.Delegate.Action<Entity>();

		protected virtual void OnDeath(Entity entity, AbilityCastResult killingResult)
        {
			PinouApp.Pooler.Store(gameObject);

			ResetEntity();
        }
		#endregion

		#region Editor
#if UNITY_EDITOR
		/// <summary>
		/// Need base
		/// </summary>
		protected override void E_OnValidate()
        {
			AutoFindReference(ref references);
        }
		/// <summary>
		/// Need base
		/// </summary>
        protected override void E_OnDrawGizmos()
        {
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveDrawGizmos();
			}
		}
		/// <summary>
		/// Need base
		/// </summary>
		protected override void E_OnDrawGizmosSelected()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveDrawGizmosSelected();
			}
		}


		//AI Editor vars
		private const string AI_Cond = "E_AIController != null";
		private const string AI_Cond_Full = "@E_AIController != null";
		private EntityEnemyControllerData.EntityEnemyController E_AIController => (controller as EntityEnemyControllerData.EntityEnemyController);

		[Title("AI")]
		[Title("Behaviour")]
		[PropertySpace]
		[ShowIf(AI_Cond_Full)]
		[ShowInInspector] private Entity E_AITarget => E_AIController != null ? E_AIController.Target : default;
		[ShowIf(AI_Cond_Full)]
		[ShowInInspector] private float E_NoTarget_NextMoveTime => E_AIController != null ? E_AIController.NoTarget_NextMoveTime : default;
		[ShowIf(AI_Cond_Full)]
		[ShowInInspector] private float E_NoTarget_NextWaitTime => E_AIController != null ? E_AIController.NoTarget_NextWaitTime : default;
		[ShowIf(AI_Cond_Full)]
		[ShowInInspector] private Vector3 E_NoTarget_NextPosition => E_AIController != null ? E_AIController.NoTarget_NextPosition : default;

		[Title("Range Detection")]
		[Space]
		[SerializeField, ShowIf(AI_Cond_Full)] public bool E_PreviewViewRange = false;
		[SerializeField, ShowIf("@E_PreviewViewRange == true && " + AI_Cond)] public Entity E_rangeTester;
		[SerializeField, ShowIf("@E_PreviewViewRange == true && " + AI_Cond), MinValue(10), Max(10000)] public int E_PreviewViewRangeSteps = 1000;
		[SerializeField, ShowIf(AI_Cond_Full)] public bool E_PreviewTarget = false;
#endif
		#endregion
	}
}