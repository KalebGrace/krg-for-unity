using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRG {

    public static class GameObjectExtensionMethods {

#region Persist New Scene

        public static void PersistNewScene(this GameObject obj) {
            Transform t = obj.transform;
            while (t.parent != null) {
                t = t.parent;
            }
            Object.DontDestroyOnLoad(t);
        }

#endregion

#region Interfaces (allows for similar functionality across vastly different objects)

        /// <summary>
        /// Calls all interfaces of the specified type that are on this GameObject,
        /// specifically by passing each as a parameter into the provided Action.
        /// </summary>
        public static void CallInterfaces<T>(this GameObject gameObject, System.Action<T> action) {
            T[] interfaces = GetInterfaces<T>(gameObject);
            for (int i = 0; i < interfaces.Length; i++) {
                action(interfaces[i]);
            }
        }

        /// <summary>
        /// Gets all interfaces of the specified type that are on this GameObject.
        /// </summary>
        public static T[] GetInterfaces<T>(this GameObject gameObject) {
            if (typeof(T).IsInterface) {
                return gameObject.GetComponents<Component>().OfType<T>().ToArray<T>();
            } else {
                G.U.Error("Error while getting interfaces for {0}: {1} is not an interface.",
                    gameObject.name, typeof(T));
                return new T[0];
            }
        }

#endregion

    }
}
