#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou.UI
{
	public class UI_FeedbackUtilities : PinouBehaviour
	{
        [System.Serializable]
        private class TransitionParameters : CustomDrawedProperty
        {
            public bool Instant = false;
            public bool RandomDelays = false;
            public float WaitTime;
            public float MinWaitTime;
            public float MaxWaitTime;
            public float Duration;
            public float MinDuration;
            public float MaxDuration;
            public bool UseCurve = false;
            public bool SizeUseCurve = false;
            public bool ScaleUseCurve = false;
            public bool PositionUseCurve = false;
            public bool RotationUseCurve = false;
            public bool ColorUseCurve = false;
            public bool FontSizeUseCurve = false;
            public float Power;
            public float SizePower;
            public float ScalePower;
            public float PositionPower;
            public float RotationPower;
            public float ColorPower;
            public float FontSizePower;
            public AnimationCurve Curve;
            public AnimationCurve SizeCurve;
            public AnimationCurve ScaleCurve;
            public AnimationCurve PositionCurve;
            public AnimationCurve RotationCurve;
            public AnimationCurve ColorCurve;
            public AnimationCurve FontSizeCurve;

#if UNITY_EDITOR
            public bool E_SeparateSize;
            public bool E_SeparateScale;
            public bool E_SeparatePosition;
            public bool E_SeparateRotation;
            public bool E_SeparateColor;
            public bool E_SeparateFontSize;
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
        private class GraphicState : CustomDrawedProperty
        {
            public bool ChangeSize = false;
            public bool AbsoluteSize = true;
            public bool RandomSize = false;
            public bool SyncSizeAxises = true;
            public Vector2 SizeDelta;
            public float SyncSize;
            public Vector2 MinSizeDelta;
            public Vector2 MaxSizeDelta;
            public float MinSyncSize;
            public float MaxSyncSize;
            public bool Size_SeparateTransition = false;

            public bool ChangeScale = false;
            public bool AbsoluteScale = true;
            public bool RandomScale = false;
            public bool SyncScaleAxises = true;
            public Vector3 ScaleDelta;
            public float SyncScale;
            public Vector3 MinScaleDelta;
            public Vector3 MaxScaleDelta;
            public float MinSyncScale;
            public float MaxSyncScale;
            public bool Scale_SeparateTransition = false;

            public bool ChangeRotation = false;
            public bool AbsoluteRotation = true;
            public bool RandomRotation = false;
            public float Rotation;
            public float MinRotation;
            public float MaxRotation;
            public bool Rotation_SeparateTransition = false;

            public bool ChangePosition = false;
            public bool AbsolutePosition = true;
            public bool RandomPosition = false;
            public bool PositionSeparateAxisRandom = false;
            public Vector3 Position;
            public Vector3 MinPosition;
            public Vector3 MaxPosition;
            public bool Position_SeparateTransition = false;

            public bool ChangeColor = false;
            public bool RandomColor = false;
            public Color Color;
            public Color MinColor;
            public Color MaxColor;
            public bool Color_SeparateTransition = false;

#if UNITY_EDITOR
            public bool E_Instant = false;
#endif
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
                        rect.sizeDelta = Vector2.one * Mathf.Lerp(origin.SyncSize, target.SyncSize, progress);
                    }
                    else
                    {
                        rect.sizeDelta = Vector2.Lerp(origin.SizeDelta, target.SizeDelta, progress);
                    }
                }

                if (state.ChangeScale == true)
                {
                    UpdateProgress(ref progress, t, state.Scale_SeparateTransition,
                        transition.ScaleUseCurve, transition.ScaleCurve, transition.ScalePower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    if (state.SyncScaleAxises == true)
                    {
                        rect.localScale = Vector3.one * Mathf.Lerp(origin.SyncScale, target.SyncScale, progress);
                    }
                    else
                    {
                        rect.localScale = Vector3.Lerp(origin.ScaleDelta, target.ScaleDelta, progress);
                    }
                }

                if (state.ChangePosition == true)
                {
                    UpdateProgress(ref progress, t, state.Position_SeparateTransition,
                        transition.PositionUseCurve, transition.PositionCurve, transition.PositionPower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    rect.anchoredPosition3D = Vector3.Lerp(origin.Position, target.Position, progress);
                }

                if (state.ChangeRotation == true)
                {
                    UpdateProgress(ref progress, t, state.Rotation_SeparateTransition,
                        transition.RotationUseCurve, transition.RotationCurve, transition.RotationPower,
                        transition.UseCurve, transition.Curve, transition.Power);

                    rect.localEulerAngles = rect.localEulerAngles.SetZ(Mathf.Lerp(origin.Rotation, target.Rotation, progress));
                }

                if (state.ChangeColor == true)
                {
                    UpdateProgress(ref progress, t, state.Color_SeparateTransition,
                         transition.ColorUseCurve, transition.ColorCurve, transition.ColorPower,
                         transition.UseCurve, transition.Curve, transition.Power);

                    graphic.color = Color.Lerp(origin.Color, target.Color, progress);
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
            public bool ChangeFontSize = false;
            public bool AbsoluteFontSize = true;
            public bool RandomFontSize = false;
            public float FontSize;
            public float MinFontSize;
            public float MaxFontSize;
            public bool FontSize_SeparateTransition = false;

            public bool ChangeStyle = false;
            public FontStyles Style;
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

                    text.fontSize = Mathf.Lerp(origin.FontSize, target.FontSize, t);
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
        private class GraphicFeedback : CustomDrawedProperty
        {
            public TransitionParameters[] _transitions;
            public GraphicState[] _states;
        }
        [System.Serializable]
        private class TextFeedback : CustomDrawedProperty
        {
            public TransitionParameters[] _transitions;
            public TextState[] _states;
        }

        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _scaledTime = true;

        [SerializeField] private GraphicFeedback[] _graphicsFeedbacks;
        [SerializeField] private GraphicFeedback[] _imagesFeedbacks;
        [SerializeField] private TextFeedback[] _textsFeedbacks;

        [SerializeField] private bool _overrideGraphicsArray;
        [SerializeField] private bool _overrideImagesArray;
        [SerializeField] private bool _overrideTextsArray;

        [SerializeField] private MaskableGraphic[] _graphics;
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _tmps;


        private bool _graphicsFinished = false;
        private bool _imagesFinished = false;
        private bool _textFinished = false;

        private Coroutine[] _graphicsCoroutines;
        private Coroutine[] _imagesCoroutines;
        private Coroutine[] _textCoroutines;

        public bool ScaledTime { get => _scaledTime; set => _scaledTime = value; }
        private float _DeltaTime { get { return _scaledTime ? Time.deltaTime : Time.unscaledDeltaTime; } }
        public PinouUtils.Delegate.Action OnFeedbackEnd = new PinouUtils.Delegate.Action();


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
                    if (_graphicsFeedbacks[i]._states.Length > 0)
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
                    if (_imagesFeedbacks[i]._states.Length > 0)
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
                    if (_textsFeedbacks[i]._states.Length > 0)
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
            yield return GenericGraphicsCoroutine(feedback);
            _graphicsFinished = true;
            CheckEnd();
        }
        private IEnumerator ImagesCoroutine(GraphicFeedback feedback)
        {
            yield return GenericGraphicsCoroutine(feedback);
            _imagesFinished = true;
            CheckEnd();
        }
        private IEnumerator TextsCoroutine(TextFeedback feedback)
        {
            yield return GenericTextsCoroutine(feedback);
            _textFinished = true;
            CheckEnd();
        }
        private IEnumerator GenericGraphicsCoroutine(GraphicFeedback feedback)
        {
            GraphicSnapshot[] _baseSnaps = new GraphicSnapshot[_graphics.Length];
            GraphicSnapshot[] _targetSnaps = new GraphicSnapshot[_graphics.Length];

            for (int i = 0; i < _baseSnaps.Length; i++)
            {
                _baseSnaps[i] = new GraphicSnapshot(_graphics[i]);
                _targetSnaps[i] = new GraphicSnapshot(feedback._states[0], _baseSnaps[i]);
            }

            TransitionSnapshot transition;
            for (int t = 0; t < feedback._transitions.Length; t++)
            {
                transition = new TransitionSnapshot(feedback._transitions[t]);

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

                    for (int i = 0; i < _graphics.Length; i++)
                    {
                        GraphicSnapshot.ApplySnapshotLerp(_graphics[i], feedback._states[t], feedback._transitions[t], _baseSnaps[i], _targetSnaps[i], progress);
                    }

                    if (transition.Parameters.Instant == false)
                    {
                        yield return null;
                    }
                }

                for (int i = 0; i < _graphics.Length; i++)
                {
                    GraphicSnapshot.ApplySnapshotLerp(_graphics[i], feedback._states[t], feedback._transitions[t], _baseSnaps[i], _targetSnaps[i], 1f);
                }

                if (t >= feedback._transitions.Length - 1)
                {
                    break;
                }
                for (int i = 0; i < _baseSnaps.Length; i++)
                {
                    _baseSnaps[i].CopyFrom(feedback._states[t], _baseSnaps[i]);
                    _targetSnaps[i].CopyFrom(feedback._states[t + 1], _baseSnaps[i]);
                }
            }

            for (int i = 0; i < _graphics.Length; i++)
            {
                GraphicSnapshot.ApplySnapshotLerp(_graphics[i], feedback._states[feedback._states.Length - 1], feedback._transitions[feedback._states.Length - 1], _baseSnaps[i], _targetSnaps[i], 1f);
            }
        }
        private IEnumerator GenericTextsCoroutine(TextFeedback feedback)
        {
            TextSnapshot[] _baseSnaps = new TextSnapshot[_tmps.Length];
            TextSnapshot[] _targetSnaps = new TextSnapshot[_tmps.Length];

            for (int i = 0; i < _baseSnaps.Length; i++)
            {
                _baseSnaps[i] = new TextSnapshot(_tmps[i]);
                _targetSnaps[i] = new TextSnapshot(feedback._states[0], _baseSnaps[i]);
            }

            TransitionSnapshot transition;
            for (int t = 0; t < feedback._transitions.Length; t++)
            {
                transition = new TransitionSnapshot(feedback._transitions[t]);

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
                        TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback._states[t], feedback._transitions[t], _baseSnaps[i], _targetSnaps[i], progress);
                    }

                    if (transition.Parameters.Instant == false)
                    {
                        yield return null;
                    }
                }

                for (int i = 0; i < _tmps.Length; i++)
                {
                    TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback._states[t], feedback._transitions[t], _baseSnaps[i], _targetSnaps[i], 1f);
                }

                if (t >= feedback._transitions.Length - 1)
                {
                    break;
                }
                for (int i = 0; i < _baseSnaps.Length; i++)
                {
                    _baseSnaps[i].CopyFrom(feedback._states[t], _baseSnaps[i]);
                    _targetSnaps[i].CopyFrom(feedback._states[t + 1], _baseSnaps[i]);
                }
            }

            for (int i = 0; i < _tmps.Length; i++)
            {
                TextSnapshot.ApplySnapshotLerp(_tmps[i], feedback._states[feedback._states.Length - 1], feedback._transitions[feedback._states.Length - 1], _baseSnaps[i], _targetSnaps[i], 1f);
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
        [CustomPropertyDrawer(typeof(TransitionParameters))]
        private class TransitionParametersDrawer : PropertyDrawerExtended
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                base.OnGUI(position, property, label);

                PropField("Instant");

                if (Prop("Instant").boolValue == false)
                {
                    PropField("RandomDelays");
                    if (Prop("RandomDelays").boolValue == false)
                    {
                        PropField("WaitTime");
                        PropField("Duration");
                    }
                    else
                    {
                        PropField("MinWaitTime");
                        PropField("MaxWaitTime");
                        Space(2f);
                        PropField("MinDuration");
                        PropField("MaxDuration");
                    }

                    Space(5f);

                    if (EverySeparated() == false)
                    {
                        PowerCurveFields("");
                    }

                    SeparatePowerCurveFields("Size");
                    SeparatePowerCurveFields("Scale");
                    SeparatePowerCurveFields("Position");
                    SeparatePowerCurveFields("Rotation");
                    SeparatePowerCurveFields("Color");
                    SeparatePowerCurveFields("FontSize");
                }
            }
            private bool EverySeparated()
            {
                return Prop("E_SeparateSize").boolValue &&
                    Prop("E_SeparateScale").boolValue &&
                    Prop("E_SeparatePosition").boolValue &&
                    Prop("E_SeparateRotation").boolValue &&
                    Prop("E_SeparateColor").boolValue &&
                    Prop("E_SeparateFontSize").boolValue;

            }

            private void SeparatePowerCurveFields(string prefix)
            {
                if (Prop("E_Separate" + prefix).boolValue == true)
                {
                    PowerCurveFields(prefix);
                }
            }

            private void PowerCurveFields(string prefix)
            {
                PropField(prefix + "UseCurve");
                if (Prop(prefix + "UseCurve").boolValue == true)
                {
                    PropField(prefix + "Curve");
                }
                else
                {
                    PropField(prefix + "Power");
                }
            }
        }
        [CustomPropertyDrawer(typeof(GraphicState))]
        private class GraphicStateDrawer : PropertyDrawerExtended
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                base.OnGUI(position, property, label);

                PropField("ChangeSize");
                if (Prop("ChangeSize").boolValue == true)
                {
                    Indent();
                    PropField("RandomSize");
                    PropField("AbsoluteSize");
                    PropField("SyncSizeAxises");

                    if (Prop("RandomSize").boolValue == false)
                    {
                        if (Prop("SyncSizeAxises").boolValue == false)
                        {
                            PropField("SizeDelta");
                        }
                        else
                        {
                            PropField("SyncSize");
                        }
                    }
                    else
                    {
                        if (Prop("SyncSizeAxises").boolValue == false)
                        {
                            PropField("MinSizeDelta");
                            PropField("MaxSizeDelta");
                        }
                        else
                        {
                            PropField("MinSyncSize");
                            PropField("MaxSyncSize");
                        }
                       
                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("Size_SeparateTransition");
                    }
                    Unindent();
                }

                Space(5f);
                PropField("ChangeScale");
                if (Prop("ChangeScale").boolValue == true)
                {
                    Indent();
                    PropField("RandomScale");
                    PropField("AbsoluteScale");
                    PropField("SyncScaleAxises");

                    if (Prop("RandomScale").boolValue == false)
                    {
                        if (Prop("SyncScaleAxises").boolValue == false)
                        {
                            PropField("ScaleDelta");
                        }
                        else
                        {
                            PropField("SyncScale");
                        }
                    }
                    else
                    {
                        if (Prop("SyncScaleAxises").boolValue == false)
                        {
                            PropField("MinScaleDelta");
                            PropField("MaxScaleDelta");
                        }
                        else
                        {
                            PropField("MinSyncScale");
                            PropField("MaxSyncScale");
                        }

                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("Scale_SeparateTransition");
                    }
                    Unindent();
                }

                Space(5f);
                PropField("ChangeRotation");
                if (Prop("ChangeRotation").boolValue == true)
                {
                    Indent();
                    PropField("AbsoluteRotation");
                    PropField("RandomRotation");

                    if (Prop("RandomRotation").boolValue == false)
                    {
                        PropField("Rotation");
                    }
                    else
                    {
                        PropField("MinRotation");
                        PropField("MaxRotation");
                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("Rotation_SeparateTransition");
                    }
                    Unindent();
                }

                Space(5f);
                PropField("ChangePosition");
                if (Prop("ChangePosition").boolValue == true)
                {
                    Indent();
                    PropField("AbsolutePosition");
                    PropField("RandomPosition");

                    if (Prop("RandomPosition").boolValue == false)
                    {
                        PropField("Position");
                    }
                    else
                    {
                        PropField("PositionSeparateAxisRandom");
                        PropField("MinPosition");
                        PropField("MaxPosition");
                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("Position_SeparateTransition");
                    }
                    Unindent();
                }

                Space(5f);
                PropField("ChangeColor");
                if (Prop("ChangeColor").boolValue == true)
                {
                    Indent();
                    PropField("RandomColor");

                    if (Prop("RandomColor").boolValue == false)
                    {
                        PropField("Color");
                    }
                    else
                    {
                        PropField("MinColor");
                        PropField("MaxColor");
                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("Color_SeparateTransition");
                    }
                    Unindent();
                }
            }
        }
        [CustomPropertyDrawer(typeof(TextState))]
        private class TextStateDrawer : GraphicStateDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                base.OnGUI(position, property, label);

                Space(5f);
                PropField("ChangeFontSize");
                if (Prop("ChangeFontSize").boolValue == true)
                {
                    Indent();
                    PropField("AbsoluteFontSize");
                    PropField("RandomFontSize");

                    if (Prop("RandomFontSize").boolValue == false)
                    {
                        PropField("FontSize");
                    }
                    else
                    {
                        PropField("MinFontSize");
                        PropField("MaxFontSize");
                    }

                    if (Prop("E_Instant").boolValue == false)
                    {
                        PropField("FontSize_SeparateTransition");
                    }
                    Unindent();
                }

                PropField("ChangeStyle");
                if (Prop("ChangeStyle").boolValue == true)
                {
                    Indent();
                    PropField("Style");
                    Unindent();
                }
            }
        }
        [CustomPropertyDrawer(typeof(GraphicFeedback))]
        private class GraphicFeedbackDrawer : PropertyDrawerExtended
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                base.OnGUI(position, property, label);

                if (OpenClosedBehaviour())
                {
                    Indent();
                    Prop("_transitions").arraySize = Prop("_states").arraySize;

                    SyncInstant();
                    SyncSeparate("Size");
                    SyncSeparate("Scale");
                    SyncSeparate("Rotation");
                    SyncSeparate("Color");
                    if (this is TextFeedbackDrawer)
                    {
                        SyncSeparate("FontSize");
                    }

                    for (int i = 0; i < Prop("_states").arraySize; i++)
                    {
                        if (i > 0)
                        {
                            Space(20f);
                        }

                        if (Button("Delete", 50f, true, Position.Right, 17f))
                        {
                            if (EditorUtility.DisplayDialog("Confirm supression", "Are you sure you wanna delete this state ?", "Confirm", "Cancel"))
                            {
                                Prop("_states").DeleteArrayElementAtIndex(i);
                                Prop("_transitions").arraySize = Prop("_states").arraySize;
                                break;
                            }
                        }

                        CenteredLabel("Transition from " + (i == 0 ? "original state" : (i-1).ToString()) + " to " + i);
                        PropField(Prop("_transitions").GetArrayElementAtIndex(i));
                        Space(5f);
                        CenteredLabel("State " + i);
                        PropField(Prop("_states").GetArrayElementAtIndex(i));
                    }

                    Space(20f);
                    if (Button("Add State", Screen.width * 0.75f))
                    {
                        Prop("_transitions").arraySize = Prop("_states").arraySize = Prop("_transitions").arraySize + 1;
                    }
                    Space(20f);
                    Unindent();
                }
            }

            private void SyncInstant()
            {
                for (int i = 0; i < Prop("_states").arraySize; i++)
                {
                    Prop("_states").GetArrayElementAtIndex(i).FindPropertyRelative("E_Instant").boolValue =
                        Prop("_transitions").GetArrayElementAtIndex(i).FindPropertyRelative("Instant").boolValue;
                }

            }
            private void SyncSeparate(string prefix)
            {
                for (int i = 0; i < Prop("_states").arraySize; i++)
                {
                    Prop("_transitions").GetArrayElementAtIndex(i).FindPropertyRelative("E_Separate" + prefix).boolValue =
                        Prop("_states").GetArrayElementAtIndex(i).FindPropertyRelative(prefix + "_SeparateTransition").boolValue;
                }
            }
        }
        [CustomPropertyDrawer(typeof(TextFeedback))]
        private class TextFeedbackDrawer : GraphicFeedbackDrawer
        {
            
        }


        [CustomEditor(typeof(UI_FeedbackUtilities))]
        private class UI_FeedbackUtilitiesEditor : PinouEditor
        {
            UI_FeedbackUtilities Instance => ((UI_FeedbackUtilities)target);
            protected override void InspectorGUI()
            {
                CenteredHeader("Main");
                PropField("_playOnAwake");
                PropField("_scaledTime");
                CenteredHeader("Feedbacks");
                PropField("_graphicsFeedbacks");
                PropField("_imagesFeedbacks");
                PropField("_textsFeedbacks");

                GUILayout.Space(10f);
                CenteredHeader("Advanced Overrides");
                PropField("_overrideGraphicsArray");
                if (Instance._overrideGraphicsArray == true)
                {
                    PropField("_graphics");
                }
                else
                {
                    Instance._graphics = Instance.GetComponentsInChildren<MaskableGraphic>();
                }
                PropField("_overrideImagesArray");
                if (Instance._overrideImagesArray == true)
                {
                    PropField("_images");
                }
                else
                {
                    Instance._images = Instance.GetComponentsInChildren<Image>();
                }
                PropField("_overrideTextsArray");
                if (Instance._overrideTextsArray == true)
                {
                    PropField("_tmps");
                }
                else
                {
                    Instance._tmps = Instance.GetComponentsInChildren<TextMeshProUGUI>();
                }

                if (EditorApplication.isPlaying == true)
                {
                    GUILayout.Space(20f);

                    if (GUILayout.Button("Play"))
                    {
                        Instance.PlayFeedback();
                    }
                }
            }
        }
#endif
    }


}