using UnityEngine;

using DG.Tweening;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(EnemyMoveManager))]
    public class EnemyPatrolManager : MonoBehaviour {



        [Header("Properties")]
        public float walkSpeed = 1f;
        public float turningSpeed = 90f;
        public float waitingTimeOnNode;

        [Header("REFS")]
        public PathHolder path;


        float _pendingTurnAngleAtEndPoint = 0f;


        public bool IsInPath             {get; private set;} = false;
        public bool IsIndexAscendingWard {get; private set;} = true;
        public bool IsOnNode => (_prevNodeAscendedIndex == _nextNodeAscendedIndex && _prevNodeAscendedIndex != -1);
        public Vector2 PrevPoint => path != null ? path.GetPoint(_prevNodeAscendedIndex) : Vector2.zero;
        public Vector2 NextPoint => path != null ? path.GetPoint(_nextNodeAscendedIndex) : Vector2.zero;


        // Components
        Enemy            _enemy;
        EnemyMoveManager _moveManager;


        bool _isPatrolling = false;

        int _prevNodeAscendedIndex = -1;
        int _nextNodeAscendedIndex = -1;

        Sequence _waitingSeq;


        void Awake () {
            _enemy = GetComponent<Enemy>();
            _moveManager = GetComponent<EnemyMoveManager>();
        }

        void FixedUpdate () {
            if (!_enemy.IsActable)
                return;

            if (_moveManager != null) {

                bool isNowPatrolling = _moveManager.CurrentState == EnemyMoveManager.State.Patrolling;

                if (isNowPatrolling != _isPatrolling) {
                    if (isNowPatrolling) {
                        OnStartPatrolling();
                    }
                    else {
                        OnStopPatrolling();
                    }

                    _isPatrolling = isNowPatrolling;
                }
            }
        }


        public void SetToPathPosition (float positionInPath) {
            if (path == null)
                return;

            Vector2[] segment = path.GetSegment((int) positionInPath);

            transform.SetPosXY( segment[0] + (segment[1] - segment[0]) * (positionInPath % 1) );

            _prevNodeAscendedIndex = (int) positionInPath;
            _nextNodeAscendedIndex = Mathf.CeilToInt(positionInPath);
            IsInPath = true;

            HeadToNext();
        }


        public EnemyMoveManager.RigidbodyPosRot AccessNextPosRot (Rigidbody2D rb, float timeStep) {
            EnemyMoveManager.RigidbodyPosRot result = new EnemyMoveManager.RigidbodyPosRot() {
                position = rb.position,
                rotation = rb.rotation
            };


            Vector2 targetPos = rb.position;

            if (IsInPath) {

                if (_prevNodeAscendedIndex < _nextNodeAscendedIndex) {
                    IsIndexAscendingWard = true;
                }
                else if (_prevNodeAscendedIndex > _nextNodeAscendedIndex) {
                    IsIndexAscendingWard = false;
                }

                targetPos = NextPoint;

            }
            else {
                // Not In Path

                int closestSegmentIndex = 0;
                Vector2 closestPositionInPath = path.GetClosestPointInPath(rb.position, out closestSegmentIndex);

                if (closestPositionInPath - rb.position == Vector2.zero) {
                    // Already Got In Path
                    rb.position = closestPositionInPath;
                    IsInPath = true;

                    if (IsIndexAscendingWard) {
                        _prevNodeAscendedIndex = closestSegmentIndex;
                        _nextNodeAscendedIndex = closestSegmentIndex + 1;
                    }
                    else {
                        _prevNodeAscendedIndex = closestSegmentIndex + 1;
                        _nextNodeAscendedIndex = closestSegmentIndex;
                    }

                    targetPos = NextPoint;
                }
                else {
                    // Still Out of Path
                    targetPos = closestPositionInPath;
                }
            }

            // Draw the route
            Debug.DrawLine(rb.position, targetPos, Color.green);


            Vector2 dirToTarget = (targetPos - rb.position).normalized;

            if (dirToTarget == Vector2.zero) {
                rb.position = targetPos;

                if (IsInPath) {
                    OnNextArrived();
                }
            }
            else {
                float signedAngleToTarget = Vector2.SignedAngle(_enemy.FacingDirection, dirToTarget);

                if (Mathf.Abs(signedAngleToTarget) < GlobalManager.minDeltaAngle) {
                    // walk
                    if (_moveManager != null)
                        _moveManager.TurnToward(dirToTarget, timeStep);

                    result.position = Vector2.MoveTowards(rb.position, targetPos, walkSpeed * timeStep);
                }
                else {
                    // turn
                    if (_pendingTurnAngleAtEndPoint != 0) {

                        float sign = Mathf.Sign(_pendingTurnAngleAtEndPoint);
                        float unsignedDelta = turningSpeed * timeStep;

                        _pendingTurnAngleAtEndPoint = sign * Mathf.Max(Mathf.Abs(_pendingTurnAngleAtEndPoint) - unsignedDelta, 0f);

                        result.rotation = rb.rotation + sign * unsignedDelta;
                    }
                    else {
                        result.rotation = Mathf.MoveTowardsAngle(rb.rotation, rb.rotation + signedAngleToTarget, turningSpeed * timeStep);
                    }
                }
            }


            return result;
        }


        void OnNextArrived () {
            if (IsOnNode)
                return;

            IsInPath = true;
            _prevNodeAscendedIndex = _nextNodeAscendedIndex;

            _pendingTurnAngleAtEndPoint = path.GetTurnDirectionAtEndPoint(_prevNodeAscendedIndex) * GlobalManager.minDeltaAngle;

            _waitingSeq = DOTween.Sequence()
                .AppendInterval(waitingTimeOnNode)
                .AppendCallback(HeadToNext);
        }

        void HeadToNext () {
            _nextNodeAscendedIndex = _prevNodeAscendedIndex + 1;
        }


        void OnStartPatrolling () {

        }

        void OnStopPatrolling () {
            _waitingSeq.Kill(false);
            IsInPath = false;
        }

    }
}
