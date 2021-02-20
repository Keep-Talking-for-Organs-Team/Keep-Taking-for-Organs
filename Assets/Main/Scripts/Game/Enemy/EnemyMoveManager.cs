using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMoveManager : MonoBehaviour {

        public enum State {
            Standing,
            Patrolling,
            Targeting,
            Chasing
        }

        public float maxTurningSpeed;
        public float chaseSpeed;

        [Header("State")]
        public State defaultState;

        [Header("Parameters")]
        public LayerMask moveCollisionLayerMask;


        // Components
        Enemy              _enemy;
        Rigidbody2D        _rigidbody;
        EnemyPatrolManager _patrolManager;

        State _state;


        void Awake () {
            _enemy         = GetComponent<Enemy>();
            _rigidbody     = GetComponent<Rigidbody2D>();
            _patrolManager = GetComponent<EnemyPatrolManager>();
        }

        void Start () {
            _state = defaultState;
        }


        void FixedUpdate () {

            if (_state == State.Patrolling) {
                if (_patrolManager != null) {

                    // test
                    if (!_patrolManager.IsInPath)
                        _patrolManager.SetToPathPosition(0f);
                    // ====

                    _patrolManager.IsPatrolling = true;

                    Vector2 deltaPos = _patrolManager.AccessNextPosition(_rigidbody, Time.fixedDeltaTime * Time.timeScale) - _rigidbody.position;
                    Vector2 toNextDir = deltaPos.normalized;

                    deltaPos = PhysicsTools2D.GetFinalDeltaPosAwaringObstacle(_rigidbody, toNextDir, Vector2.Dot(deltaPos, toNextDir), moveCollisionLayerMask);

                    _rigidbody.MovePosition(_rigidbody.position + deltaPos);
                    _rigidbody.MoveRotation(_patrolManager.GetNextRotation(_rigidbody, Time.fixedDeltaTime * Time.timeScale));
                }
            }

        }


        public void Target (Vector2 targetPos, float timeStep) {
            _state = State.Targeting;

            Vector2 targetDir = transform.position.DirectionTo(targetPos);
            float toTargetAngle = Vector2.SignedAngle(_enemy.FacingDirection, targetDir);
            float toVisionAngle = Vector2.SignedAngle(_enemy.FacingDirection, _enemy.visionSpan.FacingDirection);

            if (Mathf.Sign(toTargetAngle) == Mathf.Sign(toVisionAngle)) {
                if (Mathf.Abs(toTargetAngle) < Mathf.Abs(toVisionAngle)) {

                    TurnToward(transform.position.DirectionTo(targetPos), timeStep);
                }
                else {

                    TurnToward(_enemy.visionSpan.FacingDirection, timeStep);
                }
            }
        }

        public void Chase (Vector2 targetPos, float timeStep) {
            _state = State.Chasing;

            Vector2 deltaPos = PhysicsTools2D.GetFinalDeltaPosAwaringObstacle(_rigidbody, _rigidbody.position.DirectionTo(targetPos), chaseSpeed * timeStep, moveCollisionLayerMask);
            _rigidbody.MovePosition(_rigidbody.position + deltaPos);
        }


        public void TurnToward (Vector2 destinationDir, float timeStep) {
            _rigidbody.MoveRotation( Mathf.MoveTowardsAngle(_rigidbody.rotation, Vector2.SignedAngle(_enemy.defaultDir, destinationDir), maxTurningSpeed * timeStep) );
        }

    }
}
