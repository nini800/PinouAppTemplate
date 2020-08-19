using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou.InputSystem
{
	[System.Serializable]
	public class PinouAxisParameters : CustomDrawedProperty
	{
        public string AxisName;
        public bool Is2D;
        public bool Revert;
        public bool Revert2D;
        public bool ClampToOne;
        public StickSide StickSide;

        public ComputerAxisType ComputerAxisType;
        public KeyCode PositiveKey;
        public KeyCode NegativeKey;
        public ComputerAxis ComputerAxis;

        public ComputerAxisType ComputerAxisType2D;
        public KeyCode Positive2DKey;
        public KeyCode Negative2DKey;
        public ComputerAxis ComputerAxis2D;

        public ControllerAxis Axis;
        public ControllerAxis Axis2D;

        public float AxisValue(int controllerId)
        {
            float value = 0f;
            //keyboard
            if (controllerId == 0)
            {
                if (ComputerAxisType == ComputerAxisType.SimulatedByKeys)
                {
                    if (Input.GetKey(PositiveKey))
                    {
                        value += 1f;
                    }
                    if (Input.GetKey(NegativeKey))
                    {
                        value -= 1f;
                    }
                }
                else
                {
                    switch (ComputerAxis)
                    {
                        case ComputerAxis.MouseX:
                            value = Input.GetAxis("Mouse X");
                            break;
                        case ComputerAxis.MouseY:
                            value = Input.GetAxis("Mouse Y");
                            break;
                        case ComputerAxis.Wheel:
                            value = Input.GetAxis("Mouse ScrollWheel");
                            break;
                    }
                }
            }
            //Controller
            else
            {
                value = PinouApp.Input.Axis(controllerId, Axis);
            }

            if (ClampToOne == true)
            {
                if (value > 1f)
                {
                    return 1f;
                }
                else if (value < -1f)
                {
                    return -1f;
                }
            }

            if (Revert == true) { value *= -1f; }
            return value;
        }
        public Vector2 Axis2DValue(int controllerId)
        {
            Vector2 value;
            //keyboard
            if (controllerId == 0)
            {
                value = Vector2.zero;
                if (ComputerAxisType == ComputerAxisType.SimulatedByKeys)
                {
                    if (Input.GetKey(PositiveKey))
                    {
                        value.x += 1f;
                    }
                    if (Input.GetKey(NegativeKey))
                    {
                        value.x -= 1f;
                    }
                }
                else
                {
                    switch (ComputerAxis)
                    {
                        case ComputerAxis.MouseX:
                            value.x = Input.GetAxis("Mouse X");
                            break;
                        case ComputerAxis.MouseY:
                            value.x = Input.GetAxis("Mouse Y");
                            break;
                        case ComputerAxis.Wheel:
                            value.x = Input.GetAxis("Mouse ScrollWheel");
                            break;
                    }
                }

                if (ComputerAxisType2D == ComputerAxisType.SimulatedByKeys)
                {
                    if (Input.GetKey(Positive2DKey))
                    {
                        value.y += 1f;
                    }
                    if (Input.GetKey(Negative2DKey))
                    {
                        value.y -= 1f;
                    }
                }
                else
                {
                    switch (ComputerAxis2D)
                    {
                        case ComputerAxis.MouseX:
                            value.y = Input.GetAxis("Mouse X");
                            break;
                        case ComputerAxis.MouseY:
                            value.y = Input.GetAxis("Mouse Y");
                            break;
                        case ComputerAxis.Wheel:
                            value.y = Input.GetAxis("Mouse ScrollWheel");
                            break;
                    }
                }
            }
            //Controller
            else
            {
                value = new Vector2(PinouApp.Input.Axis(controllerId, Axis), PinouApp.Input.Axis(controllerId, Axis2D));
            }

            if (ClampToOne == true)
            {
                float sqrMagn = value.sqrMagnitude;
                if (sqrMagn > 1)
                {
                    return value / Mathf.Sqrt(sqrMagn);
                }
            }

            if (Revert == true) { value.x *= -1f; }
            if (Revert2D == true) { value.y *= -1f; }
            return value;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PinouAxisParameters))]
    public class PinouAxisParametersEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            if (EditorOpenClosed(Prop("AxisName").stringValue))
            {
                Indent();
                PropField("AxisName");
                PropField("Is2D");
                if (Prop("Is2D").boolValue == true)
                {
                    IndentPixel(5);
                    PropField("StickSide");
                    UnindentPixel(5);
                }
                PropField("Revert");
                if (Prop("Is2D").boolValue == true)
                {
                    PropField("Revert2D");
                }
                PropField("ClampToOne");
                Space(5);
                BoldLabel("Computer");
                PropField("ComputerAxisType");
                if (((ComputerAxisType)Prop("ComputerAxisType").enumValueIndex) == ComputerAxisType.SimulatedByKeys)
                {
                    PropField("PositiveKey");
                    PropField("NegativeKey");
                }
                else
                {
                    PropField("ComputerAxis");
                }
                Space(5);

                if (Prop("Is2D").boolValue == true)
                {
                    PropField("ComputerAxisType2D");

                    if (((ComputerAxisType)Prop("ComputerAxisType2D").enumValueIndex) == ComputerAxisType.SimulatedByKeys)
                    {
                        PropField("Positive2DKey");
                        PropField("Negative2DKey");
                    }
                    else
                    {
                        PropField("ComputerAxis2D");
                    }
                }
                
                Space(7);
                BoldLabel("Controller");
                PropField("Axis");
                if (Prop("Is2D").boolValue == true)
                {
                    PropField("Axis2D");
                }
                Unindent();
            }
        }
    }
#endif
}