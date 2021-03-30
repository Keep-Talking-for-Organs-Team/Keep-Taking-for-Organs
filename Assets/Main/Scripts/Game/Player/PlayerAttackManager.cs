using Math = System.Math;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerAttackManager : MonoBehaviour {

        public enum AttackMethod {
            Melee,
            Ranged,
            None
        }

        [Header("Properties")]
        public int   defaultBulletsAmount = -1;
        public float meleeDistance = 1f;
        public float meleeCooldownTime = 1f;
        public float rangedDistance = 1f;
        public float rangedCooldownTime = 1f;
        public float requiredAimingDuration = 1f;
        public LayerMask targetDetectLayerMask;
        public float aimingFinalFovRate = 0.5f;

        [Header("REFS")]
        public Transform targetDetectStartPoint;


        public AttackMethod CurrentWeapon {get; private set;} = AttackMethod.Melee;
        public Enemy        CurrentTarget {get; private set;} = null;

        public int   BulletsLeft {
            get => _bulletsLeft;
            set {
                if (_bulletsLeft != value) {
                    _bulletsLeft = value;
                    GameSceneManager.current.operatorManager.hudManager.rangedDisplay.UpdateBulletsDisplay(_bulletsLeft);
                }
            }
        }

        int _bulletsLeft = -1;

        Dictionary<AttackMethod, float> _lastestAttackStartTimeOfAttackMethods = new Dictionary<AttackMethod, float>() {
            { AttackMethod.Melee, 0f },
            { AttackMethod.Ranged, 0f }
        };
        Dictionary<AttackMethod, float> _cooldownTimeOfAttackMethods = new Dictionary<AttackMethod, float>();


        // Components
        Player _player;
        PlayerAnimManager _animManager;


        void Awake () {
            _player = GetComponent<Player>();
            _animManager = GetComponent<PlayerAnimManager>();

            _cooldownTimeOfAttackMethods.Add(AttackMethod.Melee, meleeCooldownTime);
            _cooldownTimeOfAttackMethods.Add(AttackMethod.Ranged, rangedCooldownTime);

            BulletsLeft = defaultBulletsAmount;
        }

        void Start () {
            // bullet amount old way
            // if (GameSceneManager.current != null && GameSceneManager.current.operatorManager != null) {
            //
            //     int[] dividers = GameSceneManager.current.operatorManager.differentBulletsAmountIntervalDividers;
            //
            //     if (dividers != null) {
            //         int amount = 0;
            //
            //         for (int i = 0 ; i < dividers.Length; i++) {
            //             if (GameSceneManager.current.EnemiesSpawnedCount >= dividers[i]) {
            //                 amount = i + 1;
            //             }
            //         }
            //
            //         BulletsLeft = amount;
            //     }
            // }
        }

        void FixedUpdate () {
            RaycastHit2D hit = Physics2D.Raycast(targetDetectStartPoint.position, _player.FacingDirection, rangedDistance, targetDetectLayerMask);


#if UNITY_EDITOR
            Vector2 endPoint = hit.point;
            if (hit.collider == null) {
                endPoint = targetDetectStartPoint.position + (Vector3) _player.FacingDirection * rangedDistance;
            }
            Debug.DrawLine(targetDetectStartPoint.position, endPoint, Color.blue);
#endif


            Enemy newTarget = null;

            if (hit.collider != null) {
                EnemyAttackedHandler enemyAttackedHandker = hit.collider.gameObject.GetComponent<EnemyAttackedHandler>();

                if (enemyAttackedHandker != null && !enemyAttackedHandker.enemy.IsDead) {
                    newTarget = enemyAttackedHandker.enemy;
                }
            }

            if (CurrentTarget != newTarget) {
                OnTargetChanged();
                CurrentTarget = newTarget;
            }

        }

        void Update () {

            if (_animManager != null)
                _animManager.ClearRangedAttackable();

            if (IsAttackable(AttackMethod.Ranged)) {
                CurrentTarget.IsTargetedByPlayer();

                if (_animManager != null)
                    _animManager.DrawRangedAttackable(transform.position, CurrentTarget.transform.position);
            }

            HUDManager hudManager = GameSceneManager.current.operatorManager.hudManager;
            hudManager.UpdateWeaponStatusDisplay(AttackMethod.Melee, GetCurrentCooldownTimeRemainedRate(AttackMethod.Melee));
            hudManager.UpdateWeaponStatusDisplay(AttackMethod.Ranged, GetCurrentCooldownTimeRemainedRate(AttackMethod.Ranged), BulletsLeft);

        }



        public void TryToAttack (AttackMethod atkMethod) {
            if (atkMethod == AttackMethod.None)
                return;


            if (IsAttackable(atkMethod)) {
                if (atkMethod == AttackMethod.Ranged && BulletsLeft == 0) {
                    // Out of Ammo
                    GameSceneManager.current.operatorManager.PlayOutOfAmmoOverlayFX();
                }
                else {
                    Attack(atkMethod);
                }
            }
        }

        public float GetCurrentCooldownTimeLeft (AttackMethod atkMethod) {
            if (_lastestAttackStartTimeOfAttackMethods[atkMethod] == 0) {
                return 0f;
            }
            return Mathf.Max(_cooldownTimeOfAttackMethods[atkMethod] - (Time.time - _lastestAttackStartTimeOfAttackMethods[atkMethod]), 0f);
        }

        public float GetCurrentCooldownTimeRemainedRate (AttackMethod atkMethod) {
            return GetCurrentCooldownTimeLeft(atkMethod) / _cooldownTimeOfAttackMethods[atkMethod];
        }

        public bool IsAttackMethodActive (AttackMethod atkMethod) {
            if (atkMethod == AttackMethod.None)
                return false;

            return _lastestAttackStartTimeOfAttackMethods[atkMethod] == 0 || Time.time - _lastestAttackStartTimeOfAttackMethods[atkMethod] > _cooldownTimeOfAttackMethods[atkMethod];
        }

        public void RemoveWeaponsCooldown () {
            _cooldownTimeOfAttackMethods[AttackMethod.Melee] = 0f;
            _cooldownTimeOfAttackMethods[AttackMethod.Ranged] = 0f;
        }


        bool IsAttackable (AttackMethod atkMethod) {

            if (!_player.IsControllable)
                return false;


            bool isReady = _lastestAttackStartTimeOfAttackMethods[atkMethod] == 0 || Time.time - _lastestAttackStartTimeOfAttackMethods[atkMethod] > _cooldownTimeOfAttackMethods[atkMethod];

            if (atkMethod == AttackMethod.Melee)
                return isReady;


            if (CurrentTarget == null)
                return false;

            bool isInRange = false;
            bool hasBullets = true;

            if (atkMethod == AttackMethod.Ranged) {
                isInRange = IsInRange(atkMethod, CurrentTarget.transform.position);
                hasBullets = BulletsLeft != 0;
            }

            return isReady && isInRange && hasBullets;
        }

        bool IsInRange (AttackMethod atkMethod, Vector2 targetPos) {
            float rangeDistance = 0f;

            if (atkMethod == AttackMethod.Melee)
                rangeDistance = meleeDistance;
            else if (atkMethod == AttackMethod.Ranged)
                rangeDistance = rangedDistance;

            return (targetPos - (Vector2) transform.position).sqrMagnitude < rangeDistance * rangeDistance;
        }

        void Attack (AttackMethod atkMethod) {

            _lastestAttackStartTimeOfAttackMethods[atkMethod] = Time.time;

            if (atkMethod == AttackMethod.Melee) {
                _player.OnMeleeAttack();
            }
            else if (atkMethod == AttackMethod.Ranged) {
                if (BulletsLeft > 0) {
                    BulletsLeft--;
                }
                _player.OnRangedAttack();
            }

            if (CurrentTarget != null && IsInRange(atkMethod, CurrentTarget.transform.position))
                CurrentTarget.Attacked(atkMethod);
        }


        void OnTargetChanged () {

        }


    }
}
