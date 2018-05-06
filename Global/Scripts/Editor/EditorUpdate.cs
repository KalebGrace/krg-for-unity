using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRG {

    [InitializeOnLoad]
    public static class EditorUpdate {

        private static bool isPressingPlay {
            get { return !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode; }
        }

        static EditorUpdate() {
            //EditorLoad code
#if NS_UGIF
            GifPlayerUpdate();
#endif

            //EditorUpdate code
#if NS_UGIF
            EditorApplication.update += GifPlayerUpdate;
#endif
            EditorApplication.update += PrefabUpdate;
        }

#if NS_UGIF
        private static void GifPlayerUpdate() {
            if (!EditorApplication.isPlaying) {
                foreach (GifPlayer p in Object.FindObjectsOfType<GifPlayer>()) {
                    p.LoadPreview();
                }
            }
        }
#endif

        /// <summary>
        /// Destroy all prefab component previews before running the game.
        /// </summary>
        private static void PrefabUpdate() {
            if (isPressingPlay) {
                foreach (Prefab p in Object.FindObjectsOfType<Prefab>()) {
                    if (p.instance != null) {
                        Object.DestroyImmediate(p.instance.gameObject);
                    }
                }
            }
        }
    }
}
