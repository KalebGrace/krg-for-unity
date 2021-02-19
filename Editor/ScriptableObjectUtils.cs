using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    public static class ScriptableObjectUtils
    {
        /// <summary>
        /// Gets all ScriptableObjects of type T.
        /// </summary>
        /// <returns>The ScriptableObjects.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] get_all<T>() where T : ScriptableObject
        {
            var _ = new string[0];
            return get_all<T>(ref _);
        }

        /// <summary>
        /// Gets all ScriptableObjects of type T.
        /// </summary>
        /// <returns>The ScriptableObjects.</returns>
        /// <param name="paths">Their paths.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] get_all<T>(ref string[] paths) where T : ScriptableObject
        {
            //NOTE: FindAssets uses tags
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            int n = guids.Length;
            paths = new string[n];
            T[] sos = new T[n];
            for (int i = 0; i < n; ++i)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                sos[i] = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            }
            return sos;
        }
    }
}