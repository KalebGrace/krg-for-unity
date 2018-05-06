using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class EditorSave : UnityEditor.AssetModificationProcessor {
    
        private static string[] OnWillSaveAssets(string[] paths) {
#if NS_UGIF
            GifPlayerSave();
#endif
            
            return paths;
        }

#if NS_UGIF
        private static void GifPlayerSave() {
            //upon saving, the resource that the m_previewGif variable is pointing to will be unloaded
            //however, the variable will not be set to null, so it will never be reloaded,
            //and all that will be seen of the preview is a blank gray square (until the GameObject is re-selected)
            //so, let's explicitly set m_previewGif to null so it will be reloaded after the save is completed
            foreach (GifPlayer p in Object.FindObjectsOfType<GifPlayer>()) {
                p.UnloadPreview();
            }
        }
#endif
    }
}
