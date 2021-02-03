using System.Collections.Generic;

using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class TargetedByEnemies : MonoBehaviour {

        public static List<TargetedByEnemies> InstanceList = new List<TargetedByEnemies>();


        public bool isEnabled = true;


        void Awake () {
            InstanceList.Add(this);
        }


        void OnDestroy () {
            InstanceList.Remove(this);
        }

    }
}
