#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityVisualData : EntityComponentData
	{
        public enum RotationMethod
		{
            TowardMoveInput,
            TowardAim,
            TowardVelocity
		}
        #region Fields, Getters
        [Header("Parameters")]
        [Space]
        [SerializeField] protected RotationMethod defaultRotationMethod;
        [SerializeField] protected float rotationSpeed = 600f;
        [Header("FXs")]
        [Space]
        [SerializeField] private AbilityVisualData.AbilityVisualFX _deathFX;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityVisual, EntityVisualData>(master, references, this);
        }
        #endregion

        public class EntityVisual : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityVisualData)((EntityComponent)this).Data;
            }

            private EntityVisualData _data = null;
            public new EntityVisualData Data => _data;
            #endregion

            #region Vars, Getters
            protected float currentBodyAngle = 0f;
            protected float currentBodyTargetAngle = 0f;
			#endregion

			#region Behaviour
			/// <summary>
			/// Need base.
			/// </summary>
			public override void SlaveUpdate()
            {
                HandleVisualStickToBody();
                HandleRotateBody(_data.defaultRotationMethod);
            }
            private void HandleVisualStickToBody()
            {
                references.VisualBody.position = Position;
            }
            protected virtual void HandleRotateBody(RotationMethod method)
			{
				switch (method)
				{
					case RotationMethod.TowardMoveInput:
                        HandleRotateTowardMoveInput();
                        break;
					case RotationMethod.TowardAim:
                        HandleRotateTowardAim();
						break;
                    case RotationMethod.TowardVelocity:
                        HandleRotateTowardVelocity();
						break;
                }
			}
            protected virtual void HandleRotateTowardMoveInput()
			{
                RotateTowardsDirection(Controller.MoveVector.normalized, Mathf.Clamp(Controller.MoveVector.magnitude, 0f, 1f) * _data.rotationSpeed);
            }
            protected virtual void HandleRotateTowardAim()
            {
                RotateTowardsDirection(Controller.AimDirection, _data.rotationSpeed);
            }
            protected virtual void HandleRotateTowardVelocity()
            {

            }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveEnabled()
            {
                master.OnAbilityHitResult.Subscribe(OnAbilityHitResult);
                if (HasBeing)
                {
                    Being.OnDeath.Subscribe(OnDeath);
                }
                if (HasAbilities)
                {
                    Abilities.OnPerformAbility.Subscribe(OnPerformAbility);
                }
            }
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveDisabled()
            {
                master.OnAbilityHitResult.Unsubscribe(OnAbilityHitResult);
                if (HasBeing)
                {
                    Being.OnDeath.Unsubscribe(OnDeath);
                }
                if (HasAbilities)
                {
                    Abilities.OnPerformAbility.Unsubscribe(OnPerformAbility);
                }
            }
			#endregion

			#region Utilities
			protected virtual void RotateTowardsDirection(Vector3 direction, float speed)
            {
                if (PinouApp.Entity.Mode2D == false)
				{
                    if (Mathf.Abs(direction.y) > 0) { direction = direction.SetY(0).normalized; }
                    currentBodyTargetAngle = (direction.z > 0 ? Mathf.Acos(direction.x) : -Mathf.Acos(direction.x)) * Mathf.Rad2Deg;
                }
                else
				{
                    if (Mathf.Abs(direction.z) > 0) { direction = direction.SetZ(0).normalized; }
                    currentBodyTargetAngle = (direction.y > 0 ? Mathf.Acos(direction.x) : -Mathf.Acos(direction.x)) * Mathf.Rad2Deg;
                }

                currentBodyAngle = Mathf.MoveTowardsAngle(currentBodyAngle, currentBodyTargetAngle, speed * Time.deltaTime);

                if (PinouApp.Entity.Mode2D == false)
                {
                    references.VisualBody.eulerAngles = new Vector3(0f, currentBodyAngle, 0f);
                }
                else
                {
                    references.VisualBody.eulerAngles = new Vector3(0f, 0f, currentBodyAngle);
                }
            }
            protected virtual void RotateTowardsCastDirection(AbilityCastData castData)
            {
                RotateTowardsDirection(castData.CastDirection, Mathf.Infinity);
            }
            #endregion
            
            #region Events
            protected virtual void OnPerformAbility(Entity ent, AbilityCastData castData)
            {
                AbilityPerformer.HandleAbilityCastDataVisual(castData);
                if (castData.AbilityCast.Main.RotateOnCast == true)
                {
                    RotateTowardsCastDirection(castData);
                }
            }
            private void OnAbilityHitResult(Entity ent, AbilityCastResult result)
            {
                AbilityPerformer.HandleAbilityCastResultVisual(result);
            }
            private void OnDeath(Entity ent, AbilityCastResult killingResult)
            {
                AbilityPerformer.HandleAbilityCastResultFX(killingResult, Data._deathFX);
            }
            #endregion
        }
    }
}