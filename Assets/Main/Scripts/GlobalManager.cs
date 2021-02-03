using UnityEngine;

using DG.Tweening;

namespace KeepTalkingForOrgansGame {

    public class GlobalManager : MonoBehaviour {

        public static GlobalManager current;

        [Header("Output Shows")]
        public int visionSpansMaxEdgesResolveIterationsSoFar = 0;

        void Awake () {
            current = this;
            DontDestroyOnLoad(gameObject);

            DOTween.Init();
        }

        void OnApplicationQuit () {
            print("[Vision Span] Vision Spans Max Edge Resolve Iterations Count is " + visionSpansMaxEdgesResolveIterationsSoFar);
        }
    }
}
