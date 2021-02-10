using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class PathHolder : MonoBehaviour {

        [Header("Options")]
        public bool isTailToHead;

        [Header("Gizmos Settings")]
        public Color gizmosColor = Color.white;
        public float gizmosSphereSize = 1f;


        public int PointCount => transform.childCount;
        public int SegmentsAmount => isTailToHead ? PointCount : PointCount - 1;


        void OnDrawGizmos () {

            Gizmos.color = gizmosColor;

            for (int i = 0 ; i < transform.childCount ; i++) {
                Gizmos.DrawSphere(transform.GetChild(i).position, gizmosSphereSize);

                if (i > 0) {
                    Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);

                    if (i == transform.childCount - 1 && isTailToHead) {
                        Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
                    }
                }
            }
        }


        public Vector2 GetPoint (int index) {
            return transform.GetChild(index % PointCount).position;
        }

        public Vector2[] GetSegment (int index) {

            Vector2[] result = new Vector2[2] {Vector2.zero, Vector2.zero};

            if (PointCount == 0) {
                return result;
            }
            else if (SegmentsAmount == 0) {
                result[0] = GetPoint(0);
                result[1] = GetPoint(0);
                return result;
            }
            else {

                if (isTailToHead) {
                    // circle
                    for (int i = 0 ; i < result.Length ; i++) {
                        result[i] = GetPoint(index + i);
                    }
                }
                else {
                    // ping-pong
                    bool reverseDir = false;
                    if ((index / SegmentsAmount) % 2 == 1)
                        reverseDir = true;

                    for (int i = 0 ; i < result.Length ; i++) {
                        if (reverseDir) {
                            result[i] = GetPoint(SegmentsAmount - (index + i) % SegmentsAmount);
                        }
                        else {
                            result[i] = GetPoint((index + i) % SegmentsAmount);
                        }
                    }
                }

                return result;
            }

        }

        public Vector2 GetClosetPointInPath (Vector2 source, out int segmentIndex) {

            segmentIndex = 0;

            if (PointCount == 0) {
                return Vector2.zero;
            }
            else {

                Vector2 resultPoint = GetPoint(0);
                float minSqrDist = (resultPoint - source).sqrMagnitude;

                for (int i = 0 ; i < SegmentsAmount ; i++) {
                    Vector2 point = GetClosetPointInSegment(GetSegment(i), source);
                    float sqrDist = (point - source).sqrMagnitude;

                    if (sqrDist < minSqrDist) {
                        resultPoint = point;
                        minSqrDist = sqrDist;
                        segmentIndex = i;
                    }
                }

                return resultPoint;
            }
        }


        public Vector2 GetClosetPointInSegment (Vector2[] segment, Vector2 source) {
            Vector2 segmentDir = (segment[1] - segment[0]).normalized;
            return segment[0] + Vector2.Dot(source - segment[0], segmentDir) * segmentDir;
        }

    }
}
