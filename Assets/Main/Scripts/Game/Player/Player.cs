using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class Player : MonoBehaviour {


        public static Player current;


        public float sprintSpeed;
        public float walkSpeed;
        public float crouchSpeed;

        public Vector2 initDir = Vector2.up;

        [Header("REFS")]
        public VisionSpan visionSpan;


        public Vector2 DirToMouse => (GameSceneManager.current != null) ? (CameraTools.GetMouseWorldPosition(GameSceneManager.current.mainCam) - (Vector2) transform.position).normalized : Vector2.zero;


        // Components
        Rigidbody2D _rigidbody;

        Vector2 _dir;

        void Awake () {
            ComponentsTools.SetAndKeepAttachedGameObjectUniquely<Player>(ref current, this);

            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnDestroy () {
            current  = null;
        }


        void FixedUpdate () {
            // === temp === ///
            Vector2 moveDir = GameSceneManager.current.mainCam.transform.rotation * (SimpleInput.GetAxisRaw("Horizontal") * Vector2.right + SimpleInput.GetAxisRaw("Vertical") * Vector2.up).normalized;
            // === ==== === ///

            if (_rigidbody != null) {
                _rigidbody.MovePosition(_rigidbody.position + moveDir * walkSpeed * Time.fixedDeltaTime);
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



        public bool IsInVision (Vector2 position) {
            return visionSpan.IsInSight(position);
        }

    }
}
