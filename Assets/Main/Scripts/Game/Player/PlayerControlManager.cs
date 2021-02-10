using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Player))]
    public class PlayerControlManager : MonoBehaviour {

        public bool IsControllable => true;
        public Vector2 MoveDirection => _moveDir;

        // Components
        Player _player;
        PlayerMoveManager _moveManager;

        Vector2 _moveDir;

        void Awake () {
            _player = GetComponent<Player>();
            _moveManager = GetComponent<PlayerMoveManager>();
        }

        void Update () {
            if (!IsControllable)
                return;


            _moveDir = GameSceneManager.current.mainCam.transform.rotation * (SimpleInput.GetAxisRaw("Horizontal") * Vector2.right + SimpleInput.GetAxisRaw("Vertical") * Vector2.up).normalized;

            if (Input.GetKeyDown(KeyCode.C)) {
                _player.ToggleCrouch();
            }
        }

    }
}
