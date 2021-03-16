using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class InGameMenuManager : MonoBehaviour {

        public void Restart () {
            GlobalManager.RestartLevel();
        }

        public void BackToMainMenu () {
            GlobalManager.BackToMenuScene();
        }

    }
}
