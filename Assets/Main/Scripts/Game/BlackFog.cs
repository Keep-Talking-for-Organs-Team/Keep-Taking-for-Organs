using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class BlackFog : MonoBehaviour {

        Camera _attachedCam;

        void Awake () {
            if (transform.parent != null)
                _attachedCam = transform.parent.gameObject.GetComponent<Camera>();
        }

        void Start () {
            if (_attachedCam == null) {
                if (GameSceneManager.current != null && GameSceneManager.current.operatorManager != null) {
                    _attachedCam = GameSceneManager.current.operatorManager.cam;
                }
            }
        }

        void Update () {

            float height = _attachedCam.orthographicSize * 2 * 100;
            float width = height * _attachedCam.aspect;

            SetSize(width, height);
        }

        void SetSize (float width, float height) {
            transform.localScale = new Vector3(width, height, 1f);
        }

    }
}
