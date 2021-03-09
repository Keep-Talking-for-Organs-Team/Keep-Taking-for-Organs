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
            GlobalManager.current.StartLevel(GetLevelName(), false);
        }

        public void StartAsMapViewer () {
            GlobalManager.current.StartLevel(GetLevelName(), true);
        }

        string GetLevelName () {
            return "Level " + (int) levelNumberSelector.value;
        }

    }
}
