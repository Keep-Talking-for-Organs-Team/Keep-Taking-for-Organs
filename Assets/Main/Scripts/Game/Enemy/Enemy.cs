using UnityEngine;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    public class Enemy : MonoBehaviour {


        public float awareRateIncreaseSpeed;
        public float awareRateDecreaseSpeed;

        public Vector2 defaultDir = Vector2.up;

        [Header("REFS")]
        public SpriteRenderer sr;
        public VisionSpan     visionSpan;

        [Header("Output Shows")]
        public float awareRateShows;


        public Vector2 FacingDirection => transform.rotation * defaultDir;


        // Components
        EnemyVisionManager _visionManager;
        EnemyMoveManager   _moveManager;
        EnemyAttackManager _attackManager;


        float   _awareRate = 0f;


        void Awake () {
            _visionManager = GetComponent<EnemyVisionManager>();
            _moveManager   = GetComponent<EnemyMoveManager>();
            _attackManager = GetComponent<EnemyAttackManager>();
        }

        void Start () {
            if (!GameSceneManager.current.showAllEnemies) {
                Hide();
            }
        }


        void FixedUpdate () {

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
                if (player.IsInVision(transform.position)) {
                    Show();
                }
                else {
                    if (!GameSceneManager.current.showAllEnemies) {
                        Hide();
                    }
                }

                // Has spotted player?
                if (visionSpan.IsInSight(player.transform.position) && !player.IsHiding) {

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

                    _visionManager.Target(player.transform.position, Time.fixedDeltaTime * Time.timeScale);
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


    }
}
