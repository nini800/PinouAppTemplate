#pragma warning disable 0649
using UnityEngine;
using UnityEditor;
using UnityEditor.EventSystems;

namespace Pinou.UI
{
    [CustomEditor(typeof(UI_ScrollableEventTrigger))]
    public class UI_ScrollableEventTriggerEditor : EventTriggerEditor
    {
        public override void OnInspectorGUI()
        {
            DrawScriptField();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxDragDistanceForPointerDown"));
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        protected bool DrawScriptField()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool flag = true;
            iterator.NextVisible(flag);
            using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
            {
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
    }
}
