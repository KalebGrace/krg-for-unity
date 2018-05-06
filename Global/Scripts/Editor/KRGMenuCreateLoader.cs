using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KRG {

    public static class KRGMenuCreateLoader {

        [MenuItem("KRG/Create KRGLoader", false, 0)]
        public static void CreateLoader() {
            const string s = "Assets/KRG/Global/Prefabs/KRGLoaderOriginal.prefab";
            const string t = "Assets/KRGLoader.prefab";
            //see if the prefab source and target exist
            if (!File.Exists(s)) {
                G.Err(s + " is missing!", null);
                return;
            }
            if (File.Exists(t)) {
                G.Err(t + " already exists.", null);
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
			GameObject i = (GameObject)PrefabUtility.InstantiatePrefab(p, scene);
			Selection.activeGameObject = i;
			EditorSceneManager.MarkSceneDirty(scene);
			//finish
            G.U.Log("A KRGLoader prefab was created at {0} and instantiated in the scene.", t);
        }
    }
}
