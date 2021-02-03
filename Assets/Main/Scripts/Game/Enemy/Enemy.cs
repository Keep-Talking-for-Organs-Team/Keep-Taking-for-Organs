using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class Enemy : MonoBehaviour {


        public float awareRateIncreaseSpeed;
        public float awareRateDecreaseSpeed;
        public bool  invisiableWhenOutOfSight;

        public Vector2 initDir = Vector2.up;

        [Header("REFS")]
        public SpriteRenderer sr;
        public VisionSpan     visionSpan;

        [Header("Output Shows")]
        public float awareRateShows;


        public Vector2 FacingDirection => transform.rotation * initDir;


        // Components
        EnemyVisionManager _visionManager;
        EnemyMoveManager   _moveManager;
        EnemyAttackManager _attackManager;


        bool    _isSpottingPlayer = false;
        float   _awareRate = 0f;


        void Awake () {
            _visionManager = GetComponent<EnemyVisionManager>();
            _moveManager   = GetComponent<EnemyMoveManager>();
            _attackManager = GetComponent<EnemyAttackManager>();
        }

        void Start () {
            if (invisiableWhenOutOfSight) {
                Hide();
            }
        }


        void FixedUpdate () {

            if (Player.current != null) {
                Player player = Player.current;

                // Is spotted by player?
                if (player.IsInVision(transform.position)) {
                    Show();
                }
                else {
                    if (invisiableWhenOutOfSight) {
                        Hide();
                    }
                }


                // Has spotted player?
                if (visionSpan.IsInSight(player.transform.position)) {

                    if (!_isSpottingPlayer)
                        print("The player go into the enemy's sight");

                    _isSpottingPlayer = true;
                }
                else {
                    _isSpottingPlayer = false;
                }


                if (_isSpottingPlayer) {

                    _visionManager.Target(player.transform.position, Time.fixedDeltaTime, Time.timeScale);
                    _moveManager.Target(player.transform.position, Time.fixedDeltaTime, Time.timeScale);


                    if (_awareRate < 1) {
                        _awareRate = Mathf.Min(_awareRate + awareRateIncreaseSpeed * Time.timeScale * Time.fixedDeltaTime, 1f);
                    }

                }
                else {

                    if (_awareRate > 0) {
                        _awareRate = Mathf.Max(_awareRate - awareRateDecreaseSpeed * Time.timeScale * Time.fixedDeltaTime, 0f);
                    }

                }

                if (_awareRate == 1) {

                    if (_attackManager != null) {

                        if (_attackManager.IsInRange(player.transform.position)) {
                            _attackManager.TryToAttack();
                        }
                        else {
                            if (_moveManager != null)
                                _moveManager.Chase(player.transform.position, Time.fixedDeltaTime, Time.timeScale);
                        }
                    }
                }


            }
            else {
                Hide();
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
