using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{

    public class RelativeToParentPosition : PinouBehaviour
    {
        public Vector3 AbsoluteOffset;
        public Vector3 RelativeOffset;

        private new Vector3 Position { get { return transform.parent.position + AbsoluteOffset + transform.TransformVector(RelativeOffset); } }

        private void Update()
        {
            transform.position = Position;
        }

        #region Editor
        #if UNITY_EDITOR
        protected override void E_OnValidate()
        {
            transform.position = Position;
        }
        protected override void E_OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Position, 0.1f);
        }
        #endif
        #endregion
    }
}