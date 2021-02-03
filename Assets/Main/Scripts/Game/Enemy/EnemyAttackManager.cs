using UnityEngine;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    public class EnemyAttackManager : MonoBehaviour {

        public float attackDistance;
        public float attackCooldownTime;


        float _lastestAttackStartTime = 0f;


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
            print("Enemy attacked!");
        }

    }
}
