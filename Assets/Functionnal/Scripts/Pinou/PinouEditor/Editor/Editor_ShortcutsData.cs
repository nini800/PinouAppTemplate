#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
    [CreateAssetMenu(fileName = "Editor_ShortcutData", menuName = "Editor/Editor_ShortcutData", order = 1000)]
	public class Editor_ShortcutsData : ScriptableObject
	{
        [System.Serializable]
        public class ShortcutData
        {
            public string name;
            public string path;
        }
        [SerializeField] private ShortcutData[] _shortcuts;

        public ShortcutData[] Shortcuts { get => _shortcuts; }
    }
}