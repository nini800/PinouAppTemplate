#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pinou.UI;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou
{
    public class EffectSlave : PinouBehaviour
    {
        [System.Serializable]
        public class Effect : CustomDrawedProperty
        {
            public enum EffectType
            {
                PlayAnimation,
                AnimationLayerWeightChange,
                EmissionChange,
                StartFeedback
            }
            [System.Serializable]
            public class EmissionColor
            {
                public enum EmissionColorType
                {
                    Original,
                    Custom
                }

                [SerializeField] private EmissionColorType _type;
                [SerializeField] private float _originalFactor;
                [SerializeField, ColorUsage(false, true)] private Color _emissionColor;

                private Color _originalColor;

                public void Build(Effect effect)
                {
                    _originalColor = effect.Slave.OriginalEmissionColors[effect._emissionMaterialIndex];
                }

                public Color Color
                {
                    get
                    {
                        switch (_type)
                        {
                            case EmissionColorType.Original:
                                return _originalColor * _originalFactor * _originalFactor;
                            case EmissionColorType.Custom:
                                return _emissionColor;
                        }

                        return default;
                    }
                }
            }
            [System.Serializable]
            public class EmissionColorTransition
            {
                public void Build(Effect effect)
                {
                    if (_start == null)
                    {
                        _start = new EmissionColor();
                    }
                    if (_end == null)
                    {
                        _end = new EmissionColor();
                    }

                    _start.Build(effect);
                    _end.Build(effect);
                }

                [SerializeField] private float _duration;
                [SerializeField] private EmissionColor _start;
                [SerializeField] private EmissionColor _end;
                [SerializeField] private AnimationCurve _curve = PinouUtils.AnimationCurve.WeightedExponential;

                public float Duration { get => _duration; }

                public Color GetColor(float t)
                {
                    t = Mathf.Clamp01(t);

                    return Color.LerpUnclamped(_start.Color, _end.Color, _curve.Evaluate(t));
                }
            }

            public void Build(EffectSlave slave)
            {
                _slave = slave;
                for (int i = 0; i < _transitions.Length; i++)
                {
                    _transitions[i].Build(this);
                }
            }

            private EffectSlave _slave;
            public EffectSlave Slave { get { return _slave; } }

            //Main
            [SerializeField] private EffectType _type;

            //Play Animation
            [SerializeField, ShowIf("_type", EffectType.PlayAnimation)] private string _animationName;
            [SerializeField, ShowIf("_type", EffectType.PlayAnimation)] private int _animationLayer;
            [SerializeField, ShowIf("_type", EffectType.PlayAnimation)] private bool _instantAnimationTransition;

            //AnimationLayer Weight
            [SerializeField, ShowIf("_type", EffectType.AnimationLayerWeightChange)] private float _weightChangeTime;
            [SerializeField, ShowIf("_type", EffectType.AnimationLayerWeightChange)] private float _weightForcedEndValue;
            [SerializeField, ShowIf("_type", EffectType.AnimationLayerWeightChange)] private AnimationCurve _weightOverTime;

            //EmissionChange
            [SerializeField, ShowIf("_type", EffectType.EmissionChange)] private int _emissionMaterialIndex;
            [SerializeField, ShowIf("_type", EffectType.EmissionChange)] private EmissionColorTransition[] _transitions;

            //Start Feedback
            [SerializeField, ShowIf("_type", EffectType.StartFeedback)] private UI_FeedbackUtilities[] _feedbacks = new UI_FeedbackUtilities[] { };

            public void Invoke()
            {
                switch (_type)
                {
                    case EffectType.PlayAnimation:
                        PlayAnimation();
                        break;
                    case EffectType.AnimationLayerWeightChange:
                        PinouUtils.Coroutine.StartCoroutine(WeightLayerAnimation());
                        break;
                    case EffectType.EmissionChange:
                        PinouUtils.Coroutine.StartCoroutine(EmissionChange());
                        break;
                    case EffectType.StartFeedback:
                        _feedbacks.ForEach(f => f.PlayFeedback());
                        break;
                }
            }

            private void PlayAnimation()
            {
                if (_instantAnimationTransition == true)
				{
                    _slave.Animator?.Animator.Play(_animationName, _animationLayer, 0);
                }
                else
				{
                    _slave.Animator?.Animator.CrossFadeInFixedTime(_animationName, 0.2f, _animationLayer);
                }
            }

            private IEnumerator WeightLayerAnimation()
            {
                float count = 0f;

                while (count < _weightChangeTime)
                {
                    count += Time.deltaTime;
                    _slave.Animator?.Animator.SetLayerWeight(_animationLayer, _weightOverTime.Evaluate(count / _weightChangeTime));
                    yield return null;
                }

                _slave.Animator?.Animator.SetLayerWeight(_animationLayer, _weightForcedEndValue);
            }

            private IEnumerator EmissionChange()
            {
                float maxDuration = 0;
                for (int i = 0; i < _transitions.Length; i++)
                {
                    maxDuration += _transitions[i].Duration;
                }

                for (int i = 0; i < _transitions.Length; i++)
                {
                    float count = 0;
                    while (count < _transitions[i].Duration)
                    {
                        count += Time.deltaTime;

                        SetEmissionColor(_transitions[i].GetColor(count / _transitions[i].Duration), _emissionMaterialIndex);
                        yield return null;
                    }

                    SetEmissionColor(_transitions[i].GetColor(1), _emissionMaterialIndex);
                }
            }
            private void SetEmissionColor(Color color, int materialIndex)
            {
                _slave.MeshRenderer.materials[materialIndex].SetColor("_EmissionColor", color);
                //_slave.MeshRenderer.materials.ForEach(m => m.SetColor("_EmissionColor", color));
            }
        }

        [SerializeField] private Effect[] _effects;


        private Color[] _originalEmissionColors;
        public Color[] OriginalEmissionColors { get => _originalEmissionColors; }

        private PinouAnimator _animator;
        public PinouAnimator Animator { get => _animator; }

        private MeshRenderer _meshRenderer;
        public MeshRenderer MeshRenderer { get => _meshRenderer; }

        protected override void OnAwake()
        {
            _animator = GetComponentInChildren<PinouAnimator>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();


            if (_meshRenderer != null)
			{
                _originalEmissionColors = new Color[_meshRenderer.materials.Length];
                for (int i = 0; i < _meshRenderer.materials.Length; i++)
                {
                    _originalEmissionColors[i] = _meshRenderer.materials[i].GetColor("_EmissionColor");
                }
            }

            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].Build(this);
            }
        }

        [Button("Play")]
        public void Invoke()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false) { return; }
#endif
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].Invoke();
            }
        }
    }
}