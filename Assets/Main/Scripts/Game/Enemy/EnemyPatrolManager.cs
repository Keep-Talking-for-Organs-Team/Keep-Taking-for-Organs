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
        public bool IsOnNode => (_prevNodeIndex == _nextNodeIndex && _prevNodeIndex != -1);
        public Vector2 PrevPoint => path != null ? path.GetPoint(_prevNodeIndex) : Vector2.zero;
        public Vector2 NextPoint => path != null ? path.GetPoint(_nextNodeIndex) : Vector2.zero;


        // Components
        Enemy _enemy;


        bool _isInPath = false;
        bool _isPatrolling = false;

        int _prevNodeIndex = -1;
        int _nextNodeIndex = -1;

        Sequence _waitingSeq;


        void Awake () {
            _enemy = GetComponent<Enemy>();
        }


        public void SetToPathPosition (float positionInPath) {
            if (path == null)
                return;

            Vector2[] segment = path.GetSegment((int) positionInPath);

            transform.SetPosXY( segment[0] + (segment[1] - segment[0]) * (positionInPath % 1) );

            _prevNodeIndex = (int) positionInPath;
            _nextNodeIndex = Mathf.CeilToInt(positionInPath);
            _isInPath = true;

            HeadToNext();
        }

        public Vector2 GetNextPosition (Rigidbody2D rb, float timeStep) {
            if (_isInPath) {
                Vector2 toNextDir = (NextPoint - rb.position).normalized;

                if (Vector2.Angle(_enemy.FacingDirection, toNextDir) < Mathf.Epsilon || toNextDir == Vector2.zero) {

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

                    Vector2 targetDir = (NextPoint - rb.position).normalized;

                    if (targetDir != Vector2.zero && _enemy.FacingDirection != targetDir) {

                        return Mathf.MoveTowardsAngle( rb.rotation, rb.rotation + Vector2.SignedAngle(_enemy.FacingDirection, targetDir), turningSpeed * timeStep );
                    }
                }
            }

            return rb.rotation;
        }


        void OnNextArrived () {
            if (IsOnNode)
                return;

            _isInPath = true;
            _prevNodeIndex = _nextNodeIndex;

            _waitingSeq = DOTween.Sequence()
                .AppendInterval(waitingTimeOnNode)
                .AppendCallback(HeadToNext);
        }

        void HeadToNext () {
            if (IsOnNode)
                _nextNodeIndex = _prevNodeIndex + 1;
        }


        void StopPatrolling () {
            _waitingSeq.Kill(false);
        }

    }
}
