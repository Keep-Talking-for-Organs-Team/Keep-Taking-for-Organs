using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class DescriptionTextFromTitle : MonoBehaviour {

        [Header("Properties")]
        public float fontSizeRate = 1f;

        [Header("REFS")]
        public Text titleText;

        public Text SelfText {
            get {
                if (_text == null)
                    _text = GetComponent<Text>();

                return _text;
            }
        }

        Text _text;


        void Update () {

            if (titleText != null) {
                SelfText.fontSize = (int) Mathf.Round(titleText.cachedTextGenerator.fontSizeUsedForBestFit * fontSizeRate);
            }
        }

    }
}
