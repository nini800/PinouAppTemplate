using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public abstract class EntityComponentData : SerializedScriptableObject
    {
		protected T ConstructComponent<T, TT>(Entity master, EntityReferences references, TT data) where T : EntityComponent, new() where TT :  EntityComponentData
        {
            T component = new T();
            component.Construct(master, references, data);
            return component;
        }

        public abstract EntityComponent ConstructComponent(Entity master, EntityReferences references);

        public abstract class EntityComponent : IEntityData, IAdvancedSlave
        {
            #region OnConstruct
            public virtual void Construct(Entity master, EntityReferences references, EntityComponentData data)
            {
                this.master = master;
                this.references = references;
                this._data = data;

                OnConstruct();
            }
            #endregion

            protected Entity master = null;
            protected EntityReferences references = null;
            private EntityComponentData _data = null;
            public EntityComponentData Data => _data;

            #region IEntityData
            public string EntityName => master.EntityName;
            public EntityTeam Team => master.Team;
            public bool IsPlayer => master.IsPlayer;
            public bool Is2D => master.Is2D;

            public Entity Master => master;
            public EntityReferences References => references;

            public Vector3 Position { get => master.Position; set => master.Position = value; }
            public Quaternion Rotation { get => master.Rotation; set => master.Rotation = value; }
            public Vector3 Forward { get => master.Forward; }

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

            public virtual ControllerState ControllerState => master.ControllerState;
            public virtual BeingState BeingState => master.BeingState;
            public virtual AbilityState AbilityState => master.AbilityState;
            public virtual InteractionState InteractionState => master.InteractionState;
            public virtual MovementState MovementState => master.MovementState;
            public virtual MovementDirection MovementDirection => master.MovementDirection;
			#endregion

			#region ISlave
			/// <summary>
			/// Do not need base.
			/// </summary>
			public virtual void SlaveAwake()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveStart()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveFixedUpdate()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveUpdate()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveAfterFixedUpdate()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveLateUpdate()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveEnabled()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveDisabled()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveDestroyed()
            {

            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveDrawGizmos()
            {
            }

            /// <summary>
            /// Do not need base.
            /// </summary>
            public virtual void SlaveDrawGizmosSelected()
            {
            }
            #endregion

            #region Events
            /// <summary>
            /// Do not need base
            /// </summary>
            protected virtual void OnConstruct()
            {

            }
            #endregion


        }
    }
}