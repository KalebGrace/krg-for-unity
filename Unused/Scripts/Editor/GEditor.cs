using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KRG.Unused {

    //[CustomEditor(typeof(G))]
	[System.Obsolete]
    public class GEditor : Editor {

        public override void OnInspectorGUI() {

            //no longer used; see KRGMenuCreateConfig instead
            /*
            G g = (G)target;
            */

            DrawDefaultInspector();

            //no longer used; see KRGMenuCreateConfig instead
            /*
            if (GUILayout.Button("Create New Config (& Apply)")) {
                CreateConfigAsset(g);
            }
            */
        }

        static void CreateConfigAsset(G g) {
            
            const string path = "Assets/Resources/KRGConfig.asset";

            if (File.Exists(path)) {
                G.U.Err("{0} already exists.", path);
                return;
            }

            KRGConfig config = ScriptableObject.CreateInstance<KRGConfig>();
            AssetDatabase.CreateAsset(config, path);

            //AddChildConfig<TimeManagerConfig>(config);
            //...

            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(config);

            //previously, we were setting a reference directly on the G component,
            //but we're using the Resources folder now instead
            /*
            g.SetConfig(config);
            Object parent = PrefabUtility.GetPrefabParent(g);
            if (parent == null) {
                //we are inspecting the KRGLoader prefab asset, so now update the instance in the scene
                EditorUtility.SetDirty(g);
            } else {
                //we are inspecting an instance in the scene, so now update the KRGLoader prefab asset (the "parent")
                PrefabUtility.ReplacePrefab(g.gameObject, parent, ReplacePrefabOptions.ConnectToPrefab);
            }
            */
        }

        /*
        private static void AddChildConfig<T>(KRGConfig parent) where T : ManagerConfig {
            T child = ScriptableObject.CreateInstance<T>();
            AssetDatabase.AddObjectToAsset(child, parent);
            parent.managerConfigs.Add(child);
            child.name = typeof(T).Name;
        }
        */

#region example sub-asset management code

        //keep in mind that while extracting/readding sub-asset will maintain the parent's connection to sub-asset,
        //any connections to sub-asset from other objects will be lost
        //NOTE: "SO" = SerializedObject

        /*
        [MenuItem("Test/Extract Sub-Asset")]
        public static void ExtractSubAsset() {
            string name = "MyTestSOChild2";
            TestSO so = AssetDatabase.LoadAssetAtPath<TestSO>("Assets/Test/MyTestSO.asset");
            int childIndex = so.children.FindIndex(x => x.name == name);
            TestSOChild soChildOrig = so.children[childIndex];
            Object childCopy = Object.Instantiate(soChildOrig);
            AssetDatabase.CreateAsset(childCopy, "Assets/Test/" + name + ".asset");
            so.children.RemoveAt(childIndex);
            so.children.Insert(childIndex, (TestSOChild)childCopy);
            //
            Object.DestroyImmediate(soChildOrig, true);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Test/Readd Sub-Asset")]
        public static void ReaddSubAsset() {
            string name = "MyTestSOChild2";
            TestSO so = AssetDatabase.LoadAssetAtPath<TestSO>("Assets/Test/MyTestSO.asset");
            int childIndex = so.children.FindIndex(x => x.name == name);
            TestSOChild soChildOrig = so.children[childIndex];
            Object childCopy = Object.Instantiate(soChildOrig);
            AssetDatabase.AddObjectToAsset(childCopy, so);
            so.children.RemoveAt(childIndex);
            so.children.Insert(childIndex, (TestSOChild)childCopy);
            childCopy.name = name;
            AssetDatabase.DeleteAsset("Assets/Test/" + name + ".asset");
            AssetDatabase.SaveAssets();
        }
        */

#endregion

    }
}
