using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class GlobalManager : SingletonMonoBehaviour<GlobalManager> {

        public const float minDeltaAngle = 0.187f;


        public bool isMapViewer = false;


        [Header("Output Shows")]
        public int visionSpansMaxEdgesResolveIterationsSoFar = 0;

        protected override void Awake () {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            DOTween.Init();
        }


        public void StartLevel (string levelName, bool willPlayAsMapViewer) {
            isMapViewer = willPlayAsMapViewer;
            SceneManager.LoadScene(levelName);
        }


        void OnApplicationQuit () {
            print("[Vision Span] Vision Spans Max Edge Resolve Iterations Count is " + visionSpansMaxEdgesResolveIterationsSoFar);
        }
    }

}
