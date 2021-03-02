using UnityEngine;
using UnityEngine.UI;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class EnemiesSpawnersManager : MonoBehaviour {



        public void StartSpawn () {

            foreach (Transform child in transform) {
                EnemiesSpawnGroup spawnGroup = child.gameObject.GetComponent<EnemiesSpawnGroup>();

                if (spawnGroup != null) {
                    spawnGroup.RandomSpawn();
                }
            }
        }

    }
}
