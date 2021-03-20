using System.Collections;

using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerControlManager : MonoBehaviour {

        public Vector2 DirToMouse => (GameSceneManager.current != null && GameSceneManager.current.operatorManager != null) ? (CameraTools.GetMouseWorldPosition(GameSceneManager.current.operatorManager.cam) - (Vector2) transform.position).normalized : Vector2.zero;
        public Vector2 MoveDirection {get; private set;}


        // Components
        Player _player;
        PlayerAttackManager _attackManager;


        void Awake () {
            _player = GetComponent<Player>();
            _attackManager = GetComponent<PlayerAttackManager>();
        }

        void LateUpdate () {

            if (!_player.IsControllable) {
                MoveDirection = Vector2.zero;
            }
            else {

                if (_player.IsFacingControllable) {

                    if (GlobalManager.current.IsMouseMoving) {
                        _player.SetFacing(DirToMouse);
                    }
                    else {
                        Vector2 facingDirFromGamepad = GameSceneManager.current.operatorManager.cam.transform.rotation * (Vector2.right * Input.GetAxis("Horizontal2") + Vector2.down * Input.GetAxis("Vertical2")).normalized;

                        if (facingDirFromGamepad != Vector2.zero) {
                            _player.SetFacing(facingDirFromGamepad);
                        }
                    }

                }

                MoveDirection = GameSceneManager.current.operatorManager.cam.transform.rotation * (Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical")).normalized;


                // === temp ===
                if (Input.GetKeyDown(KeyCode.C)) {
                    _player.ToggleCrouch();
                }
                // === ==== ===

                if (Input.GetButtonDown("Fire1")) {
                    _attackManager.TryToAttack(PlayerAttackManager.AttackMethod.Melee);
                }
                else if (Input.GetButtonDown("Fire2")) {
                    _attackManager.TryToAttack(PlayerAttackManager.AttackMethod.Ranged);
                }
            }

        }

    }
}
