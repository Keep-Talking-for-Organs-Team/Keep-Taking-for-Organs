using Math = System.Math;
using System.Collections.Generic;

using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class EnemiesSpawnGroup : MonoBehaviour {

        [Header("Options")]
        [Range(0f, 1f)]
        public float spawnPosibility = 1f;
        public int minSpawnAmount = 1;
        public int maxSpawnAmount = 1;

        [Header("Gizmos")]
        public Color gizmosColor = Color.white;
        public float gizmosSphereSize = 1f;

        EnemySpawnable[] _spawns;


        void OnDrawGizmos () {

            Gizmos.color = gizmosColor;

            for (int i = 0 ; i < transform.childCount ; i++) {
                Gizmos.DrawSphere(transform.GetChild(i).position, gizmosSphereSize);
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild((i + 1) % transform.childCount).position);
            }
        }


        public void RandomSpawn () {

            if (Random.value <= spawnPosibility) {
                List<EnemySpawnable> spawnables = new List<EnemySpawnable>();

                for (int i = 0 ; i < transform.childCount ; i++) {

                    EnemySpawnable spawnable = transform.GetChild(i).gameObject.GetComponent<EnemySpawnable>();

                    if (spawnable != null) {
                        spawnables.Add(spawnable);
                    }
                }


                int spawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount + 1);

                if (spawnAmount < 0)
                    _spawns = new EnemySpawnable[spawnables.Count];
                else
                    _spawns = new EnemySpawnable[Math.Min(spawnAmount, spawnables.Count)];

                for (int i = 0 ; i < _spawns.Length ; i++) {

                    int index = Random.Range(0, spawnables.Count);

                    _spawns[i] = spawnables[index];
                    spawnables.RemoveAt(index);
                }


                foreach (EnemySpawnable spawn in _spawns) {
                    spawn.Spawn();
                }
            }

            Destroy(gameObject);
        }

    }
}
