using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    public static class MenuCreateConfig
    {
        [MenuItem("0G/Legacy/Create KRGConfig", false, 1)]
        public static void CreateConfig()
        {
            KRGConfig config;
            //see if the config already exists
            config = Resources.Load<KRGConfig>(KRGConfig.RESOURCE_PATH);
            if (config != null)
            {
                G.U.Err("A KRGConfig asset already exists in a Resources folder.", config);
                Resources.UnloadAsset(config);
                return;
            }
            //create a new config asset (in the resources folder)
            config = ScriptableObject.CreateInstance<KRGConfig>();
            AssetDatabase.CreateAsset(config, KRGConfig.ASSET_PATH);
            AssetDatabase.SaveAssets();
            //ping the asset in the Project window
            EditorGUIUtility.PingObject(config);
            //finish
            G.U.Log("A KRGConfig asset was created at {0}.", KRGConfig.ASSET_PATH);
        }
    }
}