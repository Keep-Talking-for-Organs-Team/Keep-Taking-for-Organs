using UnityEngine;

using DG.Tweening;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class Enemy : MonoBehaviour {


        [Header("Options")]
        public bool isInvincible = false;

        [Header("Properties")]
        public bool  isPatrollingEnemy = false;
        public float awareRateIncreaseSpeed;
        public float awareRateDecreaseSpeed;

        public Vector2 defaultDir = Vector2.up;

        [Header("REFS")]
        public SpriteRenderer sr;
        public VisionSpan     visionSpan;

        [Header("Output Shows")]
        public float awareRateShows;


        public Vector2 FacingDirection => transform.rotation * defaultDir;
        public bool IsAlwaysShowed => (GameSceneManager.current.showAllEnemies || (_attackManager != null ? _attackManager.HasAttackedPlayer : false));
        public bool IsDead {
            get => _isDead;
            set {
                bool oldValue = _isDead;
                _isDead = value;

                if (oldValue == false && value == true) {
                    OnDied();
                }
            }
        }


        // Components
        EnemyAnimManager   _animManager;
        EnemyVisionManager _visionManager;
        EnemyMoveManager   _moveManager;
        EnemyAttackManager _attackManager;
        Rigidbody2D        _rigidbody;


        bool _isDead = false;
        float _awareRate = 0f;


        void Awake () {
            _animManager   = GetComponent<EnemyAnimManager>();
            _visionManager = GetComponent<EnemyVisionManager>();
            _moveManager   = GetComponent<EnemyMoveManager>();
            _attackManager = GetComponent<EnemyAttackManager>();
            _rigidbody     = GetComponent<Rigidbody2D>();
        }

        void Start () {
            if (!GameSceneManager.current.showAllEnemies) {
                Hide();
            }
        }


        void FixedUpdate () {

            if (IsAlwaysShowed) {
                Show();
            }

            // Action
            if (!IsDead) {

                // foreach (TargetedByEnemies target in TargetedByEnemies.list) {
                //     // Has spotted target?
                //     if (visionSpan.IsInSight(target.transform.position)) {
                //
                //         if (!_isSpottingPlayer)
                //             print(string.Format("The target \"{0}\" go into the enemy's sight", target.name));
                //
                //         _isSpottingPlayer = true;
                //     }
                //     else {
                //         _isSpottingPlayer = false;
                //     }
                // }

                if (Player.current != null) {
                    Player player = Player.current;

                    // Is spotted by player?
                    if (!IsAlwaysShowed) {
                        if (player.IsInVision(transform.position)) {
                            Show();
                        }
                        else {
                            Hide();
                        }
                    }

                    // Has spotted player?
                    if (!player.IsDead && visionSpan.IsInSight(player.transform.position) && !player.IsHiding) {

                        if (_awareRate >= 1) {

                            if (_attackManager != null) {

                                if (_attackManager.IsInRange(player.transform.position)) {
                                    _attackManager.TryToAttack();
                                }
                                else {
                                    if (_moveManager != null)
                                        _moveManager.Chase(player.transform.position, Time.fixedDeltaTime * Time.timeScale);
                                }
                            }
                        }
                        else {
                            _awareRate = Mathf.Min(_awareRate + awareRateIncreaseSpeed * Time.timeScale * Time.fixedDeltaTime, 1f);
                        }

                        if (_visionManager != null)
                            _visionManager.Target(player.transform.position, Time.fixedDeltaTime * Time.timeScale);
                        if (_moveManager != null)
                            _moveManager.Target(player.transform.position, Time.fixedDeltaTime * Time.timeScale);

                    }
                    else {
                        if (_awareRate > 0) {
                            _awareRate = Mathf.Max(_awareRate - awareRateDecreaseSpeed * Time.timeScale * Time.fixedDeltaTime, 0f);
                        }
                    }

                }

                awareRateShows = _awareRate;
            }
        }

        void Update () {

            if (_animManager != null) {
                if (IsDead) {
                    _animManager.Play(EnemyAnimManager.State.Dead);
                }
                else if (_animManager.CurrentState != EnemyAnimManager.State.Attacking) {
                    if (_awareRate == 1) {
                        if (_animManager.CurrentState != EnemyAnimManager.State.Alert) {
                            _animManager.Play(EnemyAnimManager.State.Alert);
                        }
                    }
                    else if (_awareRate > 0) {
                        if (_animManager.CurrentState != EnemyAnimManager.State.Suspecting) {
                            _animManager.Play(EnemyAnimManager.State.Suspecting);
                        }
                    }
                    else {
                        if (_animManager.CurrentState != EnemyAnimManager.State.None) {
                            _animManager.Play(EnemyAnimManager.State.None);

                        }
                    }
                }

            }

        }


        public void IsTargetedByPlayer () {
            // higthlighted
        }

        public void Attacked (PlayerAttackManager.AttackMethod atkMethod) {
            if (!isInvincible) {
                IsDead = true;
            }
        }


        void Show () {
            if (!sr.gameObject.activeSelf)
                sr.gameObject.SetActive(true);

            visionSpan.isShowingVisionArea = true;
        }

        void Hide () {
            if (sr.gameObject.activeSelf)
                sr.gameObject.SetActive(false);

            visionSpan.isShowingVisionArea = false;
        }


        void OnDied () {

            if (_rigidbody != null) {

                _rigidbody.simulated = false;

                Collider2D[] colliders = new Collider2D[1];
                int collidersCount = _rigidbody.GetAttachedColliders(colliders);

                for (int i = 0 ; i < collidersCount ; i++) {
                    colliders[i].enabled = false;
                }
            }


            int layer = LayerMask.NameToLayer("Default");
            sr.transform.SetLayerRecursively(layer);

            AkSoundEngine.PostEvent("Play Robot Death" , gameObject);
        }

    }
}
