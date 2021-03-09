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

        [Header("Properties")]
        public VisionSpan.SpanProps walkVisionSpanProps;
        public VisionSpan.SpanProps crouchVisionSpanProps;

        public Vector2 initDir = Vector2.up;


        [Header("REFS")]
        public VisionSpan visionSpan;


        public Vector2 FacingDirection => transform.rotation * initDir;
        public bool IsCrouching {get; private set;} = false;
        public bool IsHiding {get; private set;} = false;
        public bool IsDead {get; private set;} = false;
        public bool HasGoal {get; private set;} = false;

        public bool IsMovable => GameSceneManager.current.IsMissionOnGoing && !IsDead && (_attackManager != null ? !_attackManager.IsAiming : true);
        public bool IsControllable => GameSceneManager.current.IsMissionOnGoing && !IsDead && Time.timeScale > 0;
        public bool IsFacingControllable => IsControllable && (_attackManager != null ? !_attackManager.IsAiming : true);


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
                    GameSceneManager.current.PlayAttackedOverlayFX();
                    Die();
                }

                if (IsCrouching && GameSceneManager.current.currentTerrain.IsInHidingArea(transform.position)) {
                    IsHiding = true;
                }
                else {
                    IsHiding = false;
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
                HasGoal = true;
                Destroy(other.gameObject);
            }
            else if (other.tag == "Exit") {
                if (HasGoal) {
                    GameSceneManager.current.MissionSuccess();
                }
            }
        }

        public void Die () {
            if (!isInvincible) {
                IsDead = true;

                if (_animManager != null) {
                    _animManager.PlayAction(PlayerAnimManager.ActionState.Dead);
                }

                GameSceneManager.current.MissionFailed();
            }

        }


        public void SetFacing (Vector2 dir) {
            transform.rotation = Quaternion.FromToRotation(initDir, dir);
            visionSpan.SetFacingDirection(dir);
        }

        public void ToggleCrouch () {
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

    }
}
