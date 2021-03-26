using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    public class MapViewerControlManager : MonoBehaviour {

        [Header("Properties")]
        public float cameraMoveSpeed = 1f;
        public float cameraRotateSpeed = 100f;
        public Ease  cameraRotateEase;
        public float cameraZoomSpeedMouseScroll = 1f;
        public float cameraZoomSpeedOthers = 1f;

        Vector3 _initPos = Vector3.zero;
        int     _rotatePosition = 0;
        Tween   _rotationAnim;
        float   _currentRotationAngle = 0;

        // COmponents
        Camera _cam;

        void Awake () {
            _initPos = transform.position;

            _cam = GetComponent<Camera>();
        }

        void Update () {


            if (Input.GetButtonDown("Reset Position")) {
                transform.position = _initPos;
            }

            Vector3 movement = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
            transform.position = transform.position + transform.rotation * movement * cameraMoveSpeed * Time.deltaTime * _cam.orthographicSize;


            int rotate = 0;

            if (Input.GetButtonDown("Rotate Positive")) {
                rotate += 1;
            }
            if (Input.GetButtonDown("Rotate Negative")) {
                rotate -= 1;
            }

            if (rotate != 0) {
                _rotatePosition += _rotatePosition < 0 ? 4 : -4;
                _currentRotationAngle = _rotatePosition * 90f;

                _rotatePosition += (int) Mathf.Sign(rotate);

                _rotationAnim.Kill(false);
                _rotationAnim = DOTween.To(() => _currentRotationAngle, x => _currentRotationAngle = x, _rotatePosition * 90f, cameraRotateSpeed)
                    .SetSpeedBased()
                    .SetEase(cameraRotateEase);
            }

            transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, Vector3.forward);


            _cam.orthographicSize = _cam.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeedMouseScroll;
            _cam.orthographicSize = _cam.orthographicSize + (Input.GetAxis("Zoom Out") - Input.GetAxis("Zoom In")) * cameraZoomSpeedOthers;
            _cam.orthographicSize = _cam.orthographicSize - Input.GetAxis("Zoom") * cameraZoomSpeedOthers;

        }

    }
}
