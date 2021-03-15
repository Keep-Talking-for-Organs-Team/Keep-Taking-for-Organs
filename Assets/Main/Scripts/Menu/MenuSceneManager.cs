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


        public Stage CurrentStage {get; private set;} = Stage.MainMenu;


        Dictionary<Stage, GameObject> stages = new Dictionary<Stage, GameObject>();


        protected override void Awake () {
            base.Awake();
            
            stages = new Dictionary<Stage, GameObject>() {
                { Stage.MainMenu, mainMenuStage },
                { Stage.LevelSelecting, levelSelectingStage }
            };
        }

        void Update () {

            if (Input.GetButtonDown("Cancel")) {
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
        }

        public void SelectMapViewer () {
            GlobalManager.current.isMapViewer = true;
            SwitchStage(Stage.LevelSelecting);
        }

        public void SelectOperator () {
            GlobalManager.current.isMapViewer = false;
            SwitchStage(Stage.LevelSelecting);
        }

        public void StartGame () {
            GlobalManager.StartLevel( GetLevelName(LevelSelector.currentLevelNumber) );
        }



        public void StartAsOperator () {
            // GlobalManager.StartLevel(GetLevelName(), false);
        }

        public void StartAsMapViewer () {
            // GlobalManager.StartLevel(GetLevelName(), true);
        }


        void SwitchStage (Stage newStage) {
            foreach (Stage stage in stages.Keys) {
                if (stage == newStage) {
                    stages[stage].SetActive(true);
                }
                else {
                    stages[stage].SetActive(false);
                }
            }

            CurrentStage = newStage;
        }


        string GetLevelName (int levelNumber) {
            return "Level " + levelNumber;
        }

    }
}
