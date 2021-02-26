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
        public float meleeDistance = 1f;
        public float meleeCooldownTime = 1f;
        public float rangedDistance = 1f;
        public float rangedCooldownTime = 1f;
        public float requiredAimingDuration = 1f;
        public LayerMask targetDetectLayerMask;
        public float aimingFinalFovRate = 0.5f;

        [Header("REFS")]
        public Transform targetDetectStartPoint;


        public Enemy CurrentTarget {get; private set;}
        public bool IsAiming {get; private set;}
        public AttackMethod AvailableAttackMethod {
            get {
                if (CurrentTarget == null) {
                    return AttackMethod.None;
                }
                else {
                    if ( (CurrentTarget.transform.position - transform.position).sqrMagnitude < Mathf.Pow(meleeDistance, 2) )
                        return AttackMethod.Melee;
                    else
                        return AttackMethod.Ranged;
                }
            }
        }

        // Components
        Player _player;
        PlayerAnimManager _animManager;

        Dictionary<AttackMethod, float> _lastestAttackStartTimeOfAttackMethods = new Dictionary<AttackMethod, float>() {
            { AttackMethod.Melee, 0f },
            { AttackMethod.Ranged, 0f }
        };
        Dictionary<AttackMethod, float> _cooldownTimeOfAttackMethods = new Dictionary<AttackMethod, float>();

        float _lastestAimingStartTime = 0f;


        void Awake () {
            _player = GetComponent<Player>();
            _animManager = GetComponent<PlayerAnimManager>();

            _cooldownTimeOfAttackMethods.Add(AttackMethod.Melee, meleeCooldownTime);
            _cooldownTimeOfAttackMethods.Add(AttackMethod.Ranged, rangedCooldownTime);
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


            if (IsAiming) {

                // follow
                _player.SetFacing( ((Vector2) (CurrentTarget.transform.position - transform.position)).normalized );

                float progessRate = Mathf.Min((Time.time - _lastestAimingStartTime) / requiredAimingDuration, 1f);
                _player.visionSpan.FovRateApplied = (1f - progessRate) * (1f - aimingFinalFovRate) + aimingFinalFovRate;

                // === temp ===
                if (_animManager != null) {
                    _animManager.aimingProcessText.text = (int) (progessRate * 100f) + "%";
                }
                // === ==== ===
            }
            else {
                _player.visionSpan.FovRateApplied = 1f;
            }
        }


        public void TryToAttack () {
            AttackMethod atkMethod = AvailableAttackMethod;

            if (atkMethod == AttackMethod.None)
                return;

            if (_lastestAttackStartTimeOfAttackMethods[atkMethod] == 0 || _lastestAttackStartTimeOfAttackMethods[atkMethod] > _cooldownTimeOfAttackMethods[atkMethod]) {

                if (atkMethod == AttackMethod.Melee) {
                    Attack(atkMethod);
                }
                else if (atkMethod == AttackMethod.Ranged) {
                    IsAiming = true;
                    OnStartAiming();
                }
            }
        }


        public void TryToReleaseAiming () {
            if (IsAiming) {
                if (Time.time - _lastestAimingStartTime > requiredAimingDuration) {
                    Attack(AttackMethod.Ranged);
                }

                IsAiming = false;
                OnStopAiming();
            }
        }

        public void TryToCancelAiming () {
            if (IsAiming) {
                IsAiming = false;
                OnStopAiming();
            }
        }



        void Attack (AttackMethod atkMethod) {

            _lastestAttackStartTimeOfAttackMethods[atkMethod] = Time.time;
            CurrentTarget.Attacked(atkMethod);

            if (atkMethod == AttackMethod.Melee) {
                GameSceneManager.current.PlayMeleeAttackOverlayFX();
            }
            else if (atkMethod == AttackMethod.Ranged) {
                GameSceneManager.current.PlayRangedAttackOverlayFX();
                OnStopAiming();
            }
        }


        void OnTargetChanged () {
            if (IsAiming) {
                IsAiming = false;
                OnStopAiming();
            }
        }

        void OnStartAiming () {
            _lastestAimingStartTime = Time.time;

            if (_animManager != null) {
                _animManager.Play(PlayerAnimManager.State.Aiming);
            }
        }

        void OnStopAiming () {
            if (_animManager != null) {
                _animManager.Play(PlayerAnimManager.State.Idle);
            }
        }

    }
}
