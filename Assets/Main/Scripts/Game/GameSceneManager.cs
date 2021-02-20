using UnityEngine;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : MonoBehaviour {

        public static GameSceneManager current;

        [Header("Options")]
        [Range(0f, 3f)]
        public float timeScale = 1f;
        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;
        public bool  enableRandomCamRotation = true;
        public bool  showAllEnemies = false;

        [Header("REFS")]
        public Camera mainCam;
        public Transform enemiesParent;



        void Awake () {
            if (current != null) {
                Destroy(current);
            }
            current = this;

            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;
        }

        void Start () {
            if (enableRandomCamRotation)
                mainCam.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, Vector3.forward);
        }

        void OnDestroy () {
            current = null;
        }

        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }


    }
}
