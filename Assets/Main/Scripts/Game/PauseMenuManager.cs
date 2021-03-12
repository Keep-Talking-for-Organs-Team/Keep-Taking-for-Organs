using UnityEngine;
using UnityEngine.UI;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class PauseMenuManager : MonoBehaviour {


        public void Show () {
            gameObject.SetActive(true);
        }

        public void Hide () {
            gameObject.SetActive(false);
        }

        public void BackToMainMenu () {
            GlobalManager.BackToMenuScene();
        }

    }
}
