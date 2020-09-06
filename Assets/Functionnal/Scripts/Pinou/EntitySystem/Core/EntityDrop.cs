#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class EntityDrop : MonoBehaviour
	{
		[SerializeField] private EntityDropData _data;

		private float _awakeTime = -1 / 0f;

		public float AwakeTime => _awakeTime;
		public float TimeSinceAwake => Time.time - _awakeTime;
		public float StiffnessProgress => TimeSinceAwake / Data.PickupStiffnessTime;

		public EntityDropData Data => _data;

		private void Awake()
		{
			_awakeTime = Time.time;
		}
		private void OnEnable()
		{
			_awakeTime = Time.time;
		}

		public void FillData(EntityDropData data)
		{
			_data = data;
		}

		public virtual void Collect(Entity collecter)
		{
			Data.PickupEffects.ApplyEffects(collecter);
			OnCollect.Invoke(this, collecter);
			PinouApp.Pooler.Store(gameObject);
		}

		public PinouUtils.Delegate.Action<EntityDrop, Entity> OnCollect { get; private set; } = new PinouUtils.Delegate.Action<EntityDrop, Entity>();
	}
}