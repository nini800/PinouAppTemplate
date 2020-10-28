#pragma warning disable 0649, 0414
using Pinou.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Pinou.UI
{
	public class UI_Master : PinouBehaviour
	{
		#region Data Classes
		[System.Serializable]
		public class WindowParams
		{
			[SerializeField] private UI_Window _window;
			[SerializeField] private PinouInput _windowOpenInput;

			public UI_Window Window => _window;
			public PinouInput OpenInput => _windowOpenInput;
		}
		#endregion

		#region Vars, Fields, Getters
		[Header("References")]
		[Space]
		[SerializeField] private WindowParams[] _wParams;
		[Space]
		[SerializeField] private UI_PinouInputReceiver _ir;
		[SerializeField] private GraphicRaycaster _raycaster;
		[SerializeField] private EventSystem _eventSystem;
		[SerializeField] private GameObject _gameUI;
		[SerializeField] private GameObject _pauseUI;

		#endregion
		#region Behaviour
		protected override void OnAwake()
		{
			for (int i = 0; i < _wParams.Length; i++)
			{
				_wParams[i].Window.OnOpen.SafeSubscribe(OnWindowOpen);
				_wParams[i].Window.OnClose.SafeSubscribe(OnWindowClose);
			}
		}

		protected override void OnSafeStart()
		{
			PinouApp.Scene.OnSceneLoadComplete.Subscribe(OnSceneLoadComplete);
			if (PinouApp.Scene.GameSceneLoaded == true)
			{
				EnableGameUI();
			}
		}

		private void Update()
		{
			HandleCheckMouseOverUI();
			HandleCheckPauseMenuInput();
			HandleCheckWindowsInputs();
		}
		private void HandleCheckMouseOverUI()
		{
			List<RaycastResult> _hits = new List<RaycastResult>();
			PointerEventData eventData = new PointerEventData(_eventSystem);
			eventData.position = Input.mousePosition;
			_raycaster.Raycast(eventData, _hits);
		}
		private void HandleCheckPauseMenuInput()
		{
			if (_ir.Game_Pause == true)
			{
				if (_pauseUI.activeSelf == false)
				{
					_pauseUI.SetActive(true);
					//_ir.StoreCurrentInputReceivers();
					//_ir.ReceiveAbsoluteFocus(FocusMode.Exclusive);
				}
				else
				{
					_pauseUI.SetActive(false);
					//_ir.RetrieveStoredInputReceivers();
					//_ir.ReceiveAbsoluteFocus(FocusMode.Additive);
				}
			}
		}
		private void HandleCheckWindowsInputs()
		{
			if (_pauseUI.activeSelf == true) { return; }

			for (int i = 0; i < _wParams.Length; i++)
			{
				if (_ir.GetPinouInput(_wParams[i].OpenInput))
				{
					_wParams[i].Window.OpenClose();
				}
			}
		}
		#endregion

		#region Utilities
		private void EnableGameUI()
		{
			_gameUI.SetActive(true);
		}
		private void DisableGameUI()
		{
			_gameUI.SetActive(false);
		}
		#endregion

		#region Events
		private void OnSceneLoadComplete(Scene scene)
		{
			if (PinouConstants.IS_GAME_SCENE(scene.name))
			{
				EnableGameUI();
			}
			else
			{
				DisableGameUI();
			}
		}

		private void OnWindowOpen(UI_Window window)
		{
		}
		private void OnWindowClose(UI_Window window)
		{
		}
		#endregion
	}
}