#pragma warning disable 1522, 0649
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public enum VelocityOverrideMode
    {
		Speed,
		Distance
    }
	public enum VelocityOverrideDirectionMode
	{
		Local,
		Absolute
	}

	[Serializable]
	public class VelocityOverrideData
	{
		[Header("Velocity Override")]
		[SerializeField] private float _waitDuration;
		[SerializeField] private float _duration;
		[SerializeField] private VelocityOverrideMode _overrideMode;
		[SerializeField] private VelocityOverrideDirectionMode _directionMode;

		[SerializeField, ShowIf("_overrideMode", VelocityOverrideMode.Distance)] private Vector3 _destination;

		[SerializeField, ShowIf("_overrideMode", VelocityOverrideMode.Speed)] private Vector3 _direction;

		[SerializeField] private bool _fixedProgressOverTime = true;
		[SerializeField, Tooltip("Speed over time or Distance progress over time, Range 0,1 on both axes"), ShowIf("@_fixedProgressOverTime == false", false)] private AnimationCurve _overTimeCurve;
		[SerializeField, ShowIf("_overrideMode", VelocityOverrideMode.Speed)] private float _speedFactor;
		[SerializeField, ShowIf("_overrideMode", VelocityOverrideMode.Distance), ShowIf("@_fixedProgressOverTime == false")] private float _averageCurveValue = 0f;

		[SerializeField] private bool _brakeAtEnd;

		public float WaitDuration => _waitDuration;
		public float Duration => _duration;
		public VelocityOverrideMode OverrideMode => _overrideMode;
		public VelocityOverrideDirectionMode DirectionMode => _directionMode;
		public Vector3 Direction => _direction;
		public Vector3 Destination => _destination;
		public bool FixedProgressOverTime => _fixedProgressOverTime;
		public float SpeedFactor => _speedFactor;
		public AnimationCurve OverTimeCurve => _overTimeCurve;
		public float AverageCurveValue => _averageCurveValue;
		public bool BrakeAtEnd => _brakeAtEnd;

		public VelocityOverrideAgent StartVelictyOverride(Rigidbody rb, Vector3 forward)
        {
			VelocityOverrideAgent agent = new VelocityOverrideAgent();
			agent.StartVelocityOverride(rb, forward, this);
			return agent;
		}

#if UNITY_EDITOR
		public void E_UpdateAverageCurveValue()
        {
			float step = 0.001f;
			float stepCount = 1 / step;
			float value = 0f;
            for (float i = 0; i < 1f; i+= step)
            {
				value += _overTimeCurve.Evaluate(i);
            }
			_averageCurveValue = value / (stepCount + 1);
        }
#endif
	}

	[System.Serializable]
	public class VelocityOverrideChainData
	{
		public VelocityOverrideChainData() { }
		public VelocityOverrideChainData(VelocityOverrideData voData) { _overrideChain = new VelocityOverrideData[] { voData }; }

		[SerializeField] private VelocityOverrideData[] _overrideChain;

		public VelocityOverrideData[] OverrideChain => _overrideChain;

		public VelocityOverrideAgent StartChain(Rigidbody rb, Vector3 forward)
        {
			VelocityOverrideAgent agent = new VelocityOverrideAgent();
			agent.StartVelocityOverrideChain(rb, forward, this);
			return agent;
        }

#if UNITY_EDITOR
		public void E_UpdateAverageCurvesValue()
		{
            for (int i = 0; i < _overrideChain.Length; i++)
            {
				if (_overrideChain[i].OverrideMode != VelocityOverrideMode.Distance || _overrideChain[i].FixedProgressOverTime == true)
                {
					continue;
                }

				_overrideChain[i].E_UpdateAverageCurveValue();
            }
		}
#endif
	}

	public class VelocityOverrideAgent
	{
		private int _currentChainIndex = 0;
		private float _statsSpeedFactor = 1f;
		private VelocityOverrideData _currentVoData;
		private VelocityOverrideChainData _currentChainData;
		private Coroutine _currentCoroutine;

		public int CurrentChainIndex => _currentChainIndex;
		public VelocityOverrideData CurrentVoData => _currentVoData;
		public VelocityOverrideChainData CurrentChainData => _currentChainData;
		public Coroutine CurrentCoroutine => _currentCoroutine;

		public Coroutine StartVelocityOverride(Rigidbody rb, Vector3 forward, VelocityOverrideData voData)
        {
			_currentChainIndex = 0;
			_currentChainData = null;
			PinouUtils.Coroutine.RestartCoroutine(ApplyVelocityOverrideCoroutine(rb, forward, voData, true), ref _currentCoroutine);
			return _currentCoroutine;
		}
		public Coroutine StartVelocityOverrideChain(Rigidbody rb, Vector3 forward, VelocityOverrideChainData chain)
		{
			PinouUtils.Coroutine.RestartCoroutine(StartVelocityOverrideChainCoroutine(rb, forward, chain), ref _currentCoroutine);
			return _currentCoroutine;
		}

		public void StopAgent()
		{
			_currentChainIndex = 0;
			_currentVoData = null;
			_currentChainData = null;
			PinouUtils.Coroutine.StopCoroutine(ref _currentCoroutine);
			OnAgentStop.Invoke(this);
		}

		private IEnumerator StartVelocityOverrideChainCoroutine(Rigidbody rb, Vector3 forward, VelocityOverrideChainData chain)
		{
			_currentChainIndex = 0;
			_currentChainData = chain;
			for (; _currentChainIndex < chain.OverrideChain.Length; _currentChainIndex++)
			{
				yield return ApplyVelocityOverrideCoroutine(rb, forward, chain.OverrideChain[_currentChainIndex]);
			}

			StopAgent();
		}
		private IEnumerator ApplyVelocityOverrideCoroutine(Rigidbody rb, Vector3 forward, VelocityOverrideData voData, bool stopAtEnd = false)
		{
			_currentVoData = voData;
			yield return new WaitForSeconds(voData.WaitDuration);

			if (voData.OverrideMode == VelocityOverrideMode.Speed)
			{
				yield return ExecuteSpeedOverrideCoroutine(rb, forward, voData);
			}
			else if (voData.OverrideMode == VelocityOverrideMode.Distance)
			{
				yield return ExecuteDistanceOverrideCoroutine(rb, voData);
			}

			if (voData.BrakeAtEnd == true)
			{
				rb.velocity = Vector3.zero;
			}

			if (stopAtEnd == true)
            {
				StopAgent();
            }
		}
		private IEnumerator ExecuteSpeedOverrideCoroutine(Rigidbody rb, Vector3 forward, VelocityOverrideData voData)
		{
			Vector3 right;
			Vector3 up;
			if (voData.DirectionMode == VelocityOverrideDirectionMode.Absolute)
			{
				forward = Vector3.forward;
				right = Vector3.right;
				up = Vector3.up;
			}
			else
			{
				Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
				up = PinouUtils.Transform.TransformVector(Vector3.up, rotation);
				right = Vector3.Cross(forward, up);
			}

			Vector3 direction = (forward * voData.Direction.z + right * voData.Direction.x + up * voData.Direction.y).normalized;


			float count = 0f, progress;
			while (count < voData.Duration)
			{
				count += Time.fixedDeltaTime;
				progress = count / voData.Duration;

				if (voData.FixedProgressOverTime == true)
				{
					rb.velocity = direction * voData.SpeedFactor * _statsSpeedFactor;
				}
				else
				{
					rb.velocity = direction * voData.OverTimeCurve.Evaluate(progress) * voData.SpeedFactor * _statsSpeedFactor;
				}

				yield return new WaitForFixedUpdate();
			}
		}
		private IEnumerator ExecuteDistanceOverrideCoroutine(Rigidbody rb, VelocityOverrideData voData)
		{
			Vector3 destination = Vector3.zero;
			if (voData.DirectionMode == VelocityOverrideDirectionMode.Absolute)
			{
				destination = voData.Destination;
			}
			else if (voData.DirectionMode == VelocityOverrideDirectionMode.Local)
			{
				destination = rb.position + voData.Destination;
			}

			Vector3 relativeToDestination = destination - rb.position;
			float relativeDistance = relativeToDestination.magnitude;
			Vector3 direction = relativeToDestination / relativeDistance;

			float estimatedAverageSpeed = relativeDistance / voData.Duration;
			float estimatedCurveSpeed = (1 / voData.AverageCurveValue) * estimatedAverageSpeed;

			float count = Time.fixedDeltaTime, progress;
			while (count < voData.Duration)
			{
				count += Time.fixedDeltaTime;
				progress = count / voData.Duration;

				if (voData.FixedProgressOverTime == true)
				{
					rb.velocity = direction * estimatedAverageSpeed * _statsSpeedFactor;
				}
				else
				{
					rb.velocity = direction * voData.OverTimeCurve.Evaluate(progress) * estimatedCurveSpeed * _statsSpeedFactor;
				}

				yield return new WaitForFixedUpdate();
			}
		}

		public void SetStatsSpeedFactor(float speedFactor)
		{
			_statsSpeedFactor = speedFactor;
		}

		#region Events
		public PinouUtils.Delegate.Action<VelocityOverrideAgent> OnAgentStop { get; private set; } = new PinouUtils.Delegate.Action<VelocityOverrideAgent>();
        #endregion
    }
}