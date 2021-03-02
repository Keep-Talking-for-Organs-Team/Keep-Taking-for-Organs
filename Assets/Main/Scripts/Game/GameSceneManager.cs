using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager> {

        // public static GameSceneManager current {
        //     get {
        //         print(instance);
        //         return null;
        //     }
        // }


        [Header("Options")]
        [Range(0f, 3f)]
        public float timeScale = 1f;
        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;
        public bool  enableRandomCamRotation = true;
        public bool  showAllEnemies = false;

        [Header("Properties")]
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;

        [Header("REFS")]
        public TerrainManager currentTerrain;
        public Camera         mainCam;
        public Camera         mapViewCam;
        public Transform      enemiesParent;
        public Transform      playerSpawnPointsParent;
        public EnemiesSpawnersManager[] enemiesSpawnersManagers;

        public GameObject     randomSeedInputGO;
        public GameObject     switchableInfoPanel;
        public Text           seedInfoText;
        public Text           hudInfoText;
        public Text           pausedMessage;
        public Text           missionEndMessage;
        public CanvasGroup    attackedOverlayFX;
        public CanvasGroup    meleeAttackOverlayFX;
        public CanvasGroup    rangedAttackOverlayFX;
        public CanvasGroup    outOfAmmoOverlayFX;
        public LineFactory    lineFactory;

        [Header("Prefabs")]
        public GameObject     playerPrefab;


        public bool IsMissionStarted {get; private set;} = false;
        public bool IsMissionEnded {get; private set;} = false;
        public bool IsMissionOnGoing => IsMissionStarted && !IsMissionEnded;

        public int  RandomSeed {
            get => _randSeed;
            set {
                _randSeed = value;
                if (seedInfoText != null) {
                    seedInfoText.text = "SEED: " + _randSeed;
                }
            }
        }


        int _randSeed = -1;


        protected override void Awake () {
            base.Awake();
            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;

            pausedMessage.enabled = false;
            missionEndMessage.enabled = false;
            attackedOverlayFX.alpha = 0f;
            meleeAttackOverlayFX.alpha = 0f;
            rangedAttackOverlayFX.alpha = 0f;
            outOfAmmoOverlayFX.alpha = 0f;

            RandomSeed = Random.Range(0, 10000);
        }

        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }

        void OnDrawGizmos () {
            foreach (Transform point in playerSpawnPointsParent) {
                Gizmos.DrawSphere(point.position, 0.4f);
            }
        }

        void Start () {

            if (!GlobalManager.current.isMapViewer) {

                mainCam.gameObject.SetActive(true);
                mainCam.enabled = true;
                Destroy(mapViewCam.gameObject);

                if (enableRandomCamRotation)
                    mainCam.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, Vector3.forward);

                GenerateEnemies();
                StartMission();

            }
            else {

                mapViewCam.gameObject.SetActive(true);
                mapViewCam.enabled = true;
                Destroy(mainCam.gameObject);

                randomSeedInputGO.SetActive(true);
            }
        }

        void Update () {

            if (!GlobalManager.current.isMapViewer) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    if (Time.timeScale > 0) {
                        Time.timeScale = 0f;
                        pausedMessage.enabled = true;
                    }
                    else if (Time.timeScale == 0) {
                        Time.timeScale = 1f;
                        pausedMessage.enabled = false;
                    }
                }
            }
            else {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            // === temp ===
            if (Input.GetKeyDown(KeyCode.Tab)) {
                switchableInfoPanel.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab)) {
                switchableInfoPanel.SetActive(false);
            }
            // === ==== ===

            if (IsMissionEnded) {

                // === temp ===
                if (Input.GetKeyDown(KeyCode.Return)) {
                    // Reload Scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                // === ==== ===
            }
        }


        public void StartMission () {
            IsMissionStarted = true;

            if (playerSpawnPointsParent.childCount > 0) {

                int playerSpawnPointIndex = Random.Range(0, playerSpawnPointsParent.childCount);

                Player player = Instantiate(playerPrefab, playerSpawnPointsParent.GetChild(playerSpawnPointIndex).position, Quaternion.identity, transform).GetComponent<Player>();
            }
            else {
                print("Missing Player Spawn Point!!");
            }
        }

        public void MissionSuccess () {
            IsMissionEnded = true;
            PlayMissionEndedOverlayFX(true);
        }

        public void MissionFailed () {
            IsMissionEnded = true;
            PlayMissionEndedOverlayFX(false);
        }


        public void PlayAttackedOverlayFX () {
            attackedOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayMeleeAttackOverlayFX () {
            meleeAttackOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayRangedAttackOverlayFX () {
            rangedAttackOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayOutOfAmmoOverlayFX () {
            outOfAmmoOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayMissionEndedOverlayFX (bool success) {
            missionEndMessage.enabled = true;

            if (success) {
                print("SUCCESS!");
            }
            else {
                print("FAILED");
            }
        }


        public void ApplyRandomSeed (string value) {
            int seed = -1;

            // === temp ===
            if (System.Int32.TryParse(value, out seed) && seed >= 0 && seed < 10000) {
                RandomSeed = seed;
                GenerateEnemies();
                randomSeedInputGO.SetActive(false);
            }
            else {
                ShowInvalidRandomSeedInput();
            }
        }

        void ShowInvalidRandomSeedInput () {
            print("Invaild Seed Input");
        }

        void GenerateEnemies () {
            Random.InitState(RandomSeed);
            foreach (var spawnersManager in enemiesSpawnersManagers) {
                spawnersManager.StartSpawn();
            }
        }

        void ClearDrawnLines () {
            var activeLines = lineFactory.GetActive ();

    		foreach (var line in activeLines) {
    			line.gameObject.SetActive(false);
    		}
        }

    }
}
