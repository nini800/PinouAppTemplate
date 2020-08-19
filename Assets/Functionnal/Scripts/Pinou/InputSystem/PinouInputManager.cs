#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.InputSystem
{
    public partial class PinouInputManagerData
    {
        public override PinouManager BuildManagerInstance()
        {
            return new PinouInputManager(this);
        }

        public partial class PinouInputManager : PinouManager
        {
            #region PinouManager Structure Integration
            public PinouInputManager(PinouInputManagerData dataRef) : base(dataRef)
            {
                Data = dataRef;
            }
            public new PinouInputManagerData Data { get; private set; }
            #endregion

            #region DataVars
            private readonly KeyCode[,] _joystickButtons = new KeyCode[,]
            {
            {KeyCode.JoystickButton0,KeyCode.JoystickButton1,KeyCode.JoystickButton2,KeyCode.JoystickButton3,KeyCode.JoystickButton4,KeyCode.JoystickButton5,KeyCode.JoystickButton6,KeyCode.JoystickButton7,KeyCode.JoystickButton8,KeyCode.JoystickButton9,KeyCode.JoystickButton10,KeyCode.JoystickButton11,KeyCode.JoystickButton12,KeyCode.JoystickButton13,KeyCode.JoystickButton14,KeyCode.JoystickButton15,KeyCode.JoystickButton16,KeyCode.JoystickButton17,KeyCode.JoystickButton18,KeyCode.JoystickButton19},
            {KeyCode.Joystick1Button0,KeyCode.Joystick1Button1,KeyCode.Joystick1Button2,KeyCode.Joystick1Button3,KeyCode.Joystick1Button4,KeyCode.Joystick1Button5,KeyCode.Joystick1Button6,KeyCode.Joystick1Button7,KeyCode.Joystick1Button8,KeyCode.Joystick1Button9,KeyCode.Joystick1Button10,KeyCode.Joystick1Button11,KeyCode.Joystick1Button12,KeyCode.Joystick1Button13,KeyCode.Joystick1Button14,KeyCode.Joystick1Button15,KeyCode.Joystick1Button16,KeyCode.Joystick1Button17,KeyCode.Joystick1Button18,KeyCode.Joystick1Button19},
            {KeyCode.Joystick2Button0,KeyCode.Joystick2Button1,KeyCode.Joystick2Button2,KeyCode.Joystick2Button3,KeyCode.Joystick2Button4,KeyCode.Joystick2Button5,KeyCode.Joystick2Button6,KeyCode.Joystick2Button7,KeyCode.Joystick2Button8,KeyCode.Joystick2Button9,KeyCode.Joystick2Button10,KeyCode.Joystick2Button11,KeyCode.Joystick2Button12,KeyCode.Joystick2Button13,KeyCode.Joystick2Button14,KeyCode.Joystick2Button15,KeyCode.Joystick2Button16,KeyCode.Joystick2Button17,KeyCode.Joystick2Button18,KeyCode.Joystick2Button19},
            {KeyCode.Joystick3Button0,KeyCode.Joystick3Button1,KeyCode.Joystick3Button2,KeyCode.Joystick3Button3,KeyCode.Joystick3Button4,KeyCode.Joystick3Button5,KeyCode.Joystick3Button6,KeyCode.Joystick3Button7,KeyCode.Joystick3Button8,KeyCode.Joystick3Button9,KeyCode.Joystick3Button10,KeyCode.Joystick3Button11,KeyCode.Joystick3Button12,KeyCode.Joystick3Button13,KeyCode.Joystick3Button14,KeyCode.Joystick3Button15,KeyCode.Joystick3Button16,KeyCode.Joystick3Button17,KeyCode.Joystick3Button18,KeyCode.Joystick3Button19},
            {KeyCode.Joystick4Button0,KeyCode.Joystick4Button1,KeyCode.Joystick4Button2,KeyCode.Joystick4Button3,KeyCode.Joystick4Button4,KeyCode.Joystick4Button5,KeyCode.Joystick4Button6,KeyCode.Joystick4Button7,KeyCode.Joystick4Button8,KeyCode.Joystick4Button9,KeyCode.Joystick4Button10,KeyCode.Joystick4Button11,KeyCode.Joystick4Button12,KeyCode.Joystick4Button13,KeyCode.Joystick4Button14,KeyCode.Joystick4Button15,KeyCode.Joystick4Button16,KeyCode.Joystick4Button17,KeyCode.Joystick4Button18,KeyCode.Joystick4Button19},
            {KeyCode.Joystick5Button0,KeyCode.Joystick5Button1,KeyCode.Joystick5Button2,KeyCode.Joystick5Button3,KeyCode.Joystick5Button4,KeyCode.Joystick5Button5,KeyCode.Joystick5Button6,KeyCode.Joystick5Button7,KeyCode.Joystick5Button8,KeyCode.Joystick5Button9,KeyCode.Joystick5Button10,KeyCode.Joystick5Button11,KeyCode.Joystick5Button12,KeyCode.Joystick5Button13,KeyCode.Joystick5Button14,KeyCode.Joystick5Button15,KeyCode.Joystick5Button16,KeyCode.Joystick5Button17,KeyCode.Joystick5Button18,KeyCode.Joystick5Button19},
            {KeyCode.Joystick6Button0,KeyCode.Joystick6Button1,KeyCode.Joystick6Button2,KeyCode.Joystick6Button3,KeyCode.Joystick6Button4,KeyCode.Joystick6Button5,KeyCode.Joystick6Button6,KeyCode.Joystick6Button7,KeyCode.Joystick6Button8,KeyCode.Joystick6Button9,KeyCode.Joystick6Button10,KeyCode.Joystick6Button11,KeyCode.Joystick6Button12,KeyCode.Joystick6Button13,KeyCode.Joystick6Button14,KeyCode.Joystick6Button15,KeyCode.Joystick6Button16,KeyCode.Joystick6Button17,KeyCode.Joystick6Button18,KeyCode.Joystick6Button19},
            {KeyCode.Joystick7Button0,KeyCode.Joystick7Button1,KeyCode.Joystick7Button2,KeyCode.Joystick7Button3,KeyCode.Joystick7Button4,KeyCode.Joystick7Button5,KeyCode.Joystick7Button6,KeyCode.Joystick7Button7,KeyCode.Joystick7Button8,KeyCode.Joystick7Button9,KeyCode.Joystick7Button10,KeyCode.Joystick7Button11,KeyCode.Joystick7Button12,KeyCode.Joystick7Button13,KeyCode.Joystick7Button14,KeyCode.Joystick7Button15,KeyCode.Joystick7Button16,KeyCode.Joystick7Button17,KeyCode.Joystick7Button18,KeyCode.Joystick7Button19},
            {KeyCode.Joystick8Button0,KeyCode.Joystick8Button1,KeyCode.Joystick8Button2,KeyCode.Joystick8Button3,KeyCode.Joystick8Button4,KeyCode.Joystick8Button5,KeyCode.Joystick8Button6,KeyCode.Joystick8Button7,KeyCode.Joystick8Button8,KeyCode.Joystick8Button9,KeyCode.Joystick8Button10,KeyCode.Joystick8Button11,KeyCode.Joystick8Button12,KeyCode.Joystick8Button13,KeyCode.Joystick8Button14,KeyCode.Joystick8Button15,KeyCode.Joystick8Button16,KeyCode.Joystick8Button17,KeyCode.Joystick8Button18,KeyCode.Joystick8Button19}
            };

            private readonly string[,] _joystickAxises = new string[,]
            {
            {"Joystick1AxisX","Joystick1AxisY","Joystick1Axis3","Joystick1Axis4","Joystick1Axis5","Joystick1Axis6","Joystick1Axis7","Joystick1Axis8","Joystick1Axis9","Joystick1Axis10"},
            {"Joystick2AxisX","Joystick2AxisY","Joystick2Axis3","Joystick2Axis4","Joystick2Axis5","Joystick2Axis6","Joystick2Axis7","Joystick2Axis8","Joystick2Axis9","Joystick2Axis10"},
            {"Joystick3AxisX","Joystick3AxisY","Joystick3Axis3","Joystick3Axis4","Joystick3Axis5","Joystick3Axis6","Joystick3Axis7","Joystick3Axis8","Joystick3Axis9","Joystick3Axis10"},
            {"Joystick4AxisX","Joystick4AxisY","Joystick4Axis3","Joystick4Axis4","Joystick4Axis5","Joystick4Axis6","Joystick4Axis7","Joystick4Axis8","Joystick4Axis9","Joystick4Axis10"},
            {"Joystick5AxisX","Joystick5AxisY","Joystick5Axis3","Joystick5Axis4","Joystick5Axis5","Joystick5Axis6","Joystick5Axis7","Joystick5Axis8","Joystick5Axis9","Joystick5Axis10"},
            {"Joystick6AxisX","Joystick6AxisY","Joystick6Axis3","Joystick6Axis4","Joystick6Axis5","Joystick6Axis6","Joystick6Axis7","Joystick6Axis8","Joystick6Axis9","Joystick6Axis10"},
            {"Joystick7AxisX","Joystick7AxisY","Joystick7Axis3","Joystick7Axis4","Joystick7Axis5","Joystick7Axis6","Joystick7Axis7","Joystick7Axis8","Joystick7Axis9","Joystick7Axis10"},
            {"Joystick8AxisX","Joystick8AxisY","Joystick8Axis3","Joystick8Axis4","Joystick8Axis5","Joystick8Axis6","Joystick8Axis7","Joystick8Axis8","Joystick8Axis9","Joystick8Axis10"}
            };
            public string GetJoystickAxis(int id, int axis)
            {
                id--;
                axis--;
                return _joystickAxises[id, axis];
            }
            #endregion

            #region Attributes, Accessors & Mutators
            private string[] _unityControllerNames;
            private string[] _controllerNames;
            private ControllerType[] _controllerTypes;
            public ControllerType GetControllerType(int id)
            {
                if (id == 0)
                {
                    return ControllerType.Keyboard;
                }

                if (id > _controllerTypes.Length || id < 0)
                {
                    return ControllerType.Null;
                }

                return _controllerTypes[id - 1];
            }

            private List<int> _disconnectedControllers = new List<int>();

            private const int AXES_COUNT = 10;
            private InputDirection[,] _lastAxesInputs;
            private InputDirection[,] _axesInputs;

            private float _wheelAxisValue;
            private InputDirection _wheelState;
            private InputDirection _lastWheelState;
            public InputDirection WheelState { get { return _wheelState; } }
            public InputDirection LastWheelState { get { return _lastWheelState; } }
            #endregion

            #region Behaviour
            public override void SlaveAwake()
            {
                _unityControllerNames = new string[0];
                _controllerNames = new string[0];
                _controllerTypes = new ControllerType[0];

                ComputeControllerTypes();
            }

            public override void SlaveUpdate()
            {
                ComputeControllerTypes();
                StoreAxesDirections();
            }

            private void ComputeControllerTypes()
            {
                //Remove empty string to ignore disconnected sticks
                string[] unityNames = Input.GetJoystickNames();
                List<string> tweakedNames = new List<string>(unityNames);

                for (int i = tweakedNames.Count - 1; i >= 0; i--)
                {
                    if (tweakedNames[i] == null || tweakedNames[i] == string.Empty)
                    {
                        tweakedNames.RemoveAt(i);

                        //Check to trigger disconnected event
                        if (_disconnectedControllers.Contains(i + 1) == false)
                        {
                            _disconnectedControllers.Add(i + 1);
                            //i + 1 will be the controller ID
                            OnControllerDisconnected.Invoke(i + 1);
                        }
                    }
                    //Check to trigger (re)connected event
                    else if (_disconnectedControllers.Contains(i + 1) == true)
                    {
                        _disconnectedControllers.Remove(i + 1);
                        //i + 1 will be the controller ID
                        OnControllerConnected.Invoke(i + 1);
                    }
                }

                //Check to trigger new controller connected event
                if (unityNames.Length > _unityControllerNames.Length)
                {
                    for (int i = _unityControllerNames.Length; i < unityNames.Length; i++)
                    {
                        OnControllerConnected.Invoke(i + 1);
                    }
                }

                //Only update if new controllers found
                if (tweakedNames.Count != _controllerNames.Length)
                {
                    _unityControllerNames = unityNames;
                    _controllerNames = tweakedNames.ToArray();
                    _controllerTypes = new ControllerType[_controllerNames.Length];

                    //Reset inputs aswell
                    _lastAxesInputs = new InputDirection[_controllerNames.Length, AXES_COUNT];
                    _axesInputs = new InputDirection[_controllerNames.Length, AXES_COUNT];

                    //Then check name length to know weither its a PS4 or XBOX controller
                    for (int i = 0; i < _controllerNames.Length; i++)
                    {
                        switch (_controllerNames[i].Length)
                        {
                            case 19:
                                _controllerTypes[i] = ControllerType.PS4;
                                continue;
                            case 33:
                                _controllerTypes[i] = ControllerType.xBox;
                                continue;
                        }

                        _controllerTypes[i] = ControllerType.xBox;
                    }

                    //Lastly triggger count changed event
                    OnControllerCountChange.Invoke(_controllerNames.Length);
                }

            }
            private void StoreAxesDirections()
            {
                //Wheel
                _lastWheelState = _wheelState;
                _wheelAxisValue = Input.GetAxis("Mouse ScrollWheel");
                _wheelState = (_wheelAxisValue > 0.8f ? InputDirection.Positive : (_wheelAxisValue < -0.8f ? InputDirection.Negative : InputDirection.None));

                //Controllers
                StoreLastAxesDirections();
                for (int controllerId = 0; controllerId < _controllerTypes.Length; controllerId++)
                {
                    for (int axis = 0; axis < AXES_COUNT; axis++)
                    {
                        float axisValue = Axis(controllerId + 1, (ControllerAxis)axis);
                        _axesInputs[controllerId, axis] =
                            (axisValue > 0.8f ? InputDirection.Positive :
                            (axisValue < -0.8f ? InputDirection.Negative :
                            InputDirection.None));
                    }
                }
            }
            private void StoreLastAxesDirections()
            {
                for (int i = 0; i < _controllerTypes.Length; i++)
                {
                    for (int j = 0; j < AXES_COUNT; j++)
                    {
                        _lastAxesInputs[i, j] = _axesInputs[i, j];
                    }
                }
            }
            #endregion

            #region Utilities
            #region High Level
            public bool GameInputPressed(int controllerId, PinouInput input)
            {
                int index = (int)input;
                return Data.GameInputs[index].IsPressed(controllerId);
            }
            public bool GameInputPressed(PinouInput input)
            {
                int index = (int)input;
                return Data.GameInputs[index].IsAnyPressed();
            }
            public float GameAxisValue(int controllerId, PinouAxis axis)
            {
                int index = (int)axis;
                return Data.GameAxes[index].AxisValue(controllerId);
            }
            public Vector2 GameAxis2DValue(int controllerId, Pinou2DAxis axis)
            {
                int index = (int)axis;
                Vector2 value = Data.GameAxes[index].Axis2DValue(controllerId);

                ControllerType cType = GetControllerType(controllerId);
                if (Data.GameAxes[index].StickSide != StickSide.None && cType != ControllerType.Keyboard)
                {
                    if (cType == ControllerType.PS4)
                    {
                        switch (Data.GameAxes[index].StickSide)
                        {
                            case StickSide.Left:
                                value = PinouUtils.Maths.DeadzoneVector(value,
                                    Data.PS4Sensitivity.LeftStick.InnerDeadZone,
                                    Data.PS4Sensitivity.LeftStick.OutDeadZone,
                                    Data.PS4Sensitivity.LeftStick.Linearity);
                                break;
                            case StickSide.Right:
                                value = PinouUtils.Maths.DeadzoneVector(value,
                                    Data.PS4Sensitivity.RightStick.InnerDeadZone,
                                    Data.PS4Sensitivity.RightStick.OutDeadZone,
                                    Data.PS4Sensitivity.RightStick.Linearity);
                                value.x *= Data.PS4Sensitivity.RightStick.HSensitivity;
                                value.y *= Data.PS4Sensitivity.RightStick.VSensitivity;
                                break;
                        }
                    }
                    else
                    {
                        switch (Data.GameAxes[index].StickSide)
                        {
                            case StickSide.Left:
                                value = PinouUtils.Maths.DeadzoneVector(value,
                                    Data.XBoxSensitivity.LeftStick.InnerDeadZone,
                                    Data.XBoxSensitivity.LeftStick.OutDeadZone,
                                    Data.XBoxSensitivity.LeftStick.Linearity);
                                break;
                            case StickSide.Right:
                                value = PinouUtils.Maths.DeadzoneVector(value,
                                    Data.XBoxSensitivity.RightStick.InnerDeadZone,
                                    Data.XBoxSensitivity.RightStick.OutDeadZone,
                                    Data.XBoxSensitivity.RightStick.Linearity);
                                value.x *= Data.XBoxSensitivity.RightStick.HSensitivity;
                                value.y *= Data.XBoxSensitivity.RightStick.VSensitivity;
                                break;
                        }
                    }
                }

                return value;
            }
            #endregion
            #region Low Level
            private delegate bool GetKey(KeyCode key);
            private delegate bool GetMouseButton(int button);
            public bool Button(int id, ControllerButton button, InputState inputState = InputState.Down)
            {
                if (_controllerTypes.Length < id || id == 0)
                {
                    return false;
                }

                GetKey getKey;
                if (inputState == InputState.Held)
                    getKey = Input.GetKey;
                else if (inputState == InputState.Up)
                    getKey = Input.GetKeyUp;
                else if (inputState == InputState.Down)
                    getKey = Input.GetKeyDown;
                else if (inputState == InputState.NotHeld)
                {
                    return !Button(id, button, InputState.Held);
                }
                else
                    getKey = Input.GetKey;

                switch (GetControllerType(id))
                {
                    case ControllerType.PS4:
                        switch (button)
                        {
                            case ControllerButton.RightPadDown:
                                return getKey(_joystickButtons[id, 1]);
                            case ControllerButton.RightPadRight:
                                return getKey(_joystickButtons[id, 2]);
                            case ControllerButton.RightPadUp:
                                return getKey(_joystickButtons[id, 3]);
                            case ControllerButton.RightPadLeft:
                                return getKey(_joystickButtons[id, 0]);
                            case ControllerButton.RightTrigger:
                                return getKey(_joystickButtons[id, 7]);
                            case ControllerButton.RightBumper:
                                return getKey(_joystickButtons[id, 5]);
                            case ControllerButton.RightMenu:
                                return getKey(_joystickButtons[id, 9]);
                            case ControllerButton.RightClick:
                                return getKey(_joystickButtons[id, 11]);
                            case ControllerButton.LeftClick:
                                return getKey(_joystickButtons[id, 10]);
                            case ControllerButton.LeftMenu:
                                return getKey(_joystickButtons[id, 8]);
                            case ControllerButton.LeftBumper:
                                return getKey(_joystickButtons[id, 4]);
                            case ControllerButton.LeftTrigger:
                                return getKey(_joystickButtons[id, 6]);
                            case ControllerButton.Power:
                                return getKey(_joystickButtons[id, 12]);
                            case ControllerButton.Pad:
                                return getKey(_joystickButtons[id, 13]);

                        }
                        break;
                    case ControllerType.xBox:
                        switch (button)
                        {
                            case ControllerButton.RightPadDown:
                                return getKey(_joystickButtons[id, 0]);
                            case ControllerButton.RightPadRight:
                                return getKey(_joystickButtons[id, 1]);
                            case ControllerButton.RightPadUp:
                                return getKey(_joystickButtons[id, 3]);
                            case ControllerButton.RightPadLeft:
                                return getKey(_joystickButtons[id, 2]);
                            case ControllerButton.RightBumper:
                                return getKey(_joystickButtons[id, 5]);
                            case ControllerButton.RightMenu:
                                return getKey(_joystickButtons[id, 7]);
                            case ControllerButton.RightClick:
                                return getKey(_joystickButtons[id, 9]);
                            case ControllerButton.LeftClick:
                                return getKey(_joystickButtons[id, 8]);
                            case ControllerButton.LeftMenu:
                                return getKey(_joystickButtons[id, 6]);
                            case ControllerButton.LeftBumper:
                                return getKey(_joystickButtons[id, 4]);
                            case ControllerButton.RightTrigger:
                                return AxisToButton(id, ControllerAxis.RightTrigger, inputState);
                            case ControllerButton.LeftTrigger:
                                return AxisToButton(id, ControllerAxis.LeftTrigger, inputState);
                            case ControllerButton.Power:
                                return false;
                            case ControllerButton.Pad:
                                return false;
                        }
                        break;

                }

                switch (button)
                {
                    case ControllerButton.LeftPadDown:
                        return AxisToButton(id, ControllerAxis.padVertical, inputState, InputDirection.Negative);
                    case ControllerButton.LeftPadUp:
                        return AxisToButton(id, ControllerAxis.padVertical, inputState, InputDirection.Positive);
                    case ControllerButton.LeftPadLeft:
                        return AxisToButton(id, ControllerAxis.padHorizontal, inputState, InputDirection.Negative);
                    case ControllerButton.LeftPadRight:
                        return AxisToButton(id, ControllerAxis.padHorizontal, inputState, InputDirection.Positive);
                    default:
                        return false;
                }
            }

            public float Axis(int id, ControllerAxis axis)
            {
                if (_controllerTypes.Length < id || id == 0)
                {
                    return 0f;
                }
                switch (GetControllerType(id))
                {
                    case ControllerType.PS4:
                        switch (axis)
                        {
                            case ControllerAxis.leftStickHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 1));
                            case ControllerAxis.leftStickVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 2)) * -1;
                            case ControllerAxis.rightStickHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 3));
                            case ControllerAxis.rightStickVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 6)) * -1;
                            case ControllerAxis.LeftTrigger:
                                return Input.GetAxis(GetJoystickAxis(id, 4));
                            case ControllerAxis.RightTrigger:
                                return Input.GetAxis(GetJoystickAxis(id, 5));
                            case ControllerAxis.padHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 7));
                            case ControllerAxis.padVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 8));
                            default:
                                break;
                        }
                        break;
                    case ControllerType.xBox:
                        switch (axis)
                        {
                            case ControllerAxis.leftStickHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 1));
                            case ControllerAxis.leftStickVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 2));
                            case ControllerAxis.rightStickHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 4));
                            case ControllerAxis.rightStickVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 5));
                            case ControllerAxis.LeftTrigger:
                                return (Input.GetAxis(GetJoystickAxis(id, 9)) - 0.5f) * 2f;
                            case ControllerAxis.RightTrigger:
                                return (Input.GetAxis(GetJoystickAxis(id, 10)) - 0.5f) * 2f;
                            case ControllerAxis.padHorizontal:
                                return Input.GetAxis(GetJoystickAxis(id, 6));
                            case ControllerAxis.padVertical:
                                return Input.GetAxis(GetJoystickAxis(id, 7));
                            default:
                                break;
                        }
                        break;
                }
                return 0f;
            }
            public bool AxisToButton(int id, ControllerAxis axis, InputState inputState = InputState.Held, InputDirection direction = InputDirection.Positive)
            {
                if (_controllerTypes.Length < id)
                {
                    return false;
                }

                //0 means every joysticks !
                if (id == 0)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        if (AxisToButton(i, axis, inputState, direction))
                        {
                            return true;
                        }
                    }
                    return false;
                }

                if (direction == InputDirection.Positive)
                {
                    switch (inputState)
                    {
                        case InputState.Held://held
                            return _axesInputs[id - 1, (int)axis] == InputDirection.Positive;
                        case InputState.Down://down
                            return _axesInputs[id - 1, (int)axis] == InputDirection.Positive && _lastAxesInputs[id - 1, (int)axis] != InputDirection.Positive;
                        case InputState.Up://u
                            return _axesInputs[id - 1, (int)axis] != InputDirection.Positive && _lastAxesInputs[id - 1, (int)axis] == InputDirection.Positive;
                        case InputState.NotHeld:
                            return !(_axesInputs[id - 1, (int)axis] == InputDirection.Positive);
                    }
                }
                else if (direction == InputDirection.Negative)
                {
                    switch (inputState)
                    {
                        case InputState.Held://held
                            return _axesInputs[id - 1, (int)axis] == InputDirection.Negative;
                        case InputState.Down://down
                            return _axesInputs[id - 1, (int)axis] == InputDirection.Negative && _lastAxesInputs[id - 1, (int)axis] != InputDirection.Negative;
                        case InputState.Up://up
                            return _axesInputs[id - 1, (int)axis] != InputDirection.Negative && _lastAxesInputs[id - 1, (int)axis] == InputDirection.Negative;
                        case InputState.NotHeld:
                            return !(_axesInputs[id - 1, (int)axis] == InputDirection.Negative);
                    }
                }
                else
                {
                    return false;
                }

                throw new System.NotImplementedException();
            }

            public Vector3 GetAimInputRelativeForMouse(Vector3 worldPos, Camera camera)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                Vector3 finalPoint = ray.origin + (ray.direction * (ray.origin.y - worldPos.y)) / Mathf.Abs(ray.direction.y);
                return finalPoint - worldPos;
            }
            #endregion
            #endregion

            #region Events
            public PinouUtils.Delegate.Action<int> OnControllerCountChange { get; private set; } = new PinouUtils.Delegate.Action<int>();
            public PinouUtils.Delegate.Action<int> OnControllerConnected { get; private set; } = new PinouUtils.Delegate.Action<int>();
            public PinouUtils.Delegate.Action<int> OnControllerDisconnected { get; private set; } = new PinouUtils.Delegate.Action<int>();
            #endregion
        }
    }
    
}

