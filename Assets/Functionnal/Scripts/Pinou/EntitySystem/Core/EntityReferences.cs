using UnityEngine;
using Sirenix.OdinInspector;
using Pinou.InputSystem;

namespace Pinou.EntitySystem
{
	public class EntityReferences : PinouBehaviour, IEntityData
	{
        [Header("Physics")]
        [Space]
        [SerializeField] protected Transform physicsBody;
        [SerializeField] protected Rigidbody rigidBody;
        [SerializeField] protected Collider[] colliders;

        [Header("Visual")]
        [Space]
        [SerializeField] protected Transform visualBody;

        [Header("References")]
        [Space]
        [SerializeField] protected Entity master;
        [SerializeField] protected PinouInputReceiver inputReceiver;

        public PinouInputReceiver InputReceiver => inputReceiver;
        public Transform PhysicsBody => physicsBody;
        public Rigidbody RigidBody => rigidBody;
        public Collider[] Colliders => colliders;
        public Transform VisualBody => visualBody;

        #region IEntityData
        public string EntityName => master.EntityName;
        public EntityTeam Team => master.Team;
        public bool IsPlayer => master.IsPlayer;
        public bool Is2D => master.Is2D;

        public Entity Master => master;
        public EntityReferences References => this;
        public new Vector3 Position { get => master.Position; set => master.Position = value; } 
        public new Quaternion Rotation { get => master.Rotation; set => master.Rotation = value; }
        public new Vector3 Forward { get => master.Forward; }

        public Vector2 Position2D { get => master.Position2D; set => master.Position2D = value; }
        public float Rotation2D { get => master.Rotation2D; set => master.Rotation2D = value; }
        public Vector2 Forward2D { get => master.Forward2D; }

        public bool HasController => master.HasController;
		public bool HasStats => master.HasStats;
        public bool HasBeing => master.HasBeing;
        public bool HasAbilities => master.HasAbilities;
		public bool HasInteractions => master.HasInteractions;
        public bool HasMovements => master.HasMovements;
		public bool HasAnimations => master.HasAnimations;
        public bool HasVisual => master.HasVisual;
        public bool HasLoot => master.HasLoot;

        public EntityControllerData.EntityController Controller => master.Controller;
        public EntityStatsData.EntityStats Stats => master.Stats;
        public EntityEquipmentData.EntityEquipment Equipment => master.Equipment;
        public EntityBeingData.EntityBeing Being => master.Being;
        public EntityAbilitiesData.EntityAbilities Abilities => master.Abilities;
        public EntityInteractionsData.EntityInteractions Interactions => master.Interactions;
        public EntityMovementsData.EntityMovements Movements => master.Movements;
        public EntityAnimationsData.EntityAnimations Animations => master.Animations;
        public EntityVisualData.EntityVisual Visual => master.Visual;
        public EntityLootData.EntityLoot Loot => master.Loot;

        public ControllerState ControllerState => master.ControllerState;
        public BeingState BeingState => master.BeingState;
        public AbilityState AbilityState => master.AbilityState;
		public InteractionState InteractionState => master.InteractionState;
        public MovementState MovementState => master.MovementState;
        public MovementDirection MovementDirection => master.MovementDirection;
		#endregion

		#region Editor
#if UNITY_EDITOR
		protected override void E_OnValidate()
        {
            AutoFindReference(ref master);
            AutoFindReference(ref inputReceiver);
        }
#endif
#endregion


    }
}