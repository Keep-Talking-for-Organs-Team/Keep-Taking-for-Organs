using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class MenuSceneManager : SingletonMonoBehaviour<MenuSceneManager> {


        public void StartAsOperator () {
            GlobalManager.current.StartLevel("Level 1", false);
        }

        public void StartAsMapViewer () {
            GlobalManager.current.StartLevel("Level 1", true);
        }

    }
}
