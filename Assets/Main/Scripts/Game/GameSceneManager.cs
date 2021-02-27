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
        public Camera      mainCam;
        public Transform   enemiesParent;
        public CanvasGroup attackedOverlayFX;
        public CanvasGroup meleeAttackOverlayFX;
        public CanvasGroup rangedAttackOverlayFX;
        public CanvasGroup outOfAmmoOverlayFX;



        protected override void Awake () {
            base.Awake();
            HideSpriteAtRuntime.isActive = activeHideSpriteAtRuntime;
        }

        void Start () {
            if (enableRandomCamRotation)
                mainCam.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, Vector3.forward);


            attackedOverlayFX.alpha = 0f;
        }


        void OnValidate () {

            Time.timeScale = timeScale;

            if (overrideViewSpanMaxSegmentGapAngle > 0)
                VisionSpan.maxSegmentGapAngle = overrideViewSpanMaxSegmentGapAngle;
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

    }
}
