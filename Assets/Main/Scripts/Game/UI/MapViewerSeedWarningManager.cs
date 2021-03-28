using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Text))]
    public class MapViewerSeedWarningManager : MonoBehaviour {

        public Text seedDisplayText;

        Text _text;

        void Awake () {
            _text = GetComponent<Text>();
        }

        void OnEnable () {
            if (seedDisplayText.text == "") {
                _text.enabled = false;
            }
            else {
                _text.enabled = true;
            }
        }

    }
}
