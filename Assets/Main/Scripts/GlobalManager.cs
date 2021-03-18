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

        [Header("Audio Settings")]
        public AudioSettings audioSettings;

        [Header("Output Shows")]
        public int visionSpansMaxEdgesResolveIterationsSoFar = 0;


        public string CurrentLevelName => "Level " + LevelSelector.currentLevelNumber;


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

            AssignAudioSettings();
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

        public static void AssignAudioSettings () {
            AudioSettings settings = GlobalManager.current.audioSettings;

            AkSoundEngine.SetRTPCValue("Master_Volumn", settings.masterVolume);
            AkSoundEngine.SetRTPCValue("SFX_Volumn", settings.sfxVolume);
            AkSoundEngine.SetRTPCValue("Music_Volumn", settings.musicVolume);
        }

        public void QuitGame () {
            AkSoundEngine.PostEvent("Stop_AllSFX" , gameObject);
            Application.Quit();
        }

    }

}
