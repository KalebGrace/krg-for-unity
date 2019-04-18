using System.Linq;
using UnityEngine;

namespace KRG
{
    public static class GameObjectExtensionMethods
    {
        public static void Dispose(this GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }

        public static void PersistNewScene(this GameObject obj, PersistNewSceneType persistNewSceneType)
        {
            Transform t = obj.transform;
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
                    G.U.Unsupported(obj, persistNewSceneType);
                    break;
            }
            Object.DontDestroyOnLoad(t);
        }

        //  Interfaces (allows for similar functionality across vastly different objects)
        //  NOTE: The following methods may need to be revised.

        /// <summary>
        /// Calls all interfaces of the specified type that are on this GameObject,
        /// specifically by passing each as a parameter into the provided Action.
        /// </summary>
        public static void CallInterfaces<T>(this GameObject gameObject, System.Action<T> action)
        {
            T[] interfaces = GetInterfaces<T>(gameObject);
            for (int i = 0; i < interfaces.Length; i++)
            {
                action(interfaces[i]);
            }
        }

        /// <summary>
        /// Gets all interfaces of the specified type that are on this GameObject.
        /// </summary>
        public static T[] GetInterfaces<T>(this GameObject gameObject)
        {
            if (typeof(T).IsInterface)
            {
                return gameObject.GetComponents<Component>().OfType<T>().ToArray<T>();
            }
            else
            {
                G.U.Error("Error while getting interfaces for {0}: {1} is not an interface.",
                    gameObject.name, typeof(T));
                return new T[0];
            }
        }
    }
}
