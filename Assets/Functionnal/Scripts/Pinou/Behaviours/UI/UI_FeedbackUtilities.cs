#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou.UI
{
	public class UI_FeedbackUtilities : PinouBehaviour
	{
        [System.Serializable]
        private class TransitionParameters
        {
            [SerializeField] private bool _instant = false;
            [SerializeField, ShowIf("@!_instant")] private bool _randomDelays = false;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&!_randomDelays")] private float _waitTime;
            [SerializeField, ShowIf("@!_instant&&_randomDelays")] private float _minWaitTime;
            [SerializeField, ShowIf("@!_instant&&_randomDelays")] private float _maxWaitTime;
            [SerializeField, ShowIf("@!_instant&&!_randomDelays")] private float _duration;
            [SerializeField, ShowIf("@!_instant&&_randomDelays")] private float _minDuration;
            [SerializeField, ShowIf("@!_instant&&_randomDelays")] private float _maxDuration;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant")] private bool _useCurve = false;
            [SerializeField, ShowIf("@!_instant&&!_useCurve")] private float _power = 1;
            [SerializeField, ShowIf("@!_instant&&_useCurve")] private AnimationCurve _curve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparateSize")] private bool _sizeUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparateSize)), ShowIf("@!_instant&&E_SeparateSize&&!_sizeUseCurve")] private float _sizePower = 1;
            [SerializeField, ShowIf(nameof(E_SeparateSize)), ShowIf("@!_instant&&E_SeparateSize&&_sizeUseCurve")] private AnimationCurve _sizeCurve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparateScale")] private bool _scaleUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparateScale)), ShowIf("@!_instant&&!E_SeparateScale&&_scaleUseCurve")] private float _scalePower = 1;
            [SerializeField, ShowIf(nameof(E_SeparateScale)), ShowIf("@!_instant&&E_SeparateScale&&_scaleUseCurve")] private AnimationCurve _scaleCurve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparatePosition")] private bool _positionUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparatePosition)), ShowIf("@!_instant&&!E_SeparatePosition&&_positionUseCurve")] private float _positionPower = 1;
            [SerializeField, ShowIf(nameof(E_SeparatePosition)), ShowIf("@!_instant&&E_SeparatePosition&&_positionUseCurve")] private AnimationCurve _positionCurve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparateRotation")] private bool _rotationUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparateRotation)), ShowIf("@!_instant&&E_SeparateRotation&&!_rotationUseCurve")] private float _rotationPower = 1;
            [SerializeField, ShowIf(nameof(E_SeparateRotation)), ShowIf("@!_instant&&E_SeparateRotation&&_rotationUseCurve")] private AnimationCurve _rotationCurve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparateColor")] private bool _colorUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparateColor)), ShowIf("@!_instant&&E_SeparateColor&&!_colorUseCurve")] private float _colorPower = 1;
            [SerializeField, ShowIf(nameof(E_SeparateColor)), ShowIf("@!_instant&&E_SeparateColor&&_colorUseCurve")] private AnimationCurve _colorCurve;
            [Space(3f)]
            [SerializeField, ShowIf("@!_instant&&E_SeparateFontSize")] private bool _fontSizeUseCurve = false;
            [SerializeField, ShowIf(nameof(E_SeparateFontSize)), ShowIf("@!_instant&&E_SeparateFontSize&&!_fontSizeUseCurve")] private float _fontSizePower = 1;
            [SerializeField, ShowIf(nameof(E_SeparateFontSize)), ShowIf("@!_instant&&E_SeparateFontSize&&_fontSizeUseCurve")] private AnimationCurve _fontSizeCurve;

#if UNITY_EDITOR
            [HideInInspector] public bool E_SeparateSize;
            [HideInInspector] public bool E_SeparateScale;
            [HideInInspector] public bool E_SeparatePosition;
            [HideInInspector] public bool E_SeparateRotation;
            [HideInInspector] public bool E_SeparateColor;
            [HideInInspector] public bool E_SeparateFontSize;

			public bool Instant { get => _instant; }
            public bool RandomDelays { get => _randomDelays; }
            public float WaitTime { get => _waitTime; }
            public float MinWaitTime { get => _minWaitTime; }
            public float MaxWaitTime { get => _maxWaitTime; }
            public float Duration { get => _duration; }
            public float MinDuration { get => _minDuration; }
            public float MaxDuration { get => _maxDuration; }
            public bool UseCurve { get => _useCurve; }
            public bool SizeUseCurve { get => _sizeUseCurve; }
            public bool ScaleUseCurve { get => _scaleUseCurve; }
            public bool PositionUseCurve { get => _positionUseCurve; }
            public bool RotationUseCurve { get => _rotationUseCurve; }
            public bool ColorUseCurve { get => _colorUseCurve; }
            public bool FontSizeUseCurve { get => _fontSizeUseCurve; }
            public float Power { get => _power; }
            public float SizePower { get => _sizePower; }
            public float ScalePower { get => _scalePower; }
            public float PositionPower { get => _positionPower; }
            public float RotationPower { get => _rotationPower; }
            public float ColorPower { get => _colorPower; }
            public float FontSizePower { get => _fontSizePower; }
            public AnimationCurve Curve { get => _curve; }
            public AnimationCurve SizeCurve { get => _sizeCurve; }
            public AnimationCurve ScaleCurve { get => _scaleCurve; }
            public AnimationCurve PositionCurve { get => _positionCurve; }
            public AnimationCurve RotationCurve { get => _rotationCurve; }
            public AnimationCurve ColorCurve { get => _colorCurve; }
            public AnimationCurve FontSizeCurve { get => _fontSizeCurve; }
#endif
        }
        private class TransitionSnapshot
        {
            public TransitionSnapshot(TransitionParameters parameters)
            {
                Parameters = parameters;
                if (parameters.RandomDelays == true)
                {
                    WaitTime = Random.Range(parameters.MinWaitTime, parameters.MaxWaitTime);
                    Duration = Random.Range(parameters.MinDuration, parameters.MaxDuration);
                }
                else
                {
                    WaitTime = parameters.WaitTime;
                    Duration = parameters.Duration;
                }
            }

            public TransitionParameters Parameters;
            public float WaitTime;
            public float Duration;
        }
        [System.Serializable]
        private class GraphicState
        {
            [Header("Transition")]
            [Space]
            [SerializeField] private TransitionParameters _transitionToState;

            [Header("State Parameters")]
            [Space]
            [SerializeField] private bool _changeSize = false;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize))] private bool _absoluteSize = true;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize))] private bool _randomSize = false;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize))] private bool _syncSizeAxises = true;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@!_randomSize&&!_syncSizeAxises")] private Vector2 _sizeDelta;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@!_randomSize&&_syncSizeAxises")] private float _syncSize;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@_randomSize&&!_syncSizeAxises")] private Vector2 _minSizeDelta;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@_randomSize&&!_syncSizeAxises")] private Vector2 _maxSizeDelta;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@_randomSize&&_syncSizeAxises")] private float _minSyncSize;
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize)), ShowIf("@_randomSize&&_syncSizeAxises")] private float _maxSyncSize;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeSize/Size"), ShowIfGroup(nameof(_changeSize))] private bool _size_SeparateTransition = false;

            [SerializeField] private bool _changeScale = false;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale))] private bool _absoluteScale = true;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale))] private bool _randomScale = false;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale))] private bool _syncScaleAxises = true;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@!_randomScale&&!_syncScaleAxises")] private Vector3 _scaleDelta;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@!_randomScale&&_syncScaleAxises")] private float _syncScale;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@_randomScale&&!_syncScaleAxises")] private Vector3 _minScaleDelta;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@_randomScale&&!_syncScaleAxises")] private Vector3 _maxScaleDelta;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@_randomScale&&_syncScaleAxises")] private float _minSyncScale;
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale)), ShowIf("@_randomScale&&_syncScaleAxises")] private float _maxSyncScale;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeScale/Scale"), ShowIfGroup(nameof(_changeScale))] private bool _scale_SeparateTransition = false;

            [SerializeField] private bool _changeRotation = false;
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation))] private bool _absoluteRotation = true;
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation))] private bool _randomRotation = false;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation)), ShowIf("@!_randomRotation")] private float _rotation;
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation)), ShowIf("@_randomRotation")] private float _minRotation;
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation)), ShowIf("@_randomRotation")] private float _maxRotation;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeRotation/Rotation"), ShowIfGroup(nameof(_changeRotation))] private bool _rotation_SeparateTransition = false;

            [SerializeField] private bool _changePosition = false;
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition))] private bool _absolutePosition = true;
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition))] private bool _randomPosition = false;
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition))] private bool _positionSeparateAxisRandom = false;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition)), ShowIf("@!_randomPosition")] private Vector3 _position;
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition)), ShowIf("@_randomPosition")] private Vector3 _minPosition;
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition)), ShowIf("@_randomPosition")] private Vector3 _maxPosition;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changePosition/Position"), ShowIfGroup(nameof(_changePosition))] private bool _position_SeparateTransition = false;

            [SerializeField] private bool _changeColor = false;
            [SerializeField, FoldoutGroup("_changeColor/Color"), ShowIfGroup(nameof(_changeColor))] private bool _randomColor = false;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeColor/Color"), ShowIfGroup(nameof(_changeColor)), ShowIf("@!_randomColor")] private Color _color;
            [SerializeField, FoldoutGroup("_changeColor/Color"), ShowIfGroup(nameof(_changeColor)), ShowIf("@_randomColor")] private Color _minColor;
            [SerializeField, FoldoutGroup("_changeColor/Color"), ShowIfGroup(nameof(_changeColor)), ShowIf("@_randomColor")] private Color _maxColor;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeColor/Color"), ShowIfGroup(nameof(_changeColor))] private bool _color_SeparateTransition = false;

            public TransitionParameters TransitionToState => _transitionToState;

            public bool ChangeSize { get => _changeSize; }
            public bool AbsoluteSize { get => _absoluteSize; }
            public bool RandomSize { get => _randomSize; }
            public bool SyncSizeAxises { get => _syncSizeAxises; }
            public Vector2 SizeDelta { get => _sizeDelta; }
            public float SyncSize { get => _syncSize; }
            public Vector2 MinSizeDelta { get => _minSizeDelta; }
            public Vector2 MaxSizeDelta { get => _maxSizeDelta; }
            public float MinSyncSize { get => _minSyncSize; }
            public float MaxSyncSize { get => _maxSyncSize; }
            public bool Size_SeparateTransition { get => _size_SeparateTransition; }

            public bool ChangeScale { get => _changeScale; }
            public bool AbsoluteScale { get => _absoluteScale; }
            public bool RandomScale { get => _randomScale; }
            public bool SyncScaleAxises { get => _syncScaleAxises; }
            public Vector3 ScaleDelta { get => _scaleDelta; }
            public float SyncScale { get => _syncScale; }
            public Vector3 MinScaleDelta { get => _minScaleDelta; }
            public Vector3 MaxScaleDelta { get => _maxScaleDelta; }
            public float MinSyncScale { get => _minSyncScale; }
            public float MaxSyncScale { get => _maxSyncScale; }
            public bool Scale_SeparateTransition { get => _scale_SeparateTransition; }

            public bool ChangeRotation { get => _changeRotation; }
            public bool AbsoluteRotation { get => _absoluteRotation; }
            public bool RandomRotation { get => _randomRotation; }
            public float Rotation { get => _rotation; }
            public float MinRotation { get => _minRotation; }
            public float MaxRotation { get => _maxRotation; }
            public bool Rotation_SeparateTransition { get => _rotation_SeparateTransition; }

            public bool ChangePosition { get => _changePosition; }
            public bool AbsolutePosition { get => _absolutePosition; }
            public bool RandomPosition { get => _randomPosition; }
            public bool PositionSeparateAxisRandom { get => _positionSeparateAxisRandom; }
            public Vector3 Position { get => _position; }
            public Vector3 MinPosition { get => _minPosition; }
            public Vector3 MaxPosition { get => _maxPosition; }
            public bool Position_SeparateTransition { get => _position_SeparateTransition; }

            public bool ChangeColor { get => _changeColor; }
            public bool RandomColor { get => _randomColor; }
            public Color Color { get => _color; }
            public Color MinColor { get => _minColor; }
            public Color MaxColor { get => _maxColor; }
            public bool Color_SeparateTransition { get => _color_SeparateTransition; }
		}
        private class GraphicSnapshot
        {
            protected GraphicSnapshot() { }
            public GraphicSnapshot(GraphicState state, GraphicSnapshot snap)
            {
                CopyFrom(state, snap);
            }
            public GraphicSnapshot(GraphicSnapshot toCopy)
            {
                SyncSize = toCopy.SyncSize;
                SyncScale = toCopy.SyncScale;
                SizeDelta = toCopy.SizeDelta;
                ScaleDelta = toCopy.ScaleDelta;
                Position = toCopy.Position;
                Rotation = toCopy.Rotation;
                Color = toCopy.Color;
            }

            public GraphicSnapshot(MaskableGraphic graphic)
            {
                CopyFrom(graphic);
            }
            public void CopyFrom(GraphicState state, GraphicSnapshot snap)
            {
                if (snap == this)
                {
                    snap = new GraphicSnapshot(this);
                }

                if (state.RandomSize == true)
                {
                    SyncSize = Random.Range(state.MinSyncSize, state.MaxSyncSize);
                    SizeDelta.x = Random.Range(state.MinSizeDelta.x, state.MaxSizeDelta.x);
                    SizeDelta.y = Random.Range(state.MinSizeDelta.y, state.MaxSizeDelta.y);
                }
                else
                {
                    SyncSize = state.SyncSize;
                    SizeDelta = state.SizeDelta;
                }
                if (state.AbsoluteSize == false)
                {
                    SizeDelta += snap.SizeDelta;
                    SyncSize += snap.SyncSize;
                }

                if (state.RandomScale == true)
                {
                    SyncScale = Random.Range(state.MinSyncScale, state.MaxSyncScale);
                    ScaleDelta.x = Random.Range(state.MinScaleDelta.x, state.MaxScaleDelta.x);
                    ScaleDelta.y = Random.Range(state.MinScaleDelta.y, state.MaxScaleDelta.y);
                }
                else
                {
                    SyncScale = state.SyncScale;
                    ScaleDelta = state.ScaleDelta;
                }
                if (state.AbsoluteScale == false)
                {
                    ScaleDelta += snap.ScaleDelta;
                    SyncScale += snap.SyncScale;
                }

                if (state.RandomPosition == true)
                {
                    if (state.PositionSeparateAxisRandom == true)
                    {
                        Position.x = Random.Range(state.MinPosition.x, state.MaxPosition.x);
                        Position.y = Random.Range(state.MinPosition.y, state.MaxPosition.y);
                        Position.z = Random.Range(state.MinPosition.z, state.MaxPosition.z);
                    }
                    else
                    {
                        Position = Vector3.Lerp(state.MinPosition, state.MaxPosition, Random.value);
                    }
                }
                else
                {
                    Position = state.Position;
                }
                if (state.AbsolutePosition == false)
                {
                    Position += snap.Position;
                }

                if (state.RandomRotation == true)
                {
                    Rotation = Random.Range(state.MinRotation, state.MaxRotation);
                }
                else
                {
                    Rotation = state.Rotation;
                }
                if (state.AbsoluteRotation == false)
                {
                    Rotation += snap.Rotation;
                }


                if (state.RandomColor == true)
                {
                    Color = Color.Lerp(state.MinColor, state.MaxColor, Random.value);
                }
                else
                {
                    Color = state.Color;
                }
            }
            public void CopyFrom(MaskableGraphic graphic)
            {
                UnityEngine.RectTransform rect = graphic.GetComponent<RectTransform>();

                SyncScale = rect.localScale.x;
                ScaleDelta = rect.localScale;

                SyncSize = rect.sizeDelta.x;
                SizeDelta = rect.sizeDelta;

                Position = rect.anchoredPosition3D;
                Rotation = rect.eulerAngles.z;
                Color = graphic.color;
            }

            public static void ApplySnapshotLerp(MaskableGraphic graphic, GraphicState state, TransitionParameters transition, GraphicSnapshot origin, GraphicSnapshot target, float t)
            {
                RectTransform rect = graphic.GetComponent<RectTransform>();
                float progress = 0;
                if (state.ChangeSize == true)
                {
                    UpdateProgress(ref progress, t, state.Size_SeparateTransition,
                        transition.SizeUseCurve, transition.SizeCurve, transition.SizePower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    if (state.SyncSizeAxises == true)
                    {
                        rect.sizeDelta = Vector2.one * Mathf.LerpUnclamped(origin.SyncSize, target.SyncSize, progress);
                    }
                    else
                    {
                        rect.sizeDelta = Vector2.LerpUnclamped(origin.SizeDelta, target.SizeDelta, progress);
                    }
                }

                if (state.ChangeScale == true)
                {
                    UpdateProgress(ref progress, t, state.Scale_SeparateTransition,
                        transition.ScaleUseCurve, transition.ScaleCurve, transition.ScalePower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    if (state.SyncScaleAxises == true)
                    {
                        rect.localScale = Vector3.one * Mathf.LerpUnclamped(origin.SyncScale, target.SyncScale, progress);
                    }
                    else
                    {
                        rect.localScale = Vector3.LerpUnclamped(origin.ScaleDelta, target.ScaleDelta, progress);
                    }
                }

                if (state.ChangePosition == true)
                {
                    UpdateProgress(ref progress, t, state.Position_SeparateTransition,
                        transition.PositionUseCurve, transition.PositionCurve, transition.PositionPower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    rect.anchoredPosition3D = Vector3.LerpUnclamped(origin.Position, target.Position, progress);
                }

                if (state.ChangeRotation == true)
                {
                    UpdateProgress(ref progress, t, state.Rotation_SeparateTransition,
                        transition.RotationUseCurve, transition.RotationCurve, transition.RotationPower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    rect.localEulerAngles = rect.localEulerAngles.SetZ(Mathf.LerpUnclamped(origin.Rotation, target.Rotation, progress));
                }

                if (state.ChangeColor == true)
                {
                    UpdateProgress(ref progress, t, state.Color_SeparateTransition,
                         transition.ColorUseCurve, transition.ColorCurve, transition.ColorPower,
                         transition.UseCurve, transition.Curve, transition.Power);

                    graphic.color = Color.LerpUnclamped(origin.Color, target.Color, progress);
                }

            }
            protected static void UpdateProgress(ref float progress, float t, bool SeparateTransition, bool useCurve, AnimationCurve curve, float pow, bool defaultUseCurve, AnimationCurve defaultCurve, float defaultPow)
            {
                if (SeparateTransition == true)
                {
                    progress = useCurve == true ? curve.Evaluate(t) : Mathf.Pow(t, pow);
                }
                else
                {
                    progress = defaultUseCurve == true ? defaultCurve.Evaluate(t) : Mathf.Pow(t, defaultPow);
                }
            }

            public float SyncSize;
            public float SyncScale;
            public Vector2 SizeDelta;
            public Vector2 ScaleDelta;
            public Vector3 Position;
            public float Rotation;
            public Color Color;
        }
        [System.Serializable]
        private class TextState : GraphicState
        {
            [SerializeField] private bool _changeFontSize = false;
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize))] private bool _absoluteFontSize = true;
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize))] private bool _randomFontSize = false;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize)), ShowIf("@!_randomFontSize")] private float _fontSize;
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize)), ShowIf("@_randomFontSize")] private float _minFontSize;
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize)), ShowIf("@_randomFontSize")] private float _maxFontSize;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize))] private bool _fontSize_SeparateTransition = false;
            [Space(3f)]
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize))] private bool _changeStyle = false;
            [SerializeField, FoldoutGroup("_changeFontSize/FontSize"), ShowIfGroup(nameof(_changeFontSize)), ShowIf(nameof(_changeStyle))] private FontStyles _style;

			public bool ChangeFontSize { get => _changeFontSize; set => _changeFontSize = value; }

			public bool AbsoluteFontSize { get => _absoluteFontSize; set => _absoluteFontSize = value; }
			public bool RandomFontSize { get => _randomFontSize; set => _randomFontSize = value; }
			public float FontSize { get => _fontSize; set => _fontSize = value; }
			public float MinFontSize { get => _minFontSize; set => _minFontSize = value; }
			public float MaxFontSize { get => _maxFontSize; set => _maxFontSize = value; }
			public bool FontSize_SeparateTransition { get => _fontSize_SeparateTransition; set => _fontSize_SeparateTransition = value; }

			public bool ChangeStyle { get => _changeStyle; set => _changeStyle = value; }
			public FontStyles Style { get => _style; set => _style = value; }
		}
        private class TextSnapshot : GraphicSnapshot
        {
            public TextSnapshot(TextState state, TextSnapshot snap)
            {
                CopyFrom(state, snap);
            }
            public TextSnapshot(TextSnapshot toCopy) : base(toCopy)
            {
                FontSize = toCopy.FontSize;
                Style = toCopy.Style;
            }
            public TextSnapshot(TextMeshProUGUI text) : base(text)
            {
                CopyFrom(text);
            }
            public void CopyFrom(TextState state, TextSnapshot snap)
            {
                if (snap == this)
                {
                    snap = new TextSnapshot(this);
                }

                base.CopyFrom(state, snap);
                if (state.RandomFontSize == true)
                {
                    FontSize = Random.Range(state.MinFontSize, state.MaxFontSize);
                }
                else
                {
                    FontSize = state.FontSize;
                }
                if (state.AbsoluteFontSize == false)
                {
                    FontSize += snap.FontSize;
                }

                Style = state.Style;
            }
            public void CopyFrom(TextMeshProUGUI text)
            {
                base.CopyFrom(text);
                FontSize = text.fontSize;
                Style = text.fontStyle;
            }

            public static void ApplySnapshotLerp(TextMeshProUGUI text, TextState state, TransitionParameters transition, TextSnapshot origin, TextSnapshot target, float t)
            {
                GraphicSnapshot.ApplySnapshotLerp(text, state, transition, origin, target, t);
                float progress = 0;

                if (state.ChangeFontSize == true)
                {
                    UpdateProgress(ref progress, t, state.Size_SeparateTransition,
                        transition.FontSizeUseCurve, transition.FontSizeCurve, transition.FontSizePower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    text.fontSize = Mathf.LerpUnclamped(origin.FontSize, target.FontSize, progress);
                }

                if (state.ChangeStyle == true)
                {
                    text.fontStyle = target.Style;
                }
            }

            public float FontSize;
            public FontStyles Style;
        }
        [System.Serializable]
        private class GraphicFeedback
        {
            [SerializeField, ValidateInput(nameof(ValidateStates))] private GraphicState[] _states;

            public GraphicState[] States => _states;

            private bool ValidateStates(GraphicState[] states)
			{
                Validate();
                return true;
            }
            private void Validate()
			{
                for (int i = 0; i < _states.Length; i++)
                {
                    _states[i].TransitionToState.E_SeparateColor = _states[i].Color_SeparateTransition;
                    _states[i].TransitionToState.E_SeparatePosition = _states[i].Position_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateRotation = _states[i].Rotation_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateScale = _states[i].Scale_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateSize = _states[i].Size_SeparateTransition;
                }
            }
        }
        [System.Serializable]
        private class TextFeedback
        {
            [SerializeField, ValidateInput(nameof(ValidateStates))] private TextState[] _states;

            public TextState[] States => _states;

            private bool ValidateStates(TextState[] states)
            {
                Validate();
                return true;
            }
            private void Validate()
            {
                for (int i = 0; i < _states.Length; i++)
                {
                    _states[i].TransitionToState.E_SeparateColor = _states[i].Color_SeparateTransition;
                    _states[i].TransitionToState.E_SeparatePosition = _states[i].Position_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateRotation = _states[i].Rotation_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateScale = _states[i].Scale_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateSize = _states[i].Size_SeparateTransition;
                    _states[i].TransitionToState.E_SeparateFontSize = _states[i].FontSize_SeparateTransition;
                }
            }
        }

        [Title("Main")]
        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _playOnEnabled = true;
        [SerializeField] private bool _scaledTime = true;

        [Title("Feedbacks")]
        [SerializeField] private GraphicFeedback[] _graphicsFeedbacks;
        [SerializeField] private GraphicFeedback[] _imagesFeedbacks;
        [SerializeField] private TextFeedback[] _textsFeedbacks;

        [Title("Advanced Overrides")]
        [SerializeField] private bool _overrideGraphicsArray;
        [SerializeField] private bool _overrideImagesArray;
        [SerializeField] private bool _overrideTextsArray;

        [SerializeField, ShowIf(nameof(_overrideGraphicsArray))] private MaskableGraphic[] _graphics;
        [SerializeField, ShowIf(nameof(_overrideImagesArray))] private Image[] _images;
        [SerializeField, ShowIf(nameof(_overrideTextsArray))] private TextMeshProUGUI[] _tmps;


        private bool _graphicsFinished = false;
        private bool _imagesFinished = false;
        private bool _textFinished = false;

        private Coroutine[] _graphicsCoroutines;
        private Coroutine[] _imagesCoroutines;
        private Coroutine[] _textCoroutines;

        public bool ScaledTime { get => _scaledTime; set => _scaledTime = value; }
        private float _DeltaTime { get { return _scaledTime ? Time.deltaTime : Time.unscaledDeltaTime; } }
        public PinouUtils.Delegate.Action OnFeedbackEnd { get; private set; } = new PinouUtils.Delegate.Action();

        protected override void OnAwake()
		{
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false) { return; }
#endif
            if (_playOnAwake == true)
            {
                PlayFeedback();
            }
		}
		protected override void OnEnabled()
		{
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false) { return; }
#endif
            if (_playOnEnabled == true)
            {
                PlayFeedback();
            }
        }

		[ShowIf("@UnityEditor.EditorApplication.isPlaying == true"), Button("Play Feedback")]
        public void PlayFeedback()
        {
            _graphicsFinished = false;
            _imagesFinished = false;
            _textFinished = false;

            _graphicsCoroutines?.ForEach(c => StopCoroutine(c));
            _imagesCoroutines?.ForEach(c => StopCoroutine(c));
            _textCoroutines?.ForEach(c => StopCoroutine(c));

            if (_graphicsFeedbacks.Length > 0)
            {
                _graphicsCoroutines = new Coroutine[_graphicsFeedbacks.Length];
                for (int i = 0; i < _graphicsFeedbacks.Length; i++)
                {
                    if (_graphicsFeedbacks[i].States.Length > 0)
                    {
                        _graphicsCoroutines[i] = StartCoroutine(GraphicsCoroutine(_graphicsFeedbacks[i]));
                    }
                }
            }
            else
            {
                _graphicsFinished = true;
            }
            if (_imagesFeedbacks.Length > 0)
            {
                _imagesCoroutines = new Coroutine[_imagesFeedbacks.Length];
                for (int i = 0; i < _imagesFeedbacks.Length; i++)
                {
                    if (_imagesFeedbacks[i].States.Length > 0)
                    {
                        _imagesCoroutines[i] = StartCoroutine(ImagesCoroutine(_imagesFeedbacks[i]));
                    }
                }
            }
            else
            {
                _imagesFinished = true;
            }
            if (_textsFeedbacks.Length > 0)
            {
                _textCoroutines = new Coroutine[_textsFeedbacks.Length];
                for (int i = 0; i < _textsFeedbacks.Length; i++)
                {
                    if (_textsFeedbacks[i].States.Length > 0)
                    {
                        _textCoroutines[i] = StartCoroutine(TextsCoroutine(_textsFeedbacks[i]));
                    }
                }
            }
            else
            {
                _textFinished = true;
            }
        }
        private IEnumerator GraphicsCoroutine(GraphicFeedback feedback)
        {
            yield return GenericGraphicsCoroutine(feedback, _graphics);
            _graphicsFinished = true;
            CheckEnd();
        }
        private IEnumerator ImagesCoroutine(GraphicFeedback feedback)
        {
            yield return GenericGraphicsCoroutine(feedback, _images);
            _imagesFinished = true;
            CheckEnd();
        }
        private IEnumerator TextsCoroutine(TextFeedback feedback)
        {
            yield return GenericTextsCoroutine(feedback);
            _textFinished = true;
            CheckEnd();
        }
        private IEnumerator GenericGraphicsCoroutine(GraphicFeedback feedback, MaskableGraphic[] graphics)
        {
            GraphicSnapshot[] _baseSnaps = new GraphicSnapshot[graphics.Length];
            GraphicSnapshot[] _targetSnaps = new GraphicSnapshot[graphics.Length];

            for (int i = 0; i < _baseSnaps.Length; i++)
            {
                _baseSnaps[i] = new GraphicSnapshot(graphics[i]);
                _targetSnaps[i] = new GraphicSnapshot(feedback.States[0], _baseSnaps[i]);
            }

            TransitionSnapshot transition;
            for (int t = 0; t < feedback.States.Length; t++)
            {
                transition = new TransitionSnapshot(feedback.States[t].TransitionToState);

                if (transition.Parameters.Instant == false)
                {
                    yield return new WaitForSeconds(transition.WaitTime);
                }

                float count = 0f;
                float progress = 0f;
                while (count < transition.Duration)
                {
                    count += _DeltaTime;
                    progress = count / transition.Duration;

                    for (int i = 0; i < graphics.Length; i++)
                    {
                        GraphicSnapshot.ApplySnapshotLerp(graphics[i], feedback.States[t], feedback.States[t].TransitionToState, _baseSnaps[i], _targetSnaps[i], progress);
                    }

                    if (transition.Parameters.Instant == false)
                    {
                        yield return null;
                    }
                }

                for (int i = 0; i < graphics.Length; i++)
                {
                    GraphicSnapshot.ApplySnapshotLerp(graphics[i], feedback.States[t], feedback.States[t].TransitionToState, _baseSnaps[i], _targetSnaps[i], 1f);
                }

                if (t >= feedback.States.Length - 1)
                {
                    break;
                }
                for (int i = 0; i < _baseSnaps.Length; i++)
                {
                    _baseSnaps[i].CopyFrom(feedback.States[t], _baseSnaps[i]);
                    _targetSnaps[i].CopyFrom(feedback.States[t + 1], _baseSnaps[i]);
                }
            }

            for (int i = 0; i < graphics.Length; i++)
            {
                GraphicSnapshot.ApplySnapshotLerp(graphics[i], feedback.States[feedback.States.Length - 1], feedback.States[feedback.States.Length - 1].TransitionToState, _baseSnaps[i], _targetSnaps[i], 1f);
            }
        }
        private IEnumerator GenericTextsCoroutine(TextFeedback feedback)
        {
            TextSnapshot[] _baseSnaps = new TextSnapshot[_tmps.Length];
            TextSnapshot[] _targetSnaps = new TextSnapshot[_tmps.Length];

            for (int i = 0; i < _baseSnaps.Length; i++)
            {
                _baseSnaps[i] = new TextSnapshot(_tmps[i]);
                _targetSnaps[i] = new TextSnapshot(feedback.States[0], _baseSnaps[i]);
            }

            TransitionSnapshot transition;
            for (int t = 0; t < feedback.States.Length; t++)
            {
                transition = new TransitionSnapshot(feedback.States[t].TransitionToState);

                if (transition.Parameters.Instant == false)
                {
                    yield return new WaitForSeconds(transition.WaitTime);
                }

                float count = 0f;
                float progress = 0f;
                while (count < transition.Duration)
                {
                    count += _DeltaTime;
                    progress = count / transition.Duration;

                    for (int i = 0; i < _tmps.Length; i++)
                    {
                        TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback.States[t], feedback.States[t].TransitionToState, _baseSnaps[i], _targetSnaps[i], progress);
                    }

                    if (transition.Parameters.Instant == false)
                    {
                        yield return null;
                    }
                }

                for (int i = 0; i < _tmps.Length; i++)
                {
                    TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback.States[t], feedback.States[t].TransitionToState, _baseSnaps[i], _targetSnaps[i], 1f);
                }

                if (t >= feedback.States.Length - 1)
                {
                    break;
                }
                for (int i = 0; i < _baseSnaps.Length; i++)
                {
                    _baseSnaps[i].CopyFrom(feedback.States[t], _baseSnaps[i]);
                    _targetSnaps[i].CopyFrom(feedback.States[t + 1], _baseSnaps[i]);
                }
            }

            for (int i = 0; i < _tmps.Length; i++)
            {
                TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback.States[feedback.States.Length - 1], feedback.States[feedback.States.Length - 1].TransitionToState, _baseSnaps[i], _targetSnaps[i], 1f);
            }
        }

        private void CheckEnd()
        {
            if (_textFinished == true && _graphicsFinished == true && _imagesFinished == true)
            {
                OnFeedbackEnd.Invoke();
            }
        }
#if UNITY_EDITOR
		protected override void E_OnValidate()
		{
            if (_overrideGraphicsArray == false)
			{
                _graphics = GetComponentsInChildren<MaskableGraphic>();
			}
            if (_overrideImagesArray == false)
            {
                _images = GetComponentsInChildren<Image>();
            }
            if (_overrideTextsArray == false)
            {
                _tmps = GetComponentsInChildren<TextMeshProUGUI>();
            }
        }
#endif
    }


}