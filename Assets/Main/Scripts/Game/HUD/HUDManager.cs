using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class HUDManager : MonoBehaviour {

        [Header("Properties")]
        public float timerChangeColorTimeThreshold = -1f;
        public Color timerChangedColor;

        [Header("REFS")]
        public WeaponStatusDisplay meleeDisplay;
        public WeaponStatusDisplay rangedDisplay;
        public Text timerDisplayText;


        Color _defaultTimerColor = Color.white;
        bool _isTimerColorChanged = false;

        void Awake () {
            _defaultTimerColor = timerDisplayText.color;
        }

        public void UpdateTimerDisplay (float timerTime) {
            if (timerTime < 0) {
                timerDisplayText.text = "-- : --";
                timerDisplayText.color = _defaultTimerColor;
            }
            else {
                timerDisplayText.text = TimerTimeDisplay.FromSeconds(timerTime).MinSecDisplay;

                if (!_isTimerColorChanged && timerTime < timerChangeColorTimeThreshold) {

                    timerDisplayText.color = timerChangedColor;
                    _isTimerColorChanged = true;
                }
            }
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
