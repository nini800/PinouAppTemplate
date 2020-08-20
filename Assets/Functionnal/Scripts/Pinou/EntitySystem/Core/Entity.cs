using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TAP;
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
		[SerializeField] protected EntityBeingData beingData;
		[SerializeField] protected EntityAbilitiesData abilitiesData;
		[SerializeField] protected EntityMovementsData movementsData;
		[SerializeField] protected EntityVisualData visualData;

		[Header("References")]
		[Space]
		[SerializeField] protected EntityReferences references;

		protected List<EntityComponentData.EntityComponent> components = new List<EntityComponentData.EntityComponent>();
		protected EntityControllerData.EntityController controller = null;
		protected EntityStatsData.EntityStats stats = null;
		protected EntityBeingData.EntityBeing being = null;
		protected EntityAbilitiesData.EntityAbilities abilities = null;
		protected EntityInteractionsData.EntityInteractions interactions;
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
			CreateComponent(beingData,			ref being);
			CreateComponent(abilitiesData,		ref abilities);
			CreateComponent(movementsData,		ref movements);
			CreateComponent(visualData,			ref visual);
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
			for (int i = 0; i < components.Count; i++)
			{
				components[i].SlaveEnabled();
			}

			if (HasBeing)
            {
				being.OnDeath.Subscribe(OnDeath);
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
		public float Rotation2D { get => transform.eulerAngles.z; set => transform.rotation = Quaternion.Euler(0f,0f,value); }
		public Vector2 Forward2D { get => transform.right; }

		public bool HasController => controller != null;
        public bool HasStats => stats != null;
        public bool HasBeing => being != null;
		public bool HasAbilities => abilities != null;
        public bool HasInteractions => interactions != null;
		public bool HasMovements => movements != null;
        public bool HasAnimations => animations != null;
		public bool HasVisual => visual != null;
		public bool HasLoot => loot != null;

        public EntityControllerData.EntityController Controller => controller;
        public EntityStatsData.EntityStats Stats => stats;
        public EntityBeingData.EntityBeing Being => being;
		public EntityAbilitiesData.EntityAbilities Abilities => abilities;
		public EntityInteractionsData.EntityInteractions Interactions => interactions;
		public EntityMovementsData.EntityMovements Movements => movements;
		public EntityAnimationsData.EntityAnimations Animations => animations;
		public EntityVisualData.EntityVisual Visual => visual;
		public EntityLootData.EntityLoot Loot => loot;

		public ControllerState ControllerState => controller != null ? controller.ControllerState : ControllerState.None;
		public BeingState BeingState => being != null ? being.BeingState : BeingState.None;
		public AbilityState AbilityState => abilities != null ? abilities.AbilityState : AbilityState.None;
		public InteractionState InteractionState => interactions != null ? interactions.InteractionState : InteractionState.None;
		public MovementState MovementState => movements != null ? movements.MovementState : MovementState.None;
		public MovementDirection MovementDirection => movements != null ? movements.MovementDirection : MovementDirection.None;
		#endregion

		#region Utilities
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

		private void OnDeath(Entity entity, AbilityCastResult killingResult)
        {
			Destroy(gameObject);
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

		[Header("Editor Only")]
		[Space]
		[SerializeField, ShowIf("@(this.controllerData as EntityEnemyControllerData != null)")] public bool E_PreviewViewRange = false;
        [SerializeField, ShowIf("@E_PreviewViewRange == true && (this.controllerData as EntityEnemyControllerData != null)")] public Entity E_rangeTester;
		[SerializeField, ShowIf("@E_PreviewViewRange == true && (this.controllerData as EntityEnemyControllerData != null)"), MinValue(10), Max(10000)] public int E_PreviewViewRangeSteps = 1000;
		[SerializeField, ShowIf("@(this.controllerData as EntityEnemyControllerData != null)")] public bool E_PreviewTarget = false;
#endif
		#endregion
	}
}