using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : MonoBehaviour {

        public static GameSceneManager current;


        public float overrideViewSpanMaxSegmentGapAngle;

        [Header("REFS")]
        public Camera mainCam;



        void Awake () {
            if (current != null) {
                Destroy(current);
            }

            current = this;
        }

        void OnDestroy () {
            current = null;
        }

        void Update () {

            if (overrideViewSpanMaxSegmentGapAngle > 0)
            VisionSpan.MaxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }


    }
}
