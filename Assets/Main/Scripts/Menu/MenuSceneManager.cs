using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class MenuSceneManager : SingletonMonoBehaviour<MenuSceneManager> {

        public Slider levelNumberSelector;

        public void StartAsOperator () {
            GlobalManager.StartLevel(GetLevelName(), false);
        }

        public void StartAsMapViewer () {
            GlobalManager.StartLevel(GetLevelName(), true);
        }

        string GetLevelName () {
            return "Level " + (int) levelNumberSelector.value;
        }

    }
}
