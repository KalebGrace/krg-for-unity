using System.IO;
using UnityEditor;

namespace _0G.Legacy
{
    public static class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        public static void BuildActiveTargetAssetBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";

            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildTarget bt = EditorUserBuildSettings.activeBuildTarget;

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, bt);

            G.U.Log("Asset bundles built for active target. See {0}", assetBundleDirectory);

            // If you don't see your bundle, make sure the assets
            // are properly assigned to it in the bottom of the inspector.
        }
    }
}