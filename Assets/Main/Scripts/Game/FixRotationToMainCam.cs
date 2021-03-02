using UnityEngine;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    public class FixRotationToMainCam : MonoBehaviour {

        void Update () {
            if (GameSceneManager.current != null) {
                if (GameSceneManager.current.mainCam != null) {
                    transform.rotation = GameSceneManager.current.mainCam.transform.rotation;
                }
            }
        }

    }
}
