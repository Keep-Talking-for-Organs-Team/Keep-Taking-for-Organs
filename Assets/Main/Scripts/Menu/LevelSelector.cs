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
            UpdateDisplay();
        }

        public void SelectLevel (int levelNumber) {
            currentLevelNumber = levelNumber;
            UpdateDisplay();
        }


        void UpdateDisplay () {
            for (int i = 0 ; i < selections.Length ; i++) {
                selections[i].GetComponent<Text>().color = (i == currentLevelNumber) ? selectedColor : unselectedColor;
            }
        }

    }
}
