using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Pinou
{
    [System.Serializable]
    public class SpeedStateParameters
    {
        public string[] Names = new string[] { "Stand" };
        public float Speed = 1;
    }

    [System.Serializable]
    public class TimedStateParameters
    {
        public string[] Names = new string[] { "Dodge" };
        public float Time = 1;
    }

    [RequireComponent(typeof(Animator))]
    public class PinouAnimator : PinouBehaviour
    {
        #region Fields, Getters, Vars
        private bool[] _timedAnimationPlaying;
        private int[] _currentAnimationPriority;
        private float[] _lastAnimationChangeTime;
        private string[] _currentAnimationState;
        private Animator _animator;
        private Coroutine[] _timedCoroutine;

        public bool[] TimedAnimationPlaying { get { return _timedAnimationPlaying; } }
        public int[] CurrentAnimationPriority { get { return _currentAnimationPriority; } }
        public float[] LastAnimationChangeTime { get => _lastAnimationChangeTime; }
        public string[] CurrentAnimationState { get { return _currentAnimationState; } }
        public Animator Animator { get { return _animator == null ? GetComponent<Animator>() : _animator; } }

        public bool RandomStart = false;

        #endregion

        #region Behaviour
        protected override void OnAwake()
        {
            _animator = GetComponent<Animator>();

            _timedAnimationPlaying = new bool[Animator.layerCount];
            _timedAnimationPlaying.SetAllValues(false);

            _currentAnimationPriority = new int[Animator.layerCount];
            _currentAnimationPriority.SetAllValues(-1);

            _lastAnimationChangeTime = new float[Animator.layerCount];
            _lastAnimationChangeTime.SetAllValues(-1 / 0f);

            _currentAnimationState = new string[Animator.layerCount];
            _currentAnimationState.SetAllValues("");

            _timedCoroutine = new Coroutine[Animator.layerCount];
            _timedCoroutine.SetAllValues(null);
        }
        protected override void OnStart()
        {
            if (RandomStart == true)
            {
                _animator.Play(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.value);
            }
        }
        private void OnEnable()
        {
            for (int layer = 0; layer < Animator.layerCount; layer++)
            {
                _timedAnimationPlaying[layer] = false;
                _currentAnimationState[layer] = "";
                _currentAnimationPriority[layer] = -1;
                StopCoroutine(ref _timedCoroutine[layer]);
            }
        }
        private void OnDisable()
        {
            for (int layer = 0; layer < Animator.layerCount; layer++)
            {
                _timedAnimationPlaying[layer] = false;
                _currentAnimationState[layer] = "";
                _currentAnimationPriority[layer] = -1;
                StopCoroutine(ref _timedCoroutine[layer]);
            }
        }
        #endregion

        #region Utilities
        #region Animation playing
        /// <summary>
        /// Will not crossfade if the state is already running
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="blendTime"></param>
        /// <param name="speed"></param>
        public void TryCrossfadeContinuousState(string state, int layer = 0, float speed = 1f, float blendTime = 0.2f)
        {
            if (_currentAnimationState[layer] == state || _currentAnimationPriority[layer] >= 0 || _timedAnimationPlaying[layer] == true)
            {
                return;
            }

            _currentAnimationState[layer] = state;
            _currentAnimationPriority[layer] = -1;
            _lastAnimationChangeTime[layer] = Time.time;
            Animator.SetFloat("Layer" + layer + "_Speed", speed);
            _animator.CrossFadeInFixedTime(state, blendTime, layer);
        }

        /// <summary>
        /// Will not crossfade if the state is already running
        /// </summary>
        /// <param name="stateParams"></param>
        /// <param name="layer"></param>
        /// <param name="blendTime"></param>
        public void TryCrossfadeContinuousState(SpeedStateParameters stateParams, int layer = 0, float blendTime = 0.2f)
        {
            TryCrossfadeContinuousState(stateParams.Names.Random(), layer, stateParams.Speed, blendTime);
        }

        public void ForcePlayState(string state, int layer = 0, int priority = 0, float prematureEndTime = 0f)
        {
            ForcePlaySpeedState(state, layer, priority, 1, prematureEndTime);
        }
        public void ForcePlayTimedState(string state, int layer, int priority, float time, float prematureEndTime = 0f)
        {
            if (state == "" || priority < _currentAnimationPriority[layer])
            {
                return;
            }

            _timedAnimationPlaying[layer] = true;
            _currentAnimationState[layer] = state;
            _lastAnimationChangeTime[layer] = Time.time;
            _currentAnimationPriority[layer] = priority;

            PinouUtils.Animation.PlayAnimationForDuration(Animator, state, time, 0);

            RestartCoroutine(TimedAnimationCoroutine(time - prematureEndTime, layer), ref _timedCoroutine[layer]);
        }
        public void ForcePlayTimedState(string state, int layer, float time, float prematureEndTime = 0f)
        {
            ForcePlayTimedState(state, layer, 0, time, prematureEndTime);
        }
        public void ForcePlayTimedState(TimedStateParameters stateParams, int layer = 0, int priority = 0, float prematureEndTime = 0f)
        {
            ForcePlayTimedState(stateParams.Names.Random(), layer, priority, stateParams.Time, prematureEndTime);
        }
        public void ForcePlayTimedState(string state, float time, float prematureEndTime = 0f)
        {
            ForcePlayTimedState(state, 0, 0, time, prematureEndTime);
        }

        public void ForcePlaySpeedState(string state, int layer, int priority, float speed, float prematureEndTime = 0f)
        {
            if (state == "" || priority < _currentAnimationPriority[layer])
            {
                return;
            }

            _timedAnimationPlaying[layer] = true;
            _currentAnimationState[layer] = state;
            _lastAnimationChangeTime[layer] = Time.time;
            _currentAnimationPriority[layer] = priority;

            Animator.Play(state, layer, 0);
            Animator.SetFloat("Layer" + layer + "_Speed", speed);

            float time = PinouUtils.Animation.GetNextClipLength(Animator, layer) / speed;

            RestartCoroutine(TimedAnimationCoroutine(time - prematureEndTime, layer), ref _timedCoroutine[layer]);
        }
        public void ForcePlaySpeedState(string state, int layer, float speed, float prematureEndTime = 0f)
        {
            ForcePlaySpeedState(state, layer, 0, speed, prematureEndTime);
        }
        public void ForcePlaySpeedState(SpeedStateParameters stateParams, int layer = 0, int priority = 0, float prematureEndTime = 0f)
        {
            ForcePlaySpeedState(stateParams.Names.Random(), layer, priority, stateParams.Speed, prematureEndTime);
        }
        public void ForcePlaySpeedState(string state, float speed, float prematureEndTime = 0f)
        {
            ForcePlaySpeedState(state, 0, 0, speed, prematureEndTime);
        }

        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeState(string state, int layer = 0, int priority = 0, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeSpeedState(state, layer, priority, 1, blendTime, prematureEndTime);
        }

        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="time"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeTimedState(string state, int layer, int priority, float time, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            if (state == "" || priority < _currentAnimationPriority[layer] || isActiveAndEnabled == false)
            {
                return;
            }

            _timedAnimationPlaying[layer] = true;
            _currentAnimationState[layer] = state;
            _lastAnimationChangeTime[layer] = Time.time;
            _currentAnimationPriority[layer] = priority;

            PinouUtils.Animation.CrossFadeAnimationForDuration(Animator, state, time, layer, blendTime);
            RestartCoroutine(TimedAnimationCoroutine(time - prematureEndTime, layer), ref _timedCoroutine[layer]);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="time"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeTimedState(string state, int layer, float time, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeTimedState(state, layer, 0, time, blendTime, prematureEndTime);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="time"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeTimedState(string state, float time, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeTimedState(state, 0, 0, time, blendTime, prematureEndTime);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="stateParams"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeTimedState(TimedStateParameters stateParams, int layer = 0, int priority = 0, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {

            ForceCrossFadeTimedState(stateParams.Names.Random(), layer, priority, stateParams.Time, blendTime, prematureEndTime);
        }

        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="speed"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeSpeedState(string state, int layer, int priority, float speed, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            if (state == "" || priority < _currentAnimationPriority[layer])
            {
                return;
            }

            _timedAnimationPlaying[layer] = true;
            _currentAnimationState[layer] = state;
            _lastAnimationChangeTime[layer] = Time.time;
            _currentAnimationPriority[layer] = priority;

            Animator.CrossFadeInFixedTime(state, blendTime, layer, 0f);
            Animator.SetFloat("Layer" + layer + "_Speed", speed);

            float time = PinouUtils.Animation.GetNextClipLength(Animator, layer) / speed;

            RestartCoroutine(TimedAnimationCoroutine(time - prematureEndTime, layer), ref _timedCoroutine[layer]);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="layer"></param>
        /// <param name="speed"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeSpeedState(string state, int layer, float speed, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeSpeedState(state, layer, 0, speed, blendTime, prematureEndTime);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="state"></param>
        /// <param name="speed"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeSpeedState(string state, float speed, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeSpeedState(state, 0, 0, speed, blendTime, prematureEndTime);
        }
        /// <summary>
        /// Will play an animation that will override continuous states
        /// </summary>
        /// <param name="stateParams"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="blendTime"></param>
        /// <param name="prematureEndTime"></param>
        public void ForceCrossFadeSpeedState(SpeedStateParameters stateParams, int layer = 0, int priority = 0, float blendTime = 0.05f, float prematureEndTime = 0.2f)
        {
            ForceCrossFadeSpeedState(stateParams.Names.Random(), layer, priority, stateParams.Speed, blendTime, prematureEndTime);
        }

        public void StopTimedAnimation(int maxPriority = int.MaxValue, int layer = 0)
        {
            if (_currentAnimationPriority[layer] > maxPriority)
            {
                return;
            }

            StopCoroutine(ref _timedCoroutine[layer]);
            _timedAnimationPlaying[layer] = false;
            ResetCurrentAnimationInfos(layer);
        }

        private void ResetCurrentAnimationInfos(int layer)
        {
            _currentAnimationPriority[layer] = -1;
            _currentAnimationState[layer] = "";
            _lastAnimationChangeTime[layer] = Time.time;
        }

        /// <summary>
        /// Automatically play the "rewindState" with 1 - normalizedTime.
        /// It requires to setup the rewindstate manually in the animator.
        /// </summary>
        /// <param name="rewindState"></param>
        /// <param name="layer"></param>
        public void PlayRewindState(string rewindState, int layer = 0)
        {
            float rewindTime = Mathf.Clamp(Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime, 0f, 1f);
            _animator.Play(rewindState, layer, 1 - rewindTime);
        }
        #endregion
        #region Parameters
        public float GetLayerSpeed(int layer)
        {
            return _animator.GetFloat("Layer" + layer + "_Speed");
        }
        public void SetLayerSpeed(int layer, float speed)
        {
            _animator.SetFloat("Layer" + layer + "_Speed", speed);
        }
        public void SetFloat(string floatName, float value, int layer)
        {
            _animator.SetFloat(floatName, value * GetLayerSpeed(layer));
        }
        #endregion
        #region Useful
        public void ForceUpdate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                _animator = GetComponent<Animator>();
            }
#endif

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.Update(1 / 60f);
        }
        #endregion
        #endregion

        #region Coroutines
        private IEnumerator TimedAnimationCoroutine(float time, int layer)
        {
            yield return new WaitForSeconds(time);
            _timedAnimationPlaying[layer] = false;
            _timedCoroutine[layer] = null;
            ResetCurrentAnimationInfos(layer);
        }
        #endregion

    }
}
