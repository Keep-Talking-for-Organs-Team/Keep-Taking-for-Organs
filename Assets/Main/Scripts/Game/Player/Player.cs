using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class Player : SingletonMonoBehaviour<Player> {


        // public static Player current => instance != null ? (Player) instance : null;


        [Header("Options")]
        public bool isInvincible = false;

        [Header("Properties")]
        public VisionSpan.SpanProps walkVisionSpanProps;
        public VisionSpan.SpanProps crouchVisionSpanProps;

        public Vector2 initDir = Vector2.up;


        [Header("REFS")]
        public VisionSpan visionSpan;
        public Text deathText;


        public Vector2 FacingDirection => transform.rotation * initDir;
        public bool IsCrouching => _isCrouching;
        public bool IsHiding => _isHiding;
        public bool IsDead => _isDead;


        // Components
        Rigidbody2D _rigidbody;
        TargetedByEnemies _targetedByEmenies;

        bool    _isCrouching = false;
        bool    _isHiding = false;
        bool    _isDead = false;

        protected override void Awake () {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
            _targetedByEmenies = GetComponent<TargetedByEnemies>();

            deathText.enabled = false;
        }



        void FixedUpdate () {

            if (_isCrouching && TerrainManager.current.IsInHidingArea(transform.position)) {
                _isHiding = true;
            }
            else {
                _isHiding = false;
            }

        }

        void Update () {

            // ==== temp ====
            // Camera Follow
            GameSceneManager.current.mainCam.transform.SetPosXY(transform.position);
            // ==== ==== ====


#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.Q)) {
                Die();
            }

#endif

        }

        public void Die () {
            if (!isInvincible) {
                _isDead = true;
                deathText.enabled = true;
            }
        }


        public void SetFacing (Vector2 dir) {
            transform.rotation = Quaternion.FromToRotation(initDir, dir);
            visionSpan.SetFacingDirection(dir);
        }

        public void ToggleCrouch () {
            _isCrouching = !_isCrouching;

            if (_isCrouching) {
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
