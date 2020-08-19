using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou.UI;
using Pinou.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public class SimpleMovingCamera : PinouBehaviour
	{
		[Header("Parameters")]
		[Space]
		[SerializeField] private float _speed = 2f;
		[SerializeField] private float _sprintSpeed = 5f;
		[SerializeField] private float _rotationSpeed = 5f;
		[Header("References")]
		[Space]
		[SerializeField] private PinouInputReceiver _ir;
		[SerializeField] private Rigidbody _rb;

		private float _Speed => _ir.Editor_CameraSprint ? _sprintSpeed : _speed;

		private void Update()
		{
			HandleRotate();
			HandleMove();
			HandleHideMouse();
		}
		private void HandleMove()
		{
			Vector2 moveInput = _ir.Editor_CameraMove;
			Vector3 velocity = Vector3.zero;
			velocity.z += _Speed * moveInput.y;
			velocity.x += _Speed * moveInput.x;
			velocity.y += _Speed * (_ir.Editor_CameraUp ? 1 : 0);
			velocity.y -= _Speed * (_ir.Editor_CameraDown ? 1 : 0);
			velocity = transform.TransformVector(velocity);
			_rb.velocity = velocity;
		}
		private void HandleRotate()
		{
			if (_ir.Editor_CameraLook == true)
			{
				Vector2 lookInput = _ir.Editor_CameraAim;
				transform.localEulerAngles = transform.localEulerAngles.AddX(lookInput.y * Time.deltaTime * -_rotationSpeed);
				transform.localEulerAngles = transform.localEulerAngles.AddY(lookInput.x * Time.deltaTime * _rotationSpeed);
			}
		}
		private void HandleHideMouse()
		{
			if (Input.GetMouseButtonDown(1) && Cursor.visible == true)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else if (Input.GetMouseButtonUp(1) && Cursor.visible == false)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}

		private void OnValidate()
		{
			AutoFindReference(ref _ir);
			AutoFindReferenceInParent(ref _rb);
		}
	}
}