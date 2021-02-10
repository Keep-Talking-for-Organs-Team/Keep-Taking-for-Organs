using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Player))]
    public class PlayerMoveManager : MonoBehaviour {

        public float walkSpeed;
        public float crouchSpeed;


        public bool IsMovable => true;

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
                    _rigidbody.MovePosition(_rigidbody.position + _controlManager.MoveDirection * speed * Time.fixedDeltaTime);
                }

            }
        }

    }
}
