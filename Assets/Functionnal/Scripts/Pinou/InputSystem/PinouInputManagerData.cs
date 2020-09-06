#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.InputSystem
{
    [CreateAssetMenu(fileName = "InputData", menuName = "Pinou/Managers/InputData", order = 1000)]
    public partial class PinouInputManagerData : PinouManagerData
    {
        [System.Serializable]
        public class ControllerSensitivityData
        {
            [System.Serializable]
            public class SensitivityData
            {
                [SerializeField] private float _innerDeadZone = 0.1f;
                [SerializeField] private float _outDeadZone = 0.1f;
                [SerializeField] private float _linearity = 1.3f;

                [SerializeField] private float _hSensitivity = 2f;
                [SerializeField] private float _vSensitivity = 1.5f;


                public float InnerDeadZone { get { return _innerDeadZone; } }
                public float OutDeadZone { get { return _outDeadZone; } }
                public float Linearity { get { return _linearity; } }

                public float HSensitivity { get { return _hSensitivity; } }
                public float VSensitivity { get { return _vSensitivity; } }
            }

            [SerializeField] private SensitivityData _leftStick;
            [SerializeField] private SensitivityData _rightStick;

            public SensitivityData LeftStick { get { return _leftStick; } }
            public SensitivityData RightStick { get { return _rightStick; } }

        }

        [Header("Shortcuts")]
        [Space]
        [SerializeField] private string _autoScriptPath = "Assets\\Functionnal\\Scripts\\Pinou\\InputSystem\\";
        [SerializeField] private PinouInputParameters[] _gameInputs;
        [SerializeField] private PinouAxisParameters[] _gameAxes;

        [Header("Controllers & Mouse Sensitivity")]
        [Space]
        [SerializeField] private float _mouseSensitivity;
        [SerializeField] private ControllerSensitivityData _ps4Sensitivity;
        [SerializeField] private ControllerSensitivityData _xBoxSensitivity;

        public PinouInputParameters[] GameInputs { get { return _gameInputs; } }
        public PinouAxisParameters[] GameAxes { get { return _gameAxes; } }

        public float MouseSensitivity { get { return _mouseSensitivity; } }
        public ControllerSensitivityData PS4Sensitivity { get { return _ps4Sensitivity; } }
        public ControllerSensitivityData XBoxSensitivity { get { return _xBoxSensitivity; } }

        private void OnValidate()
        {
            PinouAutoscript.UpdateInputManagerAutoscripts(_autoScriptPath, _gameInputs, _gameAxes);
            PinouAutoscript.UpdateInputReceiverAutoScript(_autoScriptPath, _gameInputs, _gameAxes);
        }
    }
}