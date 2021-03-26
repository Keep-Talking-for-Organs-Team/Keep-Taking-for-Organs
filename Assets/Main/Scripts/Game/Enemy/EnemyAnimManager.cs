using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class EnemyAnimManager : MonoBehaviour {

        public enum State {
            Idle,
            Moving,
            Suspecting,
            Alert,
            Attacking,
            Dead,
            None
        }


        public float fadeOutDuration = 1f;
        public Ease  fadeOutEase;
        public float attackWordShowingDuration = 1f;

        [Header("SeqImgAnim Properties")]
        public float idleFPS = 1f;
        public float movingFPS = 1f;

        [Header("REFS")]
        public SpriteRenderer bodySR;
        public Text suspectingText;
        public Text alertText;
        public Text attackText;
        public Text deathText;


        public State CurrentState {get; private set;} = State.None;

        Sprite[] _flyingAnimSprites = null;
        int      _currentFlyingAnimFrameIndex = 0;
        float    _flyingAnimPrevFrameTime = 0f;

        // Components
        EnemyMoveManager _moveManager;

        void Awake () {
            _moveManager = GetComponent<EnemyMoveManager>();

            Play(State.None);
        }

        void Start () {
            if (GameSceneManager.current != null && GameSceneManager.current.operatorManager != null) {
                _flyingAnimSprites = GameSceneManager.current.operatorManager.EnemyFlyingAnimSprites;
            }
        }

        void Update () {
            float fps = idleFPS;

            if (_moveManager != null && _moveManager.IsMoving) {
                fps = movingFPS;
            }

            if (Time.time - _flyingAnimPrevFrameTime > 1f / fps) {
                _currentFlyingAnimFrameIndex = (_currentFlyingAnimFrameIndex + 1) % _flyingAnimSprites.Length;

                if (_flyingAnimSprites != null && _flyingAnimSprites.Length > 0)
                    bodySR.sprite = _flyingAnimSprites[_currentFlyingAnimFrameIndex];

                _flyingAnimPrevFrameTime = Time.time;
            }
        }


        void ShutAll () {
            suspectingText.enabled = false;
            alertText.enabled = false;
            attackText.enabled = false;
            deathText.enabled = false;
        }

        public void Play (State state) {

            ShutAll();

            if (CurrentState != state) {

                CurrentState = state;

                if (state == State.Suspecting) {
                    // suspectingText.enabled = true;
                }
                else if (state == State.Alert) {
                    // alertText.enabled = true;
                }
                else if (state == State.Attacking) {
                    // attackText.enabled = true;

                    // DOTween.Sequence()
                    //     .AppendInterval(attackWordShowingDuration)
                    //     .AppendCallback( () => {
                    //         if (CurrentState == State.Attacking) {
                    //
                    //             Play(State.Alert);
                    //         }
                    //     } );
                }
                else if (state == State.Dead) {
                    // deathText.enabled = true;

                    bodySR.color = Color.red;
                    bodySR.DOFade(0f, fadeOutDuration)
                        .SetEase(fadeOutEase)
                        .OnComplete(() => {
                            Destroy(gameObject);
                        });
                }
            }
        }


    }
}
