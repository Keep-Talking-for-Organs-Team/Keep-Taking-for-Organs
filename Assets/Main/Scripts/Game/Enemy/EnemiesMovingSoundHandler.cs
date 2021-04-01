using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class EnemiesMovingSoundHandler : MonoBehaviour {


        public bool HasMovingEnemiesInPlayersSight {
            get => _hasMovingEnemiesInPlayersSight;
            set {
                if (_hasMovingEnemiesInPlayersSight != value) {
                    _hasMovingEnemiesInPlayersSight = value;

                    if (value == true)
                        Play();
                    else
                        Stop();
                }
            }
        }

        bool _hasMovingEnemiesInPlayersSight = false;



        void Update () {
            if (GlobalManager.current.isMapViewer)
                return;

            bool hasMovingEnemiesInPlayersSightThisFrame = false;

            foreach (Transform child in transform) {

                Enemy enemy = child.gameObject.GetComponent<Enemy>();

                if (enemy != null) {
                    EnemyMoveManager enemyMoveManager = enemy.GetComponent<EnemyMoveManager>();

                    if (enemyMoveManager != null && enemyMoveManager.IsMoving && enemy.IsInPlayersSight) {
                        hasMovingEnemiesInPlayersSightThisFrame = true;
                    }
                }
            }

            HasMovingEnemiesInPlayersSight = hasMovingEnemiesInPlayersSightThisFrame;
        }


        void Play () {
            GlobalManager.current.PostAudioEvent("Play_Robot_Move");
        }

        void Stop () {
            GlobalManager.current.PostAudioEvent("Stop_Robot_Move");
        }

    }
}
