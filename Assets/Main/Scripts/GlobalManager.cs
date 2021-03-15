using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class GlobalManager : SingletonMonoBehaviour<GlobalManager> {

        const string menuSceneName = "Menu Scene";

        public const float minDeltaAngle = 0.187f;


        public bool isMapViewer = false;

        [Header("Output Shows")]
        public int visionSpansMaxEdgesResolveIterationsSoFar = 0;


        public string CurrentLevelName {get; private set;} = "";


        protected override void Awake () {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            DOTween.Init();
        }

        void Start () {
            if (GameObject.FindObjectOfType<EventSystem>() == null) {
                GameObject eventSys = new GameObject("Event System");
                eventSys.AddComponent<EventSystem>();
            }
        }

        void OnApplicationQuit () {
            print("[Vision Span] Vision Spans Max Edge Resolve Iterations Count is " + visionSpansMaxEdgesResolveIterationsSoFar);
        }



        public static void StartLevel (string levelName) {
            if (current == null)
                return;

            SceneManager.LoadScene(levelName);
        }

        public static void RestartLevel () {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void BackToMenuScene () {
            SceneManager.LoadScene(menuSceneName);
        }

    }

}
