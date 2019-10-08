using UnityEditor;
using UnityEngine;

namespace KRG
{
    [InitializeOnLoad]
    public static class EditorUpdate
    {
        private static bool IsPressingPlay =>
            !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode;

        static EditorUpdate()
        {
            EditorApplication.update += PrefabUpdate;
        }

        /// <summary>
        /// Destroy all prefab component previews before running the game.
        /// </summary>
        private static void PrefabUpdate()
        {
            if (IsPressingPlay)
            {
                foreach (Prefab p in Object.FindObjectsOfType<Prefab>())
                {
                    if (p.instance != null)
                    {
                        Object.DestroyImmediate(p.instance.gameObject);
                    }
                }
            }
        }
    }
}
