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


        public bool IsPatrolling {
            get => _isPatrolling;
            set {
                if (_isPatrolling != value) {

                    if (value == true) {
                        // start patrolling
                    }
                    else {
                        // stop patrolling
                        StopPatrolling();
                    }

                    _isPatrolling = value;
                }

            }
        }

        public bool IsInPath => _isInPath;
        public bool IsOnNode => (_prevNodeAscendedIndex == _nextNodeAscendedIndex && _prevNodeAscendedIndex != -1);
        public Vector2 PrevPoint => path != null ? path.GetPoint(_prevNodeAscendedIndex) : Vector2.zero;
        public Vector2 NextPoint => path != null ? path.GetPoint(_nextNodeAscendedIndex) : Vector2.zero;


        // Components
        Enemy            _enemy;
        EnemyMoveManager _moveManager;


        bool _isInPath = false;
        bool _isPatrolling = false;

        int _prevNodeAscendedIndex = -1;
        int _nextNodeAscendedIndex = -1;

        Sequence _waitingSeq;


        void Awake () {
            _enemy = GetComponent<Enemy>();
            _moveManager = GetComponent<EnemyMoveManager>();
        }


        public void SetToPathPosition (float positionInPath) {
            if (path == null)
                return;

            Vector2[] segment = path.GetSegment((int) positionInPath);

            transform.SetPosXY( segment[0] + (segment[1] - segment[0]) * (positionInPath % 1) );

            _prevNodeAscendedIndex = (int) positionInPath;
            _nextNodeAscendedIndex = Mathf.CeilToInt(positionInPath);
            _isInPath = true;

            HeadToNext();
        }

        public Vector2 AccessNextPosition (Rigidbody2D rb, float timeStep) {
            if (_isInPath) {
                Vector2 toNextDir = (NextPoint - rb.position).normalized;

                if (Vector2.Angle(_enemy.FacingDirection, toNextDir) < GlobalManager.minDeltaAngle || toNextDir == Vector2.zero) {

                    if (_moveManager != null)
                        _moveManager.TurnToward(toNextDir, timeStep);

                    Vector2 result = Vector2.MoveTowards(rb.position, NextPoint, walkSpeed * timeStep);

                    if (result == NextPoint)
                        OnNextArrived();

                    return result;
                }
            }

            return transform.position;
        }

        public float GetNextRotation (Rigidbody2D rb, float timeStep) {

            if (_isInPath) {
                if (!IsOnNode) {

                    if (_pendingTurnAngleAtEndPoint == 0) {
                        Vector2 targetDir = (NextPoint - rb.position).normalized;

                        if (targetDir != Vector2.zero && _enemy.FacingDirection != targetDir) {

                            return Mathf.MoveTowardsAngle( rb.rotation, rb.rotation + Vector2.SignedAngle(_enemy.FacingDirection, targetDir), turningSpeed * timeStep );
                        }
                    }
                    else {
                        float sign = Mathf.Sign(_pendingTurnAngleAtEndPoint);
                        float unsignedDelta = turningSpeed * timeStep;

                        _pendingTurnAngleAtEndPoint = sign * Mathf.Max(Mathf.Abs(_pendingTurnAngleAtEndPoint) - unsignedDelta, 0f);

                        return rb.rotation + sign * unsignedDelta;
                    }

                }
            }

            return rb.rotation;
        }


        void OnNextArrived () {
            if (IsOnNode)
                return;

            _isInPath = true;
            _prevNodeAscendedIndex = _nextNodeAscendedIndex;

            _pendingTurnAngleAtEndPoint = path.GetTurnDirectionAtEndPoint(_prevNodeAscendedIndex) * GlobalManager.minDeltaAngle;

            _waitingSeq = DOTween.Sequence()
                .AppendInterval(waitingTimeOnNode)
                .AppendCallback(HeadToNext);
        }

        void HeadToNext () {
            if (IsOnNode)
                _nextNodeAscendedIndex = _prevNodeAscendedIndex + 1;
        }


        void StopPatrolling () {
            _waitingSeq.Kill(false);
        }

    }
}
