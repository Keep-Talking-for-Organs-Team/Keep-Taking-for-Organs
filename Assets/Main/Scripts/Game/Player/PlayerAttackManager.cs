using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class PlayerAttackManager : MonoBehaviour {

        [Header("Properties")]
        public float meleeDistance = 1f;
        public float meleeCooldownTime = 1f;
        public LayerMask meleeLayerMask;

        // Components
        Player _player;

        float _lastestMeleeAttackStartTime = 0f;

        void Awake () {
            _player = GetComponent<Player>();
        }


        public void TryToMeleeAttack () {
            if (_lastestMeleeAttackStartTime == 0 || Time.time - _lastestMeleeAttackStartTime > meleeCooldownTime) {
                MeleeAttack();
            }
        }


        void MeleeAttack () {
            _lastestMeleeAttackStartTime = Time.time;

            GameSceneManager.current.PlayMeleeAttackOverlayFX();

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _player.FacingDirection, meleeDistance, meleeLayerMask);

            foreach (var hit in hits) {
                if (hit.collider != null) {
                    EnemyAttackedHandler enemyAttackedHandker = hit.collider.gameObject.GetComponent<EnemyAttackedHandler>();

                    if (enemyAttackedHandker != null) {
                        enemyAttackedHandker.enemy.AttackedViaMelee();
                    }
                }
            }
        }
    }
}
