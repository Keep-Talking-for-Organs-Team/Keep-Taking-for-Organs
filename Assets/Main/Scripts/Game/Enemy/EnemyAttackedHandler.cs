using UnityEngine;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyAttackedHandler : MonoBehaviour {

        public Enemy enemy;

    }
}
