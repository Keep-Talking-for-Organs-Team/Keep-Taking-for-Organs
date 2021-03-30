using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class HUDManager : MonoBehaviour {

        [Header("REFS")]
        public WeaponStatusDisplay meleeDisplay;
        public WeaponStatusDisplay rangedDisplay;
        public Text timerDisplayText;


        public void UpdateTimerDisplay (float timerTime) {
            if (timerTime < 0)
                timerDisplayText.text = "-- : --";
            else
                timerDisplayText.text = TimerTimeDisplay.FromSeconds(timerTime).MinSecDisplay;
        }

        public void UpdateWeaponStatusDisplay (PlayerAttackManager.AttackMethod atkMethod, float cooldownTimeRemainedRate, int bulletsCount = 0) {

            if (atkMethod == PlayerAttackManager.AttackMethod.Melee) {
                meleeDisplay.UpdateCooldownTimeRemainedRate(cooldownTimeRemainedRate);
            }
            else if (atkMethod == PlayerAttackManager.AttackMethod.Ranged) {
                rangedDisplay.UpdateBulletsDisplay(bulletsCount);

                if (bulletsCount != 0)
                    rangedDisplay.UpdateCooldownTimeRemainedRate(cooldownTimeRemainedRate);
                else
                    rangedDisplay.UpdateCooldownTimeRemainedRate(1f);
            }

        }

    }
}
