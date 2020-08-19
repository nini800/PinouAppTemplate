using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pinou
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Pinou/SceneData", order = 1000)]
    public class PinouSceneManagerData : PinouManagerData
    {
        public override PinouManager BuildManagerInstance()
        {
            return new PinouSceneManager(this);
        }

        public class PinouSceneManager : PinouManager
        {
            public PinouSceneManager(PinouSceneManagerData dataRef) : base(dataRef)
            {
                Data = dataRef;
            }
            public new PinouSceneManagerData Data { get; private set; }

            private class SceneLowLevelInfos
            {
                public bool SceneLoaded = false;
                public bool IsMenuScene = false;
                public int SceneIndex = -1;
                public float LoadProgress = 0f;
                public string SceneName = "None";
                public Scene Scene;

                public void Reset()
                {
                    SceneLoaded = false;
                    IsMenuScene = false;
                    SceneIndex = -1;
                    LoadProgress = 0f;
                    SceneName = "None";
                    Scene = default;
                }
            }

            #region Attributes, Accessors & Mutators
            private Scene _appScene;
            private Scene _uiScene;
            private SceneLowLevelInfos _activeSceneLowLevelInfos = new SceneLowLevelInfos();
            private PinouSceneInfos _activeSceneInfos = null;

            public Scene AppScene => _appScene;
            public Scene UIScene => _uiScene;
            public PinouSceneInfos ActiveSceneInfos => _activeSceneInfos;
            public bool GameSceneLoaded => _activeSceneInfos != null;
            #endregion

            #region Behaviour
            public override void SlaveStart()
            {
                HandleFindSystemScenes();
                HandleFindAndLoadActiveGameScene();
            }
            private void HandleFindSystemScenes()
			{
                bool appFound = false;
                bool uiFound = false;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (PinouConstants.IS_APP_SCENE(scene.name))
                    {
                        _appScene = scene;
                        appFound = true;
                    }
                    else if (PinouConstants.IS_UI_SCENE(scene.name))
					{
                        _uiScene = scene;
                        uiFound = true;
					}
                }

                if (appFound == false)
				{
                    Debug.Log("No App Scene found.");
                }
                if (uiFound == false)
				{
                    Debug.Log("No UI Scene found.");
				}
            }
            private async void HandleFindAndLoadActiveGameScene()
			{
                if (_activeSceneLowLevelInfos.SceneIndex == -1)
                {
                    Scene activeScene = SceneManager.GetActiveScene();
                    bool isGameSceneActive = PinouConstants.IS_GAME_SCENE(activeScene.name);
                    if (isGameSceneActive == false)
					{
                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            Scene scene = SceneManager.GetSceneAt(i);
                            if (PinouConstants.IS_GAME_SCENE(scene.name))
							{
                                SceneManager.SetActiveScene(scene);
                                isGameSceneActive = true;
							}
                        }
                    }

                    if (isGameSceneActive == true)
                    {
                        _activeSceneLowLevelInfos.SceneLoaded = true;
                        _activeSceneLowLevelInfos.IsMenuScene = activeScene.name.ToLower().Contains("menu");
                        _activeSceneLowLevelInfos.SceneIndex = activeScene.buildIndex;
                        _activeSceneLowLevelInfos.LoadProgress = 1f;
                        _activeSceneLowLevelInfos.SceneName = activeScene.name;
                        _activeSceneLowLevelInfos.Scene = activeScene;

                        FindSceneInfos(ref activeScene);

                        await Task.Delay(1);
                        OnSceneLoadComplete.Invoke(_activeSceneLowLevelInfos.Scene);
                    }
                    else
					{
                        Debug.Log("No Game Scene found.");
					}
                }
            }
            #endregion

            #region Utilities
            private async Task UnloadActiveSceneAsync()
            {
                if (_activeSceneLowLevelInfos.SceneLoaded == true)
                {
                    if (_activeSceneLowLevelInfos.IsMenuScene == false)
                    {
                        OnBeforeGameSceneUnload.Invoke(_activeSceneLowLevelInfos.Scene);
                    }
                    AsyncOperation op = SceneManager.UnloadSceneAsync(_activeSceneLowLevelInfos.SceneIndex);
                    while (op.isDone == false)
                    {
                        await Task.Delay(10);
                    }

                    _activeSceneLowLevelInfos.Reset();
                    _activeSceneInfos = null;

                    if (_activeSceneLowLevelInfos.IsMenuScene == false)
                    {
                        OnAfterGameSceneUnload.Invoke();
                    }
                }
            }
            public async Task LoadSceneAsync(string sceneName)
            {
                OnBeforeSceneChange.Invoke(_activeSceneLowLevelInfos.Scene);
                await UnloadActiveSceneAsync();

                _activeSceneLowLevelInfos.Reset();
                _activeSceneInfos = null;
                _activeSceneLowLevelInfos.SceneName = sceneName;

                _activeSceneLowLevelInfos.IsMenuScene = sceneName.ToLower().Contains("menu");
                if (_activeSceneLowLevelInfos.IsMenuScene == false)
                {
                    OnBeforeGameSceneLoad.Invoke();
                }

                AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (op.isDone == false)
                {
                    _activeSceneLowLevelInfos.LoadProgress = op.progress * 0.7f;
                    await Task.Delay(10);
                }
                _activeSceneLowLevelInfos.LoadProgress = 0.7f;

                Scene scene = SceneManager.GetSceneByName(sceneName);
                _activeSceneLowLevelInfos.SceneIndex = scene.buildIndex;
                _activeSceneLowLevelInfos.Scene = scene;
                SceneManager.SetActiveScene(scene);

                FindSceneInfos(ref scene);

                if (_activeSceneLowLevelInfos.IsMenuScene == false)
                {
                    OnAfterGameSceneLoad.Invoke(_activeSceneLowLevelInfos.Scene);
                }
                OnAfterSceneChange.Invoke(_activeSceneLowLevelInfos.Scene);

                if (_activeSceneInfos != null)
                {
                    while (_activeSceneInfos.LoadingComplete == false)
                    {
                        await Task.Delay(10);
                    }
                }

                OnSceneLoadComplete.Invoke(_activeSceneLowLevelInfos.Scene);
            }
            private void FindSceneInfos(ref Scene scene)
            {
                GameObject[] gobs = _activeSceneLowLevelInfos.Scene.GetRootGameObjects();
                for (int i = 0; i < gobs.Length; i++)
                {
                    if (gobs[i].TryGetComponent(out _activeSceneInfos))
                    {
                        break;
                    }
                }

                if (_activeSceneInfos == null)
                {
                    Debug.LogError("No SceneInfos found in root gameobjects of the scene.");
                }
            }
            #endregion

            #region Events
            public PinouUtils.Delegate.Action<Scene> OnBeforeSceneChange { get; private set; } = new PinouUtils.Delegate.Action<Scene>();
            public PinouUtils.Delegate.Action<Scene> OnAfterSceneChange { get; private set; } = new PinouUtils.Delegate.Action<Scene>();

            public PinouUtils.Delegate.Action<Scene> OnBeforeGameSceneUnload { get; private set; } = new PinouUtils.Delegate.Action<Scene>();
            public PinouUtils.Delegate.Action OnAfterGameSceneUnload { get; private set; } = new PinouUtils.Delegate.Action();
            public PinouUtils.Delegate.Action OnBeforeGameSceneLoad { get; private set; } = new PinouUtils.Delegate.Action();
            public PinouUtils.Delegate.Action<Scene> OnAfterGameSceneLoad { get; private set; } = new PinouUtils.Delegate.Action<Scene>();

            public PinouUtils.Delegate.Action<Scene> OnSceneLoadComplete { get; private set; } = new PinouUtils.Delegate.Action<Scene>();
            #endregion

            #region Editor
#if UNITY_EDITOR
            public static void E_LoadScene(string scenePath)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(PinouConstants.SYSTEM_SCENES_PATH + PinouConstants.PINOU_APP_SCENE_NAME + ".unity", UnityEditor.SceneManagement.OpenSceneMode.Single);
                if (scenePath.ToLower().Contains("game"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(PinouConstants.SYSTEM_SCENES_PATH + PinouConstants.PINOU_UI_SCENE_NAME + ".unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);

                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));
            }
#endif
            #endregion
        }
    }
	
}