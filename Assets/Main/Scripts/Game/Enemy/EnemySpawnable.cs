using System.Collections.Generic;

using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class EnemySpawnable : MonoBehaviour {

        public static List<EnemySpawnable> list = new List<EnemySpawnable>();

        [Header("Properties")]
        public EnemyVisionManager.State defaultVisionState;
        public EnemyMoveManager.State   defaultMoveState;
        public PathHolder patrollingPath;
        public bool lockedOnPath;
        public float positionInPath;
        public VisionSpan.SpanProps standingEnemyVisionSpanProps;
        public VisionSpan.SpanProps patrollingEnemyVisionSpanProps;

        [Header("On Map Properties")]
        public float visionAreaLinesWidthOnMap = 1f;
        public Color visionAreaLinesColorOnMap = Color.white;

        [Header("REFS")]
        public GameObject  spriteGO;
        public LineFactory lineFactory;

        [Header("Prefabs")]
        public GameObject enemyPrefab;


        public Vector2 FacingDirection => transform.rotation * enemyPrefab.GetComponent<Enemy>().defaultDir;

        PathHolder _prevPath;

        void Awake () {
            list.Add(this);

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


            if (lockedOnPath) {
                if (patrollingPath != null && patrollingPath.PointCount > 0) {
                    transform.position = patrollingPath.GetPositionInPath(positionInPath);
                }
            }
             _prevPath = patrollingPath;
        }

        void OnDestroy () {
            if (list.Contains(this))
                list.Remove(this);
        }


        public void Spawn () {

            if (!GlobalManager.current.isMapViewer) {

                GameObject enemyGO = Instantiate(enemyPrefab, (Vector2) transform.position, transform.rotation, GameSceneManager.current.enemiesParent);

                enemyGO.GetComponent<Enemy>().visionSpan.spanProps = patrollingPath == null ? standingEnemyVisionSpanProps : patrollingEnemyVisionSpanProps;

                var visionManager = enemyGO.GetComponent<EnemyVisionManager>();
                if (visionManager != null)
                    visionManager.defaultState = defaultVisionState;

                var moveManager = enemyGO.GetComponent<EnemyMoveManager>();
                if (moveManager != null)
                    moveManager.defaultState = defaultMoveState;

                var patrolManager = enemyGO.GetComponent<EnemyPatrolManager>();
                if (patrolManager != null)
                    patrolManager.path = patrollingPath;

            }
            else {
                // Is Map Viewer
                spriteGO.transform.SetParent(GameSceneManager.current.enemiesParent);
                lineFactory.transform.SetParent(GameSceneManager.current.enemiesParent);

                DrawLinesOfVisionArea();

                if (patrollingPath != null) {
                    if (!patrollingPath.IsLineDrawn) {
                        patrollingPath.DrawPathLines();
                    }
                }
            }

            list.Remove(this);
        }


        void DrawLinesOfVisionArea () {
            lineFactory.ClearLines();

            VisionSpan.SpanProps visionSpanProps = patrollingPath == null ? standingEnemyVisionSpanProps : patrollingEnemyVisionSpanProps;

            for (int i = -1 ; i <= 1 ; i += 2) {
                lineFactory.GetLine(transform.position, transform.position + Quaternion.AngleAxis(i * visionSpanProps.fov / 2, Vector3.forward) * FacingDirection * visionSpanProps.distance, visionAreaLinesWidthOnMap, visionAreaLinesColorOnMap);
            }
        }

    }
}
