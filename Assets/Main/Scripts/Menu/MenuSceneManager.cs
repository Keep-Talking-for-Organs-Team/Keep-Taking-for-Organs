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
            LevelSelecting
        }


        [Header("REFS")]
        public GameObject mainMenuStage;
        public GameObject levelSelectingStage;
        public GameObject settingsPanel;


        public Stage CurrentStage {get; private set;} = Stage.MainMenu;


        Dictionary<Stage, GameObject> stages = new Dictionary<Stage, GameObject>();


        protected override void Awake () {
            base.Awake();

            stages = new Dictionary<Stage, GameObject>() {
                { Stage.MainMenu, mainMenuStage },
                { Stage.LevelSelecting, levelSelectingStage }
            };
        }

        void Start () {
            AkSoundEngine.SetState("Game", "NotInGame");
            AkSoundEngine.SetState("Music_Stage", "Title");

            // Intro
            GlobalManager.current.ClearLoadingDisplay();

            if (GlobalManager.current.blackScreenOverlay.blocksRaycasts) {
                GlobalManager.current.FadeScreenIn();
            }
        }

        void Update () {

            if (Input.GetButtonDown("Menu")) {
                if (GlobalManager.current.IsInTransition)
                    return;

                if (!settingsPanel.activeSelf) {
                    settingsPanel.SetActive(true);
                    AkSoundEngine.PostEvent("Play_ESCMenu", gameObject);
                }
                else {
                    settingsPanel.SetActive(false);
                    AkSoundEngine.PostEvent("Play_LeaveMenu", gameObject);
                }

            }

            if (Input.GetButtonDown("Cancel")) {
                if (GlobalManager.current.IsInTransition)
                    return;

                BackToPreviousStage();
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

        public void BackToPreviousStage () {

            Stage prevStage = Stage.MainMenu;

            foreach (Stage stage in stages.Keys) {
                if (stage == CurrentStage) {

                    if (CurrentStage != prevStage) {
                        SwitchStage(prevStage);
                    }
                    break;
                }
                else {
                    prevStage = stage;
                }
            }
        }

        public void OnQuitGameButtonClicked () {
            GlobalManager.current.QuitGame();
        }



        void SwitchStage (Stage newStage) {
            if (CurrentStage == newStage)
                return;

            GlobalManager.current.FadeScreenOut( () => {

                foreach (Stage stage in stages.Keys) {
                    if (stage == newStage) {
                        stages[stage].SetActive(true);
                    }
                    else {
                        stages[stage].SetActive(false);
                    }
                }

                CurrentStage = newStage;

                GlobalManager.current.FadeScreenIn();
            } );
        }

    }
}
