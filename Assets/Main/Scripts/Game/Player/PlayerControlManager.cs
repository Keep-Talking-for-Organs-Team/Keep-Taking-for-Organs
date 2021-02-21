using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Player))]
    public class PlayerControlManager : MonoBehaviour {

        public bool IsControllable => !_player.IsDead;
        public Vector2 DirToMouse => (GameSceneManager.current != null) ? (CameraTools.GetMouseWorldPosition(GameSceneManager.current.mainCam) - (Vector2) transform.position).normalized : Vector2.zero;
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


            _player.SetFacing(DirToMouse);
            
            _moveDir = GameSceneManager.current.mainCam.transform.rotation * (SimpleInput.GetAxisRaw("Horizontal") * Vector2.right + SimpleInput.GetAxisRaw("Vertical") * Vector2.up).normalized;


            // === temp ===
            if (Input.GetKeyDown(KeyCode.C)) {
                _player.ToggleCrouch();
            }
            // === ==== ===

        }

    }
}
