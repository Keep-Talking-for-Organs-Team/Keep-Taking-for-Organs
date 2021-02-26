using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerAnimManager : MonoBehaviour {

        public enum State {
            Idle,
            Dead,
            Aiming,
            None
        }


        [Header("REFS")]
        public Text deathText;
        public Text aimText;
        public Text aimingProcessText;
        public Text meleeText;
        public Text rangedText;


        public State CurrentState {get; private set;} = State.None;


        // Components
        Player _player;
        PlayerAttackManager _attackManager;

        void Awake () {
            _player = GetComponent<Player>();
            _attackManager = GetComponent<PlayerAttackManager>();

            Play(State.Idle);
        }

        void ShutAll () {
            deathText.enabled = false;
            aimText.enabled = false;
            aimingProcessText.enabled = false;
        }

        void Update () {

            meleeText.enabled = false;
            rangedText.enabled = false;

            if (_player.IsControllable && _player.IsFacingControllable && _attackManager != null) {
                PlayerAttackManager.AttackMethod atkMethod = _attackManager.AvailableAttackMethod;

                if (atkMethod == PlayerAttackManager.AttackMethod.Melee) {
                    meleeText.enabled = true;
                }
                else if (atkMethod == PlayerAttackManager.AttackMethod.Ranged) {
                    rangedText.enabled = true;
                }
            }
        }


        public void Play (State state) {

            CurrentState = state;

            ShutAll();

            if (state == State.Dead) {
                deathText.enabled = true;
            }
            else if (state == State.Aiming) {
                aimText.enabled = true;
                aimingProcessText.enabled = true;
                aimingProcessText.text = "0%";
            }
        }



    }
}
