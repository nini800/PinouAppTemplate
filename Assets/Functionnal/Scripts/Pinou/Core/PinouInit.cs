#pragma warning disable 0649
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Pinou
{
	public class PinouInit : PinouBehaviour
	{
		[SerializeField] private string[] _systemScenesToLoad;
		[SerializeField] private string _gameSceneToLoad;
		protected override async void OnAwake()
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying == false) return;
#endif
			List<AsyncOperation> ops = new List<AsyncOperation>();
			for (int i = 0; i < _systemScenesToLoad.Length; i++)
			{
				ops.Add(UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_systemScenesToLoad[i], UnityEngine.SceneManagement.LoadSceneMode.Additive));
			}
			bool everyOpsComplete = false;
			while (everyOpsComplete != true)
			{
				await Task.Delay(10);
				everyOpsComplete = true;
				for (int i = 0; i < ops.Count; i++)
				{
					if (ops[i].isDone == false)
					{
						everyOpsComplete = false;
						break;
					}
				}
			}
			_ = PinouApp.Scene.LoadSceneAsync(_gameSceneToLoad);
		}

		protected override void OnStart()
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying == false) return;
#endif
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(gameObject.scene.name);
		}
	}
}