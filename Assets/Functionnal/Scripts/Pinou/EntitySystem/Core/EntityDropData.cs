#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    [CreateAssetMenu(fileName = "EntityDropData", menuName = "Pinou/Entity/EntityDropData", order = 1000)]
	public class EntityDropData : SerializedScriptableObject
	{
		[Header("Main")]
		[Space]
		[SerializeField] private GameObject _dropModel;

		[Header("Physics Parameters")]
		[Space]
		[SerializeField, Min(0f)] private float _pickupDistance = 3f;
		[SerializeField, Min(0f)] private float _pickupSpeed = 20f;
		[SerializeField, Min(0f)] private float _pickupStiffness = 20f;
		[SerializeField, Min(0f)] private float _pickupStiffnessTime = 1f;
		[Space]
		[SerializeField, Min(0f)] private float _velocityDrag = 0.2f;
		[SerializeField, Min(0f)] private float _angularDrag = 0.2f;
		[Space]
		[SerializeField, Min(0f)] private float _minDropHorizontalVelocity;
		[SerializeField, MinValue("@_minDropHorizontalVelocity")] private float _maxDropHorizontalVelocity;
		[Space]
		[SerializeField, Min(0f), ShowIf("@PinouApp.Entity.Mode2D == false")] private float _minDropVerticalVelocity;
		[SerializeField, MinValue("@_minDropVerticalVelocity"), ShowIf("@PinouApp.Entity.Mode2D == false")] private float _maxDropVerticalVelocity;
		[Space]
		[SerializeField] private float _minDropAngularVelocity;
		[SerializeField, MinValue("@_minDropAngularVelocity")] private float _maxDropAngularVelocity;

		[Header("Pickup Effects")]
		[Space]
		[SerializeField] private EntityEffectBundle _pickupEffects;




		public GameObject DropModel => _dropModel;

		public float PickupDistance => _pickupDistance;
		public float PickupSpeed => _pickupSpeed;
		public float PickupStiffness => _pickupStiffness;
		public float PickupStiffnessTime => _pickupStiffnessTime;
		public float VelocityDrag => _velocityDrag;
		public float AngularDrag => _angularDrag;
		public float MinDropHorizontalVelocity => _minDropHorizontalVelocity;
		public float MaxDropHorizontalVelocity => _maxDropHorizontalVelocity;
		public float MinDropVerticalVelocity => _minDropVerticalVelocity;
		public float MaxDropVerticalVelocity => _maxDropVerticalVelocity;
		public float MinDropAngularVelocity => _minDropAngularVelocity;
		public float MaxDropAngularVelocity => _maxDropAngularVelocity;

		public EntityEffectBundle PickupEffects => _pickupEffects;

	}
}