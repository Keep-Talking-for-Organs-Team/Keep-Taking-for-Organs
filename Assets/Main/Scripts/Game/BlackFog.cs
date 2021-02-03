using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class BlackFog : MonoBehaviour {

        Camera _attachedCam;

        void Awake () {
            if (transform.parent != null)
                _attachedCam = transform.parent.gameObject.GetComponent<Camera>();
        }

        void Start () {
            _attachedCam = _attachedCam ?? GameSceneManager.current.mainCam;
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
