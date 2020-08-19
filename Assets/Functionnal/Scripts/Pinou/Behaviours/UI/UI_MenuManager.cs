using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.UI
{
	public class UI_MenuManager : UI_PinouBehaviour
	{
		[SerializeField] private UI_Menu[] _menus;

		private UI_Menu _openedMenu;

		[ShowInInspector] public UI_Menu OpenedMenu => _openedMenu;

		public void OpenMenu(UI_Menu menu)
		{
			if (_openedMenu != null)
			{
				_openedMenu.Close();
			}

			menu.Open();
			_openedMenu = menu;
		}
	}
}