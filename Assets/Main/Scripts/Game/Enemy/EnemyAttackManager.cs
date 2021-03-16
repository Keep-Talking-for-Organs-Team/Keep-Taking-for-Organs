using UnityEngine;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    public class EnemyAttackManager : MonoBehaviour {

        public enum GunType {
            Laser,
            Electric
        }

        [Header("Properties")]
        public GunType gunType;
        public float laserGunAttackDistance = 5f;
        public float electricGunAttackDistance = 5f;
        public float attackCooldownTime;
        public float gunFXDuration = 1f;
        public float playerAttackedFXDelayTime = 0.1f;

        [Header("Prefabs")]
        public GameObject laserGunFX;
        public GameObject electricGunFX;

        public bool HasAttackedPlayer {get; private set;} = false;
        public float AttackDistance {
            get {
                if (gunType == GunType.Laser)
                    return laserGunAttackDistance;
                else if (gunType == GunType.Electric)
                    return electricGunAttackDistance;
                else
                    return 0f;
            }
        }


        float _lastestAttackStartTime = 0f;

        // Components
        EnemyAnimManager _animManager;

        void Awake () {
            _animManager = GetComponent<EnemyAnimManager>();
        }


        public bool IsInRange (Vector2 targetPos) {

            return (targetPos - (Vector2) transform.position).sqrMagnitude < Mathf.Pow(AttackDistance, 2);
        }


        public void TryToAttack () {

            if (_lastestAttackStartTime == 0 || Time.time - _lastestAttackStartTime > attackCooldownTime) {
                Attack();
            }

        }


        void Attack () {
            _lastestAttackStartTime = Time.time;

            HasAttackedPlayer = true;

            if (_animManager != null) {
                _animManager.Play(EnemyAnimManager.State.Attacking);
            }

            GameObject gunFX = null;

            if (gunType == GunType.Laser) {
                gunFX = Instantiate(laserGunFX, transform.position, transform.rotation);
            }
            else if (gunType == GunType.Electric) {
                gunFX = Instantiate(electricGunFX, transform.position, transform.rotation, transform);
            }

            DOTween.Sequence()
                .AppendInterval(gunFXDuration)
                .AppendCallback( () => {
                    if (gunFX != null)
                        Destroy(gunFX);
                } );

            DOTween.Sequence()
                .AppendInterval(playerAttackedFXDelayTime)
                .AppendCallback( () => {
                    GameSceneManager.current.PlayAttackedOverlayFX();

                    if (gunType == GunType.Laser)
                        Player.current.Die(GameSceneManager.FailedReason.LaserGun);
                    else if (gunType == GunType.Electric)
                        Player.current.Die(GameSceneManager.FailedReason.ElectricGun);
                    else
                        Player.current.Die();
                } );

        }

    }
}
