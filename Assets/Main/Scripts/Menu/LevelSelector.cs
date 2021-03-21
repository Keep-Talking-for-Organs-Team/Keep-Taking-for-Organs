using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    public class LevelSelector : MonoBehaviour {

        public static int currentLevelNumber = 0;

        [Header("Properties")]
        public Color selectedColor = Color.white;
        public Color unselectedColor = Color.white;

        [Header("REFS")]
        public GameObject[] selections;


        void Awake () {
            if (PlayerPrefs.HasKey(GlobalManager.PlayerPrefsKeys.LEVEL_NUMBER_SELECTED))
                currentLevelNumber = PlayerPrefs.GetInt(GlobalManager.PlayerPrefsKeys.LEVEL_NUMBER_SELECTED);

            UpdateDisplay();
        }

        void OnEnable () {
            UpdateDisplay();
        }

        public void SelectLevel (int levelNumber) {
            currentLevelNumber = levelNumber;
            PlayerPrefs.SetInt(GlobalManager.PlayerPrefsKeys.LEVEL_NUMBER_SELECTED, levelNumber);

            UpdateDisplay();

            AkSoundEngine.PostEvent("Play_SelectStage" , gameObject); // this means "Select Level"
        }


        void UpdateDisplay () {
            for (int i = 0 ; i < selections.Length ; i++) {
                selections[i].GetComponent<Text>().color = (i == currentLevelNumber) ? selectedColor : unselectedColor;
            }
        }

    }
}
