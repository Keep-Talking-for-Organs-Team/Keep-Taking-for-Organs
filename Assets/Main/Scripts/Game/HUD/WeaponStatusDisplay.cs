using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class WeaponStatusDisplay : MonoBehaviour {

        [Header("Properties")]
        public Color UnavailableColor = Color.grey;
        public float bulletsIconIntervalDistance = 1f;

        [Header("REFS")]
        public Image      backPanelImage;
        public Image      iconImage;
        public GameObject cooldownBar;
        public Transform  bulletIconsParent;

        [Header("Prefabs")]
        public GameObject bulletIconPrefab;


        void Awake () {
            cooldownBar.SetActive(true);
            cooldownBar.transform.SetScaleX(0f);
        }

        public void UpdateCooldownTimeRemainedRate (float cooldownTimeRemainedRate) {

            float cooldownBarRate = 0f;

            if (cooldownTimeRemainedRate > 0) {
                backPanelImage.color = UnavailableColor;
                iconImage.color = UnavailableColor;
                cooldownBarRate = 1f - cooldownTimeRemainedRate;
            }
            else {
                backPanelImage.color = Color.white;
                iconImage.color = Color.white;
            }

            cooldownBar.transform.SetScaleX(cooldownBarRate);
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
