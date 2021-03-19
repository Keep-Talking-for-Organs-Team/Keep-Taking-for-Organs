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

        [Header("Properties")]
        public float screenFadeDuration = 1f;
        public Ease  screenFadeEase;

        [Header("Audio Settings")]
        public AudioSettings audioSettings;

        [Header("REFS")]
        public CanvasGroup blackScreenOverlay;
        public GameObject  loadingDisplay;

        [Header("Output Shows")]
        public int visionSpansMaxEdgesResolveIterationsSoFar = 0;


        public string CurrentLevelName => "Level " + LevelSelector.currentLevelNumber;
        public bool   IsInTransition {get; private set;} = false;


        Tween _blackScreenOverlayAnim;

        protected override void Awake () {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            DOTween.Init();
            DOTween.showUnityEditorReport = true;
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

            current.loadingDisplay.SetActive(true);
            SceneManager.LoadScene(levelName);
        }

        public static void ReloadScene () {
            current.loadingDisplay.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void BackToMenuScene () {
            current.loadingDisplay.SetActive(true);
            SceneManager.LoadScene(menuSceneName);
        }

        public static void AssignAudioSettings () {
            AudioSettings settings = GlobalManager.current.audioSettings;

            AkSoundEngine.SetRTPCValue("Master_Volumn", settings.masterVolume);
            AkSoundEngine.SetRTPCValue("SFX_Volumn", settings.sfxVolume);
            AkSoundEngine.SetRTPCValue("Music_Volumn", settings.musicVolume);
        }

        public void FadeScreenOut (TweenCallback endCallback = null) {
            IsInTransition = true;
            blackScreenOverlay.blocksRaycasts = true;
print("fade out");
            blackScreenOverlay.DOFade(1f, screenFadeDuration)
                .From(0f)
                .SetEase(screenFadeEase)
                .OnComplete( () => {
                    IsInTransition = false;

                    if (endCallback != null)
                        endCallback();
                } )
                .OnKill(() => print("fout killed"))
                .OnPause(() => print("fou paused"))
                .timeScale = 1f;
        }

        public void FadeScreenIn (TweenCallback endCallback = null) {
            IsInTransition = true;
print("fade in");
            blackScreenOverlay.DOFade(0f, screenFadeDuration)
                .From(1f)
                .SetEase(screenFadeEase)
                .OnComplete( () => {
                    blackScreenOverlay.blocksRaycasts = false;
                    IsInTransition = false;

                    if (endCallback != null)
                        endCallback();
                } )
                .OnKill(() => print("fin killed"))
                .OnPause(() => print("fin paused"))
                .timeScale = 1f;
        }

        public void ClearLoadingDisplay () {
            current.loadingDisplay.SetActive(false);
        }

        public void QuitGame () {
            AkSoundEngine.PostEvent("Stop_AllSFX" , gameObject);
            Application.Quit();
        }

    }

}
