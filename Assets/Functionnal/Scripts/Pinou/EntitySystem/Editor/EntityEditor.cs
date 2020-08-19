using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pinou.EntitySystem;
using Sirenix.OdinInspector.Editor;

namespace Pinou.Editor
{
	[CustomEditor(typeof(Entity), true)]
	public class EntityEditor : OdinEditor
	{
        private Entity _instance => (Entity)target;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying == false)
            {
                return;
            }
            EditorGUILayout.Space(13f);
            GUILayout.Label("==Editor Runtime Infos==");
            GUI.enabled = false;
            EditorGUILayout.EnumPopup("Controller State", _instance.ControllerState);
            EditorGUILayout.EnumPopup("Being State", _instance.BeingState);
            EditorGUILayout.EnumPopup("Ability State", _instance.AbilityState);
            EditorGUILayout.EnumPopup("Movement State", _instance.MovementState);

            EditorGUILayout.EnumPopup("Movement Direction", _instance.MovementDirection);
            GUI.enabled = true;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}