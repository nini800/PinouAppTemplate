using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
    public class PinouSingleton<T> : PinouBehaviour where T : PinouSingleton<T>
    {
        public static bool InstanceSet { get { return Instance != null; } }
        public static T Instance
        {
            get
            {
                if (__instance == null)
                {
                    __instance = PinouUtils.GameObject.FindObjectOfType<T>();
                    if (__instance == null)
                    {
                        return null;
                    }
                }
                return __instance;
            }
        }
        private static T __instance;

        /// <summary>
        /// Need base
        /// </summary>
        protected override void OnAwake()
        {
            SetInstance((T)this);
        }

        protected static void SetInstance(T instance)
        {
            __instance = instance;
        }
    }
}
