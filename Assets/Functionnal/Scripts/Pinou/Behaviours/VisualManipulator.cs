#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	public class VisualManipulator : PinouBehaviour
	{
		#region Data Class
		[System.Serializable]
		public class VisualParameters
		{
			[SerializeField] private bool _changeColor;
			[SerializeField, ShowIf(nameof(_changeColor))] private Color _color;

			public Color Color => _color;

			public void Apply(GameObject go)
			{
				VisualManipulator vm = go.GetComponentInChildren<VisualManipulator>();
				if (vm != null) { Apply(vm._renderers.ToArray()); }
			}

			public void Apply(Component[] components)
			{
				if (_changeColor)
				{
					SetColor(ref components);
				}
			}

			public void SetColor(ref Component[] components)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i] is SpriteRenderer sr)
					{
						sr.color = _color;
					}
					else if (components[i] is TrailRenderer tr)
					{
						Gradient grad = new Gradient();
						grad.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1, 1f) };
						grad.colorKeys = new GradientColorKey[] { new GradientColorKey(_color, 0f), new GradientColorKey(_color, 1f) };
						tr.colorGradient = grad;
					}
					else if (components[i] is ParticleSystem ps)
					{
						ParticleSystem.MainModule main = ps.main;
						main.startColor = _color;
					}
					else if (components[i] is MeshRenderer mr)
					{
						mr.material.SetColor("_Color", _color);
					}
				}
			}
		}
		#endregion
		[Header("References")]
		[Space]
		[SerializeField, ValidateInput("ValidateRenderers")] private List<Component> _renderers;

		private bool ValidateRenderers(List<Component> renderers)
		{
			for (int i = renderers.Count - 1; i >= 0; i--)
			{
				if (!(renderers[i] is SpriteRenderer) &&
					!(renderers[i] is TrailRenderer) &&
					!(renderers[i] is ParticleSystem) &&
					!(renderers[i] is MeshRenderer))
				{
					_renderers.RemoveAt(i);
				}
			}
			return true;
		}

		public void ApplyParameters(VisualParameters parameters)
		{
			parameters.Apply(_renderers.ToArray());
		}
	}
}