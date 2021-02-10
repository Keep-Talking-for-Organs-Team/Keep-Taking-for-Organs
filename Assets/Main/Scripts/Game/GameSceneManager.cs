using UnityEngine;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : MonoBehaviour {

        public static GameSceneManager current;


        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;

        [Header("REFS")]
        public Camera mainCam;
        public Transform enemiesParent;



        void Awake () {
            if (current != null) {
                Destroy(current);
            }
            current = this;

            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;


// === testing ===
//
// Vector2 v1 = new Vector2(12f, 13f);
// Vector2 v2 = new Vector2(6f, 6.5f);
// Vector2 vn = v1.normalized;
// bool b1,b2,b3,b4 ;
// b1=b2=b3=b4=false;
// float t0 = Time.realtimeSinceStartup;
// for (int i = 0 ; i < 1000000 ; i++) {
//     b1 = Mathf.Pow(Vector2.Dot(v1, v2), 2) == v1.sqrMagnitude * v2.sqrMagnitude;
// }
// float t1 = Time.realtimeSinceStartup;
// for (int i = 0 ; i < 1000000 ; i++) {
//     b2 = vn == v1.normalized;
// }
// float t2 = Time.realtimeSinceStartup;
// for (int i = 0 ; i < 1000000 ; i++) {
//     b3 = v1.normalized == v2.normalized;
// }
// float t3 = Time.realtimeSinceStartup;
// for (int i = 0 ; i < 1000000 ; i++) {
//     b4 = Mathf.Approximately(Vector2.Dot(v1, v2) , v1.magnitude * v2.magnitude);
// }
// float t4 = Time.realtimeSinceStartup;
//
// print(b1 + "," + b2 + "," + b3 + "," + b4);
// print("dt1 = " + (t1-t0));
// print("dt2 = " + (t2-t1));
// print("dt3 = " + (t3-t2));
// print("dt4 = " + (t4-t3));

// print(Vector2.Angle(Vector2.zero, Vector2.one));

        }

        void OnDestroy () {
            current = null;
        }

        void OnValidate () {

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }


    }
}
