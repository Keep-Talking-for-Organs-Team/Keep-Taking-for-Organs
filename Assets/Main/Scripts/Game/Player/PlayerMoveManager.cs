using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerMoveManager : MonoBehaviour {

        [Header("Properties")]
        public float walkSpeed;
        public float crouchSpeed;

        [Header("Parameters")]
        public LayerMask moveCollisionLayerMask;


        public bool IsWalking {
            get => _isWalking;
            set {
                if (_isWalking != value) {
                    _isWalking = value;

                    if (_isWalking)
                        _player.OnStartWalking();
                    else
                        _player.OnStopWalking();
                }
            }
        }

        bool _isWalking = false;

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

            bool isWalkingThisFrame = false;

            if (_player.IsMovable) {

                float speed = walkSpeed;
                if (_player.IsCrouching) {
                    speed = crouchSpeed;
                }

                if (_rigidbody != null && _controlManager.MoveDirection != Vector2.zero) {

                    Vector2 deltaPos = PhysicsTools2D.GetFinalDeltaPosAwaringObstacle(_rigidbody, _controlManager.MoveDirection, speed * Time.fixedDeltaTime * Time.timeScale, moveCollisionLayerMask);
                    _rigidbody.MovePosition(_rigidbody.position + deltaPos);

                    isWalkingThisFrame = true;
                }

            }

            IsWalking = isWalkingThisFrame;
        }

    }
}
