using UnityEngine;

using DG.Tweening;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    public class EnemyVisionManager : MonoBehaviour {

        public enum State {
            Blind,
            Fixed,
            Shaking,
            Targeting
        }


        public float maxAngularSpeed;
        public ShakingProps shakingProps;

        [Header("State")]
        public State defaultState;

        // Components
        Enemy        _enemy;

        State        _state;
        ShakingState _shakingState;


        void Awake () {
            _enemy = GetComponent<Enemy>();

            _shakingState = new ShakingState {
                animSeq = null,
                currentRotAngle = 0f
            };
        }

        void Start () {
            _state = defaultState;
            _enemy.visionSpan.SetFacingDirection(_enemy.FacingDirection);
        }


        void FixedUpdate () {

            if (_state == State.Fixed) {
                if (_enemy.FacingDirection != _enemy.visionSpan.FacingDirection)
                    RotateToward(_enemy.FacingDirection, Time.fixedDeltaTime, Time.timeScale);
            }

            if (_state == State.Shaking) {

                if (_shakingState.animSeq == null) {

                    _shakingState.currentRotAngle = Vector2.SignedAngle(_enemy.FacingDirection, _enemy.visionSpan.FacingDirection);

                    float destinationRotAngle = shakingProps.angle / 2;
                    if (_shakingState.currentRotAngle > 0)
                        destinationRotAngle *= -1;


                    float duration = Mathf.Abs(destinationRotAngle - _shakingState.currentRotAngle) / shakingProps.speed;
                    _shakingState.animSeq = DOTween.Sequence()
                        .Append( DOTween.To(() => _shakingState.currentRotAngle, x => _shakingState.currentRotAngle = x, destinationRotAngle, duration)
                            .SetEase(shakingProps.ease)
                        )
                        .AppendInterval( shakingProps.intervalTime )
                        .OnComplete( () => _shakingState.animSeq = null );
                }

                _enemy.visionSpan.SetFacingDirection(Quaternion.AngleAxis(_shakingState.currentRotAngle, Vector3.forward) * _enemy.FacingDirection);

            }
            else {
                if (_shakingState.animSeq != null) {
                    _shakingState.animSeq.Kill(false);
                    _shakingState.animSeq = null;
                }
            }

            _state = defaultState;

        }

        public void Target (Vector2 targetPos, float timeStep, float timeScale) {
            _state = State.Targeting;
            RotateToward( ((Vector2) transform.position).DirectionTo(targetPos), timeStep, timeScale );
        }



        void RotateToward (Vector2 destinationDir, float timeStep, float timeScale) {
            _enemy.visionSpan.SetFacingDirection(_enemy.visionSpan.FacingDirection.GetRotateTowards(destinationDir, maxAngularSpeed * timeScale * timeStep));
        }



        [System.Serializable]
        public struct ShakingProps {
            public float angle;
            public float speed;
            public Ease  ease;
            public float intervalTime;
        }

        [System.Serializable]
        class ShakingState {
            public Sequence animSeq;
            public float currentRotAngle;
        }

    }
}
