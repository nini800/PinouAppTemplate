#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Pinou;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Pinou.Editor
{
	public static class Editor_Shortcuts
	{

        #region Constants
        private const float WINDOW_WIDTH = 210f;
        public const float BUTTON_HEIGHT = 21f;
        public const float BUTTON_PADDING = 2f;
        #endregion

        #region SceneLoader
        private static Editor_SceneWindow _sceneWindow;

        [MenuItem("Tools/Windows/SceneLoader &l")]
		private static void OpenSceneWindow() 
        {
            string[] names = GetGameScenesNames(out string[] paths);
            Rect windowRect = WindowRect(names.Length + 2);
            _sceneWindow = EditorWindow.GetWindowWithRect<Editor_SceneWindow>(windowRect, true);
            _sceneWindow.minSize = _sceneWindow.maxSize = windowRect.size;
            _sceneWindow.Init(names, paths);
        }
        private static string[] GetGameScenesNames(out string[] paths)
        {
            List<string> names = new List<string>();
            List<string> _paths = new List<string>();
            foreach (string a in AssetDatabase.FindAssets("t:scene"))
            {
                string path = AssetDatabase.GUIDToAssetPath(a);
                if (path.Contains("Assets/Others") || path.Contains("Packages/com"))
                {
                    continue;
                }

                _paths.Add(path);
                string[] split = path.Split('/');
                string name = split[split.Length - 1].Split('.')[0];
                if (PinouConstants.IS_SYSTEM_SCENE(name))
                {
                    _paths.Remove(path);
                    continue;
                }

                names.Add(name);
            }

            paths = _paths.ToArray();
            return names.ToArray();
        }
        private static Rect WindowRect(int nameNumber)
        {
            Vector2 size = new Vector2(WINDOW_WIDTH, BUTTON_HEIGHT * nameNumber + BUTTON_PADDING * (nameNumber + 1));
            Vector2 pos = new Vector2(Screen.currentResolution.width * 0.5f - size.x * 0.5f, Screen.currentResolution.height * 0.5f - size.y * 0.5f);
            return new Rect(pos,size);
        }
        public static void LoadInitScene()
        {
            EditorSceneManager.OpenScene(PinouConstants.SYSTEM_SCENES_PATH + PinouConstants.PINOU_INIT_SCENE_NAME + ".unity", OpenSceneMode.Additive);

            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                if (SceneManager.GetSceneAt(i).name != PinouConstants.PINOU_INIT_SCENE_NAME)
                {
                    EditorSceneManager.CloseScene(SceneManager.GetSceneAt(i), true);
                }
            }
        }
        #endregion

        #region FolderShortcuts
        private static Editor_ShortcutsWindow _shortcutWindow;
        private static Editor_ShortcutsData _shortcutsData;

        [MenuItem("Tools/FolderShortcuts &d")]
        private static void OpenShortcutsWindows()
        {
            _shortcutsData = Resources.Load<Editor_ShortcutsData>("Editor_ShortcutsData");

            Rect windowRect = WindowRect(_shortcutsData.Shortcuts.Length);
            _shortcutWindow = EditorWindow.GetWindowWithRect<Editor_ShortcutsWindow>(windowRect, true);
            _shortcutWindow.minSize = _shortcutWindow.maxSize = windowRect.size;
            _shortcutWindow.Init(_shortcutsData);
        }

        private const string META_SUFFIX = ".meta";
        public static void OpenFolder(string path) 
        {
            List<string> names = new List<string>(Directory.GetFiles(path));
            for (int i = names.Count - 1; i >= 0; i--)
            {
                if (names[i].Substring(names[i].Length - 5).Equals(META_SUFFIX))
                {
                    names.RemoveAt(i);
                }
            }

            if (names.Count > 0)
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(names[0]);
            }
            else
            {
                names = new List<string>(Directory.GetDirectories(path));
                if (names.Count > 0)
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(names[0]);
                }
                else
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
                }
            }
        }
        #endregion

        [MenuItem("Tools/ClearConsole")]
        private static void ClearConsole()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorGUIUtility));
            System.Type type = assembly.GetType("UnityEditor.LogEntries");
            System.Reflection.MethodInfo method = type.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            method.Invoke(null, null);
        }
    }

    public class Editor_SceneWindow : EditorWindow
    {
        private string[] _names;
        private string[] _paths;

        public void Init(string[] names, string[] paths)
        {
            _names = names;
            _paths = paths;
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
            Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
            rect.height = Editor_Shortcuts.BUTTON_HEIGHT;
            rect.y += Editor_Shortcuts.BUTTON_PADDING;

            //Init special case
            if (GUI.Button(rect, "Init"))
            {
                Editor_Shortcuts.LoadInitScene();
                Close();
            }

            //Just some space between init and game scenes
            rect.y += Editor_Shortcuts.BUTTON_HEIGHT + Editor_Shortcuts.BUTTON_PADDING;
            //And the natural space for the next button
            rect.y += Editor_Shortcuts.BUTTON_HEIGHT + Editor_Shortcuts.BUTTON_PADDING;

            if (_names == null)
            {
                return;
            }

            for (int i = 0; i < _names.Length; i++)
            {
                if (GUI.Button(rect, _names[i]))
                {
                    PinouSceneManagerData.PinouSceneManager.E_LoadScene(_paths[i]);
                    Close();
                }

                rect.y += Editor_Shortcuts.BUTTON_HEIGHT + Editor_Shortcuts.BUTTON_PADDING;
            }
        }
    }

    

    public class Editor_ShortcutsWindow : EditorWindow
    {
        private Editor_ShortcutsData _data;
        public void Init(Editor_ShortcutsData data)
        {
            _data = data;
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
            Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
            rect.height = Editor_Shortcuts.BUTTON_HEIGHT;
            rect.y += Editor_Shortcuts.BUTTON_PADDING;

            if (_data == null)
            {
                return;
            }

            for (int i = 0; i < _data.Shortcuts.Length; i++)
            {
                if (_data.Shortcuts[i].path == null || _data.Shortcuts[i].path.Length == 0)//Title
                {
                    TextAnchor old = GUI.skin.label.alignment;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(rect, _data.Shortcuts[i].name);
                    GUI.skin.label.alignment = old;
                }
                else
                {
                    rect.x = 0f;
                    if (GUI.Button(rect, _data.Shortcuts[i].name))
                    {
                        Editor_Shortcuts.OpenFolder(_data.Shortcuts[i].path);
                        Close();
                    }
                }

                rect.y += Editor_Shortcuts.BUTTON_HEIGHT + Editor_Shortcuts.BUTTON_PADDING;
            }
        }
    }
}