#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pinou.UI
{
	public class UI_Master : PinouBehaviour
	{
		#region Vars, Fields, Getters
		[Header("References")]
		[Space]
		[SerializeField] private GameObject _gameUI;
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