using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    public class SeedDisplay : MonoBehaviour {

        public Text disaplayText;

        public void UpdateSeed (int seed) {
            disaplayText.text = "SEED: " + seed;
        }

    }
}
