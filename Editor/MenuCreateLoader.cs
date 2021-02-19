using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _0G.Legacy
{
    public static class MenuCreateLoader
    {
        [MenuItem("0G/Legacy/Create KRGLoader", false, 0)]
        public static void CreateLoader()
        {
            const string s = "Assets/_0G/Legacy/KRGLoaderOriginal.prefab";
            const string t = "Assets/KRGLoader.prefab";
            //see if the prefab source and target exist
            if (!File.Exists(s))
            {
                G.U.Err("Can't create KRGLoader; {0} is missing!", s);
                return;
            }
            if (File.Exists(t))
            {
                G.U.Err("Can't create KRGLoader; {0} already exists.", t);
                return;
            }
            //create (copy) the prefab asset
            AssetDatabase.CopyAsset(s, t);
            AssetDatabase.Refresh();
            //ping the asset in the Project window
            Object p = AssetDatabase.LoadMainAssetAtPath(t);
            EditorGUIUtility.PingObject(p);
            //instantiate the prefab in the active scene
            var scene = SceneManager.GetActiveScene();
            GameObject i = (GameObject) PrefabUtility.InstantiatePrefab(p, scene);
            Selection.activeGameObject = i;
            EditorSceneManager.MarkSceneDirty(scene);
            //finish
            G.U.Log("A KRGLoader prefab was created at {0} and instantiated in the scene.", t);
        }
    }
}