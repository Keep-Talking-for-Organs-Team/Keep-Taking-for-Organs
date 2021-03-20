using UnityEngine;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    public class FixRotationToMainCam : MonoBehaviour {


        void Update () {
            if (GameSceneManager.current != null) {

                if (GlobalManager.current.isMapViewer) {
                    MapViewerManager manager = GameSceneManager.current.mapViewerManager;

                    if (manager != null)
                        transform.rotation = manager.cam.transform.rotation;

                }
                else {
                    OperatorManager manager = GameSceneManager.current.operatorManager;

                    if (manager != null)
                        transform.rotation = manager.cam.transform.rotation;
                        
                }

            }
        }

    }
}
