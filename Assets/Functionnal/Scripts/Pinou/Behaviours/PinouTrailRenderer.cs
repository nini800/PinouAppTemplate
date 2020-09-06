#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	public class PinouTrailRenderer : PinouBehaviour
	{
		[SerializeField] private TrailRenderer _tr;
		[SerializeField] private bool _clearOnEnable = true;

		private bool _mustClear = false;

		protected override void OnDisabled()
		{
			if (_clearOnEnable == true)
			{
				_mustClear = true;
			}
		}
		protected override void OnEnabled()
		{
			if (_clearOnEnable == true && _mustClear == true)
			{
				_tr.Clear();
				_mustClear = false;
			}
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			_tr = GetComponent<TrailRenderer>();
		}
#endif
	}
}