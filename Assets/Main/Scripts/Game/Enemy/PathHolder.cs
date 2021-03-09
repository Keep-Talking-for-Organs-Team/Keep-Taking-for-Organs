using UnityEngine;

using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class PathHolder : MonoBehaviour {

        public enum TurnSide {
            Left,
            Right
        }


        [Header("Options")]
        public bool isTailToHead;
        public TurnSide headTurnSide;
        public TurnSide tailTurnSide;

        [Header("On Map Properties")]
        public float drawnPathLinesWidth = 0.023f;
        public Color drawnPathLinesColor = Color.white;


        [Header("Gizmos Settings")]
        public Color gizmosColor = Color.white;
        public float gizmosSphereSize = 1f;


        public bool IsLineDrawn {get; private set;} = false;
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


        public Vector2 GetPoint (int ascendedIndex) {
            return transform.GetChild(ascendedIndex % PointCount).position;
        }

        public Vector2[] GetSegment (int ascendedIndex) {

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
                        result[i] = GetPoint(ascendedIndex + i);
                    }
                }
                else {
                    // ping-pong
                    bool reverseDir = false;
                    if ((ascendedIndex / SegmentsAmount) % 2 == 1)
                        reverseDir = true;

                    for (int i = 0 ; i < result.Length ; i++) {
                        if (reverseDir) {
                            result[i] = GetPoint(SegmentsAmount - (ascendedIndex + i) % SegmentsAmount);
                        }
                        else {
                            result[i] = GetPoint((ascendedIndex + i) % SegmentsAmount);
                        }
                    }
                }

                return result;
            }

        }

        public Vector2 GetPositionInPath (float positionValue) {
            positionValue = Mathf.Max(positionValue, 0f);

            Vector2 prevPoint = GetPoint((int) positionValue);
            Vector2 segmentVector = GetPoint((int) positionValue + 1) - prevPoint;

            return prevPoint + (positionValue % 1) * segmentVector;
        }

        public Vector2 GetClosestPointInPath (Vector2 source, out int segmentIndex) {

            segmentIndex = 0;

            if (PointCount == 0) {
                return Vector2.zero;
            }
            else {

                Vector2 resultPoint = GetPoint(0);
                float minSqrDist = (resultPoint - source).sqrMagnitude;

                for (int i = 0 ; i < SegmentsAmount ; i++) {
                    Vector2 point = source.GetClosestPointInSegment(GetSegment(i));
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



        public int GetTurnDirectionAtEndPoint (int ascendedIndex) {
            if (!isTailToHead) {
                TurnSide turnSide = TurnSide.Left;
                int index = ascendedIndex % PointCount;

                if (index == 0)
                    turnSide = headTurnSide;
                else if (index == PointCount - 1)
                    turnSide = tailTurnSide;
                else
                    return 0;

                if (turnSide == TurnSide.Left)
                    return 1;
                else if (turnSide == TurnSide.Right)
                    return -1;
            }
            return 0;
        }


        public void DrawPathLines () {

            for (int i = 0 ; i < SegmentsAmount ; i++) {
                Vector2[] segment = GetSegment(i);
                GameSceneManager.current.lineFactory.GetLine(segment[0], segment[1], drawnPathLinesWidth, drawnPathLinesColor);
            }

            IsLineDrawn = true;
        }

    }
}
