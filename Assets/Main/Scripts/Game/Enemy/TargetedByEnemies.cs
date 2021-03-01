using System.Collections.Generic;

using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class TargetedByEnemies : MonoBehaviour {

        public static List<TargetedByEnemies> list = new List<TargetedByEnemies>();


        [Header("Options")]
        public bool isEnabled = true;
        public bool isChasable = true;
        public bool isAttactable = true;


        public bool IsHiding => _isHiding;


        bool _isHiding;

        void Awake () {
            list.Add(this);
        }


        void OnDestroy () {
            list.Remove(this);
        }

        void FixedUpdate () {
            if (GameSceneManager.current.currentTerrain.IsInHidingArea(transform.position)) {
                _isHiding = true;
            }
            else {
                _isHiding = false;
            }
        }

    }
}
