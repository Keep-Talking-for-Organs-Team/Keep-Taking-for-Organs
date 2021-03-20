using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player))]
    public class PlayerAnimManager : MonoBehaviour {

        public enum AttitudeState {
            Standing,
            Crouching
        }

        public enum VisibilityState {
            Visible,
            Invisible
        }

        public enum ActionState {
            Idle,
            Dead,
            Aiming,
            None
        }


        [Header("Properties")]
        public float hidingOpacity;
        public float rangedAttackableLineWidth = 0.23f;
        public Color rangedAttackableLineColor = Color.red;

        [Header("REFS")]
        public SpriteRenderer[] bodySRs;
        public Text deathText;
        public Text aimText;
        public Text aimingProcessText;
        public Text meleeText;
        public Text rangedText;


        public ActionState CurrentActionState {get; private set;} = ActionState.None;


        AttitudeState _attitudeState = AttitudeState.Standing;
        VisibilityState _visibilityState = VisibilityState.Visible;

        // Components
        Player _player;
        PlayerAttackManager _attackManager;

        void Awake () {
            _player = GetComponent<Player>();
            _attackManager = GetComponent<PlayerAttackManager>();

            PlayAction(ActionState.Idle);
        }

        void ShutAll () {
            deathText.enabled = false;
            aimText.enabled = false;
            aimingProcessText.enabled = false;
        }

        void Update () {


            // Attackable Instructions
            meleeText.enabled = false;
            rangedText.enabled = false;

            // if (_player.IsControllable && _player.IsFacingControllable && _attackManager != null) {
            //     PlayerAttackManager.AttackMethod atkMethod = _attackManager.AvailableAttackMethod;
            //
            //     if (atkMethod == PlayerAttackManager.AttackMethod.Melee) {
            //         meleeText.enabled = true;
            //     }
            //     else if (atkMethod == PlayerAttackManager.AttackMethod.Ranged) {
            //         rangedText.enabled = true;
            //     }
            // }
        }


        public void PlayAction (ActionState state) {

            CurrentActionState = state;

            ShutAll();

            if (state == ActionState.Dead) {
                deathText.enabled = true;
            }
            else if (state == ActionState.Aiming) {
                aimText.enabled = true;
                aimingProcessText.enabled = true;
                aimingProcessText.text = "0%";
            }
        }

        public void ClearRangedAttackableLine () {
            GameSceneManager.current.operatorManager.playerRangedAttackableLineFactory.ClearLines();
        }

        public void DrawRangedAttackableLine (Vector2 startPoint, Vector2 targetPoint) {
            GameSceneManager.current.operatorManager.playerRangedAttackableLineFactory.GetLine(startPoint, targetPoint, rangedAttackableLineWidth, rangedAttackableLineColor);
        }


        public void OnStartWalking () {

        }

        public void OnStopWalking () {
            
        }

        public void OnStartHiding () {
            foreach (var sr in bodySRs)
                sr.SetOpacity(hidingOpacity);
        }

        public void OnStopHiding () {
            foreach (var sr in bodySRs)
                sr.SetOpacity(1f);
        }

    }
}
