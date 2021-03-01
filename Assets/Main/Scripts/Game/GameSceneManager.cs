using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager> {

        // public static GameSceneManager current {
        //     get {
        //         print(instance);
        //         return null;
        //     }
        // }


        [Header("Options")]
        [Range(0f, 3f)]
        public float timeScale = 1f;
        public float overrideViewSpanMaxSegmentGapAngle;
        public bool  activeHideSpriteAtRuntime = true;
        public bool  enableRandomCamRotation = true;
        public bool  showAllEnemies = false;

        [Header("Properties")]
        public float attackedOverlayFXDuration = 1f;
        public Ease  attackedOverlayFXEase;

        [Header("REFS")]
        public TerrainManager currentTerrain;
        public Camera         mainCam;
        public Transform      enemiesParent;
        public Transform      playerSpawnPointsParent;
        public GameObject     playerPrefab;

        public Text           hudInfoText;
        public CanvasGroup    attackedOverlayFX;
        public CanvasGroup    meleeAttackOverlayFX;
        public CanvasGroup    rangedAttackOverlayFX;
        public CanvasGroup    outOfAmmoOverlayFX;


        public bool IsMissionOnGoing {get; private set;} = false;


        protected override void Awake () {
            base.Awake();
            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;
        }

        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
        }

        void OnDrawGizmos () {
            foreach (Transform point in playerSpawnPointsParent) {
                Gizmos.DrawSphere(point.position, 0.4f);
            }
        }

        void Start () {
            if (enableRandomCamRotation)
                mainCam.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, Vector3.forward);

            attackedOverlayFX.alpha = 0f;
            meleeAttackOverlayFX.alpha = 0f;
            rangedAttackOverlayFX.alpha = 0f;
            outOfAmmoOverlayFX.alpha = 0f;

            StartMission();
        }


        public void StartMission () {
            IsMissionOnGoing = true;

            if (playerSpawnPointsParent.childCount > 0) {

                int playerSpawnPointIndex = Random.Range(0, playerSpawnPointsParent.childCount);

                Player player = Instantiate(playerPrefab, playerSpawnPointsParent.GetChild(playerSpawnPointIndex).position, Quaternion.identity, transform).GetComponent<Player>();
            }
            else {
                print("Missing Player Spawn Point!!");
            }
        }

        public void MissionSuccess () {
            IsMissionOnGoing = false;
            PlayMissionSuccessOverlayFX();
        }


        public void PlayAttackedOverlayFX () {
            attackedOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayMeleeAttackOverlayFX () {
            meleeAttackOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayRangedAttackOverlayFX () {
            rangedAttackOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayOutOfAmmoOverlayFX () {
            outOfAmmoOverlayFX.DOFade(0f, attackedOverlayFXDuration)
                .From(1f)
                .SetEase(attackedOverlayFXEase);
        }

        public void PlayMissionSuccessOverlayFX () {
            print("SUCCESS!");
        }

    }
}
