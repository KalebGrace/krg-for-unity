using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class ComponentExtensionMethods {

        public static void PersistNewScene(this Component obj) {
            Transform t = obj.transform;
            while (t.parent != null) {
                t = t.parent;
            }
            Object.DontDestroyOnLoad(t);
        }
    }
}
