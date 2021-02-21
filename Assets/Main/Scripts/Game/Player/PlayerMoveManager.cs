using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Player))]
    public class PlayerMoveManager : MonoBehaviour {

        [Header("Properties")]
        public float walkSpeed;
        public float crouchSpeed;

        [Header("Parameters")]
        public LayerMask moveCollisionLayerMask;

        public bool IsMovable => !_player.IsDead;

        // Components
        Player _player;
        PlayerControlManager _controlManager;
        Rigidbody2D _rigidbody;

        void Awake () {
            _player = GetComponent<Player>();
            _controlManager = GetComponent<PlayerControlManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate () {
            if (_controlManager != null && _controlManager.IsControllable) {

                float speed = walkSpeed;
                if (_player.IsCrouching) {
                    speed = crouchSpeed;
                }

                if (_rigidbody != null) {

                    Vector2 deltaPos = PhysicsTools2D.GetFinalDeltaPosAwaringObstacle(_rigidbody, _controlManager.MoveDirection, speed * Time.fixedDeltaTime * Time.timeScale, moveCollisionLayerMask);

                    _rigidbody.MovePosition(_rigidbody.position + deltaPos);
                }

            }
        }

    }
}
