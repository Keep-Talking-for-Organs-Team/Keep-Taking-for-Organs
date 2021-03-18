using UnityEngine;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    public class FixRotationToMainCam : MonoBehaviour {

        public Camera targetCam;

        void Update () {
            if (targetCam != null) {
                transform.rotation = targetCam.transform.rotation;
            }
        }

    }
}
