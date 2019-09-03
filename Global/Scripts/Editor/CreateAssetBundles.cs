using UnityEditor;
using System.IO;

namespace KRG
{
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            var bt = BuildTarget.StandaloneOSX;
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, bt);

            G.U.Log("Asset bundles built. If you don't see your bundle, make sure the assets are properly assigned to it in the bottom of the inspector.");
        }
    }
}
