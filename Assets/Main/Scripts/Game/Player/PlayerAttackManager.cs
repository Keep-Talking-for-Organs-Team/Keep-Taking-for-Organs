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
        public SpriteRenderer rangedRangeIndicator;

        [Header("Output Shows")]
        public float meleeCooldown = 0f;
        public float rangedCooldown = 0f;


        public bool  IsTakingRanged {get; private set;} = false;
        public Enemy CurrentTarget {get; private set;} = null;
        public int   BulletsLeft {get; private set;} = -1;

        public bool  IsAiming {
            get => _isAiming;
            set {
                bool oldValue = _isAiming;
                _isAiming = value;

                if (oldValue != value) {
                    if (value)
                        OnStartAiming();
                    else
                        OnStopAiming();
                }
            }
        }

        public AttackMethod AvailableAttackMethod {
            get {
                if (CurrentTarget != null) {

                    AttackMethod method = AttackMethod.None;

                    if (IsTakingRanged) {
                        method = AttackMethod.Ranged;
                    }
                    else if ( (CurrentTarget.transform.position - transform.position).sqrMagnitude < Mathf.Pow(meleeDistance, 2) ) {
                        method = AttackMethod.Melee;
                    }

                    if (method != AttackMethod.None && IsAttackMethodActive(method))
                        return method;
                }

                return AttackMethod.None;
            }
        }

        // Components
        Player _player;
        PlayerAnimManager _animManager;

        bool _isAiming = false;

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

            BulletsLeft = defaultBulletsAmount;
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
                IsAiming = IsAttackMethodActive(AttackMethod.Ranged);
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

        void Update () {

            // === temp ===
            rangedRangeIndicator.enabled = false;
            if (IsTakingRanged) {
                rangedRangeIndicator.enabled = true;
                rangedRangeIndicator.transform.position = targetDetectStartPoint.position + (Vector3) _player.FacingDirection * rangedDistance / 2;
                rangedRangeIndicator.transform.SetScaleY(rangedDistance * 25);
            }
            // === ==== ===

            string[] lines = GameSceneManager.current.hudInfoText.text.Split('\n');
            for (int i = 0 ; i < lines.Length ; i++) {

                string[] parts = lines[i].Split(':');

                if (parts.Length >= 2) {
                    if (i == 0) {
                        lines[i] = parts[0] + ": " + GetCurrentCooldownTimeLeft(AttackMethod.Melee).ToString("0.0");
                    }
                    else if (i == 1) {
                        lines[i] = parts[0] + ": " + GetCurrentCooldownTimeLeft(AttackMethod.Ranged).ToString("0.0");
                    }
                    else if (i == 2) {
                        lines[i] = parts[0] + ":" + (BulletsLeft >= 0 ? BulletsLeft.ToString() : "Inf");
                    }
                }
            }

            GameSceneManager.current.hudInfoText.text = string.Join("\n", lines);

#if UNITY_EDITOR
            meleeCooldown = GetCurrentCooldownTimeLeft(AttackMethod.Melee);
            rangedCooldown = GetCurrentCooldownTimeLeft(AttackMethod.Ranged);
#endif
        }


        public void PickRanged () {
            if (GetCurrentCooldownTimeLeft(AttackMethod.Ranged) == 0) {
                IsTakingRanged = true;
            }
        }

        public void DropRanged () {
            IsTakingRanged = false;
            IsAiming = false;
        }

        public void TryToAttack () {
            AttackMethod atkMethod = AvailableAttackMethod;

            if (atkMethod == AttackMethod.None)
                return;

            if (atkMethod == AttackMethod.Melee) {
                Attack(atkMethod);
            }
            else if (atkMethod == AttackMethod.Ranged) {
                if (BulletsLeft != 0) {
                    IsAiming = true;
                }
                else {
                    GameSceneManager.current.PlayOutOfAmmoOverlayFX();
                }
            }
        }

        public void TryToReleaseAiming () {
            if (IsAiming) {
                if (Time.time - _lastestAimingStartTime > requiredAimingDuration) {
                    Attack(AttackMethod.Ranged);
                }

                IsAiming = false;
            }
        }

        public void TryToCancelAiming () {
            if (IsAiming) {
                IsAiming = false;
            }
        }

        public float GetCurrentCooldownTimeLeft (AttackMethod atkMethod) {
            if (_lastestAttackStartTimeOfAttackMethods[atkMethod] == 0) {
                return 0f;
            }
            return Mathf.Max(_cooldownTimeOfAttackMethods[atkMethod] - (Time.time - _lastestAttackStartTimeOfAttackMethods[atkMethod]), 0f);
        }

        public bool IsAttackMethodActive (AttackMethod atkMethod) {
            if (atkMethod == AttackMethod.None)
                return false;

            return _lastestAttackStartTimeOfAttackMethods[atkMethod] == 0 || Time.time - _lastestAttackStartTimeOfAttackMethods[atkMethod] > _cooldownTimeOfAttackMethods[atkMethod];
        }



        void Attack (AttackMethod atkMethod) {

            _lastestAttackStartTimeOfAttackMethods[atkMethod] = Time.time;
            CurrentTarget.Attacked(atkMethod);

            if (atkMethod == AttackMethod.Melee) {
                GameSceneManager.current.PlayMeleeAttackOverlayFX();
            }
            else if (atkMethod == AttackMethod.Ranged) {
                if (BulletsLeft > 0) {
                    BulletsLeft--;
                }

                GameSceneManager.current.PlayRangedAttackOverlayFX();
                IsAiming = false;
            }
        }


        void OnTargetChanged () {
            IsAiming = false;
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
