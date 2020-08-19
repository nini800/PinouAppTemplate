using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
    public class CustomDrawedProperty
    {
        [HideInInspector] public List<bool> E_OpenList;
        [HideInInspector] public List<bool> E_ToggleList;
    }
}

#if UNITY_EDITOR
namespace Pinou.Editor
{
    public class PropertyDrawerExtended : PropertyDrawer
    {
        public enum PropertyType
        {
            String,
            Float,
            Int,
            Object
        }
        public enum Position
        {
            Center,
            Left,
            Right
        }

        protected const int LINE_SIZE = 17;
        protected const int INDENT_SIZE = 15;
        protected const int SPACE_SIZE = 10;

        protected List<SerializedProperty> allInitializedProps;
        protected SerializedProperty currentProp;
        protected Rect currentPos;
        protected float startPosY;
        protected int openListIndex = 0;
        protected int toggleListIndex = 0;
        protected int lastPixelIndent = 0;
        protected bool isFakingGUI = false;
        protected bool firstSpace = true;
        protected GUIStyle boldLabelStyle = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property, label);

            return GetHeight();
        }
        protected void Init(SerializedProperty property, GUIContent label)
        {
            isFakingGUI = true;
            OnGUI(new Rect(), property, label);
            isFakingGUI = false;
        }

        /// <summary>
        /// Need base
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);
            currentPos.x += position.x;
            currentPos.width -= position.x;

            firstSpace = true;
            currentProp = property;
            currentPos = position;
            startPosY = currentPos.y;
            MakePosUsable();
            openListIndex = -1;
            toggleListIndex = -1;
        }

        protected void MakePosUsable()
        {
            currentPos.height = LINE_SIZE;
        }

        protected void Space()
        {
            Space(LINE_SIZE);
        }
        protected void LineSpace()
        {
            Space(LINE_SIZE);
        }

        protected void Space(float size)
        {
            if (firstSpace == true)
            {
                firstSpace = false;
                return;
            }
            currentPos.y += size;
        }

        protected float GetHeight()
        {
            return currentPos.y - startPosY + currentPos.height;
        }

        protected void ForceRepaintAll()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
            System.Type type = assembly.GetType("UnityEditor.InspectorWindow");
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

            System.Reflection.FieldInfo listInfo = type.GetField("m_AllInspectors", flags);
            var acutalList = listInfo.GetValue(type);

            for (int i = 0; i < (int)listInfo.FieldType.GetProperty("Count").GetValue(acutalList, null); i++)
            {
                object[] index = { i };
                var item = listInfo.FieldType.GetProperty("Item").GetValue(acutalList, index);
                item.GetType().GetMethod("Repaint").Invoke(item, null);
            }
        }

        protected void Indent(int indentNumber = 1)
        {
            currentPos.x += INDENT_SIZE * indentNumber;
            currentPos.width -= INDENT_SIZE * indentNumber;
        }
        protected void IndentPixel(int indentPixelSize)
        {
            lastPixelIndent = indentPixelSize;

            currentPos.x += indentPixelSize;
            currentPos.width -= indentPixelSize;
        }

        protected void Unindent(int unindentNumber = 1)
        {
            currentPos.x -= INDENT_SIZE * unindentNumber;
            currentPos.width += INDENT_SIZE * unindentNumber;
        }
        protected void UnindentLastPixel()
        {
            UnindentPixel(lastPixelIndent);
        }
        protected void UnindentPixel(int indentPixelSize)
        {
            currentPos.x -= indentPixelSize;
            currentPos.width += indentPixelSize;
        }

        protected SerializedProperty Prop(string name)
        {
            return currentProp.FindPropertyRelative(name);
        }
        protected string PropString(string name)
        {
            return Prop(name).stringValue;
        }
        protected int PropInt(string name)
        {
            return Prop(name).intValue;
        }
        protected float PropFloat(string name)
        {
            return Prop(name).floatValue;
        }
        protected System.Type PropType(string name)
        {
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            System.Type parentType = Prop(name).serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(Prop(name).propertyPath, flags);

            string[] perDot = Prop(name).propertyPath.Split('.');
            bool doubleContinue = false;

            foreach (string fieldName in perDot)
            {
                if (fieldName == "Array")
                {
                    doubleContinue = true;
                    continue;
                }
                if (doubleContinue == true)
                {
                    doubleContinue = false;
                    continue;
                }

                fi = parentType.GetField(fieldName, flags);

                if (fi != null)
                    parentType = fi.FieldType;
                else
                    parentType = null;

                if (parentType.IsArray)
                    parentType = parentType.GetElementType();
            }

            return parentType;
        }

        protected void BoldLabel(string content)
        {
            if (isFakingGUI == true)
            {
                LineSpace();
                return;
            }

            if (boldLabelStyle == null)
            {
                boldLabelStyle = new GUIStyle(GUI.skin.label);
                boldLabelStyle.fontStyle = FontStyle.Bold;
            }

            LineSpace();
            EditorGUI.LabelField(currentPos, content, boldLabelStyle);
        }
        protected void CenteredLabel(string content, bool bold = true)
        {
            TextAnchor old = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            if (bold == true)
            {
                BoldLabel(content);
            }
            else
            {
                Label(content);
            }
            GUI.skin.label.alignment = old;
        }
        protected void Label(string content)
        {
            if (isFakingGUI == true)
            {
                LineSpace();
                return;
            }

            LineSpace();
            EditorGUI.LabelField(currentPos, content);
        }

        protected void FakeFloatField(string name, float value)
        {
            if (isFakingGUI == true)
            {
                LineSpace();
                return;
            }

            LineSpace();

            bool old = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.FloatField(currentPos, name, value);
            GUI.enabled = old;
        }

        protected void PropField(SerializedProperty prop, string nameOverride = null)
        {
            if (prop == null) return;
            if (isFakingGUI == true)
            {
                Space(EditorGUI.GetPropertyHeight(prop));
                return;
            }

            LineSpace();

            float propHeight = EditorGUI.GetPropertyHeight(prop);
            float oldHeight = currentPos.height;
            currentPos.height = propHeight;
            if (nameOverride == null)
            {
                EditorGUI.PropertyField(currentPos, prop, true);
            }
            else
            {
                EditorGUI.PropertyField(currentPos, prop, new GUIContent(nameOverride), true);
            }
            currentPos.height = oldHeight;

            Space(EditorGUI.GetPropertyHeight(prop) - LINE_SIZE);
        }
        protected void PropField(string name, string nameOverride = null)
        {
            PropField(Prop(name), nameOverride);
        }
        protected void PropIntToEnum<T>(string name) where T : System.Enum
        {
            if (isFakingGUI == true)
            {
                LineSpace();
                return;
            }

            LineSpace();
            Prop(name).intValue = System.Convert.ToInt32(EditorGUI.EnumPopup(currentPos, name, (T)(object)Prop(name).intValue));
        }
        protected bool Button(string name, bool visible)
        {
            return Button(name, currentPos.width, visible);
        }
        protected bool Button(string name, float width = 0f, bool visible = true, Position position = Position.Center, float yOffset = 0f)
        {
            if (isFakingGUI == true)
            {
                LineSpace();
                return false;
            }

            float oldWidth = currentPos.width;
            float oldX = currentPos.x;

            if (width > 0)
            {
                currentPos.width = width;
                switch (position)
                {
                    case Position.Center:
                        currentPos.x = Screen.width * 0.5f - currentPos.width * 0.5f;
                        break;
                    case Position.Left:
                        currentPos.x = 5f;
                        break;
                    case Position.Right:
                        currentPos.x = Screen.width - currentPos.width - 20f;
                        break;
                    default:
                        currentPos.x = Screen.width * 0.5f - currentPos.width * 0.5f;
                        break;
                }
            }


            bool toReturn;
            LineSpace();

            currentPos.y += yOffset;
            if (visible == true)
            {
                toReturn = GUI.Button(currentPos, name, GUI.skin.button);
            }
            else
            {
                toReturn = GUI.Button(currentPos, "", GUI.skin.label);
            }
            currentPos.y -= yOffset;

            currentPos.width = oldWidth;
            currentPos.x = oldX;
            return toReturn;
        }
        protected bool OpenClosedBehaviour()
        {
            return EditorOpenClosed(currentProp.name);
        }
        protected bool EditorOpenClosed(string name)
        {
            openListIndex++;
            CheckOpenListSize();

            bool value = currentProp.FindPropertyRelative("E_OpenList").GetArrayElementAtIndex(openListIndex).boolValue;
            if (isFakingGUI == true)
            {
                LineSpace();
                return value;
            }

            LineSpace();
            string labelName = (value ? "▼ " : "► ") + name;
            GUIStyle style = new GUIStyle(GUI.skin.label);
            if (value == true)
            {
                style.fontStyle = FontStyle.Bold;
            }
            GUI.Label(currentPos, labelName, style);

            if (GUI.Button(currentPos, "", GUI.skin.label))
            {
                value = !value;
                currentProp.FindPropertyRelative("E_OpenList").GetArrayElementAtIndex(openListIndex).boolValue = value;
            }

            return value;
        }
        protected bool EditorToggle(string name)
        {
            toggleListIndex++;
            CheckToggleListSize();

            bool value = Prop("E_ToggleList").GetArrayElementAtIndex(toggleListIndex).boolValue;
            if (isFakingGUI == true)
            {
                LineSpace();
                return value;
            }

            LineSpace();
            string labelName = " " + name;
            GUIStyle style = new GUIStyle(GUI.skin.toggle);
            if (value == true)
            {
                style.fontStyle = FontStyle.Bold;
            }
            Prop("E_ToggleList").GetArrayElementAtIndex(toggleListIndex).boolValue = GUI.Toggle(currentPos, value, labelName, style);
            value = Prop("E_ToggleList").GetArrayElementAtIndex(toggleListIndex).boolValue;

            return value;
        }

        protected bool Toggle(string togglePropertyName)
        {
            bool value = currentProp.FindPropertyRelative(togglePropertyName).boolValue;
            if (isFakingGUI == true)
            {
                LineSpace();
                return value;
            }

            LineSpace();
            string labelName = togglePropertyName;

            GUIStyle style = new GUIStyle(GUI.skin.toggle);
            if (value == true)
            {
                style.fontStyle = FontStyle.Bold;
            }
            currentProp.FindPropertyRelative(togglePropertyName).boolValue = GUI.Toggle(currentPos, value, " " + labelName, style);

            return currentProp.FindPropertyRelative(togglePropertyName).boolValue;
        }

        protected void Slider(string name)
        {
            Slider(name, 0, 1);
        }
        protected void Slider(string name, float min, float max)
        {
            Slider(Prop(name), min, max);
        }
        protected void Slider(SerializedProperty prop, float min, float max, string overridenName = "")
        {
            LineSpace();

            if (isFakingGUI == true)
            {
                return;
            }

            string name = overridenName == "" ? prop.name : overridenName;
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float:
                    prop.floatValue = EditorGUI.Slider(currentPos, name, prop.floatValue, min, max);
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)EditorGUI.Slider(currentPos, name, prop.intValue, min, max);
                    break;
            }
        }

        private void CheckOpenListSize()
        {
            if (currentProp.FindPropertyRelative("E_OpenList").arraySize <= openListIndex)
            {
                openListIndex = currentProp.FindPropertyRelative("E_OpenList").arraySize;
                currentProp.FindPropertyRelative("E_OpenList").InsertArrayElementAtIndex(openListIndex);
            }
        }
        private void CheckToggleListSize()
        {
            if (Prop("E_ToggleList").arraySize <= toggleListIndex)
            {
                Prop("E_ToggleList").InsertArrayElementAtIndex(toggleListIndex);
            }
        }
    }
}
#endif