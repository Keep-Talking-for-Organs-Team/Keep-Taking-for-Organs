using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager> {

        public enum FailedReason {
            RunOutOfTime,
            Trap,
            LaserGun,
            ElectricGun,
            None
        }

        [Header("Options")]
        [Range(0f, 3f)]
        public float timeScale = 1f;
        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;
        public bool  enableRandomCamRotation = true;
        public bool  showAllEnemies = false;

        [Header("Properties")]
        public float timeLimit = -1f;
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;
        public FailedReasonMessages failedReasonMessages;

        [Header("REFS")]
        public TerrainManager currentTerrain;
        public Transform      enemiesParent;
        public Transform      playerSpawnPointsParent;
        public Camera         mainCam;
        public SpriteRenderer fogSR;
        public HUDManager     operatorHUDManager;
        public Camera         mapViewCam;

        public GameObject     randomSeedInputGO;
        public GameObject     switchableInfoPanel;
        public GameObject     inGameMenu;
        public GameObject     missionSuccessMessages;
        public GameObject     missionFailedMessages;
        public Text           howDiedMessageText;
        public Text           seedInfoText;
        public CanvasGroup    attackedOverlayFX;
        public CanvasGroup    meleeAttackOverlayFX;
        public CanvasGroup    rangedAttackOverlayFX;
        public CanvasGroup    outOfAmmoOverlayFX;
        public LineFactory    lineFactory;
        public LineFactory    playerRangedAttackableLineFactory;

        [Header("Prefabs")]
        public GameObject     playerPrefab;

        [Header("Register Enemies Spawners")]
        public EnemiesSpawnersManager[] enemiesSpawnersManagers;

        public bool IsMissionStarted {get; private set;} = false;
        public bool IsMissionEnded {get; private set;} = false;
        public bool IsMissionOnGoing => IsMissionStarted && !IsMissionEnded;
        public float MissionTimePassed => !IsMissionStarted ? -1f : Time.time - _missionStartTime;
        public float MissionTimeRemained => timeLimit - MissionTimePassed;

        public int  RandomSeed {
            get => _randSeed;
            set {
                _randSeed = value;
                if (seedInfoText != null) {
                    seedInfoText.text = "SEED: " + _randSeed;
                }
            }
        }


        int   _randSeed = -1;
        float _missionStartTime = 0f;

        // Components
        SecretCodeHandler[] _secretCodeHandlers;


        protected override void Awake () {
            base.Awake();
            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;

            attackedOverlayFX.alpha = 0f;
            meleeAttackOverlayFX.alpha = 0f;
            rangedAttackOverlayFX.alpha = 0f;
            outOfAmmoOverlayFX.alpha = 0f;

            _secretCodeHandlers = GetComponents<SecretCodeHandler>();

            RandomSeed = Random.Range(0, 10000);
        }

        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }

        void Start () {

            if (GlobalManager.current == null) {
                Instantiate(Resources.Load<GameObject>("Prefabs/Global Manager"));
            }

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

            AkSoundEngine.SetState("Game", "Normal");
            AkSoundEngine.SetState("Music_Stage", "lv" + LevelSelector.currentLevelNumber);
        }

        void Update () {

            if (!GlobalManager.current.isMapViewer) {

                if (operatorHUDManager.timerDisplayText != null) {
                    if (timeLimit < 0) {
                        operatorHUDManager.timerDisplayText.enabled = false;
                    }
                    else if (IsMissionOnGoing) {
                        operatorHUDManager.UpdateTimerDisplay(MissionTimeRemained);
                    }
                }

                if (IsMissionOnGoing && timeLimit > 0 && MissionTimeRemained <= 0) {
                    MissionFailed(FailedReason.RunOutOfTime);
                }


                // handle input
                if (IsMissionEnded) {

                    if (Input.GetButtonDown("Submit")) {
                        GlobalManager.RestartLevel();
                    }
                    else if (Input.GetButtonDown("Menu")) {
                        GlobalManager.BackToMenuScene();
                        AkSoundEngine.PostEvent("Play_ESCLeave" , gameObject);
                    }
                }

                // Remove Fog Secret Code
                foreach (var handler in _secretCodeHandlers) {
                    if (handler.IsActiveSecretCode) {

                        if (handler.actionName == "Remove Fog") {
                            RemoveFog();
                        }

                        handler.IsActiveSecretCode = false;
                    }
                }
            }
            else {
                // Map Viewer

            }

            if (!IsMissionEnded) {
                if (Input.GetButtonDown("Menu")) {
                    if (!inGameMenu.activeSelf) {
                        // no pause
                        inGameMenu.SetActive(true);
                        AkSoundEngine.PostEvent("Play_ESCMenu" , gameObject);
                    }
                    else {
                        inGameMenu.SetActive(false);
                        AkSoundEngine.PostEvent("Play_LeaveMenu" , gameObject);
                    }
                }
            }


            if (Input.GetButtonDown("Info")) {
                switchableInfoPanel.SetActive(true);
                AkSoundEngine.PostEvent("Play_Tab" , gameObject);
            }
            else if (Input.GetButtonUp("Info")) {
                switchableInfoPanel.SetActive(false);
            }
            // === ==== ===


        }


        public void StartMission () {
            _missionStartTime = Time.time;
            IsMissionStarted = true;

            if (playerSpawnPointsParent.childCount > 0) {

                int playerSpawnPointIndex = Random.Range(0, playerSpawnPointsParent.childCount);

                Player player = Instantiate(playerPrefab, playerSpawnPointsParent.GetChild(playerSpawnPointIndex).position, Quaternion.identity, transform).GetComponent<Player>();

                Destroy(playerSpawnPointsParent.gameObject);
            }
            else {
                print("Missing Player Spawn Point!!");
            }
        }

        public void MissionSuccess () {
            IsMissionEnded = true;
            PlayMissionEndedOverlayFX(true);

            Time.timeScale = 0f;

            AkSoundEngine.SetState("Game", "Success");
        }

        public void MissionFailed (FailedReason reason = FailedReason.None) {

            howDiedMessageText.text = failedReasonMessages.GetMessage(reason);

            IsMissionEnded = true;
            PlayMissionEndedOverlayFX(false);

            AkSoundEngine.SetState("Game", "Over");
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

            if (success) {
                missionSuccessMessages.SetActive(true);
                print("SUCCESS!");
            }
            else {
                missionFailedMessages.SetActive(true);
                print("FAILED");
            }
        }


        public void OnInputingRandomSeed () {
            AkSoundEngine.PostEvent("Play_EnterDigit" , gameObject);
        }

        public void ApplyRandomSeed (string value) {

            AkSoundEngine.PostEvent("Play_PressEnter" , gameObject);

            int seed = -1;

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

        void RemoveFog () {
            if (!GlobalManager.current.isMapViewer) {
                fogSR.enabled = false;
                showAllEnemies = true;
            }
        }

        void ClearDrawnLines () {
            lineFactory.ClearLines();
        }


        [System.Serializable]
        public class FailedReasonMessages {

            public string runOutOfTime;
            public string trap;
            public string laserGun;
            public string electricGun;

            public string GetMessage (FailedReason reason) {
                if (reason == FailedReason.RunOutOfTime)
                    return runOutOfTime;
                else if (reason == FailedReason.Trap)
                    return trap;
                else if (reason == FailedReason.LaserGun)
                    return laserGun;
                else if (reason == FailedReason.ElectricGun)
                    return electricGun;

                return "";
            }
        }

    }
}
