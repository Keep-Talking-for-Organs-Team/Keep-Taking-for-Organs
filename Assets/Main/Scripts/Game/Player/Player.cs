using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class Player : SingletonMonoBehaviour<Player> {



        [Header("Options")]
        public bool isInvincible = false;
        public bool mustCrouchToHide = false;
        public bool isCrouchable = false;

        [Header("Properties")]
        public VisionSpan.SpanProps walkVisionSpanProps;
        public VisionSpan.SpanProps crouchVisionSpanProps;
        public float                lightningFXDuration = 1f;

        public Vector2 initDir = Vector2.up;


        [Header("REFS")]
        public VisionSpan visionSpan;

        [Header("Prefabs")]
        public GameObject lightningFXPrefab;

        public Vector2 FacingDirection => transform.rotation * initDir;
        public bool IsCrouching {get; private set;} = false;
        public bool IsHiding {get; private set;} = false;
        public bool IsDead {get; private set;} = false;
        public bool HasGoal {get; private set;} = false;

        public bool IsMovable => GameSceneManager.current.IsMissionOnGoing && !IsDead;
        public bool IsControllable => GameSceneManager.current.IsMissionOnGoing && !IsDead && Time.timeScale > 0;
        public bool IsFacingControllable => IsControllable;


        Coroutine _currentTrapFX;

        // Components
        PlayerAnimManager _animManager;
        Rigidbody2D _rigidbody;
        PlayerAttackManager _attackManager;
        // TargetedByEnemies _targetedByEmenies;


        protected override void Awake () {
            base.Awake();
            _animManager = GetComponent<PlayerAnimManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _attackManager = GetComponent<PlayerAttackManager>();
            // _targetedByEmenies = GetComponent<TargetedByEnemies>();

        }

        void Start () {
            visionSpan.spanProps = walkVisionSpanProps;
        }

        void FixedUpdate () {

            if (!IsDead) {

                if (GameSceneManager.current.currentTerrain.IsInTrapArea(transform.position)) {
                    if (_currentTrapFX == null)
                        _currentTrapFX = StartCoroutine(TrapFX());
                    Die(GameSceneManager.FailedReason.Trap);
                }

                if (GameSceneManager.current.currentTerrain.IsInHidingArea(transform.position) && (!mustCrouchToHide || IsCrouching)) {
                    if (!IsHiding) {
                        IsHiding = true;
                        OnStartHiding();
                    }
                }
                else {
                    if (IsHiding) {
                        IsHiding = false;
                        OnStopHiding();
                    }
                }
            }
        }

        void Update () {

            // ==== temp ====
            // Camera Follow
            GameSceneManager.current.mainCam.transform.SetPosXY(transform.position);
            // ==== ==== ====

        }

        void OnTriggerEnter2D (Collider2D other) {
            if (other.tag == "Goal") {
                if (!HasGoal) {
                    HasGoal = true;
                    Destroy(other.gameObject);

                    AkSoundEngine.PostEvent("Play_Get_Organ", gameObject);
                }
            }
            else if (other.tag == "Exit") {
                if (HasGoal) {
                    GameSceneManager.current.MissionSuccess();
                }
            }
        }

        public void Die (GameSceneManager.FailedReason reason = GameSceneManager.FailedReason.None) {
            if (!isInvincible) {
                IsDead = true;

                if (_animManager != null) {
                    _animManager.PlayAction(PlayerAnimManager.ActionState.Dead);
                }

                GameSceneManager.current.MissionFailed(reason);

                AkSoundEngine.PostEvent("Play_Player_Death" , gameObject);
            }

        }


        public void SetFacing (Vector2 dir) {
            transform.rotation = Quaternion.FromToRotation(initDir, dir);
            visionSpan.SetFacingDirection(dir);
        }

        public void ToggleCrouch () {
            if (!isCrouchable)
                return;

            IsCrouching = !IsCrouching;

            if (IsCrouching) {
                visionSpan.spanProps = crouchVisionSpanProps;
            }
            else {
                visionSpan.spanProps = walkVisionSpanProps;
            }
        }

        public bool IsInVision (Vector2 position) {
            return visionSpan.IsInSight(position);
        }


        IEnumerator TrapFX () {

            GameSceneManager.current.PlayAttackedOverlayFX();

            GameObject lightningFX = Instantiate(lightningFXPrefab, transform.position, Quaternion.identity, transform);
            AkSoundEngine.PostEvent("Play_Ele_Trap" , gameObject);

            yield return new WaitForSeconds(lightningFXDuration);

            Destroy(lightningFX);
            _currentTrapFX = null;
        }



        void OnStartHiding () {
            AkSoundEngine.PostEvent("Play_Start_Hiding" , gameObject);
        }

        void OnStopHiding () {
            AkSoundEngine.PostEvent("Play_End_Hiding" , gameObject);
        }

    }
}
