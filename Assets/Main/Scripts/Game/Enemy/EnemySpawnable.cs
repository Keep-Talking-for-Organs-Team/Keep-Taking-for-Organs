using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class EnemySpawnable : MonoBehaviour {

        public EnemyVisionManager.State defaultVisionState;
        public EnemyMoveManager.State   defaultMoveState;
        public PathHolder patrollingPath;
        public bool clickedToResetPositionToPath;

        [Header("REFS")]
        public GameObject enemyPrefab;


        PathHolder _prevPath;

        void Awake () {
            _prevPath = patrollingPath;
        }

        void OnValidate () {
            bool pathChanged = false;

            if (patrollingPath !=  _prevPath) {
                pathChanged = true;
            }

            if (pathChanged) {
                if (patrollingPath == null) {
                    defaultMoveState = EnemyMoveManager.State.Standing;
                }
                else {
                    defaultMoveState = EnemyMoveManager.State.Patrolling;
                }
            }

            if (pathChanged || clickedToResetPositionToPath) {
                if (patrollingPath != null && patrollingPath.PointCount > 0) {
                    transform.position = patrollingPath.GetPoint(0);
                }
            }
             _prevPath = patrollingPath;
        }


        public GameObject Spawn () {
            GameObject result = Instantiate(enemyPrefab, transform.position, transform.rotation, GameSceneManager.current.enemiesParent);

            var visionManager = result.GetComponent<EnemyVisionManager>();
            if (visionManager != null)
                visionManager.defaultState = defaultVisionState;

            var moveManager = result.GetComponent<EnemyMoveManager>();
            if (moveManager != null)
                moveManager.defaultState = defaultMoveState;

            var patrolManager = result.GetComponent<EnemyPatrolManager>();
            if (patrolManager != null)
                patrolManager.path = patrollingPath;

            return result;
        }

    }
}
