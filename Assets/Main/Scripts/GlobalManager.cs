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
        public static class PlayerPrefsKeys {
            public const string MASTER_VOLUME = "MasterVol";
            public const string SFX_VOLUME    = "SFXVol";
            public const string MUSIC_VOLUME  = "MusicVol";
            public const string LEVEL_NUMBER_SELECTED = "LevelNum";
        }

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
        public bool   IsMouseMoving  {get; private set;} = false;


        Vector2 _prevMousePosition = Vector2.zero;
        Tween _blackScreenOverlayAnim;

        protected override void Awake () {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            DOTween.Init();
            DOTween.showUnityEditorReport = true;


            // Load PlayerPrefs
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.MASTER_VOLUME))
                audioSettings.masterVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MASTER_VOLUME);
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.SFX_VOLUME))
                audioSettings.sfxVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFX_VOLUME);
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.MUSIC_VOLUME))
                audioSettings.musicVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MUSIC_VOLUME);
        }

        void Start () {
            if (GameObject.FindObjectOfType<EventSystem>() == null) {
                GameObject eventSys = new GameObject("Event System");
                eventSys.AddComponent<EventSystem>();
            }

            AssignAudioSettings();
        }

        void Update () {
            Vector2 currentMousePos = Input.mousePosition;

            if (_prevMousePosition == currentMousePos) {
                IsMouseMoving = false;
            }
            else {
                IsMouseMoving = true;
            }

            _prevMousePosition = currentMousePos;
        }

        void OnApplicationQuit () {
            print("[Vision Span] Vision Spans Max Edge Resolve Iterations Count is " + visionSpansMaxEdgesResolveIterationsSoFar);
        }


        public void FadeScreenOut (TweenCallback endCallback = null) {
            IsInTransition = true;
            blackScreenOverlay.blocksRaycasts = true;

            blackScreenOverlay.DOFade(1f, screenFadeDuration)
                .From(0f)
                .SetEase(screenFadeEase)
                .OnComplete( () => {
                    IsInTransition = false;

                    if (endCallback != null)
                        endCallback();
                } );
        }

        public void FadeScreenIn (TweenCallback endCallback = null) {
            IsInTransition = true;

            blackScreenOverlay.DOFade(0f, screenFadeDuration)
                .From(1f)
                .SetEase(screenFadeEase)
                .OnComplete( () => {
                    blackScreenOverlay.blocksRaycasts = false;
                    IsInTransition = false;

                    if (endCallback != null)
                        endCallback();
                } );
        }

        public void ClearLoadingDisplay () {
            current.loadingDisplay.SetActive(false);
        }

        public void QuitGame () {
            AkSoundEngine.PostEvent("Stop_AllSFX" , gameObject);
            Application.Quit();
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

        public static void AssignAudioSettings (int index = -1) {
            AudioSettings settings = GlobalManager.current.audioSettings;

            SetPlayerPrefsAudioSettings(settings, index);

            if (index == 0 || index == -1)
                AkSoundEngine.SetRTPCValue("Master_Volumn", settings.masterVolume * 100);
            if (index == 1 || index == -1)
                AkSoundEngine.SetRTPCValue("SFX_Volumn", settings.sfxVolume * 100);
            if (index == 2 || index == -1)
                AkSoundEngine.SetRTPCValue("Music_Volumn", settings.musicVolume * 100);
        }


        public static void SetPlayerPrefsAudioSettings (AudioSettings settings, int index = -1) {
            if (index == 0 || index == -1)
                PlayerPrefs.SetFloat(PlayerPrefsKeys.MASTER_VOLUME, settings.masterVolume);
            if (index == 1 || index == -1)
                PlayerPrefs.SetFloat(PlayerPrefsKeys.SFX_VOLUME, settings.sfxVolume);
            if (index == 2 || index == -1)
                PlayerPrefs.SetFloat(PlayerPrefsKeys.MUSIC_VOLUME, settings.musicVolume);
        }


    }

}
