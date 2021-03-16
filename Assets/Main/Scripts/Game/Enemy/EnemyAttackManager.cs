using UnityEngine;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Enemy))]
    public class EnemyAttackManager : MonoBehaviour {

        public float attackDistance;
        public float attackCooldownTime;

        public bool HasAttackedPlayer {get; private set;} = false;

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

<<<<<<< Updated upstream
            GameSceneManager.current.PlayAttackedOverlayFX();
            Player.current.Die();
=======
            GameObject gunFX = null;

            if (gunType == GunType.Laser) {
                gunFX = Instantiate(laserGunFX, transform.position, transform.rotation);
                AkSoundEngine.PostEvent("Play_Robot_LaserGun", gameObject);
            }
            else if (gunType == GunType.Electric) {
                gunFX = Instantiate(electricGunFX, transform.position, transform.rotation, transform);
                AkSoundEngine.PostEvent("Play_Robot_EleGun", gameObject);
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

>>>>>>> Stashed changes
        }

    }
}
