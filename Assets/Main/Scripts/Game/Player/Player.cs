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
        public bool IsCrouching {get; private set;} = false;
        public bool IsHiding {get; private set;} = false;
        public bool IsDead {get; private set;} = false;


        // Components
        Rigidbody2D _rigidbody;
        PlayerAttackManager _attackManager;
        // TargetedByEnemies _targetedByEmenies;


        protected override void Awake () {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
            _attackManager = GetComponent<PlayerAttackManager>();
            // _targetedByEmenies = GetComponent<TargetedByEnemies>();

            deathText.enabled = false;
        }



        void FixedUpdate () {

            if (IsCrouching && TerrainManager.current.IsInHidingArea(transform.position)) {
                IsHiding = true;
            }
            else {
                IsHiding = false;
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
                IsDead = true;
                deathText.enabled = true;
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
