using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// G.Methods.cs is a partial class of G (G.cs).
    /// This contains static methods that can be used both in the editor (i.e. edit mode) and during runtime;
    /// a necessity for any functionality that is required before G and its Managers are instanced and fully set up.
    /// </summary>
    partial class G {

#region New

        /// <summary>
        /// Create a new instance of the specified prefab on the specified parent (use *null* for hierarchy root).
        /// This is essentially the same as Object.Instantiate, but allows for additional functionality.
        /// </summary>
        /// <param name="prefab">Prefab (original).</param>
        /// <param name="parent">Parent.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T New<T>(T prefab, Transform parent) where T : Object {
            if (prefab == null) {
                G.U.Error("Null prefab/original supplied for new object on {0}.", parent.name);
                return null;
            }

            T clone = Object.Instantiate(prefab, parent);

            clone.name = prefab.name; //remove "(Clone)" from name

            return clone;
        }

#endregion

#region End

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(GameObject obj) {
            if (EndNull(obj)) return;
            obj.CallInterfaces<IEnd>(MarkAsEnded);
            Object.Destroy(obj);
        }

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(IEnd obj) {
            if (EndNull(obj)) return;
            MarkAsEnded(obj);
            Object.Destroy(obj.end.owner);
        }

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(Object obj) {
            if (EndNull(obj)) return;
            //
            Object.Destroy(obj);
        }

        static bool EndNull(object o) {
            if (o == null) {
                G.U.Warning("The object you wish to end is null.");
                return true;
            } else {
                return false;
            }
        }

        static void MarkAsEnded(IEnd iEnd) {
            iEnd.end.MarkAsEnded();
        }

#endregion

#region Err

        public static void Err(object message, Object context) {
            Debug.LogError(message, context);
        }

#endregion

    }
}
