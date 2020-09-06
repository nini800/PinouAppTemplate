#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pinou.EntitySystem
{
	public class EntityProgressBar : PinouBehaviour
	{
		public enum ProgressBarMethod
		{
			MaterialNumber,
			TransformPosition,
			AnchoredPosition,
		}

		[System.Serializable]
		public class EntityProgressBarParameters
		{
			[SerializeField, ValueDropdown("ComputePossibleLinkedInfoType")] private EntityLinkedInfoType _linkedInfoType = EntityLinkedInfoType.ResourceProgress;

			[ShowIf("@_linkedInfoType == EntityLinkedInfoType.ResourceProgress || _linkedInfoType == EntityLinkedInfoType.MaxResource")]
			[SerializeField] private EntityBeingResourceType _linkedResourceType;
			[ShowIf("@_linkedInfoType == EntityLinkedInfoType.LevelProgress || _linkedInfoType == EntityLinkedInfoType.Level")]
			[SerializeField] private EntityStatsLevelType _linkedLevelType;

			[SerializeField] private ProgressBarMethod _barMethod;
			[SerializeField] private bool _instant;
			[SerializeField, ShowIf("@_instant == false"), Range(0.001f, 10f)] private float _smoothTime;
			[SerializeField, ShowIf("_barMethod", ProgressBarMethod.MaterialNumber)] private Renderer _renderer;
			[SerializeField, ShowIf("_barMethod", ProgressBarMethod.MaterialNumber)] private string _rendererMatName;
			[SerializeField, ShowIf("_barMethod", ProgressBarMethod.MaterialNumber)] private int _rendererMatIndex;
			[ShowIf("_barMethod", ProgressBarMethod.TransformPosition)]
			[SerializeField] private Transform _targetTransform;
			[ShowIf("_barMethod", ProgressBarMethod.TransformPosition)]
			[SerializeField] private bool _localPos = true;
			[ShowIf("_barMethod", ProgressBarMethod.AnchoredPosition)]
			[SerializeField] private RectTransform _targetRectTransform;
			[ShowIf("@_barMethod == ProgressBarMethod.TransformPosition || _barMethod == ProgressBarMethod.AnchoredPosition")]
			[SerializeField] private Vector3 _emptyPos;
			[ShowIf("@_barMethod == ProgressBarMethod.TransformPosition || _barMethod == ProgressBarMethod.AnchoredPosition")]
			[SerializeField] private Vector3 _fullPos;

			private IEnumerable<EntityLinkedInfoType> ComputePossibleLinkedInfoType()
			{
				return new EntityLinkedInfoType[] { 
					EntityLinkedInfoType.ResourceProgress,
					EntityLinkedInfoType.LevelProgress,
					EntityLinkedInfoType.DashCooldownProgress};
			}

			private float _currentProgress = 1f;
			private float _smoothVel = 0f;

			public void Construct(Entity target)
			{
				switch (_linkedInfoType)
				{
					case EntityLinkedInfoType.LevelProgress:
						if (target.HasStats == false) { return; }
						target.Stats.GetLevelExperienceData(_linkedLevelType)?.OnLevelChange.SafeSubscribe(OnLinkedLevelChange);
						break;
				}

				_currentProgress = GetProgress(target);
			}

			public void Update(Entity target)
			{
				if (target.HasBeing == false) { return; }

				HandleUpdate(GetProgress(target));
			}
			private float GetProgress(Entity target)
			{
				switch (_linkedInfoType)
				{
					case EntityLinkedInfoType.ResourceProgress:
						if (target.HasBeing == false) { return 0f; }
						return target.Being.GetCurrentResource(_linkedResourceType) / target.Being.GetMaxResource(_linkedResourceType);
					case EntityLinkedInfoType.MaxResource:
						if (target.HasBeing == false) { return 0f; }
						return target.Being.GetMaxResource(_linkedResourceType);
					case EntityLinkedInfoType.LevelProgress:
						if (target.HasStats == false) { return 0f; }
						return target.Stats.GetLevelProgress(_linkedLevelType);
					case EntityLinkedInfoType.Level:
						if (target.HasStats == false) { return 0f; }
						return target.Stats.GetCurrentLevel(_linkedLevelType);
					case EntityLinkedInfoType.DashCooldownProgress:
						if (target.HasMovements == false) { return 0f; }
						return target.Movements.LastDashClampedDuration / target.Movements.LastDashMaxDuration;
				}

				return 0f;
			}
			private void HandleUpdate(float progress, bool instant = false)
			{
				switch (_barMethod)
				{
					case ProgressBarMethod.MaterialNumber:
						HandleMaterialNumber(progress, instant);
						break;
					case ProgressBarMethod.TransformPosition:
						HandleTransformPosition(progress, instant);
						break;
					case ProgressBarMethod.AnchoredPosition:
						HandleAnchoredPosition(progress, instant);
						break;
				}
			}

			private void HandleMaterialNumber(float progress, bool instant = false)
			{
				if (_instant == true || instant == true)
				{
					_renderer.materials[_rendererMatIndex].SetFloat(_rendererMatName, progress);
				}
				else
				{
					_currentProgress = Mathf.SmoothDamp(
						_currentProgress,
						progress,
						ref _smoothVel,
						_smoothTime);
					_renderer.materials[_rendererMatIndex].SetFloat(
						_rendererMatName,
						_currentProgress);
				}
			}
			private void HandleAnchoredPosition(float progress, bool instant = false)
			{
				if (_instant == true || instant == true)
				{
					_targetRectTransform.anchoredPosition = _fullPos;
				}
				else
				{
					_targetRectTransform.anchoredPosition = Vector3.Lerp(_emptyPos, _fullPos, progress);
				}
			}
			private void HandleTransformPosition(float progress, bool instant = false)
			{
				if (_instant == true || instant == true)
				{
					if (_localPos == true)
					{
						_targetTransform.localPosition = Vector3.Lerp(_emptyPos, _fullPos, progress);
					}
					else
					{
						_targetTransform.position = Vector3.Lerp(_emptyPos, _fullPos, progress);
					}
				}
				else
				{
					_currentProgress = Mathf.SmoothDamp(
											_currentProgress,
											progress,
											ref _smoothVel,
											_smoothTime);
					if (_localPos == true)
					{
						_targetTransform.localPosition = Vector3.Lerp(_emptyPos, _fullPos, _currentProgress);
					}
					else
					{
						_targetTransform.position = Vector3.Lerp(_emptyPos, _fullPos, _currentProgress);
					}
				}
			}

			private void OnLinkedLevelChange(EntityStatsData.LevelExperienceData expData, int newLevel) 
			{
				HandleUpdate(0f, true);
			}
		}

		[Header("Parameter")]
		[Space]
		[SerializeField] private Entity _target;
		[SerializeField] private EntityProgressBarParameters[] _parameters;

		protected override void OnStart()
		{
			if (_target == null) { return; }

			for (int i = 0; i < _parameters.Length; i++)
			{
				_parameters[i].Construct(_target);
			}
		}

		private void Update()
		{
			if (_target == null) { return; }

			for (int i = 0; i < _parameters.Length; i++)
			{
				_parameters[i].Update(_target);
			}
		}

	}
}