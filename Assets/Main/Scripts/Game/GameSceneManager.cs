using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager> {

        [Header("Options")]
        [Range(0f, 3f)]
        public float timeScale = 1f;
        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;
        public bool  enableRandomCamRotation = true;

        [Header("Properties")]
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;

        [Header("REFS")]
        public TerrainManager currentTerrain;
        public Transform      enemiesParent;
        public Transform      pathsParent;
        public GameObject     inGameMenu;

        [Header("Register Enemies Spawners")]
        public EnemiesSpawnersManager[] enemiesSpawnersManagers;


        public int EnemiesSpawnedCount {
            get {
                int sum = 0;
                foreach (var manager in enemiesSpawnersManagers) {
                    sum += manager.SpawnedCount;
                }
                return sum;
            }
        }

        // Components
        public OperatorManager  operatorManager {get; private set;}
        public MapViewerManager mapViewerManager {get; private set;}


        // Components
        SecretCodeHandler[] _secretCodeHandlers;


        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }

        protected override void Awake () {
            base.Awake();
            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;

            operatorManager    = GetComponent<OperatorManager>();
            mapViewerManager   = GetComponent<MapViewerManager>();
            _secretCodeHandlers = GetComponents<SecretCodeHandler>();

            AkSoundEngine.SetState("Game", "Normal");
            AkSoundEngine.SetState("Music_Stage", "lv" + LevelSelector.currentLevelNumber);
        }

        void Start () {

            if (GlobalManager.current == null) {
                Instantiate(Resources.Load<GameObject>("Prefabs/Global Manager"));
            }


            bool isMapViewer = GlobalManager.current.isMapViewer;

            if (mapViewerManager != null) {
                mapViewerManager.enabled = isMapViewer;

                if (mapViewerManager.cam != null) {
                    mapViewerManager.cam.gameObject.SetActive(isMapViewer);
                }
            }

            if (operatorManager != null) {
                operatorManager.enabled = !isMapViewer;

                if (operatorManager.cam != null) {
                    operatorManager.cam.gameObject.SetActive(!isMapViewer);
                }
            }

            GlobalManager.current.ClearLoadingDisplay();
        }

        void Update () {

            if (Input.GetButtonDown("Menu")) {
                if (!inGameMenu.activeSelf) {

                    if ( !(operatorManager != null && operatorManager.enabled && operatorManager.IsMissionEnded) ) {
                        OpenInGameMenu();
                            inGameMenu.SetActive(true);
                            AkSoundEngine.PostEvent("Play_ESCMenu" , gameObject);
                    }
                }
                else {
                    CloseInGameMenu();
                }
            }


            if (Input.GetButtonDown("Info")) {

                if (operatorManager != null && operatorManager.enabled)
                    operatorManager.switchableInfoPanel.SetActive(true);

                if (mapViewerManager != null && mapViewerManager.enabled)
                    mapViewerManager.switchableInfoPanel.SetActive(true);

                AkSoundEngine.PostEvent("Play_Tab" , gameObject);

            }
            else if (Input.GetButtonUp("Info")) {

                if (operatorManager != null && operatorManager.enabled)
                    operatorManager.switchableInfoPanel.SetActive(false);

                if (mapViewerManager != null && mapViewerManager.enabled)
                    mapViewerManager.switchableInfoPanel.SetActive(false);

            }


            // Secret Code
            foreach (var handler in _secretCodeHandlers) {
                if (handler.IsActiveSecretCode) {

                    if (handler.actionName == "Remove Fog") {

                        if (operatorManager != null && operatorManager.enabled)
                            operatorManager.RemoveFog();

                    }
                    else if (handler.actionName == "No Time Limit") {

                        if (operatorManager != null && operatorManager.enabled)
                            operatorManager.RemoveTimeLimit();

                    }
                    else if (handler.actionName == "Test") {

                        if (Player.current != null) {

                            Player.current.isInvincible = true;
                            Player.current.maxAngularSpeed = Mathf.Infinity;

                            var playerMoveManager = Player.current.GetComponent<PlayerMoveManager>();
                            if (playerMoveManager != null) {
                                playerMoveManager.walkSpeed = 15f;
                            }

                            var playerAttackManager = Player.current.GetComponent<PlayerAttackManager>();
                            if (playerAttackManager != null) {
                                playerAttackManager.BulletsLeft = -1;
                                playerAttackManager.RemoveWeaponsCooldown();
                            }
                        }

                    }

                    handler.IsActiveSecretCode = false;
                }
            }

        }

        public void GenerateEnemies (int randomSeed) {
            Random.InitState(randomSeed);
            foreach (var spawnersManager in enemiesSpawnersManagers) {
                spawnersManager.StartSpawn();
            }
        }


        public void OpenInGameMenu () {
            inGameMenu.SetActive(true);
            GlobalManager.current.PostAudioEvent("Play_ESCMenu");
        }

        public void CloseInGameMenu () {
            inGameMenu.SetActive(false);
            GlobalManager.current.PostAudioEvent("Play_LeaveMenu");
        }

        public void RestartLevel () {
            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.ReloadScene();
            } );

            GlobalManager.current.PostAudioEvent("Play_Tab");
        }

        public void BackToMainMenu () {
            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.BackToMenuScene();
            } );

            GlobalManager.current.PostAudioEvent("Play_ESCLeave");
        }

    }
}
