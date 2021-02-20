using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class TerrainManager : MonoBehaviour {

        public static TerrainManager current;

        [Header("REFS")]
        public GameObject trapArea;
        public GameObject hidingArea;


        Collider2D[] _trapAreaColliders;
        Collider2D[] _hidingAreaColliders;


        void Awake () {
            ComponentsTools.SetAndKeepAttachedGameObjectUniquely<TerrainManager>(ref current, this);
        }

        void Start () {
            _trapAreaColliders = trapArea.GetComponents<Collider2D>();
            _hidingAreaColliders = hidingArea.GetComponents<Collider2D>();
        }



        public bool IsInTrapArea (Vector2 pos) {
            return IsInCollidersBounds(pos, _trapAreaColliders);
        }

        public bool IsInHidingArea (Vector2 pos) {
            return IsInCollidersBounds(pos, _hidingAreaColliders);
        }


        bool IsInCollidersBounds (Vector2 pos, Collider2D[] colliders) {
            foreach (Collider2D collider in colliders) {
                Bounds bounds = collider.bounds;
                if (pos.x > bounds.min.x && pos.y > bounds.min.y && pos.x < bounds.max.x && pos.y < bounds.max.y) {
                    return true;
                }
            }
            return false;
        }

    }
}
