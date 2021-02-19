using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    public class EditPrefabAssetScope : System.IDisposable
    {
        // Written by Baste, Nov 5, 2019 (with additions)
        // https://forum.unity.com/threads/how-do-i-edit-prefabs-from-scripts.685711/#post-5140328

        // Usage:
        /*
        using (var editScope = new EditPrefabAssetScope(assetPath))
        {
            editScope.prefabRoot.AddComponent<BoxCollider>();
        }
        */

        public readonly string assetPath;
        public readonly GameObject prefabRoot;
        public readonly GameObjectBody prefabBody;

        public EditPrefabAssetScope(string assetPath)
        {
            this.assetPath = assetPath;
            prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            prefabBody = prefabRoot.GetComponent<GameObjectBody>();
        }

        public void Dispose()
        {
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }
}