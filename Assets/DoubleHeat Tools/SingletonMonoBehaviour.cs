using System;
using System.Collections.Generic;

using UnityEngine;

namespace DoubleHeat {

    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

        public enum KeepRule {
            KeepOld,
            KeepNew
        }

        public static T current = null;
        // public static Dictionary<Type, SingletonMonoBehaviour> singletonInstances = new Dictionary<Type, SingletonMonoBehaviour>();


        [Header("Singleton Options")]
        public KeepRule keepRule = KeepRule.KeepOld;


        protected virtual void Awake () {

            if (current == null) {
                current = GetComponent<T>();
            }
            else {
                if (keepRule == KeepRule.KeepOld) {

                    if (this.gameObject != null)
                        Destroy(this.gameObject);

                }
                else if (keepRule == KeepRule.KeepNew) {

                    if (current.gameObject != null)
                        Destroy(current.gameObject);

                    current = GetComponent<T>();
                }
            }

        }

        protected virtual void OnDestroy () {
            if (current == this)
                current = null;
        }

    }
}
