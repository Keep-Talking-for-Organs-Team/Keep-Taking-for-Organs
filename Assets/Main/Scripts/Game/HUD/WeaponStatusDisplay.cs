using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class WeaponStatusDisplay : MonoBehaviour {

        [Header("Properties")]
        public float bulletsIconIntervalDistance = 1f;

        [Header("REFS")]
        public GameObject centerFilled;
        public Transform  bulletIconsParent;

        [Header("Prefabs")]
        public GameObject bulletIconPrefab;


        void Awake () {
            centerFilled.SetActive(true);
        }

        public void UpdateCooldownTimeRemainedRate (float cooldownTimeRemainedRate) {
            centerFilled.transform.SetScaleY(1f - cooldownTimeRemainedRate);
        }

        public void UpdateBulletsDisplay (int bulletsCount) {
            ShowBullets(bulletsCount);
        }

        // public void SetWeapon (PlayerAttackManager.AttackMethod type, int bulletsCount = 0) {
        //     if (type == PlayerAttackManager.AttackMethod.Melee) {
        //         meleeIcon.SetActive(true);
        //         rangedIcon.SetActive(false);
        //         ClearBulletIcons();
        //     }
        //     else if (type == PlayerAttackManager.AttackMethod.Ranged) {
        //         meleeIcon.SetActive(false);
        //         rangedIcon.SetActive(true);
        //         ShowBullets(bulletsCount);
        //     }
        // }

        void ShowBullets (int bulletsCount) {
            if (bulletIconsParent == null || bulletIconPrefab == null)
                return;

            ClearBulletIcons();
            for (int i = 0 ; i < bulletsCount ; i++) {
                GameObject bulletIcon = Instantiate(bulletIconPrefab, bulletIconsParent);
                bulletIcon.GetComponent<RectTransform>().SetAnchoredPosX( ((bulletsCount - 1) / 2f - i) * bulletsIconIntervalDistance );
            }
        }

        void ClearBulletIcons () {
            if (bulletIconsParent == null || bulletIconPrefab == null)
                return;

            foreach (Transform child in bulletIconsParent) {
                Destroy(child.gameObject);
            }
        }

    }
}
