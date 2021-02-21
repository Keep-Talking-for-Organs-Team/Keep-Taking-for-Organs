using UnityEngine;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    public class EnemyAttackManager : MonoBehaviour {

        public float attackDistance;
        public float attackCooldownTime;


        float _lastestAttackStartTime = 0f;

        // Components
        EnemyAnimManager _animManager;

        void Awake () {
            _animManager = GetComponent<EnemyAnimManager>();
        }


        public bool IsInRange (Vector2 targetPos) {

            return (targetPos - (Vector2) transform.position).sqrMagnitude < Mathf.Pow(attackDistance, 2);
        }


        public void TryToAttack () {

            if (Time.time - _lastestAttackStartTime > attackCooldownTime) {
                Attack();
            }

        }


        void Attack () {
            _lastestAttackStartTime = Time.time;

            if (_animManager != null) {
                _animManager.Play(EnemyAnimManager.State.Attacking);
            }

            GameSceneManager.current.PlayAttackedOverlayFX();
            Player.current.Die();
        }

    }
}
