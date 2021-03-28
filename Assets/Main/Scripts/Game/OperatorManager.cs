using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(GameSceneManager))]
    public class OperatorManager : MonoBehaviour {


        public enum FailedReason {
            RunOutOfTime,
            Trap,
            LaserGun,
            ElectricGun,
            None
        }

        [Header("Options")]
        public bool  enableRandomCamRotation = true;
        public bool  showAllEnemies = false;

        [Header("Properties")]
        public float timeLimit = -1f;
        public int[] differentBulletsAmountIntervalDividers;
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;
        public float killedEnemyOverlayFXDuration = 1f;
        public Ease  killedEnemyOverlayFXEase;
        public FailedReasonMessages failedReasonMessages;

        [Header("REFS")]
        public Transform      playerSpawnPointsParent;
        public Camera         cam;
        public SpriteRenderer fogSR;
        public HUDManager     hudManager;

        public GameObject     goalIcon;
        public GameObject     switchableInfoPanel;
        public GameObject     missionSuccessMessages;
        public GameObject     missionFailedMessages;
        public Text           howDiedMessageText;
        public SeedDisplay    seedDisplay;
        public CanvasGroup    attackedOverlayFX;
        public CanvasGroup    killedEnemyOverlayFX;
        public CanvasGroup    outOfAmmoOverlayFX;
        public LineFactory    playerRangedAttackableLineFactory;

        [Header("Prefabs")]
        public GameObject     playerPrefab;


        public bool IsMissionStarted {get; private set;} = false;
        public bool IsMissionEnded {get; private set;} = false;
        public bool IsMissionOnGoing => IsMissionStarted && !IsMissionEnded;
        public float MissionTimePassed => !IsMissionStarted ? 0f : Time.time - _missionStartTime;
        public float MissionTimeRemained => timeLimit - MissionTimePassed;

        public Sprite[] EnemyFlyingAnimSprites {
            get {
                if (_enemyFlyingAnimSprites == null) {
                    _enemyFlyingAnimSprites = LoadEnemyFlyingAnimSprites();
                }
                return _enemyFlyingAnimSprites;
            }
        }

        public int RandomSeed {
            get => _randSeed;
            set {
                _randSeed = value;
                if (seedDisplay != null)
                    seedDisplay.UpdateSeed(_randSeed);
                // if (seedInfoText != null) {
                //     seedInfoText.text = "SEED: " + _randSeed;
                // }
            }
        }

        int   _randSeed = -1;
        float _missionStartTime = 0f;
        Sprite[] _enemyFlyingAnimSprites = null;

        // Components
        GameSceneManager _gameSceneManager;


        void Awake () {
            _gameSceneManager = GetComponent<GameSceneManager>();

            _enemyFlyingAnimSprites = LoadEnemyFlyingAnimSprites();

            if (goalIcon.activeSelf)
                goalIcon.SetActive(false);

            attackedOverlayFX.alpha = 0f;
            killedEnemyOverlayFX.alpha = 0f;
            outOfAmmoOverlayFX.alpha = 0f;

            RandomSeed = Random.Range(0, 10000);
        }


        void Start () {
            if (GlobalManager.current.isMapViewer) {
                this.enabled = false;
                return;
            }


            cam.gameObject.SetActive(true);
            cam.enabled = true;

            if (enableRandomCamRotation)
                cam.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, Vector3.forward);

            _gameSceneManager.GenerateEnemies(RandomSeed);
            GeneratePlayer();
            UpdateTimerDisplay();

            GlobalManager.current.FadeScreenIn( () => {
                StartMission();
            } );
        }


        void Update () {

            if (IsMissionOnGoing) {

                UpdateTimerDisplay();

                if (timeLimit > 0 && MissionTimeRemained <= 0) {
                    MissionFailed(FailedReason.RunOutOfTime);
                }
            }


            // Handle input
            if (IsMissionEnded) {

                if (Input.GetButtonDown("Submit")) {
                    _gameSceneManager.RestartLevel();
                }
                else if (Input.GetButtonDown("Menu")) {
                    _gameSceneManager.BackToMainMenu();
                    AkSoundEngine.PostEvent("Play_ESCLeave" , gameObject);
                }
            }

        }


        public void StartMission () {

            _missionStartTime = Time.time;
            IsMissionStarted = true;
        }

        public void MissionSuccess () {
            IsMissionEnded = true;
            PlayMissionEndedOverlayFX(true);

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

        public void PlayKilledEnemyOverlayFX () {
            killedEnemyOverlayFX.DOFade(0f, killedEnemyOverlayFXDuration)
                .From(1f)
                .SetEase(killedEnemyOverlayFXEase);
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

        public void OnPlayerGetGoal () {
            goalIcon.SetActive(true);
        }

        public void RemoveFog () {
            fogSR.enabled = false;
            showAllEnemies = true;
        }


        void GeneratePlayer () {
            if (playerSpawnPointsParent.childCount > 0) {

                int playerSpawnPointIndex = Random.Range(0, playerSpawnPointsParent.childCount);
                Transform playerSpawnPointTrans = playerSpawnPointsParent.GetChild(playerSpawnPointIndex);

                Player player = Instantiate(playerPrefab, playerSpawnPointTrans.position, cam.transform.rotation, transform).GetComponent<Player>();

                Destroy(playerSpawnPointsParent.gameObject);
            }
            else {
                print("Missing Player Spawn Point!!");
            }
        }

        void UpdateTimerDisplay () {
            if (hudManager.timerDisplayText != null) {
                if (timeLimit < 0) {
                    hudManager.timerDisplayText.enabled = false;
                }
                else {
                    hudManager.UpdateTimerDisplay(MissionTimeRemained);
                }
            }
        }

        Sprite[] LoadEnemyFlyingAnimSprites () {
            return Resources.LoadAll<Sprite>("Sprites/Enemy");
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
