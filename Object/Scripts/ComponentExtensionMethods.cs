using UnityEngine;

namespace KRG
{
    public static class ComponentExtensionMethods
    {
        public static void PersistNewScene(this Component obj, PersistNewSceneType persistNewSceneType)
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
    }
}
