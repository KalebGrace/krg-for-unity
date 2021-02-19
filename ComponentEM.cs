using UnityEngine;

namespace _0G.Legacy
{
    public static class ComponentEM // Component extension methods
    {
        public static void Dispose(this Component me)
        {
            if (me == null)
            {
                G.U.Warn("The Component you wish to dispose of is null.");
                return;
            }
            Object.Destroy(me);
        }

        public static void PersistNewScene(this Component me, PersistNewSceneType persistNewSceneType)
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
        /// REQUIRE COMPONENT to be on SPECIFIED COMPONENT'S GAME OBJECT:
        /// Require the specified Component type to exist on the specified source Component's GameObject.
        /// </summary>
        /// <param name="me">Source Component.</param>
        /// <param name="throwException">If set to <c>true</c> throw exception.</param>
        /// <typeparam name="T">The required Component type.</typeparam>
        public static T Require<T>(this Component me, bool throwException = true) where T : Component
        {
            if (!G.U.SourceExists(me, typeof(T), throwException)) return null;
            T comp = me.GetComponent<T>();
            if (G.U.IsNull(comp))
            {
                string s = string.Format("A {0} Component must exist on the {1}'s {2} GameObject.",
                    typeof(T), me.GetType(), me.name);
                G.U.ErrorOrException(s, throwException);
                return null;
            }
            return comp;
        }
    }
}