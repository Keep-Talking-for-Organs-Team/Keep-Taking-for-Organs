using UnityEngine;
using UnityEngine.UI;

namespace KeepTalkingForOrgansGame {

    [RequireComponent(typeof(Text))]
    public class GameVersionDisplay : MonoBehaviour {


        void Awake () {
            GetComponent<Text>().text = GetVersionDisplayString(Application.version);
        }


        static string GetVersionDisplayString (string version) {
            return "v" + version;
        }

    }
}
