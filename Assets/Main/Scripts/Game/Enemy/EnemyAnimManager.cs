using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    public class EnemyAnimManager : MonoBehaviour {

        public enum State {
            Suspecting,
            Alert,
            Attacking,
            Dead,
            None
        }


        public float attackWordShowingDuration = 1f;

        [Header("REFS")]
        public RotationConstraint rotConstraint;
        public Text suspectingText;
        public Text alertText;
        public Text attackText;
        public Text deathText;


        public State CurrentState => _state;


        State _state = State.None;

        void Awake () {
            Play(State.None);
        }

        void Start () {
            rotConstraint.SetSource(0, new ConstraintSource {
                sourceTransform = GameSceneManager.current.transform,
                weight = 1f
            });
        }

        void ShutAll () {
            suspectingText.enabled = false;
            alertText.enabled = false;
            attackText.enabled = false;
            deathText.enabled = false;
        }

        public void Play (State state) {

            _state = state;

            ShutAll();

            if (state == State.Suspecting) {
                suspectingText.enabled = true;
            }
            else if (state == State.Alert) {
                alertText.enabled = true;
            }
            else if (state == State.Attacking) {
                attackText.enabled = true;

                DOTween.Sequence()
                    .AppendInterval(attackWordShowingDuration)
                    .AppendCallback( () => {
                        if (_state == State.Attacking) {
                            Play(State.Alert);
                        }
                    } );
            }
            else if (state == State.Dead) {
                deathText.enabled = true;
            }
        }

    }
}
