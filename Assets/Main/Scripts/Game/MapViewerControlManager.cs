using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    [DisallowMultipleComponent]
    public class MapViewerControlManager : MonoBehaviour {

        public float cameraMoveSpeed = 1f;
        public float cameraZoomSpeed = 1f;

        Vector3 _initPos = Vector3.zero;

        // COmponents
        Camera _cam;

        void Awake () {
            _initPos = transform.position;

            _cam = GetComponent<Camera>();
        }

        void Update () {

            // === temp ===
            if (Input.GetKeyDown(KeyCode.Space)) {
                transform.position = _initPos;
            }

            Vector3 movement = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
            transform.position = transform.position + transform.rotation * movement * cameraMoveSpeed * Time.deltaTime;


            if (Input.GetKeyDown(KeyCode.Q)) {
                transform.rotation = Quaternion.AngleAxis(90, Vector3.back) * transform.rotation;
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                transform.rotation = Quaternion.AngleAxis(-90, Vector3.back) * transform.rotation;
            }

            _cam.orthographicSize = _cam.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
            // === ==== ===
        }

    }
}
