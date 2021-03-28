using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;
using DoubleHeat.Animation;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerAnimManager : MonoBehaviour {

        public enum State {
            Idle,
            Walk,
            Run,
            Melee,
            Gun
        }



        [Header("Properties")]
        public State defaultMovingState;
        public float hidingOpacity;
        public float rangedAttackableLineWidth = 0.23f;
        public Color rangedAttackableLineColor = Color.red;

        [Header("SeqImgAnim Properties")]
        public SeqImgAnim.FixedFrameRateAnimProperties idleAnimProps;
        public SeqImgAnim.FixedFrameRateAnimProperties walkAnimProps;
        public SeqImgAnim.FixedFrameRateAnimProperties runAnimProps;
        public SeqImgAnim.FixedDurationAnimProperties  meleeAnimProps;
        public SeqImgAnim.FixedDurationAnimProperties  gunAnimProps;


        [Header("REFS")]
        public SpriteRenderer bodySR;
        public SpriteRenderer facingArrowSR;
        public SpriteRenderer[] shootableIconSRs;


        public bool IsActionAnimPlaying => (CurrentState == State.Melee || CurrentState == State.Gun);
        public State CurrentState {
            get => _currentState;
            set {
                if (_currentState != value) {
                    _currentState = value;
                    OnStateChanged();
                }
            }
        }

        Dictionary<State, Sprite[]> _spritesOfState = new Dictionary<State, Sprite[]>();

        State _currentState;
        Coroutine _currentSeqImgAnim = null;

        // Components
        Player _player;
        PlayerMoveManager   _moveManager;
        PlayerAttackManager _attackManager;

        void Awake () {
            _player = GetComponent<Player>();
            _moveManager = GetComponent<PlayerMoveManager>();
            _attackManager = GetComponent<PlayerAttackManager>();

            LoadSprites();

        }


        void Update () {

            if (IsActionAnimPlaying) {
                facingArrowSR.enabled = false;
            }
            else {
                facingArrowSR.enabled = true;
            }

        }



        public void ClearRangedAttackable () {
            GameSceneManager.current.operatorManager.playerRangedAttackableLineFactory.ClearLines();

            if (shootableIconSRs != null) {
                foreach (SpriteRenderer sr in shootableIconSRs) {
                    sr.enabled = false;
                }
            }
        }

        public void DrawRangedAttackable (Vector2 startPoint, Vector2 targetPoint) {
            GameSceneManager.current.operatorManager.playerRangedAttackableLineFactory.GetLine(startPoint, targetPoint, rangedAttackableLineWidth, rangedAttackableLineColor);

            if (shootableIconSRs != null) {
                foreach (SpriteRenderer sr in shootableIconSRs) {
                    sr.enabled = true;
                }
            }
        }


        public void OnStartWalking () {
            CurrentState = defaultMovingState;
        }

        public void OnStopWalking () {
            if (CurrentState == defaultMovingState)
                CurrentState = State.Idle;
        }

        public void OnStartHiding () {
            bodySR.SetOpacity(hidingOpacity);
        }

        public void OnStopHiding () {
            bodySR.SetOpacity(1f);
        }

        public void OnDie () {
            bodySR.color = Color.red;
        }


        void LoadSprites () {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Player");

            List<Sprite> idleSprites = new List<Sprite>();
            List<Sprite> walkSprites = new List<Sprite>();
            List<Sprite> runSprites = new List<Sprite>();
            List<Sprite> meleeSprites = new List<Sprite>();
            List<Sprite> gunSprites = new List<Sprite>();


            foreach (Sprite sprite in sprites) {
                if (sprite.name.Contains("idle"))
                    idleSprites.Add(sprite);
                else if (sprite.name.Contains("walk"))
                    walkSprites.Add(sprite);
                else if (sprite.name.Contains("run"))
                    runSprites.Add(sprite);
                else if (sprite.name.Contains("melee"))
                    meleeSprites.Add(sprite);
                else if (sprite.name.Contains("gun"))
                    gunSprites.Add(sprite);
            }

            // _spritesOfState.Add(State.Idle, idleSprites.ToArray());
            _spritesOfState.Add(State.Walk, walkSprites.ToArray());
            _spritesOfState.Add(State.Run,  runSprites.ToArray());
            _spritesOfState.Add(State.Melee, meleeSprites.ToArray());
            _spritesOfState.Add(State.Gun, gunSprites.ToArray());
        }


        void OnStateChanged () {

            if (_currentSeqImgAnim != null) {
                StopCoroutine(_currentSeqImgAnim);
                _currentSeqImgAnim = null;
            }

            Sprite[] sprites = null;

            if (_spritesOfState.ContainsKey(CurrentState)) {
                sprites = _spritesOfState[CurrentState];

                float fps = 0f;
                bool loop = false;
                bool pingPong = false;
                SeqImgAnim.AnimEndCallback endCallback = null;

                if (CurrentState == State.Idle) {
                    fps = idleAnimProps.fps;
                    loop = idleAnimProps.loop;
                    pingPong = idleAnimProps.pingPong;
                }
                else if (CurrentState == State.Walk) {
                    fps = walkAnimProps.fps;
                    loop = walkAnimProps.loop;
                    pingPong = walkAnimProps.pingPong;
                }
                else if (CurrentState == State.Run) {
                    fps = runAnimProps.fps;
                    loop = runAnimProps.loop;
                    pingPong = runAnimProps.pingPong;
                }
                else if (CurrentState == State.Melee) {
                    fps = sprites.Length / meleeAnimProps.duration;
                    loop = meleeAnimProps.loop;
                    pingPong = meleeAnimProps.pingPong;
                    endCallback = () => {
                        CurrentState = State.Idle;
                    };
                }
                else if (CurrentState == State.Gun) {
                    fps = sprites.Length / gunAnimProps.duration;
                    loop = gunAnimProps.loop;
                    pingPong = gunAnimProps.pingPong;
                    endCallback = () => {
                        CurrentState = State.Idle;
                    };
                }

                _currentSeqImgAnim = StartCoroutine(SeqImgAnim.Anim(bodySR, sprites, fps, loop, pingPong, endCallback));
            }
            else if (CurrentState == State.Idle) {
                Sprite[] walkAnimSprites = _spritesOfState[State.Walk];

                if (!walkAnimSprites.Contains(bodySR.sprite)) {
                    bodySR.sprite = walkAnimSprites[walkAnimSprites.Length - 1];
                }
            }

        }

    }
}
