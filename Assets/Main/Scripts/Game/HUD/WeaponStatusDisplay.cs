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
        public GameObject meleeIcon;
        public GameObject rangedIcon;
        public Transform  bulletIconsParent;

        [Header("Prefabs")]
        public GameObject bulletIconPrefab;


        void Awake () {
            centerFilled.SetActive(true);

            if (meleeIcon.activeSelf)
                meleeIcon.SetActive(false);

            if (rangedIcon.activeSelf)
                rangedIcon.SetActive(false);
        }

        public void UpdateCooldownTimeRemainedRate (float cooldownTimeRemainedRate) {
            centerFilled.transform.SetScaleY(1f - cooldownTimeRemainedRate);
        }

        public void UpdateBulletsDisplay (int bulletsCount) {
            ShowBullets(bulletsCount);
        }

        public void SetWeapon (PlayerAttackManager.AttackMethod type, int bulletsCount = 0) {
            if (type == PlayerAttackManager.AttackMethod.Melee) {
                meleeIcon.SetActive(true);
                rangedIcon.SetActive(false);
                ClearBulletIcons();
            }
            else if (type == PlayerAttackManager.AttackMethod.Ranged) {
                meleeIcon.SetActive(false);
                rangedIcon.SetActive(true);
                ShowBullets(bulletsCount);
            }
        }

        void ShowBullets (int bulletsCount) {
            ClearBulletIcons();
            for (int i = 0 ; i < bulletsCount ; i++) {
                GameObject bulletIcon = Instantiate(bulletIconPrefab, bulletIconsParent);
                bulletIcon.GetComponent<RectTransform>().SetAnchoredPosX( ((bulletsCount - 1) / 2f - i) * bulletsIconIntervalDistance );
            }
        }

        void ClearBulletIcons () {
            foreach (Transform child in bulletIconsParent) {
                Destroy(child.gameObject);
            }
        }

    }
}
