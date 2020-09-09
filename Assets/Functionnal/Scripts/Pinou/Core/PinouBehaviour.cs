#pragma warning disable 0649
using System.Collections;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{

    public class PinouBehaviour : SerializedMonoBehaviour
    {
        #region Utilities Fields
        private GameObject _gameObject = null;
        public new GameObject gameObject => _gameObject == null ? base.gameObject : _gameObject;
        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        public Vector3 Forward { get { return transform.forward; } set { transform.forward = value; } }
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
        public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }
        public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }
        public Quaternion LocalRotation { get { return transform.localRotation; } set { transform.localRotation = value; } }
        public Vector3 LocalScale { get { return transform.localScale; } set { transform.localScale = value; } }

        [System.NonSerialized] private bool _awaken;
        [System.NonSerialized] private bool _needToCallSafeStart = false;
        public bool Awaken => _awaken;
        #endregion

        #region Base Behaviour
        [SerializeField, HideInInspector] private bool _hasAfterFixedUpdate;
        protected void Awake()
        {
            _gameObject = base.gameObject;
            if(_hasAfterFixedUpdate == true)
            {
                PinouUtils.MonoBehaviour.AfterFixedUpdate_Subscribe(OnAfterFixedUpdate);
            }
            if (PinouApp.SafeStarted == false)
			{
                PinouApp.OnSafeStart.SafeSubscribe(OnSafeStart);
            }
            else
			{
                _needToCallSafeStart = true;
            }
            _awaken = true;
            OnAwake();
        }
		protected void Start()
		{
            OnStart();
            if (_needToCallSafeStart == true)
			{
                OnSafeStart();
            }
        }
		/// <summary>
		/// Do not need base.
		/// </summary>
		protected virtual void OnAwake() { }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Do not need base. Triggers after PinouApp's loading.
        /// </summary>
        protected virtual void OnSafeStart() { }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnAfterFixedUpdate() { }

        private void OnEnable()
        {
            PinouUtils.MonoBehaviour.AfterFixedUpdate_Subscribe(OnAfterFixedUpdate);
            OnEnabled();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnEnabled() { }
        private void OnDisable()
        {
            PinouUtils.MonoBehaviour.AfterFixedUpdate_Unsubscribe(OnAfterFixedUpdate);
            OnDisabled();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnDisabled() { }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false)
			{
                return;
			}
#endif
            PinouUtils.MonoBehaviour.AfterFixedUpdate_Unsubscribe(OnAfterFixedUpdate);
            OnDestroyed();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnDestroyed() { }
        #endregion

        #region Utilities
        #region Misc.
        public T GetComponentInParent<T>(bool includeDisabled)
        {
            if (includeDisabled == false)
            {
                return GetComponentInParent<T>();
            }
            else
            {
                Transform parent = transform;
                while (parent != null)
                {
                    if (parent.TryGetComponent(out T comp))
                    {
                        return comp;
                    }
                    else
                    {
                        parent = parent.parent;
                    }
                }
            }

            return default;
        }
        protected void AutoFindReference<T>(ref T component) where T : Component
        {
            component = GetComponent<T>();
        }
        protected void AutoFindReferenceInParent<T>(ref T component) where T : Component
        {
            component = GetComponentInParent<T>(true);
        }
        protected void AutoFindReferenceInChildren<T>(ref T component) where T : Component
        {
            component = GetComponentInChildren<T>(true);
        }
        #endregion
        #region Coroutines
        /// <summary>
        /// Automatically start the coroutine and restart it if it was already playing
        /// </summary>
        /// <param name="function"></param>
        /// <param name="refToCoroutine"></param>
        public void RestartCoroutine(IEnumerator function, ref Coroutine refToCoroutine)
        {
            if (refToCoroutine != null)
            {
                StopCoroutine(refToCoroutine);
            }

            refToCoroutine = StartCoroutine(function);
        }

        /// <summary>
        /// Automatically set the refToCoroutine to null if the coroutine is stopped
        /// </summary>
        /// <param name="refToCoroutine"></param>
        /// <returns></returns>
        public bool StopCoroutine(ref Coroutine refToCoroutine)
        {
            bool toReturn = false;
            if (refToCoroutine != null)
            {
                StopCoroutine(refToCoroutine);
                toReturn = true;
            }

            refToCoroutine = null;

            return toReturn;
        }

        public Coroutine Invoke(System.Action method, float delay, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, scaledTime));
        }
        public Coroutine Invoke<T>(System.Action<T> method, float delay, T parameter, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, parameter, scaledTime));
        }
        public Coroutine Invoke<T, TT>(System.Action<T, TT> method, float delay, T parameter, TT parameter2, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, parameter, parameter2, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT>(System.Action<T, TT, TTT> method, float delay, T parameter, TT parameter2, TTT parameter3, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, parameter, parameter2, parameter3, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT, TTTT>(System.Action<T, TT, TTT, TTTT> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, parameter, parameter2, parameter3, parameter4, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT, TTTT, Y>(System.Action<T, TT, TTT, TTTT, Y> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(method, delay, parameter, parameter2, parameter3, parameter4, parameter5, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT, TTTT, Y, YY>(System.Action<T, TT, TTT, TTTT, Y, YY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(
                method, delay, parameter, parameter2, parameter3, parameter4, parameter5, parameter6, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT, TTTT, Y, YY, YYY>(System.Action<T, TT, TTT, TTTT, Y, YY, YYY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, YYY parameter7, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(
                method, delay, parameter, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, scaledTime));
        }
        public Coroutine Invoke<T, TT, TTT, TTTT, Y, YY, YYY, YYYY>(System.Action<T, TT, TTT, TTTT, Y, YY, YYY, YYYY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, YYY parameter7, YYYY parameter8, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineAction(
                method, delay, parameter, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, scaledTime));
        }

        public Coroutine Invoke(string methodName, float delay, bool scaledTime)
        {
            return Invoke(this, methodName, delay, null, scaledTime);
        }
        public Coroutine Invoke(string methodName, float delay, object parameter, bool scaledTime = true)
        {
            return Invoke(this, methodName, delay, new object[] { parameter }, scaledTime);
        }
        public Coroutine Invoke(string methodName, float delay, object[] parameters, bool scaledTime = true)
        {
            return Invoke(this, methodName, delay, parameters, scaledTime);
        }
        public Coroutine Invoke<T>(T objectToCallInvoke, string methodName, float delay, object[] parameters, bool scaledTime = true)
        {
            return StartCoroutine(InvokeCoroutineReflection(objectToCallInvoke, methodName, delay, parameters, scaledTime));
        }
        private IEnumerator InvokeCoroutineReflection<T>(T objectToCallInvoke, string methodName, float delay, object[] parameters, bool scaledTime)
        {
            MethodInfo method = PinouUtils.Reflection.GetInstanceMethod(objectToCallInvoke, methodName);
            bool succeed = true;

            if (parameters != null)
            {
                succeed = PinouUtils.Reflection.FitParametersToMethod(method, ref parameters);
            }

            if (succeed == false)
            {
                yield break;
            }

            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            method.Invoke(objectToCallInvoke, parameters);
        }
        private IEnumerator InvokeCoroutineAction(System.Action method, float delay, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            method.Invoke();
        }
        private IEnumerator InvokeCoroutineAction<T>(System.Action<T> method, float delay, T parameter, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter);
        }
        private IEnumerator InvokeCoroutineAction<T, TT>(System.Action<T, TT> method, float delay, T parameter, TT parameter2, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT>(System.Action<T, TT, TTT> method, float delay, T parameter, TT parameter2, TTT parameter3, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT, TTTT>(System.Action<T, TT, TTT, TTTT> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3, parameter4);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT, TTTT, Y>(System.Action<T, TT, TTT, TTTT, Y> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3, parameter4, parameter5);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT, TTTT, Y, YY>(System.Action<T, TT, TTT, TTTT, Y, YY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3, parameter4, parameter5, parameter6);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT, TTTT, Y, YY, YYY>(System.Action<T, TT, TTT, TTTT, Y, YY, YYY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, YYY parameter7, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7);
        }
        private IEnumerator InvokeCoroutineAction<T, TT, TTT, TTTT, Y, YY, YYY, YYYY>(System.Action<T, TT, TTT, TTTT, Y, YY, YYY, YYYY> method, float delay, T parameter, TT parameter2, TTT parameter3, TTTT parameter4, Y parameter5, YY parameter6, YYY parameter7, YYYY parameter8, bool scaledTime)
        {
            if (scaledTime == true)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            method.Invoke(parameter, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8);
        }
        #endregion
        #endregion

        #region Editor
#if UNITY_EDITOR
        /// <summary>
        /// Need base
        /// </summary>
        protected virtual void E_OnReload()
        {
            if (enabled == true)
            {
                Awake();
            }
        }
        protected virtual void E_OnReload_SecondPass()
        {
            if (enabled == true)
            {
                MethodInfo startMethod = this.GetType().GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                startMethod?.Invoke(this, null);
            }
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void E_OnPreprocessBuild()
        {
        }

        private void OnDrawGizmos()
        {
            if (_awaken == false)
            {
                return;
            }

            E_OnDrawGizmos();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void E_OnDrawGizmos()
        {

        }
        private void OnDrawGizmosSelected()
        {
            if (_awaken == false)
            {
                return;
            }

            E_OnDrawGizmosSelected();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void E_OnDrawGizmosSelected()
        {

        }
        private void OnValidate()
        {
            if (_awaken == false)
            {
                return;
            }

            E_OnValidate();
        }
        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void E_OnValidate()
        {

        }
#endif
        #endregion

    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class PinouStaticBehaviour
    {
        static PinouStaticBehaviour()
        {
#if UNITY_EDITOR
            PinouUtils.Editor.OnReloadScripts -= ES_OnScriptsReload;
            PinouUtils.Editor.OnReloadScripts -= ES_OnScriptsReload;
            PinouUtils.Editor.OnPreprocessBuild -= ES_OnPreprocessBuild;
            PinouUtils.Editor.OnPreprocessBuild -= ES_OnPreprocessBuild;

            PinouUtils.Editor.OnReloadScripts += ES_OnScriptsReload;
            PinouUtils.Editor.OnPreprocessBuild += ES_OnPreprocessBuild;
#endif
        }

#if UNITY_EDITOR
        private static void ES_OnScriptsReload()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode == true || PinouUtils.CustomEditorBuildProcessor.Building == true)
            {
                return;
            }

            foreach (PinouBehaviour p in Object.FindObjectsOfType<PinouBehaviour>())
            {
                p.GetType().GetMethod("E_OnReload", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(p, null);
            }
            foreach (PinouBehaviour p in Object.FindObjectsOfType<PinouBehaviour>())
            {
                p.GetType().GetMethod("E_OnReload_SecondPass", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(p, null);
            }
        }
        private static void ES_OnPreprocessBuild()
        {
            foreach (PinouBehaviour p in Object.FindObjectsOfType<PinouBehaviour>())
            {
                p.GetType().GetMethod("E_OnPreprocessBuild", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(p, null);
            }
        }
#endif
    }
}

