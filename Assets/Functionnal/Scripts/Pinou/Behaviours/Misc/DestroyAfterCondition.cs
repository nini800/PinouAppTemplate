using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou
{

    public class DestroyAfterCondition : PinouBehaviour
    {
        public enum DestroyCondition
        {
            Time
        }

        [SerializeField, HideInInspector] private DestroyCondition _condition;
        [SerializeField, HideInInspector] private float _destroyTime = 1f;

		protected override void OnAwake()
		{
            StartCoroutine(TimedDestroyCoroutine());
        }

        private IEnumerator TimedDestroyCoroutine()
        {
            yield return new WaitForSeconds(_destroyTime);
            Destroy(gameObject);
        }

        public DestroyCondition Condition { get => _condition; set => _condition = value; }
        public float DestroyTime { get => _destroyTime; set => _destroyTime = value; }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DestroyAfterCondition)), CanEditMultipleObjects]
    public class DestroyAfterConditionEditor : PinouEditor
    {
        protected override void InspectorGUI()
        {
            base.OnInspectorGUI();
            PropField("_condition");

            switch (((DestroyAfterCondition)target).Condition)
            {
                case DestroyAfterCondition.DestroyCondition.Time:
                    PropField("_destroyTime");
                    break;
            }
        }
    }
#endif

}