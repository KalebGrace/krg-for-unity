using System.Linq;
using UnityEngine;

namespace _0G.Legacy
{
    public static class GameObjectEM // GameObject extension methods
    {
        public static void Dispose(this GameObject me)
        {
            if (me == null)
            {
                G.U.Warn("The GameObject you wish to dispose of is null.");
                return;
            }
            Object.Destroy(me);
        }

        public static void PersistNewScene(this GameObject me, PersistNewSceneType persistNewSceneType)
        {
            Transform t = me.transform;
            switch (persistNewSceneType)
            {
                case PersistNewSceneType.PersistAllParents:
                    while (t.parent != null)
                    {
                        t = t.parent;
                    }
                    break;
                case PersistNewSceneType.MoveToHierarchyRoot:
                    t.SetParent(null);
                    break;
                default:
                    G.U.Unsupported(me, persistNewSceneType);
                    break;
            }
            Object.DontDestroyOnLoad(t);
        }

        /// <summary>
        /// REQUIRE COMPONENT to be on SPECIFIED GAME OBJECT:
        /// Require the specified Component type to exist on the specified source GameObject.
        /// </summary>
        /// <param name="me">Source GameObject.</param>
        /// <param name="throwException">If set to <c>true</c> throw exception.</param>
        /// <typeparam name="T">The required Component type.</typeparam>
        public static T Require<T>(this GameObject me, bool throwException = true) where T : Component
        {
            if (!G.U.SourceExists(me, typeof(T), throwException)) return null;
            T comp = me.GetComponent<T>();
            if (G.U.IsNull(comp))
            {
                string s = string.Format("A {0} Component must exist on the {1} GameObject.",
                    typeof(T), me.name);
                G.U.ErrorOrException(s, throwException);
                return null;
            }
            return comp;
        }

        //  Interfaces (allows for similar functionality across vastly different objects)
        //  NOTE: The following methods may need to be revised.

        /// <summary>
        /// Calls all interfaces of the specified type that are on this GameObject,
        /// specifically by passing each as a parameter into the provided Action.
        /// </summary>
        public static void CallInterfaces<T>(this GameObject me, System.Action<T> action)
        {
            T[] interfaces = GetInterfaces<T>(me);
            for (int i = 0; i < interfaces.Length; i++)
            {
                action(interfaces[i]);
            }
        }

        /// <summary>
        /// Gets all interfaces of the specified type that are on this GameObject.
        /// </summary>
        public static T[] GetInterfaces<T>(this GameObject me)
        {
            if (typeof(T).IsInterface)
            {
                return me.GetComponents<Component>().OfType<T>().ToArray<T>();
            }
            else
            {
                G.U.Err("Error while getting interfaces for {0}: {1} is not an interface.",
                    me.name, typeof(T));
                return new T[0];
            }
        }
    }
}