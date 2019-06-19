using UnityEngine;

namespace KRG
{
    public static class ComponentEM
    {
        public static void Dispose(this Component me)
        {
            if (me == null)
            {
                G.U.Warning("The Component you wish to dispose of is null.");
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
    }
}
