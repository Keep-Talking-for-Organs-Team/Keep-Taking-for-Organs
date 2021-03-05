using UnityEngine;


namespace KeepTalkingForOrgansGame {

    public class SecretCodeHandler : MonoBehaviour {

        public string actionName;
        public KeyCode[] keySeries;

        public bool IsActiveSecretCode {get; set;} = false;


        int _progress = 0;

        void Update () {

            if (Input.GetKeyDown(keySeries[_progress])) {
                _progress++;

                if (_progress >= keySeries.Length) {

                    IsActiveSecretCode = true;
                    _progress = 0;
                }
            }

        }
    }
}
