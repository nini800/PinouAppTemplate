#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pinou.EntitySystem
{
	public class EntityResourceBar : PinouBehaviour
	{
		public enum ResourceBarMethod
		{
			MaterialNumber,
			AnchoredPosition
		}

		[System.Serializable]
		public class EntityResourceBarParameters
		{
			[SerializeField] private EntityBeingResourcesType _linkedResourceType;
			[SerializeField] private ResourceBarMethod _barMethod;
			[SerializeField] private bool _instant;
			[SerializeField, ShowIf("@_instant == false"), Range(0.001f, 10f)] private float _smoothTime;
			[SerializeField, ShowIf("_barMethod", ResourceBarMethod.MaterialNumber)] private Renderer _renderer;
			[SerializeField, ShowIf("_barMethod", ResourceBarMethod.MaterialNumber)] private string _rendererMatName;
			[SerializeField, ShowIf("_barMethod", ResourceBarMethod.MaterialNumber)] private int _rendererMatIndex;

			private float _smoothVel = 0f;

			public void Update(Entity target)
			{
				if (target.HasBeing == false) { return; }

				float progress = target.Being.GetCurrentResource(_linkedResourceType) / target.Being.GetMaxResource(_linkedResourceType);
				switch (_barMethod)
				{
					case ResourceBarMethod.MaterialNumber:
						HandleMaterialNumber(progress);
						break;
					case ResourceBarMethod.AnchoredPosition:
						break;
				}
			}

			private void HandleMaterialNumber(float progress)
			{
				if (_instant == true)
				{
					_renderer.materials[_rendererMatIndex].SetFloat(_rendererMatName, progress);
				}
				else
				{
					_renderer.materials[_rendererMatIndex].SetFloat(
						_rendererMatName,
						Mathf.SmoothDamp(
							_renderer.materials[_rendererMatIndex].GetFloat(_rendererMatName),
							progress,
							ref _smoothVel,
							_smoothTime));
				}
			}
		}

		[Header("Parameter")]
		[Space]
		[SerializeField] private Entity _target;
		[SerializeField] private EntityResourceBarParameters[] _parameters;

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