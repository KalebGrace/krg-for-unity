using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// SCRIPT EXECUTION ORDER: #10
    /// </summary>
    public class Prefab : MonoBehaviour {

        public Transform prefab;
        public PositionType positionType = PositionType.Relative;
        public bool autoInstantiate = true;
        public bool isExposed = true;
        public string key = string.Empty;

        [HideInInspector]
        public Transform instance;

        void Awake() {
            if (autoInstantiate) {
                Instantiate();
            }
        }

        public void Instantiate() {
            if (prefab == null) {
                G.U.Error("{0}'s prefab component has no prefab reference.", gameObject.name);
                return;
            }

            instance = Instantiate(prefab);
            Vector3 pos = instance.localPosition;

            instance.name = prefab.name; //remove "(Clone)"
            instance.parent = transform;

            if (positionType == PositionType.Relative) {
                instance.localPosition = pos;
            }

            if (isExposed) {
                key = key.Trim();
                key = string.IsNullOrEmpty(key) ? instance.name : key;
                gameObject.CallInterfaces<IPrefab>(x => {
                    x.prefabs.Add(key, instance);
                    x.OnPrefabAdd(key, instance);
                });
            }

            this.Dispose();
        }
    }
}
