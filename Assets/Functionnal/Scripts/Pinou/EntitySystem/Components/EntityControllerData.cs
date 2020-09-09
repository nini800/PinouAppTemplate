using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityControllerData : EntityComponentData
	{
        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityController, EntityControllerData>(master, references, this);
        }
        #endregion

        public class EntityController : EntityComponent, IController
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityControllerData)((EntityComponent)this).Data;
            }

            private EntityControllerData _data = null;
            protected new EntityControllerData Data => _data;
            #endregion

            public override ControllerState ControllerState => ControllerState.None;

            #region IController
            protected bool inputingMovement;
            protected Vector3 moveVector;

            protected Vector3 aimDirection;
            protected Vector3 aimTarget;

            public bool InputingMovement => inputingMovement;
            public Vector3 MoveVector => moveVector;

            public Vector3 AimDirection => aimDirection;
            public Vector3 AimTarget => aimTarget;

            protected bool shoot;
            protected bool shootHeld;
            public bool Shoot => shoot;
            public bool ShootHeld => shootHeld;

            protected bool interact;
            public bool Interact => interact;
            #endregion
        }
    }
}