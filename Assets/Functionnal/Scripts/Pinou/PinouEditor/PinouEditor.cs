using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Pinou.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PinouBehaviour), true)]
    public class PinouEditor : OdinEditor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        #region Styles vars
        protected bool styleInitialized = false;
        protected GUIStyle boldLabel;
        protected GUIStyle titleLabel;
        #endregion

        #region Behaviour
        public sealed override void OnInspectorGUI()
        {
            HandleStyles();
            InspectorGUI();
            InspectorSuffixGUI();
        }
        protected virtual void HandleStyles()
        {
            if (styleInitialized == false)
            {
                boldLabel = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                };
                titleLabel = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 21
                };
                styleInitialized = true;
            }
        }
        protected virtual void InspectorGUI()
        {
            DrawDefaultInspector();
        }
        protected virtual void InspectorSuffixGUI()
        {

        }

		#endregion

		#region Utilities
		protected void DrawScriptField()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
            {
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        protected void DefaultInspectorGUI(bool skipScriptField)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        protected void RecordUndo(string changeName)
        {
            Undo.RecordObject(target, changeName + " on " + target.name + " change");
        }
        #endregion

        #region Properties
        protected void PropField(SerializedProperty prop)
        {
            RecordUndo(prop.name);
            EditorGUILayout.PropertyField(prop, true);
            serializedObject.ApplyModifiedProperties();
        }
        protected void PropField(string serializedName)
        {
            RecordUndo(serializedName);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(serializedName), true);
            serializedObject.ApplyModifiedProperties();
        }
        protected void PropField(Rect rect, string serializedName)
        {
            RecordUndo(serializedName);
            EditorGUI.PropertyField(rect, serializedObject.FindProperty(serializedName), true);
            serializedObject.ApplyModifiedProperties();
        }

        protected void CenteredHeader(string content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(content, boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        protected void Title(string content)
        {
            GUILayout.Label(content, titleLabel);
        }
        protected void CenteredTitle(string content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(content, titleLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        protected bool Button(string name)
        {
            RecordUndo("Button effect: " + name);
            return GUILayout.Button(name);
        }
        protected bool Button(Rect rect, string name)
        {
            RecordUndo("Button effect: " + name);
            return GUI.Button(rect, name);
        }
        #endregion

    }
#endif
}
