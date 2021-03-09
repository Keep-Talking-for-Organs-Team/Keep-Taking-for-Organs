using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class HUDManager : MonoBehaviour {

        [Header("REFS")]
        public WeaponStatusDisplay weaponStatusDisplay;
        public Text timerDisplayText;


        public void UpdateTimerDisplay (float timerTime) {
            timerDisplayText.text = TimerTimeDisplay.FromSeconds(timerTime).MinSecDisplay;
        }

        public void UpdateWeaponStatusDisplay (PlayerAttackManager.AttackMethod atkMethod, float cooldownTimeRemainedRate, int bulletsCount) {
            weaponStatusDisplay.SetWeapon(atkMethod, bulletsCount);
            weaponStatusDisplay.UpdateCooldownTimeRemainedRate(cooldownTimeRemainedRate);
        }

    }
}
