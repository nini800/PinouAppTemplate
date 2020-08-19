using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou.InputSystem
{
    [System.Serializable]
    public class PinouInputParameters : CustomDrawedProperty
    {
        [System.Serializable]
        public class ComputerInputParameters
        {
            [System.Serializable]
            public class ComputerInputInfos
            {
                public ComputerInputType ComputerInputType;
                public int Key;
                public int MouseButtonId;
                public InputDirection WheelDirection;
            }
            public InputState InputState;
            public bool HasModifiers = false;
            public ComputerInputInfos[] BlockingModifiers;
            public ComputerInputInfos[] AllowingModifiers;
            public ComputerInputInfos Inputs;

        }
        [System.Serializable]
        public class ControllerInputParameters : CustomDrawedProperty
        {
            [System.Serializable]
            public class ControllerInputInfos
            {
                public ControllerInputType ControllerInputType;
                public ControllerButton Button;
                public ControllerAxis Axis;
                public InputDirection InputDirection;
            }
            public InputState InputState;

            public bool HasModifiers = false;
            public ControllerInputInfos[] BlockingModifiers;
            public ControllerInputInfos[] AllowingModifiers;
            public ControllerInputInfos Inputs;
        }

        public string InputName = "New Input";
        public ComputerInputParameters[] _computerInputs;
        public ControllerInputParameters[] _controllerInputs;

        public bool IsPressed(int controllerId)
        {
            if (controllerId == 0)
            {
                return IsKeyboardPressed();
            }
            else
            {
                return IsControllerPressed(controllerId);
            }
        }
        public bool IsAnyPressed()
        {
            for (int i = 1; i <= 8; i++)
            {
                if (IsControllerPressed(i))
                {
                    return true;
                }
            }
            return IsKeyboardPressed();
        }
        private bool IsKeyboardPressed()
        {
            for (int i = 0; i < _computerInputs.Length; i++)
            {
                if (_computerInputs[i].HasModifiers == true)
                {
                    //First check blocking modifiers
                    for (int j = 0; j < _computerInputs[i].BlockingModifiers.Length; j++)
                    {
                        switch (_computerInputs[i].BlockingModifiers[j].ComputerInputType)
                        {
                            case ComputerInputType.Mouse:
                                if (Input.GetMouseButton(_computerInputs[i].BlockingModifiers[j].MouseButtonId) == true)
                                    return false;
                                break;
                            case ComputerInputType.Keyboard:
                                if (Input.GetKey((KeyCode)_computerInputs[i].BlockingModifiers[j].Key) == true)
                                    return false;
                                break;
                        }
                    }

                    //Then check allowing modifiers
                    for (int j = 0; j < _computerInputs[i].AllowingModifiers.Length; j++)
                    {
                        switch (_computerInputs[i].AllowingModifiers[j].ComputerInputType)
                        {
                            case ComputerInputType.Mouse:
                                if (Input.GetMouseButton(_computerInputs[i].AllowingModifiers[j].MouseButtonId) == false)
                                    return false;
                                break;
                            case ComputerInputType.Keyboard:
                                if (Input.GetKey((KeyCode)_computerInputs[i].AllowingModifiers[j].Key) == false)
                                    return false;
                                break;
                        }
                    }
                }

                //Finally check inputs
                switch (_computerInputs[i].Inputs.ComputerInputType)
                {
                    case ComputerInputType.Mouse:
                        switch (_computerInputs[i].InputState)
                        {
                            case InputState.Held:
                                if (Input.GetMouseButton(_computerInputs[i].Inputs.MouseButtonId)) return true;
                                break;
                            case InputState.Down:
                                if (Input.GetMouseButtonDown(_computerInputs[i].Inputs.MouseButtonId)) return true;
                                break;
                            case InputState.Up:
                                if (Input.GetMouseButtonUp(_computerInputs[i].Inputs.MouseButtonId)) return true;
                                break;
                        }
                        break;
                    case ComputerInputType.Keyboard:
                        switch (_computerInputs[i].InputState)
                        {
                            case InputState.Held:
                                if (Input.GetKey((KeyCode)_computerInputs[i].Inputs.Key)) return true;
                                break;
                            case InputState.Down:
                                if (Input.GetKeyDown((KeyCode)_computerInputs[i].Inputs.Key)) return true;
                                break;
                            case InputState.Up:
                                if (Input.GetKeyUp((KeyCode)_computerInputs[i].Inputs.Key)) return true;
                                break;
                        }
                        break;
                    case ComputerInputType.Wheel:
                        bool cur = false, last = false;
                        switch (_computerInputs[i].Inputs.WheelDirection)
                        {
                            case InputDirection.Positive:
                                cur = PinouApp.Input.WheelState == InputDirection.Positive;
                                last = PinouApp.Input.LastWheelState == InputDirection.Positive;
                                break;
                            case InputDirection.Negative:
                                cur = PinouApp.Input.WheelState == InputDirection.Negative;
                                last = PinouApp.Input.LastWheelState == InputDirection.Negative;
                                break;
                        }
                        switch (_computerInputs[i].InputState)
                        {
                            case InputState.Held:
                                if (cur == true && last == true) return true;
                                break;
                            case InputState.Down:
                                if (cur == true && last == false) return true;
                                break;
                            case InputState.Up:
                                if (cur == false && last == true) return true;
                                break;
                        }
                        break;
                }
            }

            return false;
        }
        private bool IsControllerPressed(int controllerId)
        {
            for (int i = 0; i < _controllerInputs.Length; i++)
            {
                switch (_controllerInputs[i].Inputs.ControllerInputType)
                {
                    case ControllerInputType.Button:
                        if (PinouApp.Input.Button(controllerId, _controllerInputs[i].Inputs.Button, _controllerInputs[i].InputState)) return true;
                        break;
                    case ControllerInputType.Axis:
                        if (PinouApp.Input.AxisToButton(controllerId, _controllerInputs[i].Inputs.Axis, _controllerInputs[i].InputState, _controllerInputs[i].Inputs.InputDirection)) return true;
                        break;
                }
            }

            return false;
        }

        public float AxisValue(int controllerId)
        {
            float biggerAxis = 0f;
            for (int i = 0; i < _controllerInputs.Length; i++)
            {
                float axis = PinouApp.Input.Axis(controllerId, _controllerInputs[i].Inputs.Axis);
                if (Mathf.Abs(axis) > Mathf.Abs(biggerAxis))
                {
                    biggerAxis = axis;
                }
            }
            return biggerAxis;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PinouInputParameters))]
    public class InputParametersEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);

            if (EditorOpenClosed(Prop("InputName").stringValue))
            {
                Indent();
                PropField("InputName");
                Space(5);
                PropField("InputMode");
                IndentPixel(5);
                Space(5);
                BoldLabel("Computer");
                PropField("_computerInputs");
                Space(7);
                BoldLabel("Controller");
                PropField("_controllerInputs");
                UnindentLastPixel();
                Unindent();
            }
        }
    }
    [CustomPropertyDrawer(typeof(PinouInputParameters.ComputerInputParameters))]
    public class ComputerInputParametersEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            Space(3);

            PropField("InputState");
            PropField("HasModifiers");
            if (Prop("HasModifiers").boolValue == true)
            {
                Indent(2);
                PropField("BlockingModifiers");
                PropField("AllowingModifiers");
                Unindent(2);
            }
            PropField("Inputs");
        }
    }
    [CustomPropertyDrawer(typeof(PinouInputParameters.ComputerInputParameters.ComputerInputInfos))]
    public class ComputerInputInfosEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            Space(0);
            Unindent(2);

            PropField("ComputerInputType");
            switch ((ComputerInputType)Prop("ComputerInputType").enumValueIndex)
            {
                case ComputerInputType.Mouse:
                    PropField("MouseButtonId");
                    break;
                case ComputerInputType.Keyboard:
                    PropIntToEnum<KeyCode>("Key");
                    break;
                case ComputerInputType.Wheel:
                    PropField("WheelDirection");
                    break;
            }
            Indent(2);
        }
    }
    [CustomPropertyDrawer(typeof(PinouInputParameters.ControllerInputParameters))]
    public class ControllerInputParametersEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            Space(3);

            PropField("InputState");
            PropField("HasModifiers");
            if (Prop("HasModifiers").boolValue == true)
            {
                Indent(2);
                PropField("BlockingModifiers");
                PropField("AllowingModifiers");
                Unindent(2);
            }
            PropField("Inputs");
        }
    }
    [CustomPropertyDrawer(typeof(PinouInputParameters.ControllerInputParameters.ControllerInputInfos))]
    public class ControllerInputInfosEditor : PropertyDrawerExtended
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            Space(0);
            Unindent(2);

            PropField("ControllerInputType");
            switch ((ControllerInputType)Prop("ControllerInputType").enumValueIndex)
            {
                case ControllerInputType.Button:
                    PropField("Button");
                    break;
                case ControllerInputType.Axis:
                    PropField("Axis");
                    PropField("InputDirection");
                    break;
            }

            Indent(2);

        }
    }
#endif

}

