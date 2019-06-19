using UnityEngine;
using UnityEditor;
using System.Collections;

namespace KRG {

    [CustomEditor(typeof(Prefab))]
    public class PrefabInspector : Editor {

        public override void OnInspectorGUI() {

            Prefab prefab = (Prefab)target;

            GUI.changed = false;

            prefab.prefab = (Transform)EditorGUILayout.ObjectField(prefab.prefab, typeof(Transform), false);

            if (!prefab.instance) {
                if (GUILayout.Button("Preview")) {
                    Preview(prefab);
                }
            } else {
                if (GUILayout.Button("Destroy Preview")) {
                    DestroyPreview(prefab);
                }
            }

            string[] displayedOptions = { PositionType.Absolute.ToString(), PositionType.Relative.ToString() };
            prefab.positionType = (PositionType)EditorGUILayout.Popup(
                "Position Type",
                (int)prefab.positionType,
                displayedOptions
            );

            prefab.autoInstantiate = EditorGUILayout.Toggle("Auto-Instantiate", prefab.autoInstantiate);

            prefab.isExposed = EditorGUILayout.Toggle("Is Exposed", prefab.isExposed);

            if (prefab.isExposed) {
                prefab.key = EditorGUILayout.TextField("Key (Optional)", prefab.key);
                string s = "Script execution order ensures the instance is added to " +
                           "all components implementing IPrefab before they Awake.";
                EditorGUILayout.HelpBox(s, MessageType.Info);
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(prefab); //TODO: SetDirty might not be appropriate for Unity 5.3+. Find out.
            }
        }

        void Preview(Prefab prefab) {
            if (prefab.instance) {
                G.U.Err("{0}'s prefab component already has a preview.", prefab.gameObject.name);
            } else if (!prefab.prefab) {
                G.U.Err("{0}'s prefab component has no prefab reference.", prefab.gameObject.name);
            } else {
                prefab.instance = (Transform)PrefabUtility.InstantiatePrefab(prefab.prefab);
                Vector3 pos = prefab.instance.localPosition;

                prefab.instance.parent = prefab.transform;
				
                if (prefab.positionType == PositionType.Relative) {
                    prefab.instance.localPosition = pos;
                }
                
                if (prefab.isExposed) {
                    string key = prefab.key.Trim();
                    key = string.IsNullOrEmpty(key) ? prefab.instance.name : key;
                    prefab.gameObject.CallInterfaces<IPrefab>(x => {
                        x.prefabs.Add(key, prefab.instance);
                        x.OnPrefabAdd(key, prefab.instance);
                    });
                }

                Prefab[] childPrefabs = prefab.instance.GetComponentsInChildren<Prefab>();
                foreach (Prefab childPrefab in childPrefabs) {
                    Preview(childPrefab);
                }
            }
        }

        void DestroyPreview(Prefab prefab) {
            if (prefab.instance) {
                if (prefab.isExposed) {
                    string key = prefab.key.Trim();
                    key = string.IsNullOrEmpty(key) ? prefab.instance.name : key;
                    prefab.gameObject.CallInterfaces<IPrefab>(x => {
                        if (x.prefabs.ContainsKey(key)) {
                            x.prefabs.Remove(key);
                        }
                    });
                }
                DestroyImmediate(prefab.instance.gameObject);
            } else {
                G.U.Err(
                    "{0}'s prefab component has no preview to destroy.",
                    prefab.gameObject.name
                );
            }
        }
    }
}
