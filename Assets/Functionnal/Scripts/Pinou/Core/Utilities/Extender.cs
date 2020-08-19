using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
    public static class Extender
    {
        #region float
        public static float SubtractAbsolute(this float f, float amount)
        {
            bool signed = f < 0;
            f = Mathf.Abs(f);
            if (f <= amount)
            {
                return 0f;
            }
            else
            {
                return (f - amount) * (signed ? -1 : 1);
            }
        }
        public static float Abs(this float f)
        {
            return f < 0 ? -f : f;
        }
        public static float Pow(this float f, float p)
        {
            return Mathf.Pow(f, p);
        }
        public static float MoveToward(this float f, float target, float amount)
        {
            return Mathf.MoveTowards(f, target, amount);
        }
        public static float OneMinus(this float f)
        {
            return 1 - f;
        }
        #endregion

        #region Vector 3
        public static Vector3 SetX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }
        public static Vector3 SetY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }
        public static Vector3 SetZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }
        public static Vector3 AddX(this Vector3 v, float x)
        {
            v.x += x;
            return v;
        }
        public static Vector3 AddY(this Vector3 v, float y)
        {
            v.y += y;
            return v;
        }
        public static Vector3 AddZ(this Vector3 v, float z)
        {
            v.z += z;
            return v;
        }
        public static Vector3 SwapXY(this Vector3 v)
        {
            float temp = v.x;
            v.x = v.y;
            v.y = temp;
            return v;
        }
        public static Vector3 SwapXZ(this Vector3 v)
        {
            float temp = v.x;
            v.x = v.z;
            v.z = temp;
            return v;
        }
        public static Vector3 SwapYZ(this Vector3 v)
        {
            float temp = v.y;
            v.y = v.z;
            v.z = temp;
            return v;
        }
        public static Vector2 ToV2(this Vector3 v, bool Z_AxisIs_Y_Axis = false)
        {
            return new Vector2(v.x, Z_AxisIs_Y_Axis ? v.z : v.y);
        }
        public static Vector3 Round(this Vector3 v)
        {
            v.x = Mathf.Round(v.x);
            v.y = Mathf.Round(v.y);
            v.z = Mathf.Round(v.z);
            return v;
        }
        public static Vector3 Floor(this Vector3 v)
        {
            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return v;
        }
        public static Vector3Int RoundToInt(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z));
        }
        public static Vector3Int FloorToInt(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y),
                Mathf.FloorToInt(v.z));
        }
        public static Vector3 ToV3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static float Heuristic(this Vector3 v)
        {
            float value = v.x + v.y + v.z;
            if (value < 0)
                value = -value;
            return value;
        }


        /// <summary>
        /// Does not touch Y axis
        /// </summary>
        /// <param name="v"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Vector3 SubtractMagnitudeHorizontal(this Vector3 v, float amount)
        {
            float y = v.y;
            v.y = 0;
            v = v.SubtractMagnitude(amount);
            v.y = y;
            return v;
        }
        public static Vector3 SubtractMagnitude(this Vector3 v, float amount)
        {
            //Store magntiude
            float magnitude = v.magnitude;
            //if magnitude is 0
            if (magnitude == 0f)
            {
                return v;
            }
            //Normalize vector
            v /= magnitude;
            //Calculate new magnitude
            magnitude = Mathf.Clamp(magnitude - amount, 0f, Mathf.Infinity);
            //Return vector with correct magnitude
            return v * magnitude;
        }
        public static bool IsApproxZero(this Vector3 v)
        {
            return (Mathf.Approximately(v.x, 0f) && Mathf.Approximately(v.y, 0f) && Mathf.Approximately(v.z, 0f));
        }

        /// <summary>
        /// Will consider this vector as a (0,0,0) euler angles and rotates it to Rotation provided
        /// </summary>
        /// <param name="v"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 Transform(this Vector3 v, Quaternion rotation)
        {
            return PinouUtils.Transform.TransformVector(v, rotation);
        }
        /// <summary>
        /// Will consider this vector as a (0,0,0) euler angles and rotates it to Rotation provided around position
        /// </summary>
        /// <param name="v"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(this Vector3 v, Vector3 position, Quaternion rotation)
        {
            return PinouUtils.Transform.TransformPoint(v, position, rotation);
        }

        public static Vector3 ClampMagnitude(this Vector3 v, float maxMagnitude)
        {
            float sqrMagn = v.sqrMagnitude;
            if (sqrMagn > maxMagnitude * maxMagnitude)
            {
                return v.normalized * maxMagnitude;
            }
            return v;
        }
        #endregion

        #region Vector2
        public static Vector2 SetX(this Vector2 v, float x)
        {
            v.x = x;
            return v;
        }
        public static Vector2 SetY(this Vector2 v, float y)
        {
            v.y = y;
            return v;
        }
        public static Vector2 AddX(this Vector2 v, float x)
        {
            v.x += x;
            return v;
        }
        public static Vector2 AddY(this Vector2 v, float y)
        {
            v.y += y;
            return v;
        }
        public static Vector2 Add(this Vector2 v, Vector2 v2)
        {
            v.x += v2.x;
            v.y += v2.y;
            return v;
        }
        public static Vector2 Add(this Vector2 v, Vector3 v3, bool useZforY = false)
        {
            v.x += v3.x;
            if (useZforY == true)
            {
                v.y += v3.z;
            }
            else
            {
                v.y += v3.y;
            }
            return v;
        }

        public static Vector2 SwapXY(this Vector2 v)
        {
            float temp = v.x;
            v.x = v.y;
            v.y = temp;
            return v;
        }
        public static Vector3 ToV3(this Vector2 v, float zValue = 0)
        {
            return new Vector3(v.x, v.y, zValue);
        }
        public static Vector3 ToV3_YisZ(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }
        public static Vector2Int RoundToInt(this Vector2 v)
        {
            return new Vector2Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y));
        }
        public static Vector2Int FloorToInt(this Vector2 v)
        {
            return new Vector2Int(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y));
        }

        public static Vector2 Lerp(this Vector2 v, Vector2 target, float t)
        {
            return Vector2.Lerp(v, target, t);
        }
        public static Vector2 MoveTowards(this Vector2 v, Vector2 target, float step, bool deltaTime = false, bool scaled = true)
        {
            return Vector2.MoveTowards(v, target, step * (deltaTime ? (scaled ? Time.deltaTime : Time.unscaledDeltaTime) : 1f));
        }
        public static Vector2 ClampMagnitude(this Vector2 v, float maxMagnitude)
        {
            float sqrMagn = v.sqrMagnitude;
            if (sqrMagn > maxMagnitude * maxMagnitude)
            {
                return v.normalized * maxMagnitude;
            }
            return v;
        }
        #endregion

        #region Colors
        public static Color SetA(this Color c, float a)
        {
            c.a = a;
            return c;
        }
        public static Color SetR(this Color c, float r)
        {
            c.r = r;
            return c;
        }
        public static Color SetG(this Color c, float g)
        {
            c.g = g;
            return c;
        }
        public static Color SetB(this Color c, float b)
        {
            c.b = b;
            return c;
        }
        public static Color AddA(this Color c, float a)
        {
            c.a += a;
            return c;
        }
        public static Color AddR(this Color c, float r)
        {
            c.r += r;
            return c;
        }
        public static Color AddG(this Color c, float g)
        {
            c.g += g;
            return c;
        }
        public static Color AddB(this Color c, float b)
        {
            c.b += b;
            return c;
        }
        public static Color MoveTowards(this Color c, Color t, float amount)
        {
            return PinouUtils.Color.MoveToward(c, t, amount);
        }
        #endregion

        #region Array
        public static void ForEach<T>(this T[] array, System.Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action.Invoke(array[i]);
            }
        }
        public static TT[] ConvertAll<T, TT>(this T[] array) where TT : T
        {
            return Array.ConvertAll(array, item => (TT)item);
        }
        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (item.Equals(array[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        public static bool Contains<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (item.Equals(array[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static void SetAllValues<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        public static T Last<T>(this T[] array)
        {
            return array[array.Length - 1];
        }
        public static T Random<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        #endregion

        #region List
        public static bool ContainsAny<T>(this List<T> list, List<T> list2)
        {
            for (int i = 0; i < list2.Count; i++)
            {
                if (list.Contains(list2[i]) == true)
                {
                    return true;
                }
            }
            return false;
        }
        public static T Last<T>(this List<T> list)
        {
            if (list.Count <= 0)
            {
                return default;
            }

            return list[list.Count - 1];
        }
        public static T Random<T>(this List<T> list)
        {
            if (list.Count <= 0)
            {
                return default;
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        #endregion

        #region Transform
        public static Transform FindRecursive(this Transform t, string name)
        {
            return PinouUtils.Transform.FindRecursive(t, name);
        }
        public static Transform[] AllChilds(this Transform t)
        {
            return PinouUtils.Transform.AllChilds(t);
        }
        #endregion

        #region Object
        public static void Destroy(this UnityEngine.Object o, float time = 0)
        {
            if (o is Transform)
            {
                o = ((Transform)o).gameObject;
            }
            if (time <= 0)
            {
                UnityEngine.Object.Destroy(o);
            }
            else
            {
                UnityEngine.Object.Destroy(o, time);
            }
        }
        #endregion

        #region Materials
        public static void MoveEmissiveTowards(this UnityEngine.Material mat, Color targetEmissive, float step)
        {
            mat.SetColor("_EmissionColor", mat.GetColor("_EmissionColor").MoveTowards(targetEmissive, step));
        }
        public static void LerpEmissive(this UnityEngine.Material mat, Color targetEmissive, float t)
        {
            mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), targetEmissive, t));
        }
        public static void SetEmissive(this UnityEngine.Material mat, Color emissive)
        {
            mat.SetColor("_EmissionColor", emissive);
        }
        public static void LerpEmissive(this UnityEngine.Material mat, Color startEmissive, Color targetEmissive, float t)
        {
            mat.SetColor("_EmissionColor", Color.Lerp(startEmissive, targetEmissive, t));
        }
        #endregion

    }
}