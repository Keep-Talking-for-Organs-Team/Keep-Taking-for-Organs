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
                    Vector2 nextPos = _patrolManager.GetNextPosition(_rigidbody, Time.fixedDeltaTime * Time.timeScale);

                    _rigidbody.MovePosition(nextPos);
                    _rigidbody.MoveRotation(_patrolManager.GetNextRotation(_rigidbody, Time.fixedDeltaTime * Time.timeScale));
                }
            }

        }


        public void Target (Vector2 targetPos, float timeStep, float timeScale) {
            _state = State.Targeting;

            Vector2 targetDir = transform.position.DirectionTo(targetPos);
            float toTargetAngle = Vector2.SignedAngle(_enemy.FacingDirection, targetDir);
            float toVisionAngle = Vector2.SignedAngle(_enemy.FacingDirection, _enemy.visionSpan.FacingDirection);

            if (Mathf.Sign(toTargetAngle) == Mathf.Sign(toVisionAngle)) {
                if (Mathf.Abs(toTargetAngle) < Mathf.Abs(toVisionAngle)) {

                    TurnToward(targetPos, timeStep, timeScale);
                }
                else {

                    TurnToward(_enemy.visionSpan.FacingDirection, timeStep, timeScale);
                }
            }
        }

        public void Chase (Vector2 targetPos, float timeStep, float timeScale) {
            _state = State.Chasing;
            _rigidbody.MovePosition(_rigidbody.position + (Vector2) transform.position.DirectionTo(targetPos) * chaseSpeed * timeScale * timeStep);
        }


        void TurnToward (Vector2 destinationDir, float timeStep, float timeScale) {
            _rigidbody.MoveRotation( Mathf.MoveTowardsAngle(_rigidbody.rotation, Vector2.SignedAngle(_enemy.defaultDir, destinationDir), maxTurningSpeed * timeScale * timeStep) );
        }

    }
}
