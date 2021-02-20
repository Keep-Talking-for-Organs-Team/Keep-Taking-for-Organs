using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class Player : MonoBehaviour {


        public static Player current;


        [Header("Properties")]
        public VisionSpan.SpanProps walkVisionSpanProps;
        public VisionSpan.SpanProps crouchVisionSpanProps;

        public Vector2 initDir = Vector2.up;


        [Header("REFS")]
        public VisionSpan visionSpan;


        public Vector2 DirToMouse => (GameSceneManager.current != null) ? (CameraTools.GetMouseWorldPosition(GameSceneManager.current.mainCam) - (Vector2) transform.position).normalized : Vector2.zero;
        public bool IsCrouching => _isCrouching;
        public bool IsHiding => _isHiding;


        // Components
        Rigidbody2D _rigidbody;
        TargetedByEnemies _targetedByEmenies;

        Vector2 _dir;
        bool    _isCrouching = false;
        bool    _isHiding = false;

        void Awake () {
            ComponentsTools.SetAndKeepAttachedGameObjectUniquely<Player>(ref current, this);

            _rigidbody = GetComponent<Rigidbody2D>();
            _targetedByEmenies = GetComponent<TargetedByEnemies>();
        }

        void OnDestroy () {
            current  = null;
        }


        void FixedUpdate () {
            // // === temp === ///
            // Vector2 moveDir = GameSceneManager.current.mainCam.transform.rotation * (SimpleInput.GetAxisRaw("Horizontal") * Vector2.right + SimpleInput.GetAxisRaw("Vertical") * Vector2.up).normalized;
            // // === ==== === ///
            //
            // if (_rigidbody != null) {
            //     _rigidbody.MovePosition(_rigidbody.position + moveDir * walkSpeed * Time.fixedDeltaTime);
            // }


            if (_isCrouching && TerrainManager.current.IsInHidingArea(transform.position)) {
                _isHiding = true;
            }
            else {
                _isHiding = false;
            }

        }

        void Update () {

            // ==== temp ====
            GameSceneManager.current.mainCam.transform.SetPosXY(transform.position);
            // ==== ==== ====

            _dir = DirToMouse;
            transform.rotation = Quaternion.FromToRotation(initDir, _dir);

            visionSpan.SetFacingDirection(_dir);

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
