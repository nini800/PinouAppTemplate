#pragma warning disable 0649, 0414
using Pinou.EntitySystem;
using Pinou.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pinou
{
	public class UI_DamageText : PinouBehaviour
	{
		[Header("Movement Parameters")]
		[Space]
		[SerializeField] private float _totalMoveDistance;
		[SerializeField] private float _moveDuration;
		[SerializeField] private AnimationCurve _moveCurve;

		[Header("References")]
		[Space]
		[SerializeField] private Transform _textTransform;
		[SerializeField] private TextMeshProUGUI _text;

		private float _moveStartTime;
		private float _moveCurrentDuration => Time.time - _moveStartTime;
		private Vector2 _worldPos;

		public float MoveCurrentDuration => _moveCurrentDuration;
		public Vector2 WorldPos => _worldPos;

		public void Build(AbilityCastResult result, AbilityResourceImpactData impactData)
		{
			_moveStartTime = Time.time;
			_worldPos = result.Impact;

			RectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(result.Impact) * (1920f / Screen.width);
			Vector2 impactScreenPoint = Camera.main.WorldToScreenPoint(result.Impact);
			Vector2 directionScreenPoint;
			if (result.KnockbackApplied.sqrMagnitude > 0f)
			{
				directionScreenPoint = Camera.main.WorldToScreenPoint(result.Impact + result.KnockbackApplied.normalized);
			}
			else
			{
				directionScreenPoint = Camera.main.WorldToScreenPoint(result.Impact + result.CastData.CastDirection);
			}

			Vector2 textDirection = (directionScreenPoint - impactScreenPoint);
			if (textDirection.sqrMagnitude > 0f)
			{
				textDirection.Normalize();
			}
			else
			{
				textDirection = Vector2.right;
			}

			float angle = (textDirection.y > 0 ? Mathf.Acos(textDirection.x) : -Mathf.Acos(textDirection.x)) * Mathf.Rad2Deg;
			RectTransform.rotation = Quaternion.Euler(0, 0, angle);
			_textTransform.rotation = Quaternion.Euler(0, 0, 0);

			_text.text = Mathf.FloorToInt(Mathf.Clamp(Mathf.Abs(impactData.ResourceChange), 1f, Mathf.Infinity)).ToString();
		}

		public bool UpdatePosition(ref float screenRatioFactor)
		{
			float durationProgress = _moveCurrentDuration / _moveDuration;
			RectTransform.anchoredPosition = 
				Camera.main.WorldToScreenPoint(_worldPos) * screenRatioFactor +
				RectTransform.right * _totalMoveDistance * _moveCurve.Evaluate(durationProgress);

			return durationProgress >= 1f;
		}
	}
}