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
        public bool  showAllEnemies = false;

        [Header("Properties")]
        public float timeLimit = -1f;
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;

        [Header("REFS")]
        public TerrainManager currentTerrain;
        public Transform      enemiesParent;
        public Transform      pathsParent;
        public GameObject     inGameMenu;

        [Header("Register Enemies Spawners")]
        public EnemiesSpawnersManager[] enemiesSpawnersManagers;


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
                        inGameMenu.SetActive(true);
                        AkSoundEngine.PostEvent("Play_ESCMenu" , gameObject);
                    }
                }
                else {
                    inGameMenu.SetActive(false);
                    AkSoundEngine.PostEvent("Play_LeaveMenu" , gameObject);
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


        public void RestartLevel () {
            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.ReloadScene();
            } );
        }

        public void BackToMainMenu () {
            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.BackToMenuScene();
            } );
        }

    }
}
