using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class EnemiesSpawnersManager : MonoBehaviour {

        public int SpawnedCount {get; private set;} = 0;

        public void StartSpawn () {

            foreach (Transform child in transform) {
                EnemiesSpawnGroup spawnGroup = child.gameObject.GetComponent<EnemiesSpawnGroup>();

                if (spawnGroup != null) {
                    
                    int spawnedCount;
                    spawnGroup.RandomSpawn(out spawnedCount);

                    SpawnedCount += spawnedCount;
                }
            }
        }

    }
}
