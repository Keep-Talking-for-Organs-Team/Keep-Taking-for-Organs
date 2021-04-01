using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class MenuSceneManager : SingletonMonoBehaviour<MenuSceneManager> {

        public enum Stage {
            MainMenu,
            LevelSelecting,
            StoryPages,
            Credits
        }

        [Header("REFS")]
        public StoryPagesManager storyPagesManager;
        public GameObject mainMenuStage;
        public GameObject levelSelectingStage;
        public GameObject creditsStage;
        public GameObject settingsPanel;
        

        public Stage CurrentStage {get; private set;} = Stage.MainMenu;


        Dictionary<Stage, GameObject> _stages = new Dictionary<Stage, GameObject>();


        protected override void Awake () {
            base.Awake();

            _stages = new Dictionary<Stage, GameObject>() {
                { Stage.MainMenu, mainMenuStage },
                { Stage.LevelSelecting, levelSelectingStage },
                { Stage.Credits, creditsStage },
                { Stage.StoryPages, storyPagesManager.gameObject }
            };

            AkSoundEngine.SetState("Game", "NotInGame");
            AkSoundEngine.SetState("Music_Stage", "Title");
        }

        void Start () {

            // Intro
            GlobalManager.current.ClearLoadingDisplay();

            if (GlobalManager.current.playStoryPagesAtStart && !StoryPagesManager.hasPlayed) {
                CurrentStage = Stage.StoryPages;
                _stages[CurrentStage].SetActive(true);
            }

            if (GlobalManager.current.blackScreenOverlay.blocksRaycasts) {
                GlobalManager.current.FadeScreenIn();
            }
        }

        void Update () {

            if (CurrentStage != Stage.StoryPages) {

                if (Input.GetButtonDown("Menu")) {
                    if (GlobalManager.current.IsInTransition)
                        return;

                    if (!settingsPanel.activeSelf) {
                        OpenSettingsPanel();
                    }
                    else {
                        CloseSettingsPanel();
                    }

                }

                if (Input.GetButtonDown("Cancel")) {
                    if (GlobalManager.current.IsInTransition)
                        return;

                    BackToPreviousStage();
                }

            }
        }


        public void SelectMapViewer () {
            if (GlobalManager.current.IsInTransition)
                return;

            AkSoundEngine.PostEvent("Play_PlayerBStart" , gameObject);

            GlobalManager.current.isMapViewer = true;
            SwitchStage(Stage.LevelSelecting);
        }

        public void SelectOperator () {
            if (GlobalManager.current.IsInTransition)
                return;

            AkSoundEngine.PostEvent("Play_PlayerAStart" , gameObject);

            GlobalManager.current.isMapViewer = false;
            SwitchStage(Stage.LevelSelecting);
        }

        public void StartGame () {
            if (GlobalManager.current.IsInTransition)
                return;

            AkSoundEngine.PostEvent("Play_Start" , gameObject);

            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.StartLevel(GlobalManager.current.CurrentLevelName);
            } );

        }


        public void WatchStory () {
            if (CurrentStage == Stage.StoryPages)
                return;

            _stages[CurrentStage].SetActive(false);
            CurrentStage = Stage.StoryPages;
            _stages[CurrentStage].SetActive(true);

            GlobalManager.current.PostAudioEvent("Play_SelectStage");
        }

        public void GoToCredits () {
            SwitchStage(Stage.Credits);

            GlobalManager.current.PostAudioEvent("Play_SelectStage");
        }

        public void BackToPreviousStage () {

            Stage prevStage = Stage.MainMenu;
            SwitchStage(prevStage);

            GlobalManager.current.PostAudioEvent("Play_ESCLeave");
        }

        public void OpenSettingsPanel () {
            settingsPanel.SetActive(true);
            AkSoundEngine.PostEvent("Play_ESCMenu", gameObject);
        }

        public void CloseSettingsPanel () {
            settingsPanel.SetActive(false);
            AkSoundEngine.PostEvent("Play_LeaveMenu", gameObject);
        }


        public void OnStoryPagesEnded () {
            SwitchStage(Stage.MainMenu);
        }

        public void OnQuitGameButtonClicked () {
            GlobalManager.current.FadeScreenOut( () => {
                GlobalManager.current.QuitGame();
            } );

            GlobalManager.current.PostAudioEvent("Play_ESCLeave");
        }



        void SwitchStage (Stage newStage) {
            if (CurrentStage == newStage)
                return;

            GlobalManager.current.FadeScreenOut( () => {

                foreach (Stage stage in _stages.Keys) {
                    if (stage == newStage) {
                        _stages[stage].SetActive(true);
                    }
                    else {
                        _stages[stage].SetActive(false);
                    }
                }

                CurrentStage = newStage;

                GlobalManager.current.FadeScreenIn();
            } );
        }

    }
}
