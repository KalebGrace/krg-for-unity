using UnityEngine;

namespace _0G.Legacy
{
    // to be used with MenuReplacePrefabs
    public class PrefabReplacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_NewPrefab = default;

        public GameObject NewPrefab => m_NewPrefab;
    }
}