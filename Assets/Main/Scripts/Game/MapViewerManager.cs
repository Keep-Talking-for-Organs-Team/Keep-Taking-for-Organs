using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(GameSceneManager))]
    public class MapViewerManager : MonoBehaviour {


        [Header("Properties")]
        public float drawnPathLinesWidth = 0.023f;
        public Color drawnPathLinesColor = Color.white;

        [Header("REFS")]
        public Camera      cam;
        public GameObject  randomSeedInputPanel;
        public GameObject  switchableInfoPanel;
        public SeedDisplay seedDisplay;
        public LineFactory pathLineFactory;


        public int RandomSeed {
            get => _randSeed;
            set {
                _randSeed = value;
                if (seedDisplay != null)
                    seedDisplay.UpdateSeed(_randSeed);
            }
        }


        int _randSeed = -1;

        // Components
        GameSceneManager _gameSceneManager;


        void Awake () {
            _gameSceneManager = GetComponent<GameSceneManager>();


            int pathSegmentsCount = 0;
            foreach (Transform pathGO in _gameSceneManager.pathsParent) {
                PathHolder pathHolder = pathGO.GetComponent<PathHolder>();

                if(pathHolder != null) {
                    pathSegmentsCount += pathHolder.SegmentsAmount;
                }
            }

            pathLineFactory.maxLines = pathSegmentsCount;
            pathLineFactory.Init();
        }


        void Start () {
            if (!GlobalManager.current.isMapViewer) {
                this.enabled = false;
                return;
            }


            cam.gameObject.SetActive(true);
            cam.enabled = true;

            randomSeedInputPanel.SetActive(true);

            GlobalManager.current.FadeScreenIn();
        }


        public void OnInputingRandomSeed () {
            AkSoundEngine.PostEvent("Play_EnterDigit" , gameObject);
        }

        public void ApplyRandomSeed (string value) {

            AkSoundEngine.PostEvent("Play_PressEnter" , gameObject);

            int seed = -1;

            if (System.Int32.TryParse(value, out seed) && seed >= 0 && seed < 10000) {

                RandomSeed = seed;
                _gameSceneManager.GenerateEnemies(seed);

                randomSeedInputPanel.SetActive(false);
            }
            else {
                ShowInvalidRandomSeedInput();
            }
        }


        void ShowInvalidRandomSeedInput () {
            print("Invaild Seed Input");
        }


    }
}
