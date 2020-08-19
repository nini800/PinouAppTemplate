#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou.EntitySystem
{
	public class EntityEnemyControllerData : EntityControllerData
	{
        #region Fields, Getters
        [Title("Base AI")]
        [Space]
        [SerializeField] private EnemyBaseBehaviour _baseBehaviour;

        [Header("Target Detection Parameters")]
        [SerializeField, Min(0.02f)] protected float detectionCheckPeriod = 0.2f;
        [SerializeField, MinValue("targetBackwardDetectionDistance")] protected float targetForwardDetectionDistance = 3f;
        [SerializeField] protected bool detectionCheckForWalls = false;
        [SerializeField] protected bool complexDetection = false;
        [SerializeField, Min(0f), ShowIf("complexDetection")] protected float targetBackwardDetectionDistance = 1f;
        [SerializeField, Min(1f), MaxValue(360f), ShowIf("complexDetection")] protected float targetForwardDetectionAngle = 120f;
        [SerializeField, Min(1f), MaxValue("@360f - targetForwardDetectionAngle"), ShowIf("complexDetection")] protected float targetDetectionTransitionAngle = 30f;
        [SerializeField, Range(0.001f, 5f), ShowIf("complexDetection")] protected float targetDetectionTransitionPow = 1f;
        [Space]
        [Header("Movement Parameters")]
        [Tooltip("Tries to get to the smallest value, then, only start moving again when the biggest value is reached.")]
        [SerializeField] protected Vector2 aproachRange;

        [ShowIf("_baseBehaviour", EnemyBaseBehaviour.RotateAround)]
        [Tooltip("Proportion to maxSpeed")]
        [SerializeField, Range(0f, 1f)] protected float rotateAroundSpeed;

        [ShowIf("_baseBehaviour", EnemyBaseBehaviour.RotateAround)]
        [Tooltip("Negative numbers means never")]
        [SerializeField] protected Vector2 rotateAroundDirectionChangeWaitRange;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityEnemyController, EntityEnemyControllerData>(master, references, this);
        }
        #endregion

        public class EntityEnemyController : EntityController
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityEnemyControllerData)((EntityComponent)this).Data;
            }

            private EntityEnemyControllerData _data = null;
            protected new EntityEnemyControllerData Data => _data;
            #endregion

            #region Vars, Getters
            protected float targetSqrDst = 0f;
            protected Entity target = null;

            public bool HasTarget => target != null;
            public Entity Target => target;

            private float _lastDetectionTry = -1 / 0f;

            public override ControllerState ControllerState => ControllerState.Empty;
            #endregion

            #region Behaviour
            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveUpdate()
            {
                if (HasTarget == false)
                {
                    HandleDetection();
                }
                else
                {
                    HandleComputeTargetInfos();
                    HandleBehaviour();
                }
            }
            protected virtual void HandleDetection()
            {
                if (Time.time - _lastDetectionTry > _data.detectionCheckPeriod)
                {
                    TryDetect();
                }
            }
            protected void HandleComputeTargetInfos()
			{
                targetSqrDst = (target.Position - Position).sqrMagnitude;
            }
            protected void HandleBehaviour()
            {
                switch (_data._baseBehaviour)
                {
                    case EnemyBaseBehaviour.BrainlessRush:
                        HandleBrainlessRush();
                        break;
                    case EnemyBaseBehaviour.RotateAround:
                        HandleRotateAround();
                        break;
                }
            }
            /// <summary>
            /// Do not need base
            /// </summary>
            protected virtual void HandleBrainlessRush()
            {

            }
            /// <summary>
            /// Do not need base
            /// </summary>
            protected virtual void HandleRotateAround()
            {

            }
            #endregion


            #region Utilities
            protected virtual void TryDetect()
            {
                _lastDetectionTry = Time.time;
                List<Entity> detectedEntities = new List<Entity>(PinouUtils.Entity.DetectEntitiesInSphere<Entity>(Position, _data.targetForwardDetectionDistance));
                for (int i = detectedEntities.Count - 1; i >= 0; i--)
                {
                    if (detectedEntities[i] == master ||
                        IsEntityInViewRange(detectedEntities[i]) == false ||
                        IsEntityAuthorizedTarget(detectedEntities[i]) == false)
                    {
                        detectedEntities.RemoveAt(i);
                    }
                }

                float minDistance = 1 / 0f;
                Entity choosenTarget = null;
                for (int i = 0; i < detectedEntities.Count; i++)
                {
                    float dist = Vector3.SqrMagnitude(detectedEntities[i].Position - Position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        choosenTarget = detectedEntities[i];
                    }
                }

                if (choosenTarget != null)
                {
                    target = choosenTarget;
                }
            }
            public bool IsEntityInViewRange(Entity ent)
            {
                if (_data.detectionCheckForWalls == true)
				{
                    if (Physics.Linecast(Position, ent.Position, PinouApp.Resources.Data.Layers.Statics))
					{
                        return false;
					}
				}
                if (_data.complexDetection == false)
				{
                    return true;
                }
                else
				{
                    return Is2D ?
                        HandleEntityInViewRange2D(ent) :
                        HandleEntityInViewRange3D(ent);
				}
            }
            private bool HandleEntityInViewRange3D(Entity ent)
			{
                return false;
			}
            private bool HandleEntityInViewRange2D(Entity ent)
            {
                Vector2 relativePos = ent.Position2D - Position2D;
                float sqrDist = relativePos.sqrMagnitude;
                float maxDist;
                float curAngle = Mathf.Abs(Mathf.DeltaAngle(Vector2.SignedAngle(relativePos, Vector2.right), Vector2.SignedAngle(Forward2D, Vector2.right)));
                if (curAngle <= _data.targetForwardDetectionAngle * 0.5f)
                {
                    maxDist = _data.targetForwardDetectionDistance;
                }
                else if (curAngle <= (_data.targetForwardDetectionAngle + _data.targetDetectionTransitionAngle) * 0.5f)
                {
                    float progress = (curAngle - (_data.targetForwardDetectionAngle * 0.5f)) / (_data.targetDetectionTransitionAngle * 0.5f);
                    maxDist = Mathf.Lerp(_data.targetForwardDetectionDistance, _data.targetBackwardDetectionDistance, Mathf.Pow(progress, _data.targetDetectionTransitionPow));
                }
                else
                {
                    maxDist = _data.targetBackwardDetectionDistance;
                }

                maxDist *= maxDist;

                return sqrDist <= maxDist;
            }
            public virtual bool IsEntityAuthorizedTarget(Entity ent)
			{
                if (ent.Team == master.Team)
				{
                    return false;
				}
                else if (ent.Team == EntityTeam.Players && master.Team == EntityTeam.Allies)
				{
                    return false;
				}
                return true;
			}
            #endregion

            #region Editor
            /// <summary>
            /// Need base
            /// </summary>
#if UNITY_EDITOR
            public override void SlaveDrawGizmos()
            {
                if (master.E_PreviewViewRange == true)
                {
                    if (Is2D) { HandlePreviewViewRange2D(); }
                    else { HandlePreviewViewRange3D(); }
                }
                if (master.E_PreviewTarget == true)
				{
                    HandlePreviewTarget();
				}
            }
            protected void HandlePreviewViewRange3D()
            { 

            }
            protected void HandlePreviewViewRange2D()
            {
                Vector2 oldPos = Vector2.zero;
                Vector2 firstPos = Vector2.zero;
                for (int i = 0; i < master.E_PreviewViewRangeSteps; i++)
                {
                    float curAngle = (float)i / master.E_PreviewViewRangeSteps * Mathf.PI * 2f;
                    Vector2 localVector = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
                    curAngle *= Mathf.Rad2Deg;

                    float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(curAngle, 0f));
                    if (_data.complexDetection == false || deltaAngle <= _data.targetForwardDetectionAngle * 0.5f)
                    {
                        localVector *= _data.targetForwardDetectionDistance;
                    }
                    else if (deltaAngle <= (_data.targetForwardDetectionAngle + _data.targetDetectionTransitionAngle) * 0.5f)
                    {
                        float progress = (deltaAngle - (_data.targetForwardDetectionAngle * 0.5f)) / (_data.targetDetectionTransitionAngle * 0.5f);
                        localVector *= Mathf.Lerp(_data.targetForwardDetectionDistance, _data.targetBackwardDetectionDistance, Mathf.Pow(progress, _data.targetDetectionTransitionPow));
                    }
                    else
                    {
                        localVector *= _data.targetBackwardDetectionDistance;
                    }

                    Vector3 finalPos;
                    if (_data.complexDetection == false)
                    {
                        finalPos = Position2D + localVector;
                    }
                    else
                    {
                        finalPos = Position2D.ToV3() + PinouUtils.Transform.TransformVector(localVector.ToV3(localVector.x).SetX(0f), Quaternion.LookRotation(Forward, Vector3.up));
                    }

                    if (i > 0)
                    {
                        Gizmos.DrawLine(finalPos, oldPos);
                    }
                    else
                    {
                        firstPos = finalPos;
                    }
                    oldPos = finalPos;
                }
                Gizmos.DrawLine(firstPos, oldPos);

                if (master.E_rangeTester != null)
                {
                    Gizmos.color = IsEntityInViewRange(master.E_rangeTester) ? Color.green : Color.red;
                    Gizmos.DrawSphere(master.E_rangeTester.Position, 0.2f);
                }
            }
            protected virtual void HandlePreviewTarget()
			{
                if (target == null) { return; }
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Position, target.Position);
			}
#endif
#endregion
        }
    }
}