#pragma warning disable 0649, 0414
using Pinou.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.UI
{
	public class UI_Window : UI_PinouBehaviour
	{
		[Header("Parameters")]
		[Space]
		[SerializeField] private bool _interceptMouse;
		[SerializeField] private bool _interceptInputs;
		[Header("References")]
		[Space]
		[SerializeField] private UI_PinouInputReceiver _ir;

		public bool InterceptMouse => _interceptMouse;
		public bool InterceptInputs => _interceptInputs;
		public UI_PinouInputReceiver InputReceiver => _ir;

		private bool _opened = false;
		public bool Opened => _opened;

		public virtual void OpenClose()
		{
			if (_opened) { Close(); }
			else { Open(); }
		}
		public virtual void Open()
		{
			gameObject.SetActive(true);
			_opened = true;
			OnOpen.Invoke(this);
		}

		public virtual void Close()
		{
			gameObject.SetActive(false);
			_opened = false;
			OnClose.Invoke(this);
		}

		public PinouUtils.Delegate.Action<UI_Window> OnOpen { get; private set; } = new PinouUtils.Delegate.Action<UI_Window>();
		public PinouUtils.Delegate.Action<UI_Window> OnClose { get; private set; } = new PinouUtils.Delegate.Action<UI_Window>();

#if UNITY_EDITOR
		protected override void E_OnValidate()
		{
			if (_interceptInputs && _ir == null)
			{
				_ir = GetComponent<UI_PinouInputReceiver>();
				if (_ir == null)
				{
					_ir = gameObject.AddComponent<UI_PinouInputReceiver>();
				}
			}
		}
#endif
	}
}