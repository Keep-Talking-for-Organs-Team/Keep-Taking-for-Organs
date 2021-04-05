using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    public class SeedDisplay : MonoBehaviour {

        [Header("REFS")]
        public Text disaplayText;
        public GameObject[] bindedObjects;

        string _prefix = "";
        int    _seed = -1;

        void Awake () {
            _prefix = disaplayText.text;
        }

        void OnEnable () {
            UpdateDisplay();
        }

        public void UpdateSeed (int seed) {
            _seed = seed;
            UpdateDisplay();
        }

        void UpdateDisplay () {
            if (_seed == -1)
                disaplayText.text = "";
            else
                disaplayText.text = _prefix + _seed;

            foreach (GameObject binded in bindedObjects) {
                if (disaplayText.text == "") {
                    binded.SetActive(false);
                }
                else {
                    binded.SetActive(true);
                }
            }
        }

    }
}
