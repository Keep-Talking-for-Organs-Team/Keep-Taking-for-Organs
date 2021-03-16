using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    public class LevelSelectingStageManager : MonoBehaviour {


        [Header("REFS")]
        public Image  bgMan;
        public Text[] startButtonTexts;
        public Text[] mapViewerTexts;
        public Text[] operatorTexts;

        [Header("Images")]
        public Sprite mapViewerBG;
        public Sprite operatorBG;


        Color _operatorColor = new Color(0.03921569f, 0.1098039f, 0.1098039f, 1f);

        void OnEnable () {

            if (GlobalManager.current.isMapViewer) {

                bgMan.sprite = mapViewerBG;
                bgMan.color  = Color.white;

                startButtonTexts[0].text = mapViewerTexts[0].text;
                startButtonTexts[1].text = mapViewerTexts[1].text;

            }
            else {

                bgMan.sprite = operatorBG;
                bgMan.color  = _operatorColor;

                startButtonTexts[0].text = operatorTexts[0].text;
                startButtonTexts[1].text = operatorTexts[1].text;

            }

        }

    }
}
