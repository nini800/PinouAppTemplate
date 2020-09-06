#pragma warning disable 0649, 0414
using Pinou.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Pinou.UI
{
	public class UI_Master : PinouBehaviour
	{
		#region Vars, Fields, Getters
		[Header("References")]
		[Space]
		[SerializeField] private UI_PinouInputReceiver _ir;
		[SerializeField] private GameObject _gameUI;
		[SerializeField] private GameObject _pauseUI;

		[SerializeField] private UnityEvent _customUnityEvent;
		#endregion
		#region Behaviour
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
			HandleCheckPauseMenuInput();
		}
		private void HandleCheckPauseMenuInput()
		{
			if (_ir.Game_Pause == true)
			{
				if (_pauseUI.activeSelf == false)
				{
					_pauseUI.SetActive(true);
					_ir.StoreCurrentInputReceivers();
					_ir.ReceiveAbsoluteFocus(FocusMode.Exclusive);
				}
				else
				{
					_pauseUI.SetActive(false);
					_ir.RetrieveStoredInputReceivers();
					_ir.ReceiveAbsoluteFocus(FocusMode.Additive);
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
		#endregion
	}
}