#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public class RigSkeleton : PinouBehaviour
	{
        [System.Serializable]
        public class JointInfos
        {
            public Quaternion Rot { get { return Joint.rotation; } set { Joint.rotation = value; } }
            public Vector3 Pos { get { return Joint.position; } set { Joint.position = value; } }
            public Transform Joint;
            public Transform Parent;
            public float ParentDistance;
            public int DistanceFromDeadEnd;
        }

        [SerializeField] public float _baseRadius = 0.3f;
        [SerializeField, Range(0f, 1f)] public float _lerpAmount = 0.3f;
        [SerializeField, Range(0f, 1f)] public float _deadLerpAmount = 0.3f;
        [SerializeField] public float _deadLerpFactor = 1f;
        [SerializeField] private Mesh _sphereMesh;
        [SerializeField] private JointInfos[] _joints;

        [SerializeField, ReadOnly] private bool _initialized = false;

#if UNITY_EDITOR
        [CustomEditor(typeof(RigSkeleton))]
        public class RigSkeletonEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            private RigSkeleton Instance => target as RigSkeleton;
            protected override void OnEnable()
            {
                base.OnEnable();

                Tools.hidden = true;
            }

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

                InitButton();
            }

            private void InitButton()
            {
                if (GUILayout.Button("Initialize Skeleton"))
                {
                    Instance._initialized = true;

                    Transform[] allTs = Instance.transform.GetComponentsInChildren<Transform>();
                    List<JointInfos> joints = new List<JointInfos>();

                    for (int i = 0; i < allTs.Length; i++)
                    {
                        if (allTs[i] != Instance.transform && allTs[i].childCount > 0)
                        {
                            int deadEndDist = 0;
                            Transform deadEndFinder = allTs[i].GetChild(0);
                            do
                            {
                                deadEndDist++;

                                if (deadEndFinder.childCount > 0)
                                {
                                    deadEndFinder = deadEndFinder.GetChild(0);
                                }

                            } while (deadEndFinder.childCount > 0);

                            JointInfos joint = new JointInfos
                            {
                                Joint = allTs[i],
                                Parent = allTs[i].parent,
                                ParentDistance = (allTs[i].position - allTs[i].parent.position).magnitude,
                                DistanceFromDeadEnd = deadEndDist
                            };

                            joints.Add(joint);
                        }
                    }

                    joints.Add(new JointInfos() { Joint = Instance.transform, Parent = Instance.transform, ParentDistance = 0.5f, DistanceFromDeadEnd = 5 });
                    Instance._joints = joints.ToArray();
                }
            }


            private int _currentSelected = -1;

            private void OnSceneGUI()
            {
                if (Instance._initialized == false)
                {
                    return;
                }

                foreach (JointInfos j in Instance._joints)
                {
                    if (Handles.Button(j.Pos, Quaternion.identity, Size(j), Size(j) * .5f, Handles.SphereHandleCap))
                    {
                        _currentSelected = Instance._joints.IndexOf(j);
                    }
                    Handles.DrawLine(j.Pos, j.Parent.position);
                }

                if (_currentSelected >= 0)
                {
                    JointInfos jj = Instance._joints[_currentSelected];

                    switch (Tools.current)
                    {
                        case Tool.View:
                        case Tool.Move:
                        case Tool.Scale:
                        case Tool.Rect:
                        case Tool.Transform:
                        case Tool.Custom:
                        case Tool.None:
                            Vector3 pos = Handles.PositionHandle(jj.Pos, Quaternion.identity);

                            if (pos != jj.Pos)
                            {
                                Undo.RecordObject(jj.Joint, "Joint Position");
                                jj.Pos = pos;
                            }
                            break;
                        case Tool.Rotate:
                            Quaternion rot = Handles.RotationHandle(jj.Rot, jj.Pos);

                            if (rot != jj.Rot)
                            {
                                Undo.RecordObject(jj.Joint, "Joint Rotation");
                                jj.Rot = rot;
                            }
                            break;
                    }

                    
                }
            }

            private float Size(JointInfos j)
            {
                float deadEndFactor = ((float)j.DistanceFromDeadEnd) / 5f * Instance._deadLerpFactor;
                return Mathf.Lerp(Mathf.Lerp(1, j.ParentDistance, Instance._lerpAmount) * Instance._baseRadius, deadEndFactor, Instance._deadLerpAmount);
            }

            protected override void OnDisable()
            {
                base.OnDisable();

                Tools.hidden = false;
            }

        }
#endif
    }


}