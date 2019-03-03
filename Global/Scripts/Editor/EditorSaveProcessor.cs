using UnityEngine;

namespace KRG
{
    public class EditorSaveProcessor : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
#if NS_UGIF
            unload_gif_player_previews();
#endif
            return paths;
        }

#if NS_UGIF
        static void unload_gif_player_previews()
        {
            //upon saving, the resource that the _previewGif variable is pointing to will be unloaded
            //however, the variable will not be set to null, so it will never be reloaded,
            //and all that will be seen of the preview is a blank gray square (until the GameObject is re-selected)
            //so, let's explicitly set _previewGif to null so it will be reloaded after the save is completed
            foreach (GifPlayer p in Object.FindObjectsOfType<GifPlayer>())
            {
                p.UnloadPreview();
            }
        }
#endif
    }
}
