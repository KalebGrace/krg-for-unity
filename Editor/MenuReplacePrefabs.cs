using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _0G.Legacy
{
    public static class MenuReplacePrefabs
    {
        [MenuItem("0G/Legacy/Replace Prefabs", false, 1800)]
        public static void ReplacePrefabs()
        {
            PrefabReplacer[] prefabReplacers = Object.FindObjectsOfType<PrefabReplacer>();

            if (prefabReplacers == null || prefabReplacers.Length == 0)
            {
                G.U.Warn("Couldn't find any PrefabReplacer components in this scene.");
                return;
            }

            foreach (PrefabReplacer pr in prefabReplacers)
            {
                Object instance = PrefabUtility.InstantiatePrefab(pr.NewPrefab, pr.transform);
                GameObject go = (GameObject) instance;
                go.transform.parent = pr.transform.parent;
                go.name = pr.name;
                Object.DestroyImmediate(pr.gameObject);
            }

            Scene activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(activeScene);

            G.U.Log("Replace Prefabs complete: {0} prefab(s) replaced.", prefabReplacers.Length);
        }
    }
}