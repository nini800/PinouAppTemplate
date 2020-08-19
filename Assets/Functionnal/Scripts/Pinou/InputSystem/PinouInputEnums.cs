namespace Pinou.InputSystem
{
    public enum ControllerType
    {
        Null = 0,
        Keyboard = 1,
        PS4 = 2,
        xBox = 3
    }
    public enum ControllerButton
    {
        None,
        RightPadDown,
        RightPadRight,
        RightPadUp,
        RightPadLeft,
        LeftPadDown,
        LeftPadRight,
        LeftPadUp,
        LeftPadLeft,
        RightTrigger,
        RightBumper,
        RightMenu,
        RightClick,
        LeftClick,
        LeftMenu,
        LeftBumper,
        LeftTrigger,
        Power,
        Pad
    }
    public enum ControllerAxis
    {
        leftStickHorizontal = 0,
        leftStickVertical = 1,
        rightStickHorizontal = 2,
        rightStickVertical = 3,
        LeftTrigger = 4,
        RightTrigger = 5,
        padHorizontal = 6,
        padVertical = 7,
        Unused = 8,
        Unused2 = 9
    }
    public enum ControllerInputType
    {
        Button = 0,
        Axis = 1
    }
    public enum StickSide
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum ComputerInputType
    {
        Mouse = 0,
        Keyboard = 1,
        Wheel = 2
    }
    public enum ComputerAxisType
    {
        SimulatedByKeys = 0,
        ComputerAxis = 1
    }
    public enum ComputerAxis
    {
        MouseX = 0,
        MouseY = 1,
        Wheel = 2,
    }

    public enum InputState
    {
        Held = 0,
        Down = 1,
        Up = 2,
        NotHeld = 3
    }
    public enum InputDirection
    {
        None,
        Positive,
        Negative
    }
}