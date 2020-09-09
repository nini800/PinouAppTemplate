#pragma warning disable 0649, 0414
using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.Networking
{
	public class PinouSyncableTransform : NetworkBehaviour, IDestroyableNetworkSyncableGameObject
	{
		[SerializeField] private NetworkIdentity _ni;
		[SerializeField] private Transform _positionTarget;
		[SerializeField, ShowIf("@_positionTarget != null")] private SyncFrequency _positionSyncFrequency;
		[Space]
		[SerializeField] private Transform _rotationTarget;
		[SerializeField, ShowIf("@_rotationTarget != null")] private SyncFrequency _rotationSyncFrequency;
		[Space]
		[SerializeField] private Rigidbody _rigidbodyTarget;
		[SerializeField, ShowIf("@_rigidbodyTarget != null")] private SyncFrequency _rigidbodySyncFrequency;

		private Vector3 _oldPos;
		private Quaternion _oldRot;
		private Vector3 _oldVel;

		private float _posSpeed;
		private float _rotSpeed;
		private float _velSpeed;
		private Vector3 _targetPos;
		private Quaternion _targetRot;
		private Vector3 _targetVel;

		public PinouUtils.Delegate.Action<GameObject> OnGameObjectDestroyed { get; private set; } = new PinouUtils.Delegate.Action<GameObject>();

		private void Start()
		{
			if (_positionTarget != null)
			{
				_oldPos = _positionTarget.position;
				PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(gameObject, SyncableVariable.Position, _positionSyncFrequency, RegisterPosition, SyncPosition);
			}
			if (_rotationTarget != null)
			{
				_oldRot = _rotationTarget.rotation;
				PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(gameObject, SyncableVariable.Rotation, _rotationSyncFrequency, RegisterRotation, SyncRotation);
			}
			if (_rigidbodyTarget != null)
			{
				_oldVel = _rigidbodyTarget.velocity;
				PinouNetworkManager.MainBehaviour.RegisterGameObjectSyncVar(gameObject, SyncableVariable.Velocity, _rigidbodySyncFrequency, RegisterVelocity, SyncVelocity);
			}
		}
		private void OnEnable()
		{
			if (_positionTarget != null)
			{
				_positionTarget.position = _positionTarget.position;
			}
			if (_rotationTarget != null)
			{
				_rotationTarget.rotation = _rotationTarget.rotation;
			}
			if (_rigidbodyTarget != null)
			{
				_rigidbodyTarget.velocity = _rigidbodyTarget.velocity;
			}
			Start();
		}

		private void Update()
		{
			if (_ni.hasAuthority == false)
			{
				HandleMoveTowardTargets();
			}
			else
			{
				HandleCheckDirty();
			}
		}

		private void HandleMoveTowardTargets()
		{
			if (_positionTarget != null)
			{
				float posFinalSpeed = 1 / _positionSyncFrequency.GetFrequencyPeriod() * _posSpeed;
				_positionTarget.position = Vector3.MoveTowards(_positionTarget.position, _targetPos, Time.deltaTime * posFinalSpeed);
			}
			if (_rotationTarget != null)
			{
				float rotFinalSpeed = 1 / _rotationSyncFrequency.GetFrequencyPeriod() * _rotSpeed;
				_rotationTarget.rotation = Quaternion.RotateTowards(_rotationTarget.rotation, _targetRot, Time.deltaTime * rotFinalSpeed);
			}
			if (_rigidbodyTarget != null)
			{
				float velFinalSpeed = 1 / _rigidbodySyncFrequency.GetFrequencyPeriod() * _velSpeed;
				_rigidbodyTarget.velocity = Vector3.MoveTowards(_rigidbodyTarget.velocity, _targetVel, Time.deltaTime * velFinalSpeed);
			}
		}

		private void HandleCheckDirty()
		{
			if (_positionTarget != null)
			{
				if (Mathf.Abs(_oldPos.sqrMagnitude - _positionTarget.position.sqrMagnitude) >= 0.1f.Squared())
				{
					PinouNetworkManager.MainBehaviour.SetDirty(gameObject, SyncableVariable.Position);
					_oldPos = _positionTarget.position;
				}
			}
			if (_rotationTarget != null)
			{
				if (Quaternion.Angle(_oldRot, _rotationTarget.rotation) >= 0.1f)
				{
					PinouNetworkManager.MainBehaviour.SetDirty(gameObject, SyncableVariable.Rotation);
					_oldRot = _rotationTarget.rotation;
				}
			}
			if (_rigidbodyTarget != null)
			{
				if (Mathf.Abs(_oldVel.sqrMagnitude - _rigidbodyTarget.velocity.sqrMagnitude) >= 0.1f.Squared())
				{
					PinouNetworkManager.MainBehaviour.SetDirty(gameObject, SyncableVariable.Velocity);
					_oldVel = _rigidbodyTarget.velocity;
				}
			}
		}
		private void RegisterPosition(SyncableVariable var, NetworkWriter writer)
		{
			if (_positionTarget == null) { return; }
			writer.WriteVector3(_positionTarget.position);
		}
		private void SyncPosition(SyncableVariable var, NetworkReader reader)
		{
			if (_positionTarget == null) { return; }
			Vector3 oldPos = _targetPos;
			_targetPos = reader.ReadVector3();
			_posSpeed = (_targetPos - oldPos).magnitude;
		}

		private void RegisterRotation(SyncableVariable var, NetworkWriter writer)
		{
			if (_rotationTarget == null) { return; }
			writer.WriteQuaternion(_rotationTarget.rotation);
		}
		private void SyncRotation(SyncableVariable var, NetworkReader reader)
		{
			if (_rotationTarget == null) { return; }
			Quaternion oldRot = _targetRot;
			_targetRot = reader.ReadQuaternion();
			_rotSpeed = Quaternion.Angle(oldRot, _targetRot);
		}
		private void RegisterVelocity(SyncableVariable var, NetworkWriter writer)
		{
			if (_rigidbodyTarget == null) { return; }
			writer.WriteVector3(_rigidbodyTarget.velocity);
		}
		private void SyncVelocity(SyncableVariable var, NetworkReader reader)
		{
			if (_rigidbodyTarget == null) { return; }
			_targetVel = reader.ReadVector3();
		}

		private void OnValidate()
		{
			_ni = GetComponent<NetworkIdentity>();
		}

		private void OnDestroy()
		{
			OnGameObjectDestroyed.Invoke(gameObject);
		}
	}
}