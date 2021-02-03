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
        Enemy       _enemy;
        Rigidbody2D _rigidbody;


        void Awake () {
            _enemy     = GetComponent<Enemy>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        public void Target (Vector2 targetPos, float timeStep, float timeScale) {

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
            _rigidbody.MovePosition(_rigidbody.position + (Vector2) transform.position.DirectionTo(targetPos) * chaseSpeed * timeScale * timeStep);
        }


        void TurnToward (Vector2 destinationDir, float timeStep, float timeScale) {
            _rigidbody.MoveRotation( Mathf.MoveTowardsAngle(_rigidbody.rotation, Vector2.SignedAngle(_enemy.initDir, destinationDir), maxTurningSpeed * timeScale * timeStep) );
        }

    }
}
