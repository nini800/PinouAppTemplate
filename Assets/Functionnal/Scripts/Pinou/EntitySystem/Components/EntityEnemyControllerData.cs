#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine.Serialization;

namespace Pinou.EntitySystem
{
	public class EntityEnemyControllerData : EntityControllerData
	{
        #region Fields, Getters
        [Title("Base AI")]
        [Space]
        [SerializeField] protected EnemyNoTargetBehaviour noTargetBehaviour;
        [SerializeField] protected EnemyHasTargetBehaviour hasTargetBehaviour;

        [Header("Target Detection Parameters")]
        [Space]
        [SerializeField, Min(0.02f)] protected float detectionCheckPeriod = 0.2f;
        [SerializeField, MinValue("targetBackwardDetectionDistance")] protected float targetForwardDetectionDistance = 3f;
        [SerializeField] protected bool detectionCheckForWalls = false;
        [SerializeField] protected bool complexDetection = false;
        [SerializeField, Min(0f), ShowIf("complexDetection")] protected float targetBackwardDetectionDistance = 1f;
        [SerializeField, Min(1f), MaxValue(360f), ShowIf("complexDetection")] protected float targetForwardDetectionAngle = 120f;
        [SerializeField, Min(1f), MaxValue("@360f - targetForwardDetectionAngle"), ShowIf("complexDetection")] protected float targetDetectionTransitionAngle = 30f;
        [SerializeField, Range(0.001f, 5f), ShowIf("complexDetection")] protected float targetDetectionTransitionPow = 1f;


        [Header("Global Parameters")]
        [Space]
        [SerializeField] protected float global_MoveTowardMargin = 0.5f;

        private const string NO_TARGET_COND = "@noTargetBehaviour == EnemyNoTargetBehaviour.MoveRandomly || noTargetBehaviour == EnemyNoTargetBehaviour.MoveRandomlyAroundSpawn";
        [Header("No Target Parameters")]
        [Header("Movements Parameters")]
        [Space]
        [SerializeField, ShowIf(NO_TARGET_COND), Min(0f)] protected float noTarget_MinMoveWaitTime = 0f; 
        [SerializeField, ShowIf(NO_TARGET_COND), Min(0f)] protected float noTarget_MaxMoveWaitTime = 2f;
        [Space]
        [SerializeField, ShowIf(NO_TARGET_COND), Min(0f)] protected float noTarget_MinIdleWaitTime = 0f;
        [SerializeField, ShowIf(NO_TARGET_COND), Min(0f)] protected float noTarget_MaxIdleWaitTime = 2f;
        [Space]
        [SerializeField, ShowIf("noTargetBehaviour", EnemyNoTargetBehaviour.MoveRandomly), Min(0f)] protected float noTarget_MinMoveDistance = 0f;
        [SerializeField, ShowIf("noTargetBehaviour", EnemyNoTargetBehaviour.MoveRandomly), Min(0f)] protected float noTarget_MaxMoveDistance = 5f;
        [SerializeField, ShowIf("noTargetBehaviour", EnemyNoTargetBehaviour.MoveRandomlyAroundSpawn), Min(0f)] protected float noTarget_maxAroundSpawnMoveRange = 10f;


        [Header("Has Target Parameters")]
        [Header("Movements Parameters")]
        [SerializeField] protected Vector2 hasTarget_AproachRange;

        [ShowIf("hasTargetBehaviour", EnemyHasTargetBehaviour.RotateAround)]
        [SerializeField, Range(0f, 1f)] protected float hasTarget_RotateAroundSpeed;

        [ShowIf("hasTargetBehaviour", EnemyHasTargetBehaviour.RotateAround)]
        [SerializeField] protected Vector2 hasTarget_RotateAroundDirectionChangeWaitRange;

        [Header("Aim Parameters")]
        [SerializeField, Min(0f)] protected float hasTarget_ExpectedProjectileSpeedAimFactor = 1f;
        [SerializeField, Min(0f)] protected float hasTarget_AimSpread = 0.3f;
        [ShowInInspector]
        protected float hasTarget_MaxAimSpreadAngle
        {
            get 
            {
                Vector2 v = new Vector2(1f, 0f);
                v += Vector2.up * hasTarget_AimSpread;
                return Vector2.Angle(Vector2.right, v);
            }
        }
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
            protected Vector3 spawnPosition;

            protected float targetSqrDst = 0f;
            protected float targetDst = 0f;
            protected Vector3 toTarget = Vector3.zero;
            protected Vector3 toTargetNorm = Vector3.zero;

            protected Entity target = null;

            protected bool noTarget_waiting = true;
            protected float noTarget_NextMoveTime = -1 / 0f;
            protected float noTarget_NextWaitTime = -1 / 0f;
            protected Vector3 noTarget_NextPosition;

            protected bool hasTarget_rangeReached = false;

            public bool HasTarget => target != null;
            public Entity Target => target;

            public float NoTarget_NextMoveTime => noTarget_NextMoveTime;
            public float NoTarget_NextWaitTime => noTarget_NextWaitTime;
            public Vector3 NoTarget_NextPosition => noTarget_NextPosition;

            private float _lastDetectionTry = -1 / 0f;

            public override ControllerState ControllerState => ControllerState.Empty;
			#endregion

			#region Behaviour
            /// <summary>
            /// Need base
            /// </summary>
			public override void SlaveAwake()
			{
                spawnPosition = Position;
                noTarget_NextPosition = ComputeNoTargetNextPosition();
                noTarget_NextMoveTime = Time.time + Random.Range(_data.noTarget_MinIdleWaitTime, _data.noTarget_MaxIdleWaitTime);
                noTarget_NextWaitTime = noTarget_NextMoveTime + Random.Range(_data.noTarget_MinMoveWaitTime, _data.noTarget_MaxMoveWaitTime);
            }

            /// <summary>
            /// Need base
            /// </summary>
            public override void SlaveUpdate()
            {
                if (HasTarget == false)
                {
                    HandleDetection();
                    HandleNoTargetBehaviour();
                }
                else
                {
                    HandleComputeTargetInfos();
                    HandleHasTargetBehaviour();
                }
            }
            protected virtual void HandleDetection()
            {
                if (Time.time - _lastDetectionTry > _data.detectionCheckPeriod)
                {
                    TryDetect();
                }
            }
            protected void HandleNoTargetBehaviour()
			{
                switch (_data.noTargetBehaviour)
				{
					case EnemyNoTargetBehaviour.StayStill:
                        HandleStayStill();
						break;
					case EnemyNoTargetBehaviour.MoveRandomly:
                        HandleMoveRandomly();
						break;
                    case EnemyNoTargetBehaviour.MoveRandomlyAroundSpawn:
                        HandleMoveRandomlyAroundPoint();
						break;
                    case EnemyNoTargetBehaviour.MoveForward:
                        HandleMoveForward();
						break;
                }

                HandleNoTarget_Abilities();
            }

            protected virtual void HandleStayStill()
			{
                StayStill();
            }
            protected virtual void HandleMoveRandomly()
            {
                //Waiting
                if (noTarget_waiting == true)
				{
                    StayStill();
                    if (Time.time >= noTarget_NextMoveTime)
					{
                        noTarget_NextMoveTime = noTarget_NextWaitTime + Random.Range(_data.noTarget_MinIdleWaitTime, _data.noTarget_MaxIdleWaitTime);
                        noTarget_waiting = false;
                        noTarget_NextPosition = ComputeNoTargetNextPosition();

                        HandleMoveRandomly();
                    }
                }
                //Moving
                else
				{
                    MoveToward(noTarget_NextPosition);
                    if (Time.time >= noTarget_NextWaitTime)
                    {
                        noTarget_NextWaitTime = noTarget_NextMoveTime + Random.Range(_data.noTarget_MinMoveWaitTime, _data.noTarget_MaxMoveWaitTime);
                        noTarget_waiting = true;

                        HandleMoveRandomly();
                    }
                }
            }
            protected virtual void HandleMoveRandomlyAroundPoint()
            {
                HandleMoveRandomly();
            }
            protected virtual void HandleMoveForward()
            {
                MoveToward(noTarget_NextPosition);
            }
            protected virtual Vector3 ComputeNoTargetNextPosition()
			{
                float randAngle = Random.Range(0f, Mathf.PI * 2f);
                Vector3 direction = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                if (PinouApp.Entity.Mode2D == false) { direction = direction.SwapYZ(); }

				switch (_data.noTargetBehaviour)
				{
					case EnemyNoTargetBehaviour.StayStill:
                        return Position;
					case EnemyNoTargetBehaviour.MoveRandomly:
                        return Position + direction * Random.Range(_data.noTarget_MinMoveDistance, _data.noTarget_MaxMoveDistance);
                    case EnemyNoTargetBehaviour.MoveRandomlyAroundSpawn:
                        return spawnPosition + direction * Random.Range(0f, _data.noTarget_maxAroundSpawnMoveRange);
                    case EnemyNoTargetBehaviour.MoveForward:
                        return Position + direction * 100000000;
                }

                throw new System.Exception("Requires update here.");
			}
            protected virtual void HandleNoTarget_Abilities()
            {
                aimTarget = default;
                aimDirection = default;
                shoot = false;
            }

            protected void HandleComputeTargetInfos()
			{
                toTarget = target.Position - Position;
                targetSqrDst = toTarget.sqrMagnitude;
                targetDst = Mathf.Sqrt(targetSqrDst);
                toTargetNorm = toTarget / targetDst;
            }
            protected void HandleHasTargetBehaviour()
            {
                switch (_data.hasTargetBehaviour)
                {
                    case EnemyHasTargetBehaviour.BrainlessRush:
                        HandleBrainlessRush();
                        break;
                    case EnemyHasTargetBehaviour.RotateAround:
                        HandleRotateAround();
                        break;
                }
            }
            /// <summary>
            /// Do not need base
            /// </summary>
            protected virtual void HandleBrainlessRush()
            {
                HandleBrainlessRush_Movements();
                HandleBrainlessRush_Abilities();
            }
            protected void HandleBrainlessRush_Movements()
			{
                HandleApproachRangeState();
                if (hasTarget_rangeReached == false)
                {
                    MoveToward(target.Position - toTargetNorm * _data.hasTarget_AproachRange.x);
                }
                else
                {
                    StayStill();
                }
            }
            protected void HandleBrainlessRush_Abilities()
            {
                aimTarget = PinouUtils.Maths.PredictAim(Position, target.Position, target.HasMovements ? target.Movements.AverageVelocity : Vector3.zero, _data.hasTarget_ExpectedProjectileSpeedAimFactor);
                aimDirection = (aimTarget - Position).normalized;
                shoot = true;
            }
            protected void HandleApproachRangeState()
			{
                if (hasTarget_rangeReached == false && targetDst <= _data.hasTarget_AproachRange.x)
				{
                    hasTarget_rangeReached = true;
				}
                else if (hasTarget_rangeReached == true && targetDst >= _data.hasTarget_AproachRange.y)
				{
                    hasTarget_rangeReached = false;
				}
			}
            /// <summary>
            /// Do not need base
            /// </summary>
            protected virtual void HandleRotateAround()
            {

            }

			public override void SlaveEnabled()
			{
                master.OnReceiveAbilityHit.SafeSubscribe(OnReceiveAbilityHit);
			}
			public override void SlaveDisabled()
			{
                master.OnReceiveAbilityHit.Unsubscribe(OnReceiveAbilityHit);
			}
			#endregion


			#region Utilities
			#region Movements
            protected void StayStill()
			{
                inputingMovement = false;
                moveVector = Vector3.zero;
			}
            protected void MoveToward(Vector3 position)
			{
                inputingMovement = true;
                float dst = (Position - position).magnitude - _data.global_MoveTowardMargin;
                float speed = Movements.Data.BaseAcceleration * (Mathf.Clamp(dst, 0f, Movements.Data.BaseMaxSpeed) / Movements.Data.BaseMaxSpeed);
                if (speed <= 0f)
				{
                    StayStill();
				}
                else
				{
                    moveVector = (position - Position).normalized * speed;
                }
            }
			#endregion
			#region Target Detection
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
			#endregion

			#region Events
			private void OnReceiveAbilityHit(Entity master, AbilityCastResult castResult)
            {
                if (HasTarget == false) 
                { 
                    if (IsEntityAuthorizedTarget(castResult.CastData.Caster))
					{
                        target = castResult.CastData.Caster;
                    }
                }
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