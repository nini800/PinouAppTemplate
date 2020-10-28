using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.UI
{
	public class UI_Menu : UI_PinouBehaviour
	{
		public virtual void Open()
		{
			gameObject.SetActive(true);
			OnMenuOpen.Invoke(this);
		}

		public virtual void Close()
		{
			gameObject.SetActive(false);
			OnMenuClose.Invoke(this);
		}

		public PinouUtils.Delegate.Action<UI_Menu> OnMenuOpen { get; private set; } = new PinouUtils.Delegate.Action<UI_Menu>();
		public PinouUtils.Delegate.Action<UI_Menu> OnMenuClose { get; private set; } = new PinouUtils.Delegate.Action<UI_Menu>();
	}
}