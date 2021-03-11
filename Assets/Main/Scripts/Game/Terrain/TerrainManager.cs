using UnityEngine;
using UnityEngine.Tilemaps;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class TerrainManager : MonoBehaviour {

        [Header("REFS")]
        public GameObject trapArea;
        public GameObject hidingArea;


        Tilemap      _trapAreaTilemap;
        Collider2D[] _trapAreaColliders;
        Collider2D[] _hidingAreaColliders;



        void Awake () {
            _trapAreaTilemap = trapArea.GetComponent<Tilemap>();
            _trapAreaColliders = trapArea.GetComponents<Collider2D>();
            _hidingAreaColliders = hidingArea.GetComponents<Collider2D>();
        }

        void Start () {
            if (!GlobalManager.current.isMapViewer) {
                if (_trapAreaTilemap != null) {
                    _trapAreaTilemap.SetOpacity(0f);
                }
            }
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
