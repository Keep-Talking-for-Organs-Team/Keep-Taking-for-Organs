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
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;
        public FailedReasonMessages failedReasonMessages;

        [Header("REFS")]
        public Transform      playerSpawnPointsParent;
        public Camera         cam;
        public SpriteRenderer fogSR;
        public HUDManager     hudManager;

        public GameObject     switchableInfoPanel;
        public GameObject     missionSuccessMessages;
        public GameObject     missionFailedMessages;
        public Text           howDiedMessageText;
        public SeedDisplay    seedDisplay;
        // public Text           seedInfoText;
        public CanvasGroup    attackedOverlayFX;
        public CanvasGroup    meleeAttackOverlayFX;
        public CanvasGroup    rangedAttackOverlayFX;
        public CanvasGroup    outOfAmmoOverlayFX;
        public LineFactory    playerRangedAttackableLineFactory;

        [Header("Prefabs")]
        public GameObject     playerPrefab;


        public bool IsMissionStarted {get; private set;} = false;
        public bool IsMissionEnded {get; private set;} = false;
        public bool IsMissionOnGoing => IsMissionStarted && !IsMissionEnded;
        public float MissionTimePassed => !IsMissionStarted ? -1f : Time.time - _missionStartTime;
        public float MissionTimeRemained => timeLimit - MissionTimePassed;

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

        // Components
        GameSceneManager _gameSceneManager;


        void Awake () {
            _gameSceneManager = GetComponent<GameSceneManager>();

            attackedOverlayFX.alpha = 0f;
            meleeAttackOverlayFX.alpha = 0f;
            rangedAttackOverlayFX.alpha = 0f;
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
            StartMission();
        }


        void Update () {

            if (hudManager.timerDisplayText != null) {
                if (timeLimit < 0) {
                    hudManager.timerDisplayText.enabled = false;
                }
                else if (IsMissionOnGoing) {
                    hudManager.UpdateTimerDisplay(MissionTimeRemained);
                }
            }

            if (IsMissionOnGoing && timeLimit > 0 && MissionTimeRemained <= 0) {
                MissionFailed(FailedReason.RunOutOfTime);
            }


            // Handle input
            if (IsMissionEnded) {

                if (Input.GetButtonDown("Submit")) {
                    GlobalManager.RestartLevel();
                }
                else if (Input.GetButtonDown("Menu")) {
                    GlobalManager.BackToMenuScene();
                    AkSoundEngine.PostEvent("Play_ESCLeave" , gameObject);
                }
            }

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

        public void RemoveFog () {
            fogSR.enabled = false;
            showAllEnemies = true;
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
