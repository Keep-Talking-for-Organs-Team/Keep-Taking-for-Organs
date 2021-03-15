using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
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
        public Text suspectingText;
        public Text alertText;
        public Text attackText;
        public Text deathText;


        public State CurrentState {get; private set;} = State.None;


        void Awake () {
            Play(State.None);
        }


        void ShutAll () {
            suspectingText.enabled = false;
            alertText.enabled = false;
            attackText.enabled = false;
            deathText.enabled = false;
        }

        public void Play (State state) {

            CurrentState = state;

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
                        if (CurrentState == State.Attacking) {
                            AkSoundEngine.PostEvent("Play_Robot_LaserGun", gameObject);
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
