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
        public float                maxAngularSpeed = 180f;

        public Vector2 initDir = Vector2.up;


        [Header("REFS")]
        public VisionSpan visionSpan;

        [Header("Prefabs")]
        public GameObject lightningFXPrefab;

        public Vector2 FacingDirection => transform.rotation * initDir;
        public bool IsCrouching {get; private set;} = false;
        public bool IsDead {get; private set;} = false;
        public bool HasGoal {get; private set;} = false;
        public bool IsHiding {
            get => _isHiding;
            set {
                if (_isHiding != value) {
                    _isHiding = value;

                    if (_isHiding)
                        OnStartHiding();
                    else
                        OnStopHiding();
                }
            }
        }

        public bool IsMovable => GameSceneManager.current.operatorManager.IsMissionOnGoing && !IsDead && (_animManager != null ? !_animManager.IsActionAnimPlaying : true);
        public bool IsControllable => GameSceneManager.current.operatorManager.IsMissionOnGoing && !IsDead && Time.timeScale > 0 && !GameSceneManager.current.inGameMenu.activeSelf && (_animManager != null ? !_animManager.IsActionAnimPlaying : true);
        public bool IsFacingControllable => IsControllable;


        bool _isHiding = false;
        Coroutine _currentTrapFX;

        // Components
        Rigidbody2D         _rigidbody;
        PlayerAnimManager   _animManager;
        PlayerMoveManager   _moveManager;
        PlayerAttackManager _attackManager;
        // TargetedByEnemies _targetedByEmenies;


        protected override void Awake () {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animManager = GetComponent<PlayerAnimManager>();
            _moveManager = GetComponent<PlayerMoveManager>();
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

                    Die(OperatorManager.FailedReason.Trap);
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

            // Camera Follow
            GameSceneManager.current.operatorManager.cam.transform.SetPosXY(transform.position);

        }

        void OnTriggerEnter2D (Collider2D other) {
            if (other.tag == "Goal") {
                if (!HasGoal) {
                    HasGoal = true;
                    Destroy(other.gameObject);

                    GameSceneManager.current.operatorManager.OnPlayerGetGoal();

                    AkSoundEngine.PostEvent("Play_Get_Organ", gameObject);
                }
            }
            else if (other.tag == "Exit") {
                if (HasGoal) {
                    GameSceneManager.current.operatorManager.MissionSuccess();
                }
            }
        }

        public void Die (OperatorManager.FailedReason reason = OperatorManager.FailedReason.None) {
            if (!isInvincible) {
                IsDead = true;

                if (_animManager != null) {
                    _animManager.OnDie();
                }

                GameSceneManager.current.operatorManager.MissionFailed(reason);

                AkSoundEngine.PostEvent("Play_Player_Death" , gameObject);
            }

        }


        public void SetFacing (Vector2 dir, float timestep) {
            float ToDestAngle = Vector2.SignedAngle(FacingDirection, dir);
            float movedAngleMagnitude = Mathf.Min(Mathf.Abs(ToDestAngle), maxAngularSpeed * timestep);

            transform.rotation = Quaternion.AngleAxis(Mathf.Sign(ToDestAngle) * movedAngleMagnitude, Vector3.forward) * transform.rotation;
            visionSpan.SetFacingDirection(FacingDirection);

            // transform.rotation = Quaternion.FromToRotation(initDir, dir);
            // visionSpan.SetFacingDirection(dir);
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

            GameSceneManager.current.operatorManager.PlayAttackedOverlayFX();

            GameObject lightningFX = Instantiate(lightningFXPrefab, transform.position, Quaternion.identity, transform);
            AkSoundEngine.PostEvent("Play_Ele_Trap" , gameObject);

            yield return new WaitForSeconds(lightningFXDuration);

            Destroy(lightningFX);
            _currentTrapFX = null;
        }



        public void OnStartWalking () {
            AkSoundEngine.PostEvent("Play_Player_Footstep" , gameObject);

            if (_animManager != null)
                _animManager.OnStartWalking();
        }

        public void OnStopWalking () {
            AkSoundEngine.PostEvent("Stop_Player_Footstep" , gameObject);

            if (_animManager != null)
                _animManager.OnStopWalking();
        }

        public void OnStartHiding () {
            AkSoundEngine.PostEvent("Play_Start_Hiding" , gameObject);

            if (_animManager != null)
                _animManager.OnStartHiding();
        }

        public void OnStopHiding () {
            AkSoundEngine.PostEvent("Play_End_Hiding" , gameObject);

            if (_animManager != null)
                _animManager.OnStopHiding();
        }


        public void OnMeleeAttack () {
            if (_animManager != null)
                _animManager.CurrentState = PlayerAnimManager.State.Melee;

            // GameSceneManager.current.operatorManager.PlayMeleeAttackOverlayFX();
            AkSoundEngine.PostEvent("Play_Player_Saber" , gameObject);
        }

        public void OnRangedAttack () {
            if (_animManager != null)
                _animManager.CurrentState = PlayerAnimManager.State.Gun;

            // GameSceneManager.current.operatorManager.PlayRangedAttackOverlayFX();
            AkSoundEngine.PostEvent("Play_Player_Gunshot" , gameObject);
        }

    }
}
