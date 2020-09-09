#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Pinou.EntitySystem;
using Steamworks;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
#endif

namespace Pinou
{
    public static class PinouUtils
    {
        private static class MonoProvider
        {
            public class CallbackBehaviour : UnityEngine.MonoBehaviour
            {
                public Delegate.Action OnFixedUpdate = new Delegate.Action();
                public Delegate.Action OnAfterFixedUpdate = new Delegate.Action();
                public Delegate.Action OnUpdate = new Delegate.Action();
                public Delegate.Action OnLateUpdate = new Delegate.Action();
                public Delegate.Action OnGizmosDrawn = new Delegate.Action();

                private void Awake()
                {
                    StartCoroutine(AfterFixedUpdateCoroutine());
                }
                private void FixedUpdate()
                {
                    OnFixedUpdate.Invoke();
                }
                private IEnumerator AfterFixedUpdateCoroutine()
                {
                    while (true)
                    {
                        yield return new WaitForFixedUpdate();
                        AfterFixedUpdate();
                    }
                }
                private void AfterFixedUpdate()
                {
                    OnAfterFixedUpdate.Invoke();
                }
                private void Update()
                {
                    OnUpdate.Invoke();
                }
                private void LateUpdate()
                {
                    OnLateUpdate.Invoke();
                }
                private void OnDrawGizmos()
                {
                    OnGizmosDrawn.Invoke();
                }
            }
            private static PinouBehaviour __mono = null;
            public static PinouBehaviour Mono { get { MonoSecurityCheck(); return __mono; } }

            private static CallbackBehaviour __callbackMono = null;
            public static CallbackBehaviour CallbackMono { get { CallbackSecurityCheck(); return __callbackMono; } }

            private static void MonoSecurityCheck()
            {
#if UNITY_EDITOR
                if (Editor.IsExitingPlayMode == true || EditorApplication.isPlaying == false)
                {
                    return;
                }
#endif
                if (__mono == null)
                {
                    UnityEngine.GameObject o = new UnityEngine.GameObject();
                    o.name = "PinouMono";
                    o.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    __mono = o.AddComponent<PinouBehaviour>();
                    UnityEngine.Object.DontDestroyOnLoad(o);
                }
            }
            private static void CallbackSecurityCheck()
            {
#if UNITY_EDITOR
                if (Editor.IsExitingPlayMode == true || EditorApplication.isPlaying == false)
                {
                    return;
                }
#endif
                if (__callbackMono == null)
                {
                    UnityEngine.GameObject o = new UnityEngine.GameObject();
                    o.name = "PinouCallback";
                    o.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    __callbackMono = o.AddComponent<CallbackBehaviour>();
                    UnityEngine.Object.DontDestroyOnLoad(o);
                }
            }
        }

        public static class Animation
        {
            /// <summary>
            /// Automatically set the animator speed so the state will be played in duration seconds.
            /// </summary>
            /// <param name="animator"></param>
            /// <param name="stateName"></param>
            /// <param name="duration"></param>
            public static void PlayAnimationForDuration(Animator animator, string stateName, float duration, int layer)
            {
                animator.Play(stateName, layer, 0);
                animator.Update(0.0f);

                //maybe problems with next use so it requires a check
                float clipDuration = animator.GetCurrentAnimatorClipInfo(layer)[0].clip.length;

                float speed = clipDuration / duration;

                animator.SetFloat("Layer" + layer + "_Speed", speed);
            }

            /// <summary>
            /// Automatically set the animator speed so the state will be played in duration seconds.
            /// </summary>
            /// <param name="animator"></param>
            /// <param name="stateName"></param>
            /// <param name="duration"></param>
            public static void CrossFadeAnimationForDuration(Animator animator, string stateName, float duration, int layer, float blendTime)
            {
                animator.CrossFadeInFixedTime(stateName, blendTime, layer);
                animator.Update(0.0f);
                if (animator.GetNextAnimatorClipInfo(layer).Length == 0)
                {
                    return;
                }

                float clipDuration = animator.GetNextAnimatorClipInfo(layer)[0].clip.length;
                float speed = clipDuration / duration;

                animator.SetFloat("Layer" + layer + "_Speed", speed);
            }


            public static float GetNextClipLength(Animator animator, int layer = 0)
            {
                animator.Update(0.0f);
                AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(layer);
                if (clipInfos.Length <= 0)
                {
                    Debug.LogError("Next clip not found. Perhaps state name typed wrong ?");
                    return 1f;
                }
                else
                {
                    return clipInfos[0].clip.length;
                }
            }

#if UNITY_EDITOR
            public static string[] E_GetAllStatesNames(Animator animator)
            {
                UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                List<string> names = new List<string>();
                UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[0].stateMachine;
                for (int i = 0; i < sm.states.Length; i++)
                {
                    names.Add(sm.states[i].state.name);
                }
                return names.ToArray();
            }
            public static string E_GetDefaultState(Animator animator)
            {
                UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                List<string> names = new List<string>();
                UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[0].stateMachine;
                return sm.defaultState.name;
            }
#endif
        }

        public static class AnimationCurve
        {
            public static UnityEngine.AnimationCurve Exponential
            {
                get
                {
                    return new UnityEngine.AnimationCurve(
                       new Keyframe[] {
                        new Keyframe(0, 0, 0, 0),
                        new Keyframe(1, 1, 1.5f, 0) });
                }
            }
            public static UnityEngine.AnimationCurve ExponentialMirrored
            {
                get
                {
                    return new UnityEngine.AnimationCurve(
                       new Keyframe[] {
                        new Keyframe(0, 1, 0, 0),
                        new Keyframe(1, 0, -1.5f, 0) });
                }
            }
            public static UnityEngine.AnimationCurve WeightedExponential
            {
                get
                {
                    return new UnityEngine.AnimationCurve(
                       new Keyframe[] {
                        new Keyframe(0, 0, 0, 0, 0.5f, 0.5f),
                        new Keyframe(1, 1, 3f, 0, 0.2f, 0.5f) });
                }
            }

            public static UnityEngine.AnimationCurve WeightedExponentialMirrored
            {
                get
                {
                    return new UnityEngine.AnimationCurve(
                       new Keyframe[] {
                        new Keyframe(0, 1, 0, 0, 0.5f, 0.5f),
                        new Keyframe(1, 0, -3f, 0, 0.2f, 0.5f) });
                }
            }
            public static UnityEngine.AnimationCurve Flat
            {
                get
                {
                    return new UnityEngine.AnimationCurve(
                       new Keyframe[] {
                        new Keyframe(0, 1, 0, 0),
                        new Keyframe(1, 1, 0, 0) });
                }
            }
        }

        public static class Physics
        {
            private static List<UnityEngine.GameObject> _ignoredGameObjects = new List<UnityEngine.GameObject>();
            private static List<int> _ignoreGameObjectsOldLayers = new List<int>();

            private const int IGNOREPHYSIC_LAYER_ID = 2;

            public static void IgnorePhysicForGameobject(UnityEngine.GameObject go)
            {
                if (_ignoredGameObjects.Contains(go))
                {
                    //In the case something overrides the physic ignore outside this script
                    if (go.layer != IGNOREPHYSIC_LAYER_ID)
                    {
                        int index = _ignoredGameObjects.IndexOf(go);

                        _ignoredGameObjects.RemoveAt(index);
                        _ignoreGameObjectsOldLayers.RemoveAt(index);
                    }
                    //If its already ignored
                    else
                    {
                        return;
                    }
                }

                _ignoredGameObjects.Add(go);
                _ignoreGameObjectsOldLayers.Add(go.layer);

                go.layer = IGNOREPHYSIC_LAYER_ID;
            }
            public static void IgnorePhysicForGameobjects(UnityEngine.GameObject[] gos)
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    IgnorePhysicForGameobject(gos[i]);
                }
            }
            /// <summary>
            /// Will recover all previously ignored gameobject layers.
            /// </summary>
            /// <param name="includeModifiedOutsideGameobjects">
            /// In the case a gameobject has a different layer than the ignore physic layer, should it still override it with old one ?
            /// </param>
            public static void RecoverGameObjectsPhysic(bool includeModifiedOutsideGameobjects = false)
            {
                for (int i = 0; i < _ignoredGameObjects.Count; i++)
                {
                    //Ignore gameobjects that has changed if we dont include thel
                    if (includeModifiedOutsideGameobjects == false)
                    {
                        if (_ignoredGameObjects[i].layer != IGNOREPHYSIC_LAYER_ID)
                        {
                            continue;
                        }
                    }

                    _ignoredGameObjects[i].layer = _ignoreGameObjectsOldLayers[i];
                }

                _ignoredGameObjects.Clear();
                _ignoreGameObjectsOldLayers.Clear();
            }
        }

        public static class Coroutine
        {
            public static UnityEngine.Coroutine Invoke(Action method, float delay, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T>(Action<T> method, float delay, T param1, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT>(Action<T, TT> method, float delay, T param1, TT param2, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT>(Action<T, TT, TTT> method, float delay, T param1, TT param2, TTT param3, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT, TTTT>(Action<T, TT, TTT, TTTT> method, float delay, T param1, TT param2, TTT param3, TTTT param4, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, param4, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT, TTTT, Y>(Action<T, TT, TTT, TTTT, Y> method, float delay, T param1, TT param2, TTT param3, TTTT param4, Y param5, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, param4, param5, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT, TTTT, Y, YY>(Action<T, TT, TTT, TTTT, Y, YY> method, float delay, T param1, TT param2, TTT param3, TTTT param4, Y param5, YY param6, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, param4, param5, param6, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT, TTTT, Y, YY, YYY>(Action<T, TT, TTT, TTTT, Y, YY, YYY> method, float delay, T param1, TT param2, TTT param3, TTTT param4, Y param5, YY param6, YYY param7, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, param4, param5, param6, param7, scaledTime);
            }
            public static UnityEngine.Coroutine Invoke<T, TT, TTT, TTTT, Y, YY, YYY, YYYY>(Action<T, TT, TTT, TTTT, Y, YY, YYY, YYYY> method, float delay, T param1, TT param2, TTT param3, TTTT param4, Y param5, YY param6, YYY param7, YYYY param8, bool scaledTime = true)
            {
                return MonoProvider.Mono.Invoke(method, delay, param1, param2, param3, param4, param5, param6, param7, param8, scaledTime);
            }


            public static UnityEngine.Coroutine StartCoroutine(IEnumerator function)
            {
                return MonoProvider.Mono.StartCoroutine(function);
            }
            public static void RestartCoroutine(IEnumerator function, ref UnityEngine.Coroutine coroutine)
            {
                MonoProvider.Mono.RestartCoroutine(function, ref coroutine);
            }
            public static void StopCoroutine(ref UnityEngine.Coroutine coroutine)
            {
                MonoProvider.Mono.StopCoroutine(ref coroutine);
            }
        }

        public static class Maths
        {
            public static Vector2 SubtractMagnitude(Vector2 v, float magnitude)
            {
                if (v.magnitude > magnitude)
                    return v.normalized * (v.magnitude - magnitude);
                else
                    return Vector3.zero;
            }
            public static Vector2 DeadzoneVector(Vector2 vec, float innerDeadzone = 0.1f, float outterDeadzone = 0f, float linearity = 1f)
            {
                if (vec.magnitude < innerDeadzone)
                {
                    return Vector2.zero;
                }
                else if (vec.magnitude > 1 - outterDeadzone)
                {
                    return vec.normalized;
                }
                else
                {
                    vec = SubtractMagnitude(vec, innerDeadzone);
                    vec *= 1 / (1 - outterDeadzone - innerDeadzone);
                    float x = Mathf.Pow(Mathf.Abs(vec.x), linearity);
                    float y = Mathf.Pow(Mathf.Abs(vec.y), linearity);

                    x *= vec.x < 0 ? -1 : 1;
                    y *= vec.y < 0 ? -1 : 1;
                    return new Vector2(x, y);
                }
            }

            /// <summary>
            /// Will make T equal to one if used in editor
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            public static float Lerp(float a, float b, float t)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying == false)
                {
                    t = 1;
                }
#endif
                return b * t + a * (1 - t);
            }

            public static Vector3 PredictAim(Vector3 shootPos, Vector3 targetPos, Vector3 targetVel, float projSpeed)
			{
                if (targetVel.sqrMagnitude <= 0f)
				{
                    return targetPos;
                }
                else
				{
                    Vector3 targetToBullet = shootPos - targetPos;
                    float distToTargetSqr = (shootPos - targetPos).sqrMagnitude;
                    float distToTarget = (shootPos - targetPos).magnitude;
                    Vector3 targetToBulletNorm = targetToBullet / distToTarget;
                    float tarSpeed = targetVel.magnitude;
                    float tarSpeedSqr = targetVel.sqrMagnitude;
                    Vector3 tarVelNorm = targetVel / tarSpeed;
                    float projSpeedSqr = projSpeed.Squared();

                    float cosTheta = Vector3.Dot(targetToBulletNorm, tarVelNorm);

                    float offset = Mathf.Sqrt((2 * distToTarget * tarSpeed * cosTheta).Squared() + 4 * (projSpeedSqr - tarSpeedSqr) * distToTargetSqr);

                    float estimatedTravelTime = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));
                    #region Offset should be ±, but works fine with just +
                    //float estimatedTravelTimeTwo = (-2 * distToTarget * tarSpeed * cosTheta + offset) / (2 * (projSpeedSqr - tarSpeedSqr));

                    /*if (estimatedTravelTimeOne < 0 && estimatedTravelTimeTwo < 0 ||
                        estimatedTravelTimeOne == float.NaN && estimatedTravelTimeTwo == float.NaN)*/
                    #endregion

                    if (estimatedTravelTime < 0 || estimatedTravelTime == float.NaN)
					{
                        return targetPos;
                    }
                    else
					{
						#region Offset should be ±, but works fine with just +
						/*if (estimatedTravelTimeOne < estimatedTravelTimeTwo)
						{
                            return targetPos + tarVelNorm * tarSpeed * estimatedTravelTimeOne;
						}
                        else
						{
                            return targetPos + tarVelNorm * tarSpeed * estimatedTravelTimeTwo;

                        }*/
						#endregion

						return targetPos + tarVelNorm * tarSpeed * estimatedTravelTime;
                    }
                }
            }

            public static int Pow2toIndex(int pow)
			{
				switch (pow)
				{
                    case 1:
                        return 0;
                    case 2:
                        return 1;
                    case 4:
                        return 2;
                    case 8:
                        return 3;
                    case 16:
                        return 4;
                    case 32:
                        return 5;
                    case 64:
                        return 6;
                    case 128:
                        return 7;
                    case 256:
                        return 8;
                    case 512:
                        return 9;
                    case 1024:
                        return 10;
                    case 2048:
                        return 11;
                    case 4096:
                        return 12;
                    case 8192:
                        return 13;
                    case 16384:
                        return 14;
                    case 32768:
                        return 15;
                    case 65536:
                        return 16;
                    case 131072:
                        return 17;
                    case 262144:
                        return 18;
                    case 524288:
                        return 19;
                    case 1048576:
                        return 20;
                    case 2097152:
                        return 21;
                    case 4194304:
                        return 22;
                    case 8388608:
                        return 23;
                    case 16777216:
                        return 24;
                    case 33554432:
                        return 25;
                    case 67108664:
                        return 26;
                    case 134217728:
                        return 27;
                    case 268435546:
                        return 28;
                    case 536870912:
                        return 29;
                    case 1073741824:
                        return 30;
                }

                return -1;
			}

            [Serializable]
            public class Formula
            {
                [SerializeField] private float _base;
                [SerializeField] private float _flatIncrease;
                [SerializeField] private float _powBase;
                [SerializeField] private float _pow = 1;
                [SerializeField] private Vector2 _amountCaps = new Vector2(-1/0f, 1/0f);
                [SerializeField] private Vector2 _resultsCaps = new Vector2(-1/0f, 1/0f);
                [SerializeField] private bool _zIsCumulative = false;
                [SerializeField] private Vector3[] _previews;

                public float Evaluate(float amount)
                {
                    amount = Mathf.Clamp(amount, _amountCaps.x, _amountCaps.y);
                    return Mathf.Clamp(_base +
                        _flatIncrease * amount +
                        Mathf.Pow(Mathf.Abs(_powBase) * amount, _pow) * Mathf.Sign(_powBase), _resultsCaps.x, _resultsCaps.y);
                }
                public int EvaluateToInt(float amount)
                {
                    return Mathf.FloorToInt(Evaluate(amount));
                }

                [Button("Update Previews")]
                private void UpdatePreviews(bool evaluateToInt = false)
                {
                    if (_previews == null) { return; }
                    for (int i = 0; i < _previews.Length; i++)
                    {
                        _previews[i].x = Mathf.Clamp(_previews[i].x, _amountCaps.x, _amountCaps.y);

                        if (evaluateToInt == false)
                        {
                            _previews[i].y = Evaluate(_previews[i].x);

                            if (_zIsCumulative == true)
							{
                                if (_previews[i].x > 1000000000)
								{
                                    return;
								}
                                float value = 0f;
                                for (int l = 1; l <= _previews[i].x; l++)
                                {
                                    value += Evaluate(l);
                                }
                                _previews[i].z = value;
                            }
                            else
							{
                                _previews[i].z = 0;
                            }
                        }
                        else
                        {
                            _previews[i].y = EvaluateToInt(_previews[i].x);
                            
                            if (_zIsCumulative == true)
							{
                                if (_previews[i].x > 1000000000)
                                {
                                    return;
                                }
                                int value = 0;
                                for (int l = 1; l <= _previews[i].x; l++)
                                {
                                    value += EvaluateToInt(l);
                                }
                                _previews[i].z = value;
                            }
                            else
							{
                                _previews[i].z = 0;
                            }
                        }
                    }
                }
            }
        }

        public static class Quaternion
        {
            public static UnityEngine.Quaternion SafeLookRotation(Vector3 direction,Vector3 defaultDirection)
            {
                if (direction.sqrMagnitude <= Mathf.Epsilon || direction == Vector3.zero)
                {
                    return UnityEngine.Quaternion.LookRotation(defaultDirection);
                }

                return UnityEngine.Quaternion.LookRotation(direction);
            }
        }

        public static class Transform
        {
            private static UnityEngine.Transform __transformHelper = null;

            public static void SetLayerOfAllChildrens(UnityEngine.Transform transform, LayerMask layer, bool includeSelf = true)
            {
                if (includeSelf)
                {
                    transform.gameObject.layer = layer;
                }

                foreach (UnityEngine.Transform t in transform)
                {
                    SetLayerOfAllChildrens(t, layer);
                }
            }

            private static void CreateTransformHelperIfNeeded()
            {
                if (__transformHelper == null)
                {
                    UnityEngine.GameObject o = new UnityEngine.GameObject();
                    o.hideFlags = HideFlags.HideAndDontSave;
                    o.name = "Transform Helper";
                    o.SetActive(false);
                    __transformHelper = o.transform;
                }
            }

            /// <summary>
            /// Get Forward of transform helper
            /// </summary>
            /// <returns></returns>
            public static Vector3 GetForward(UnityEngine.Quaternion rotation)
            {
                __transformHelper.rotation = rotation;
                return __transformHelper.forward;
            }
            /// <summary>
            /// Get up of transform helper
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Vector3 GetUp(UnityEngine.Quaternion rotation, float z = 0f)
            {
                __transformHelper.rotation = rotation;
                __transformHelper.localEulerAngles = __transformHelper.localEulerAngles.SetZ(z);
                return __transformHelper.up;
            }

            /// <summary>
            /// Vector is local space and return it expressed in world space depending on rotation
            /// </summary>
            /// <param name="vector">
            /// Vector expressed in (0,0,0) euler angles
            /// </param>
            /// <param name="rotation">
            /// rotation to rotate the vector to
            /// </param>
            public static Vector3 TransformVector(Vector3 vector, UnityEngine.Quaternion rotation)
            {
                CreateTransformHelperIfNeeded();

                __transformHelper.rotation = rotation;

                return __transformHelper.TransformVector(vector);
            }

            /// <summary>
            /// vector is world space with (0,0,0) euler angles rotation and return it expressed world space rotated around position by rotation
            /// </summary>
            /// <param name="point">The vector to rotate</param>
            /// <param name="position">The pivot of the rotation</param>
            /// <param name="rotation">The rotation</param>
            /// <returns></returns>
            public static Vector3 TransformPoint(Vector3 point, Vector3 position, UnityEngine.Quaternion rotation)
            {
                CreateTransformHelperIfNeeded();

                __transformHelper.position = position;
                __transformHelper.rotation = rotation;

                return __transformHelper.TransformPoint(point);
            }
            /// <summary>
            /// Vector is world space and return it expressed in local space depending on rotation
            /// </summary>
            /// <param name="vector">
            /// Vector expressed in rotation you want in local space
            /// </param>
            /// <param name="rotation">
            /// rotation to express localy the vector
            /// </param>
            public static Vector3 InverseTransformVector(Vector3 vector, UnityEngine.Quaternion rotation)
            {
                CreateTransformHelperIfNeeded();

                __transformHelper.rotation = rotation;

                return __transformHelper.InverseTransformVector(vector);
            }

            /// <summary>
            /// Vector is world space and return it expressed in local space depending on rotation & position
            /// </summary>
            /// <param name="point">The vector to express localy</param>
            /// <param name="position">The pivot of the rotation</param>
            /// <param name="rotation">The rotation</param>
            /// <returns></returns>
            public static Vector3 InverseTransformPoint(Vector3 point, Vector3 position, UnityEngine.Quaternion rotation)
            {
                CreateTransformHelperIfNeeded();

                __transformHelper.position = position;
                __transformHelper.rotation = rotation;

                return __transformHelper.InverseTransformPoint(point);
            }


            public static UnityEngine.Transform FindRecursive(UnityEngine.Transform parent, string name)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    if (parent.GetChild(i).name.Equals(name))
                    {
                        return parent.GetChild(i);
                    }
                    else
                    {
                        UnityEngine.Transform t;
                        if ((t = FindRecursive(parent.GetChild(i), name)) != null)
                        {
                            return t;
                        }
                    }
                }

                return null;
            }



            public static UnityEngine.Transform[] AllChilds(UnityEngine.Transform t)
            {
                UnityEngine.Transform[] ts = new UnityEngine.Transform[t.childCount];
                for (int i = 0; i < t.childCount; i++)
                {
                    ts[i] = t.GetChild(i);
                }

                return ts;
            }
        }

        public static class GameObject
        {
            /// <summary>
            /// will return even disabled objects
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static T FindObjectOfType<T>() where T : Component
            {
                UnityEngine.GameObject[] gos;

                for (int s = 0; s < SceneManager.sceneCount; s++)
                {
                    gos = SceneManager.GetSceneAt(s).GetRootGameObjects();

                    T t;
                    for (int i = 0; i < gos.Length; i++)
                    {
                        t = FindObjectOfTypeInChildren<T>(gos[i]);
                        if (t != null)
                        {
                            return t;
                        }
                    }
                }


                return null;
            }
            private static T FindObjectOfTypeInChildren<T>(UnityEngine.GameObject o) where T : Component
            {
                T t = o.GetComponent<T>();
                if (t != null) { return t; }
                else
				{
                    for (int i = 0; i < o.transform.childCount; i++)
                    {
                        t = FindObjectOfTypeInChildren<T>(o.transform.GetChild(i).gameObject);
                        if (t != null)
						{
                            return t;
						}
                    }
                }

                return null;
            }
            public static T[] FindObjectsOfType<T>() where T : UnityEngine.MonoBehaviour
            {
                UnityEngine.GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
                List<T> list = new List<T>();
                T t = null;

                for (int i = 0; i < gos.Length; i++)
                {
                    FindAndAddObjectOfTypeToList(gos[i], ref list, ref t);
                }

                return list.ToArray();
            }
            private static void FindAndAddObjectOfTypeToList<T>(UnityEngine.GameObject o, ref List<T> list, ref T t) where T : UnityEngine.MonoBehaviour
            {
                if (t = o.GetComponent<T>())
                {
                    list.Add(t);
                }
                for (int i = 0; i < o.transform.childCount; i++)
                {
                    FindAndAddObjectOfTypeToList(o.transform.GetChild(i).gameObject, ref list, ref t);
                }
            }
            public static UnityEngine.GameObject Find(string name)
            {
                UnityEngine.GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
                UnityEngine.GameObject o;
                for (int i = 0; i < gos.Length; i++)
                {
                    if ((o = FindInChildren(gos[i], ref name)) != null)
                    {
                        return o;
                    }
                }
                return null;
            }
            private static UnityEngine.GameObject FindInChildren(UnityEngine.GameObject o, ref string name)
            {
                for (int i = 0; i < o.transform.childCount; i++)
                {
                    if (o.name.Equals(name) == true)
                    {
                        return o;
                    }
                    else
                    {
                        FindInChildren(o.transform.GetChild(i).gameObject, ref name);
                    }
                }

                return null;
            }

            public static void DestroyAllChilds(UnityEngine.GameObject gameObject)
            {
                for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
                {
#if UNITY_EDITOR
                    if (EditorApplication.isPlaying == true)
                    {
#endif
                        gameObject.transform.GetChild(i).gameObject.Destroy();
#if UNITY_EDITOR
                    }
                    else
                    {
                        if (PrefabUtility.IsPartOfPrefabInstance(gameObject.transform))
                        {
                            UnityEngine.Object prefabInstance = PrefabUtility.GetPrefabInstanceHandle(gameObject.transform);
                            UnityEngine.Object.DestroyImmediate(prefabInstance);
                        }

                        UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
                    }
#endif
                }
            }
        }

        public static class MonoBehaviour
        {
            public static void FixedUpdate_Subscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnFixedUpdate.Subscribe(action, priority);
            }
            public static void FixedUpdate_Unsubscribe(Action action)
            {
                MonoProvider.CallbackMono.OnFixedUpdate.Unsubscribe(action);
            }

            public static void AfterFixedUpdate_Subscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnAfterFixedUpdate.Subscribe(action, priority);
            }
            public static void AfterFixedUpdate_Unsubscribe(Action action)
            {
                MonoProvider.CallbackMono.OnAfterFixedUpdate.Unsubscribe(action);
            }

            public static void Update_Subscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnUpdate.Subscribe(action, priority);
            }
            public static void Update_Unsubscribe(Action action)
            {
                MonoProvider.CallbackMono.OnUpdate.Unsubscribe(action);
            }

            public static void LateUpdate_Subscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnLateUpdate.Subscribe(action, priority);
            }
            public static void LateUpdate_Unsubscribe(Action action)
            {
                MonoProvider.CallbackMono.OnLateUpdate.Unsubscribe(action);
            }

            public static void OnDrawGizmos_Subscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnGizmosDrawn.Subscribe(action, priority);
            }
            public static void OnDrawGizmos_Ubsubscribe(Action action, int priority = 0)
            {
                MonoProvider.CallbackMono.OnGizmosDrawn.Unsubscribe(action);
            }
        }

        public static class Reflection
        {
            private static BindingFlags instancePrivateFlags =
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance;

            /// <summary>
            /// Return Public, nonpublic instance method of object
            /// </summary>
            /// <param name="objectToGetField"></param>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public static FieldInfo GetInstanceField(object objectToGetField, string fieldName)
            {
                return objectToGetField.GetType().GetField(fieldName, instancePrivateFlags);
            }
            /// <summary>
            /// Return Public, nonpublic instance method of object
            /// </summary>
            /// <param name="objectToGetField"></param>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public static object GetInstanceFieldValue(object objectToGetField, string fieldName)
            {
                return objectToGetField.GetType().GetField(fieldName, instancePrivateFlags).GetValue(objectToGetField);
            }

            /// <summary>
            /// Return Public, nonpublic instance method of object
            /// </summary>
            /// <param name="objectToGetMethod"></param>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public static MethodInfo GetInstanceMethod(object objectToGetMethod, string methodName)
            {
                return objectToGetMethod.GetType().GetMethod(methodName, instancePrivateFlags);
            }

            public static Assembly GetAssembly(string assemblyName)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    if (assemblies[i].FullName == assemblyName)
                    {
                        return assemblies[i];
                    }
                }

                return null;
            }

            /// <summary>
            /// return true if succeed, else return false
            /// </summary>
            /// <param name="method"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static bool FitParametersToMethod(MethodInfo method, ref object[] parameters)
            {
                ParameterInfo[] paramInfos = method.GetParameters();

                if (parameters.Length > paramInfos.Length)
                {
                    UnityEngine.Debug.LogError("Too much params to fit the method");
                    return false;
                }
                else if (parameters.Length < paramInfos.Length)
                {
                    int start = parameters.Length;
                    System.Array.Resize(ref parameters, paramInfos.Length);

                    for (int i = start; i < paramInfos.Length; i++)
                    {
                        parameters[i] = paramInfos[i].DefaultValue;
                    }
                }

                return true;
            }

            public static void CallMethod(object objectToCallMethod, string methodName)
            {
                CallMethod(objectToCallMethod, methodName, null);

            }
            public static void CallMethod(object objectToCallMethod, string methodName, object parameter)
            {
                CallMethod(objectToCallMethod, methodName, new object[] { parameter });
            }
            public static void CallMethod(object objectToCallMethod, string methodName, object[] parameters)
            {
                MethodInfo method = GetInstanceMethod(objectToCallMethod, methodName);

                bool succeed = true;
                if (parameters != null)
                {
                    succeed = FitParametersToMethod(method, ref parameters);
                }

                if (succeed == false)
                {
                    Debug.LogError("Error with parameters");
                    return;
                }

                method.Invoke(objectToCallMethod, parameters);
            }
        }

        public static class Delegate
        {
            public class Action
            {
                public class PriorityMethod
                {
                    public System.Action Method { get; private set; }
                    public System.Action<GenericDataHolder> GenericDataMethod { get; private set; }
                    public int Priority { get; private set; }
                    public GenericDataHolder GenericData { get; private set; }
                    public bool HasGenericData => GenericData != null;

                    public PriorityMethod(System.Action method, int priority)
                    {
                        Method = method;
                        GenericDataMethod = null;
                        Priority = priority;
                        GenericData = null;
                    }

                    public PriorityMethod(System.Action<GenericDataHolder> method, GenericDataHolder genericData, int priority)
                    {
                        Method = null;
                        GenericDataMethod = method;
                        Priority = priority;
                        GenericData = genericData;
                    }
                }
                private static int currentId = 0;
                private bool _enabled = true;
                public void Disable() { _enabled = false; }
                public void Enable() { _enabled = true; }

                public List<PriorityMethod> toInvoke;

                public int ID;

                public Action()
                {
                    ID = currentId;
                    currentId++;

                    toInvoke = new List<PriorityMethod>();
                }

                /// <summary>
                /// Will ensure that only one instance of this method is subscribed
                /// </summary>
                /// <param name="method"></param>
                /// <param name="priority"></param>
                public void SafeSubscribe(System.Action method, int priority = 0)
                {
                    while (Unsubscribe(method)) { }
                    Subscribe(method, priority);
                }
                public void SafeSubscribe(System.Action<GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    while (Unsubscribe(method, data)) { }
                    Subscribe(method, data, priority);
                }

                public void Subscribe(System.Action method, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }
                public void Subscribe(System.Action<GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, data, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }

                /// <summary>
                /// Returns true if unsubscribed
                /// </summary>
                /// <param name="method"></param>
                /// <returns></returns>
                public bool Unsubscribe(System.Action method)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].Method == method)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public bool Unsubscribe(System.Action<GenericDataHolder> method, GenericDataHolder data)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].GenericDataMethod == method && toInvoke[i].GenericData == data)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public void Invoke()
                {
                    if (_enabled == false) { return; }
                    PriorityMethod[] array = toInvoke.ToArray();
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].HasGenericData == true)
                        {
                            array[i].GenericDataMethod.Invoke(array[i].GenericData);
                        }
                        else
                        {
                            array[i].Method.Invoke();
                        }
                    }
                }

                public static Action operator +(Action a, System.Action func)
                {
                    if (a == null) a = new Action();
                    a.Subscribe(func, 0);
                    return a;
                }
                public static Action operator -(Action a, System.Action func)
                {
                    if (a == null) a = new Action();
                    a.Unsubscribe(func);
                    return a;
                }
            }

            public class Action<T>
            {
                public class PriorityMethod
                {
                    public System.Action<T> Method { get; private set; }
                    public System.Action<T, GenericDataHolder> GenericDataMethod { get; private set; }
                    public int Priority { get; private set; }
                    public GenericDataHolder GenericData { get; private set; }
                    public bool HasGenericData => GenericData != null;

                    public PriorityMethod(System.Action<T> method, int priority)
                    {
                        Method = method;
                        GenericDataMethod = null;
                        Priority = priority;
                        GenericData = null;
                    }

                    public PriorityMethod(System.Action<T, GenericDataHolder> method, GenericDataHolder genericData, int priority)
                    {
                        Method = null;
                        GenericDataMethod = method;
                        Priority = priority;
                        GenericData = genericData;
                    }
                }
                private static int currentId = 0;
                private bool _enabled = true;
                public void Disable() { _enabled = false; }
                public void Enable() { _enabled = true; }

                public List<PriorityMethod> toInvoke;

                public int ID;

                public Action()
                {
                    ID = currentId;
                    currentId++;

                    toInvoke = new List<PriorityMethod>();
                }

                /// <summary>
                /// Will ensure that only one instance of this method is subscribed
                /// </summary>
                /// <param name="method"></param>
                /// <param name="priority"></param>
                public void SafeSubscribe(System.Action<T> method, int priority = 0)
                {
                    while (Unsubscribe(method)) { }
                    Subscribe(method, priority);
                }
                public void SafeSubscribe(System.Action<T, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    while (Unsubscribe(method, data)) { }
                    Subscribe(method, data, priority);
                }

                public void Subscribe(System.Action<T> method, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }
                public void Subscribe(System.Action<T, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, data, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }

                /// <summary>
                /// Returns true if unsubscribed
                /// </summary>
                /// <param name="method"></param>
                /// <returns></returns>
                public bool Unsubscribe(System.Action<T> method)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].Method == method)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public bool Unsubscribe(System.Action<T, GenericDataHolder> method, GenericDataHolder data)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].GenericDataMethod == method && toInvoke[i].GenericData == data)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public void Invoke(T param)
                {
                    if (_enabled == false) { return; }
                    PriorityMethod[] array = toInvoke.ToArray();
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].HasGenericData == true)
                        {
                            array[i].GenericDataMethod.Invoke(param, array[i].GenericData);
                        }
                        else
                        {
                            array[i].Method.Invoke(param);
                        }
                    }
                }
            }
            public class SelfAction<T> : Action<T>
            {
                public SelfAction(T self) : base()
                {
                    _self = self;
                }

                private T _self;
                public void SelfInvoke()
                {
                    Invoke(_self);
                }
            }

            public class Action<T, T2>
            {
                public class PriorityMethod
                {
                    public System.Action<T, T2> Method { get; private set; }
                    public System.Action<T, T2, GenericDataHolder> GenericDataMethod { get; private set; }
                    public int Priority { get; private set; }
                    public GenericDataHolder GenericData { get; private set; }
                    public bool HasGenericData => GenericData != null;

                    public PriorityMethod(System.Action<T, T2> method, int priority)
                    {
                        Method = method;
                        GenericDataMethod = null;
                        Priority = priority;
                        GenericData = null;
                    }

                    public PriorityMethod(System.Action<T, T2, GenericDataHolder> method, GenericDataHolder genericData, int priority)
                    {
                        Method = null;
                        GenericDataMethod = method;
                        Priority = priority;
                        GenericData = genericData;
                    }
                }
                private static int currentId = 0;
                private bool _enabled = true;
                public void Disable() { _enabled = false; }
                public void Enable() { _enabled = true; }

                public List<PriorityMethod> toInvoke;

                public int ID;

                public Action()
                {
                    ID = currentId;
                    currentId++;

                    toInvoke = new List<PriorityMethod>();
                }

                /// <summary>
                /// Will ensure that only one instance of this method is subscribed
                /// </summary>
                /// <param name="method"></param>
                /// <param name="priority"></param>
                public void SafeSubscribe(System.Action<T, T2> method, int priority = 0)
                {
                    while (Unsubscribe(method)) { }
                    Subscribe(method, priority);
                }
                public void SafeSubscribe(System.Action<T, T2, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    while (Unsubscribe(method, data)) { }
                    Subscribe(method, data, priority);
                }

                public void Subscribe(System.Action<T, T2> method, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }
                public void Subscribe(System.Action<T, T2, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, data, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }

                /// <summary>
                /// Returns true if unsubscribed
                /// </summary>
                /// <param name="method"></param>
                /// <returns></returns>
                public bool Unsubscribe(System.Action<T, T2> method)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].Method == method)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public bool Unsubscribe(System.Action<T, T2, GenericDataHolder> method, GenericDataHolder data)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].GenericDataMethod == method && toInvoke[i].GenericData == data)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public void Invoke(T param, T2 param2)
                {
                    if (_enabled == false) { return; }
                    PriorityMethod[] array = toInvoke.ToArray();
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].HasGenericData == true)
                        {
                            array[i].GenericDataMethod.Invoke(param, param2, array[i].GenericData);
                        }
                        else
                        {
                            array[i].Method.Invoke(param, param2);
                        }
                    }
                }


			}
            public class SelfAction<T, T2> : Action<T, T2>
            {
                public SelfAction(T self) : base()
                {
                    _self = self;
                }

                private T _self;
                public void SelfInvoke(T2 param2)
                {
                    Invoke(_self, param2);
                }
            }

            public class Action<T, T2, T3>
            {
                public class PriorityMethod
                {
                    public System.Action<T, T2, T3> Method { get; private set; }
                    public System.Action<T, T2, T3, GenericDataHolder> GenericDataMethod { get; private set; }
                    public int Priority { get; private set; }
                    public GenericDataHolder GenericData { get; private set; }
                    public bool HasGenericData => GenericData != null;

                    public PriorityMethod(System.Action<T, T2, T3> method, int priority)
                    {
                        Method = method;
                        GenericDataMethod = null;
                        Priority = priority;
                        GenericData = null;
                    }

                    public PriorityMethod(System.Action<T, T2, T3, GenericDataHolder> method, GenericDataHolder genericData, int priority)
                    {
                        Method = null;
                        GenericDataMethod = method;
                        Priority = priority;
                        GenericData = genericData;
                    }
                }
                private static int currentId = 0;
                private bool _enabled = true;
                public void Disable() { _enabled = false; }
                public void Enable() { _enabled = true; }

                public List<PriorityMethod> toInvoke;

                public int ID;

                public Action()
                {
                    ID = currentId;
                    currentId++;

                    toInvoke = new List<PriorityMethod>();
                }

                /// <summary>
                /// Will ensure that only one instance of this method is subscribed
                /// </summary>
                /// <param name="method"></param>
                /// <param name="priority"></param>
                public void SafeSubscribe(System.Action<T, T2, T3> method, int priority = 0)
                {
                    while (Unsubscribe(method)) { }
                    Subscribe(method, priority);
                }
                public void SafeSubscribe(System.Action<T, T2, T3, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    while (Unsubscribe(method, data)) { }
                    Subscribe(method, data, priority);
                }

                public void Subscribe(System.Action<T, T2, T3> method, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }
                public void Subscribe(System.Action<T, T2, T3, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, data, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }

                /// <summary>
                /// Returns true if unsubscribed
                /// </summary>
                /// <param name="method"></param>
                /// <returns></returns>
                public bool Unsubscribe(System.Action<T, T2, T3> method)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].Method == method)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public bool Unsubscribe(System.Action<T, T2, T3, GenericDataHolder> method, GenericDataHolder data)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].GenericDataMethod == method && toInvoke[i].GenericData == data)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public void Invoke(T param, T2 param2, T3 param3)
                {
                    if (_enabled == false) { return; }
                    PriorityMethod[] array = toInvoke.ToArray();
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].HasGenericData == true)
                        {
                            array[i].GenericDataMethod.Invoke(param, param2, param3, array[i].GenericData);
                        }
                        else
                        {
                            array[i].Method.Invoke(param, param2, param3);
                        }
                    }
                }
            }
            public class SelfAction<T, T2, T3> : Action<T, T2, T3>
            {
                public SelfAction(T self) : base()
                {
                    _self = self;
                }

                private T _self;
                public void SelfInvoke(T2 param2, T3 param3)
                {
                    Invoke(_self, param2, param3);
                }
            }

            public class Action<T, T2, T3, T4>
            {
                public class PriorityMethod
                {
                    public System.Action<T, T2, T3, T4> Method { get; private set; }
                    public System.Action<T, T2, T3, T4, GenericDataHolder> GenericDataMethod { get; private set; }
                    public int Priority { get; private set; }
                    public GenericDataHolder GenericData { get; private set; }
                    public bool HasGenericData => GenericData != null;

                    public PriorityMethod(System.Action<T, T2, T3, T4> method, int priority)
                    {
                        Method = method;
                        GenericDataMethod = null;
                        Priority = priority;
                        GenericData = null;
                    }

                    public PriorityMethod(System.Action<T, T2, T3, T4, GenericDataHolder> method, GenericDataHolder genericData, int priority)
                    {
                        Method = null;
                        GenericDataMethod = method;
                        Priority = priority;
                        GenericData = genericData;
                    }
                }
                private static int currentId = 0;
                private bool _enabled = true;
                public void Disable() { _enabled = false; }
                public void Enable() { _enabled = true; }

                public List<PriorityMethod> toInvoke;

                public int ID;

                public Action()
                {
                    ID = currentId;
                    currentId++;

                    toInvoke = new List<PriorityMethod>();
                }

                /// <summary>
                /// Will ensure that only one instance of this method is subscribed
                /// </summary>
                /// <param name="method"></param>
                /// <param name="priority"></param>
                public void SafeSubscribe(System.Action<T, T2, T3, T4> method, int priority = 0)
                {
                    while (Unsubscribe(method)) { }
                    Subscribe(method, priority);
                }
                public void SafeSubscribe(System.Action<T, T2, T3, T4, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    while (Unsubscribe(method, data)) { }
                    Subscribe(method, data, priority);
                }

                public void Subscribe(System.Action<T, T2, T3, T4> method, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }
                public void Subscribe(System.Action<T, T2, T3, T4, GenericDataHolder> method, GenericDataHolder data, int priority = 0)
                {
                    PriorityMethod newMethod = new PriorityMethod(method, data, priority);

                    if (toInvoke.Count == 0)
                    {
                        toInvoke.Add(newMethod);
                    }
                    else
                    {
                        for (int i = toInvoke.Count - 1; i >= 0; i--)
                        {
                            if (priority > toInvoke[i].Priority)
                            {
                                toInvoke.Insert(i + 1, newMethod);
                                break;
                            }
                            else if (i == 0)
                            {
                                toInvoke.Insert(0, newMethod);
                                break;
                            }
                        }
                    }

                }

                /// <summary>
                /// Returns true if unsubscribed
                /// </summary>
                /// <param name="method"></param>
                /// <returns></returns>
                public bool Unsubscribe(System.Action<T, T2, T3, T4> method)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].Method == method)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public bool Unsubscribe(System.Action<T, T2, T3, T4, GenericDataHolder> method, GenericDataHolder data)
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        if (toInvoke[i].GenericDataMethod == method && toInvoke[i].GenericData == data)
                        {
                            toInvoke.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }
                public void Invoke(T param, T2 param2, T3 param3, T4 param4)
                {
                    if (_enabled == false) { return; }
                    PriorityMethod[] array = toInvoke.ToArray();
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].HasGenericData == true)
                        {
                            array[i].GenericDataMethod.Invoke(param, param2, param3, param4, array[i].GenericData);
                        }
                        else
                        {
                            array[i].Method.Invoke(param, param2, param3, param4);
                        }
                    }
                }
            }
            public class SelfAction<T, T2, T3, T4> : Action<T, T2, T3, T4>
            {
                public SelfAction(T self) : base()
                {
                    _self = self;
                }

                private T _self;
                public void SelfInvoke(T2 param2, T3 param3, T4 param4)
                {
                    Invoke(_self, param2, param3, param4);
                }
            }

            public class GenericAction
            {
                public enum ActionType
                {
                    Zero,
                    One,
                    Two,
                    Three,
                    Four
                }

                public ActionType Type { get; private set; }
                public object Action { get; private set; }
                public object[] Parameters { get; private set; }

                public GenericAction SetAction(System.Action act)
                {
                    Type = ActionType.Zero;
                    Action = act;
                    Parameters = null;
                    return this;
                }
                public GenericAction SetAction<T>(System.Action<T> act, T param)
                {
                    Type = ActionType.One;
                    Action = act;
                    Parameters = new object[] { param };
                    return this;
                }
                public GenericAction SetAction<T, T2>(System.Action<T, T2> act, T param1, T2 param2)
                {
                    Type = ActionType.Two;
                    Action = act;
                    Parameters = new object[] { param1, param2 };
                    return this;
                }
                public GenericAction SetAction<T, T2, T3>(System.Action<T, T2, T3> act, T param1, T2 param2, T3 param3)
                {
                    Type = ActionType.Three;
                    Action = act;
                    Parameters = new object[] { param1, param2, param3 };
                    return this;
                }
                public GenericAction SetAction<T, T2, T3, T4>(System.Action<T, T2, T3, T4> act, T param1, T2 param2, T3 param3, T4 param4)
                {
                    Type = ActionType.Four;
                    Action = act;
                    Parameters = new object[] { param1, param2, param3, param4 };
                    return this;
                }

                public void Invoke()
                {
                    Action.GetType().GetMethod("Invoke").Invoke(Action, Parameters);
                }
            }
        }

        public static class List
        {
            public static void RemoveAllNull<T>(ref List<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] == null || list[i].Equals(null))
                    {
                        list.RemoveAt(i);
                    }

                }
            }
            public static void RemoveAllInactive<T>(ref List<T> list) where T : UnityEngine.MonoBehaviour
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].gameObject.activeSelf == false)
                    {
                        list.RemoveAt(i);
                    }

                }
            }

        }

        public static class Array
        {
            public static void RemoveAllNull<T>(ref T[] array) where T : class
            {
                while (ArrayContainsNull(ref array))
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] == null || array[i].Equals(null))
                        {
                            for (int j = i + 1; j < array.Length; j++)
                            {
                                array[j - 1] = array[j];
                            }

                            System.Array.Resize(ref array, array.Length - 1);
                            break;
                        }
                    }
                }
            }

            private static bool ArrayContainsNull<T>(ref T[] array) where T : class
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == null || array[i].Equals(null))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static class Color
        {
            /// <summary>
            /// Will make T equal to one if used in editor
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            public static UnityEngine.Color Lerp(UnityEngine.Color a, UnityEngine.Color b, float t)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying == false)
                {
                    t = 1;
                }
#endif
                return UnityEngine.Color.Lerp(a, b, t);
            }

            public static UnityEngine.Color MoveToward(UnityEngine.Color current, UnityEngine.Color target, float amount)
            {
                float rDist = target.r - current.r;
                float gDist = target.g - current.g;
                float bDist = target.b - current.b;
                float aDist = target.a - current.a;
                float sqrMagn = rDist * rDist + gDist * gDist + bDist * bDist + aDist * aDist;
                if (sqrMagn != 0 && (amount < 0 || sqrMagn > amount * amount))
                {
                    float magn = (float)Math.Sqrt(sqrMagn);
                    return new Vector4(
                        current.r + ((rDist / magn) * amount),
                        current.g + ((gDist / magn) * amount),
                        current.b + ((bDist / magn) * amount),
                        current.a + ((aDist / magn) * amount));
                }
                else
                {
                    return target;
                }
            }
        }

        public static class Material
        {
            public static UnityEngine.Material CreateTransparentFrom(UnityEngine.Material material, UnityEngine.Material transparentMatModel)
            {
                UnityEngine.Material mat = new UnityEngine.Material(transparentMatModel);

                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromTex = ((m, m2, paramName) =>
                {
                    m.SetTexture(paramName, m2.GetTexture(paramName));
                });
                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromFloat = ((m, m2, paramName) =>
                {
                    m.SetFloat(paramName, m2.GetFloat(paramName));
                });
                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromColor = ((m, m2, paramName) =>
                {
                    m.SetColor(paramName, m2.GetColor(paramName));
                });

                mat.color = material.color;
                CopyFromTex(mat, material, "_BumpMap");
                CopyFromTex(mat, material, "_DetailAlbedoMap");
                CopyFromTex(mat, material, "_Detailmask");
                CopyFromTex(mat, material, "_DetailNormalMap");
                CopyFromTex(mat, material, "_Emissionmap");
                CopyFromTex(mat, material, "_MainTex");
                CopyFromTex(mat, material, "_MetallicGlossMap");
                CopyFromTex(mat, material, "_OcclusionMap");
                CopyFromTex(mat, material, "_ParallaxMap");

                CopyFromFloat(mat, material, "_BumpScale");
                CopyFromFloat(mat, material, "_Cutoff");
                CopyFromFloat(mat, material, "_DetailNormalMapScale");
                CopyFromFloat(mat, material, "_DstBlend");
                CopyFromFloat(mat, material, "_GlossMapScale");
                CopyFromFloat(mat, material, "_Glossiness");
                CopyFromFloat(mat, material, "_GlossyReflections");
                CopyFromFloat(mat, material, "_Metallic");
                CopyFromFloat(mat, material, "_Mode");
                CopyFromFloat(mat, material, "_OcclusionStrength");
                CopyFromFloat(mat, material, "_Parallax");
                CopyFromFloat(mat, material, "_SmoothnessTextureChannel");
                CopyFromFloat(mat, material, "_SpecularHighlights");
                CopyFromFloat(mat, material, "_SrcBlend");
                CopyFromFloat(mat, material, "_UVSec");
                CopyFromFloat(mat, material, "_ZWrite");

                CopyFromColor(mat, material, "_Color");
                CopyFromColor(mat, material, "_EmissionColor");

                return mat;
            }
            /// <summary>
            /// Will try to automatically load Resources(Materials/TransparentMaterial) for model
            /// </summary>
            /// <param name="material"></param>
            /// <returns></returns>
            public static UnityEngine.Material CreateTransparentFrom(UnityEngine.Material material)
            {
                UnityEngine.Material mat = Resources.Load<UnityEngine.Material>("Materials/TransparentMaterial");
                mat.CopyPropertiesFromMaterial(material);

                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromTex = ((m, m2, paramName) =>
                {
                    m.SetTexture(paramName, m2.GetTexture(paramName));
                });
                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromFloat = ((m, m2, paramName) =>
                {
                    m.SetFloat(paramName, m2.GetFloat(paramName));
                });
                System.Action<UnityEngine.Material, UnityEngine.Material, string> CopyFromColor = ((m, m2, paramName) =>
                {
                    m.SetColor(paramName, m2.GetColor(paramName));
                });

                mat.color = material.color;
                CopyFromTex(mat, material, "_BumpMap");
                CopyFromTex(mat, material, "_DetailAlbedoMap");
                CopyFromTex(mat, material, "_Detailmask");
                CopyFromTex(mat, material, "_DetailNormalMap");
                CopyFromTex(mat, material, "_Emissionmap");
                CopyFromTex(mat, material, "_MainTex");
                CopyFromTex(mat, material, "_MetallicGlossMap");
                CopyFromTex(mat, material, "_OcclusionMap");
                CopyFromTex(mat, material, "_ParallaxMap");

                CopyFromFloat(mat, material, "_BumpScale");
                CopyFromFloat(mat, material, "_Cutoff");
                CopyFromFloat(mat, material, "_DetailNormalMapScale");
                CopyFromFloat(mat, material, "_DstBlend");
                CopyFromFloat(mat, material, "_GlossMapScale");
                CopyFromFloat(mat, material, "_Glossiness");
                CopyFromFloat(mat, material, "_GlossyReflections");
                CopyFromFloat(mat, material, "_Metallic");
                CopyFromFloat(mat, material, "_Mode");
                CopyFromFloat(mat, material, "_OcclusionStrength");
                CopyFromFloat(mat, material, "_Parallax");
                CopyFromFloat(mat, material, "_SmoothnessTextureChannel");
                CopyFromFloat(mat, material, "_SpecularHighlights");
                CopyFromFloat(mat, material, "_SrcBlend");
                CopyFromFloat(mat, material, "_UVSec");
                CopyFromFloat(mat, material, "_ZWrite");

                CopyFromColor(mat, material, "_Color");
                CopyFromColor(mat, material, "_EmissionColor");

                return mat;
            }

            public static void ReplaceAllMatsWithTransparents(MeshRenderer meshRenderer)
            {
                UnityEngine.Material[] mats = meshRenderer.materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    //mats[j] = CreateTransparentFrom(mats[j]);
                }

                meshRenderer.materials = mats;
            }
        }

        public static class Comparison
        {
            public static T GetNearest<T>(Vector3 pos, List<T> _list) where T : PinouBehaviour
            {
                if (_list == null || _list.Count <= 0)
                {
                    return null;
                }

                float minDist = Mathf.Infinity;
                float curDist;
                int choosenIndex = -1;
                for (int i = 0; i < _list.Count; i++)
                {
                    curDist = (_list[i].Position - pos).sqrMagnitude;
                    if (curDist < minDist)
                    {
                        choosenIndex = i;
                        minDist = curDist;
                    }
                }
                if (choosenIndex < 0)
                {
                    return null;
                }

                return _list[choosenIndex];
            }
            public static T GetNearest<T>(Vector3 pos, T[] _array) where T : PinouBehaviour
            {
                if (_array == null || _array.Length <= 0)
                {
                    return null;
                }

                float minDist = Mathf.Infinity;
                float curDist;
                int choosenIndex = -1;
                for (int i = 0; i < _array.Length; i++)
                {
                    curDist = (_array[i].Position - pos).sqrMagnitude;
                    if (curDist < minDist)
                    {
                        choosenIndex = i;
                        minDist = curDist;
                    }
                }
                if (choosenIndex < 0)
                {
                    return null;
                }

                return _array[choosenIndex];
            }
            public static T GetNearestInAngle<T>(Vector3 pos, Vector3 forward, List<T> _list, float maxAngle = 90f, bool ignoreY = true) where T : PinouBehaviour
            {
                if (_list == null || _list.Count <= 0)
                {
                    return null;
                }

                float minDist = Mathf.Infinity;
                float curDist;
                int choosenIndex = -1;
                for (int i = 0; i < _list.Count; i++)
                {
                    Vector3 tarPos = ignoreY ? _list[i].Position.SetY(pos.y) : _list[i].Position;
                    curDist = Vector3.Angle(forward, (tarPos - pos).normalized);
                    if (curDist < minDist && curDist <= maxAngle)
                    {
                        choosenIndex = i;
                        minDist = curDist;
                    }
                }
                if (choosenIndex < 0)
                {
                    return null;
                }

                return _list[choosenIndex];
            }
            public static T GetNearestInAngle<T>(Vector3 pos, Vector3 forward, T[] _array, float maxAngle = 90f, bool ignoreY = true) where T : PinouBehaviour
            {
                if (_array == null || _array.Length <= 0)
                {
                    return null;
                }

                float minDist = Mathf.Infinity;
                float curDist;
                int choosenIndex = -1;
                for (int i = 0; i < _array.Length; i++)
                {
                    Vector3 tarPos = ignoreY ? _array[i].Position.SetY(pos.y) : _array[i].Position;
                    curDist = Vector3.Angle(forward, (tarPos - pos));
                    if (curDist < minDist && curDist <= maxAngle)
                    {
                        choosenIndex = i;
                        minDist = curDist;
                    }
                }
                if (choosenIndex < 0)
                {
                    return null;
                }

                return _array[choosenIndex];
            }
        }

        public static class Entity
        {
            public static T[] DetectEntitiesInSphere<T>(Vector3 center, float radius, ICollection<T> entitiesToSkip = null) where T : EntitySystem.Entity
            {
                Collider[] colls = UnityEngine.Physics.OverlapSphere(center, radius, PinouApp.Resources.Data.Layers.Entities);
                List<T> ents = new List<T>();

                for (int i = 0; i < colls.Length; i++)
                {
                    T ent = colls[i].GetComponentInParent<T>();
                    if (ent == null ||
                        ents.Contains(ent) ||
                        (entitiesToSkip != null && entitiesToSkip.Contains(ent))) 
                    { 
                        continue;
                    }
                    ents.Add(ent);
                }

                return ents.ToArray();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Context Only
        /// </summary>
        [InitializeOnLoad]
        public static class Editor
        {
            static Editor()
            {
                EditorApplication.playModeStateChanged += OnEditorChangeState;
                EditorSceneManager.sceneOpened += OnSceneOpened;
                CustomEditorBuildProcessor.ES_OnPreProcessBuild += ES_OnPreProcessBuild;
                CustomEditorBuildProcessor.ES_OnPostProcessBuild += ES_OnPostProcessBuild;
            }

            public static bool ScriptsReloaded { get { return (_scriptsReloaded || EditorApplication.isPlaying) && _processing == false; } }
            public static bool IsExitingPlayMode { get => _isQuittingPlayMode; }

            private static bool _isQuittingPlayMode;
            private static bool _scriptsReloaded;
            private static bool _processing = false;

            #region Events
            public static event Action OnEditorEnterPlayMode;
            public static event Action OnEditorExitPlayMode;
            public static event Action OnEditorEnterEditMode;
            public static event Action OnEditorExitEditMode;
            public static event Action OnEditorOpenScene;
            public static event Action OnReloadScripts;
            public static event Action OnPreprocessBuild;
            public static event Action OnPostprocessBuild;

            [UnityEditor.Callbacks.DidReloadScripts]
            private static void S_OnReloadScripts()
            {
                if (_processing == true)
                {
                    return;
                }
                _scriptsReloaded = true;
                OnReloadScripts?.Invoke();
            }

            private static void OnEditorChangeState(PlayModeStateChange playModeState)
            {
                _scriptsReloaded = false;

                switch (playModeState)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        OnEditorEnterEditMode?.Invoke();
                        S_OnReloadScripts();
                        _isQuittingPlayMode = false;
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                        OnEditorExitEditMode?.Invoke();
                        break;
                    case PlayModeStateChange.EnteredPlayMode:
                        OnEditorEnterPlayMode?.Invoke();
                        break;
                    case PlayModeStateChange.ExitingPlayMode:
                        OnEditorExitPlayMode?.Invoke();
                        _isQuittingPlayMode = true;
                        break;
                    default:
                        break;
                }
            }

            private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
            {
                OnEditorOpenScene?.Invoke();
                S_OnReloadScripts();
            }

            private static void ES_OnPreProcessBuild()
            {
                _processing = true;
                OnPreprocessBuild?.Invoke();
            }
            private static void ES_OnPostProcessBuild()
            {
                _processing = false;
                OnPostprocessBuild?.Invoke();
            }

            #endregion

            #region Icon / Label
            public enum LabelIcon
            {
                Gray = 0,
                Blue,
                Teal,
                Green,
                Yellow,
                Orange,
                Red,
                Purple
            }

            public enum Icon
            {
                CircleGray = 0,
                CircleBlue,
                CircleTeal,
                CircleGreen,
                CircleYellow,
                CircleOrange,
                CircleRed,
                CirclePurple,
                DiamondGray,
                DiamondBlue,
                DiamondTeal,
                DiamondGreen,
                DiamondYellow,
                DiamondOrange,
                DiamondRed,
                DiamondPurple
            }

            private static GUIContent[] labelIcons;
            private static GUIContent[] largeIcons;

            public static void SetIcon(UnityEngine.GameObject gObj, LabelIcon icon)
            {
                if (labelIcons == null)
                {
                    labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
                }

                SetIcon(gObj, labelIcons[(int)icon].image as Texture2D);
            }

            public static void SetIcon(UnityEngine.GameObject gObj, Icon icon)
            {
                if (largeIcons == null)
                {
                    largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
                }

                SetIcon(gObj, largeIcons[(int)icon].image as Texture2D);
            }

            public static void RemoveIcon(UnityEngine.GameObject gObj)
            {
                SetIcon(gObj, null);
            }

            private static void SetIcon(UnityEngine.GameObject gObj, Texture2D texture)
            {
                Type ty = typeof(EditorGUIUtility);
                MethodInfo mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
                mi.Invoke(null, new object[] { gObj, texture });
            }

            private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
            {
                GUIContent[] array = new GUIContent[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
                }
                return array;
            }
            #endregion
        }

        public class CustomEditorBuildProcessor : UnityEditor.Build.IPreprocessBuildWithReport
        {
            public static Action ES_OnPreProcessBuild;
            public static Action ES_OnPostProcessBuild;

            public static bool Building { get; private set; }

            public int callbackOrder { get { return 0; } }
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                ES_OnPreProcessBuild?.Invoke();
                Debug.Log("azhiafz");
            }

            public void OnPreprocessBuild(BuildReport report)
            {
                Building = true;
                ES_OnPreProcessBuild?.Invoke();
            }

            [PostProcessBuildAttribute(1)]
            public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
            {
                ES_OnPostProcessBuild?.Invoke();
                Building = false;
                Debug.Log(pathToBuiltProject);
            }
        }
#endif
    }
}