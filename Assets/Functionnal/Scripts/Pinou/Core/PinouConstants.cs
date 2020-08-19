#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public static class PinouConstants
	{
        public const string PINOU_APP_SCENE_NAME = "PinouApp";
        public const string PINOU_UI_SCENE_NAME = "PinouUI";
        public const string PINOU_INIT_SCENE_NAME = "PinouInit";

        public const string SCENES_PATH = "Assets/Functionnal/Scenes/";
        public const string SYSTEM_SCENES_PATH = "Assets/Functionnal/Scenes/Systems/";

        public static bool IS_SYSTEM_SCENE(string name)
        {
            return 
                name == PINOU_APP_SCENE_NAME ||
                name == PINOU_UI_SCENE_NAME ||
                name == PINOU_INIT_SCENE_NAME;
        }

        public static bool IS_GAME_SCENE(string name)
        {
            return 
                IS_SYSTEM_SCENE(name) == false &&
                name.Contains("Game");
        }


        public static bool IS_APP_SCENE(string name)
        {
            return name.Equals(PINOU_APP_SCENE_NAME);
        }
        public static bool IS_UI_SCENE(string name)
        {
            return name.Equals(PINOU_UI_SCENE_NAME);
        }
    }
}
