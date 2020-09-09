#pragma warning disable 0649, 0414
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pinou.EntitySystem
{
    public partial class EntityEquipmentData : EntityComponentData
    {
        #region Vars, Fields, Getters
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityEquipment, EntityEquipmentData>(master, references, this);
        }
        #endregion

        public partial class EntityEquipment : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityEquipmentData)((EntityComponent)this).Data;
            }


            private EntityEquipmentData _data = null;
            protected new EntityEquipmentData Data => _data;
			#endregion

			#region Utilities
			#endregion

			#region Behaviour
            /// <summary>
            /// Need base
            /// </summary>
			public override void SlaveStart()
			{
                BuildDefaultEquipment();
            }
            private void BuildDefaultEquipment()
            {
                Array eqValues = Enum.GetValues(typeof(EntityEquipableType));
                for (int i = 1; i < eqValues.Length; i++)
                {
                    EntityEquipableType eqType = (EntityEquipableType)i;
                    EntityEquipableBuilder builder;
                    if (builder = _data.GetDefaultEquipableBuilder(eqType))
                    {
                        int eqLevel = (HasStats ? (Stats.HasMainLevel ? Stats.MainExperience.Level : 1) : 1);
                        EntityEquipable eq = builder.BuildEquipable(eqLevel);
                        Equip(eq);
                    }
                }
            }
			#endregion

			#region Utilities
            public virtual void Equip(EntityEquipable eq)
			{
                if (eq == null) { return; }

                if (GetEquipped(eq.Type) != null)
				{
                    Unequip(eq.Type);
				}

                SetEquipped(eq.Type, eq);
                OnEquip.Invoke(master, eq);
			}
            public virtual void Unequip(EntityEquipable eq)
			{
                if (GetEquipped(eq.Type) == eq)
				{
                    SetEquipped(eq.Type, null);
                    OnUnequip.Invoke(master, eq);
                }
            }
            public virtual void Unequip(EntityEquipableType type)
			{
                EntityEquipable oldEq = GetEquipped(type);
                if (oldEq != null)
				{
                    SetEquipped(type, null);
                    OnUnequip.Invoke(master, oldEq);
                }
			}
			#endregion

			#region Events
			public PinouUtils.Delegate.Action<Entity, EntityEquipable> OnEquip { get; private set; } = new PinouUtils.Delegate.Action<Entity, EntityEquipable>();
            public PinouUtils.Delegate.Action<Entity, EntityEquipable> OnUnequip { get; private set; } = new PinouUtils.Delegate.Action<Entity, EntityEquipable>();
			#endregion
		}
	}
}