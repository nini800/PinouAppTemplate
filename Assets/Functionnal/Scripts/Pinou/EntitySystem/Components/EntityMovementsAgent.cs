#pragma warning disable 0649, 0414
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityMovementsAgent
	{
		public EntityMovementsAgent(EntityMovementsData.EntityMovements linkedMovements)
		{
			_ld = linkedMovements.Data;
			_lm = linkedMovements;
			_rb = _lm.References.RigidBody;

			switch (_ld.MovementsType)
			{
				case EntityMovementsData.MovementsMethod.Classic_3D:
					_fixedUpdate = HandleClassic3D;
					break;
				case EntityMovementsData.MovementsMethod.TopDown_2D:
					_fixedUpdate = HandleTopDown2D;
					break;
				case EntityMovementsData.MovementsMethod.SideView_2D:
					_fixedUpdate = HandleSideView2D;
					break;
				case EntityMovementsData.MovementsMethod.Custom:
					_fixedUpdate = HandleCustom;
					break;
			}
		}

		private EntityMovementsData _ld;
		private EntityMovementsData.EntityMovements _lm;
		private Rigidbody _rb;

		private Action _fixedUpdate;
		public Action FixedUpdate => _fixedUpdate;

		private void HandleClassic3D()
		{

		}
		private void HandleTopDown2D()
		{
			//Reduce brake linearly when acceleration is getting higher
			float currentAcceleration = _lm.HasController ? _lm.Controller.MoveVector.magnitude : 0f;
			float brakeFactor = Mathf.Clamp(1 - currentAcceleration, 0f, 1f);
			bool maxSpeedReached = _rb.velocity.sqrMagnitude > _lm.MaxSpeed.Squared();

			if (_lm.HasController && _lm.Controller.InputingMovement == true)
			{
				//Side Brake
				Quaternion rot = PinouUtils.Quaternion.SafeLookRotation(_lm.Controller.MoveVector, _lm.Forward);
				Vector3 localVel = PinouUtils.Transform.InverseTransformVector(_rb.velocity, rot);
				localVel.y = localVel.y.SubtractAbsolute(_lm.BrakeForce * _lm.Acceleration);
				localVel.x = localVel.x.SubtractAbsolute(_lm.BrakeForce * _lm.Acceleration * (_ld.BrakeWhileMoving == true ? brakeFactor : 0f));
				_rb.velocity = PinouUtils.Transform.TransformVector(localVel, rot);

				//Prevent gaining speed above maxspeed
				if (maxSpeedReached == true)
				{
					float oldMagnitude = _rb.velocity.magnitude;
					_rb.velocity += (_lm.Controller.MoveVector * _lm.Acceleration);
					_rb.velocity = _rb.velocity.normalized * oldMagnitude;
					if (oldMagnitude <= _lm.MaxSpeed + Mathf.Epsilon)
					{
						maxSpeedReached = false;
					}
				}
				//Adding speed
				else
				{
					_rb.velocity += (_lm.Controller.MoveVector * _lm.Acceleration);
				}
			}
			else
			{
				//Omnidirectionnal Brake
				_rb.velocity = _rb.velocity.SubtractMagnitude(_lm.BrakeForce * _lm.Acceleration * brakeFactor);
				maxSpeedReached = false;
			}

			//Limit max speed
			if (maxSpeedReached == true)
			{
				_rb.velocity = Vector3.MoveTowards(_rb.velocity, _rb.velocity.normalized * _lm.MaxSpeed, _lm.BrakeForce * _lm.Acceleration);
			}
		}
		private void HandleSideView2D()
		{

		}
		protected virtual void HandleCustom()
		{

		}
	}
}