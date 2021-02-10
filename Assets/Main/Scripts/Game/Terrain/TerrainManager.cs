using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class TerrainManager : MonoBehaviour {

        public static TerrainManager current;

        [Header("REFS")]
        public GameObject hidingArea;


        Collider2D[] _hidingAreaColliders;


        void Awake () {
            ComponentsTools.SetAndKeepAttachedGameObjectUniquely<TerrainManager>(ref current, this);
        }

        void Start () {
            _hidingAreaColliders = hidingArea.GetComponents<Collider2D>();
        }

        public bool IsInHidingArea (Vector2 pos) {
            foreach (Collider2D collider in _hidingAreaColliders) {
                Bounds bounds = collider.bounds;
                if (pos.x > bounds.min.x && pos.y > bounds.min.y && pos.x < bounds.max.x && pos.y < bounds.max.y) {
                    return true;
                }
            }
            return false;
        }

    }
}
