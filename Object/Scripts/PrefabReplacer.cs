using UnityEngine;

namespace KRG
{
    // to be used with KRGMenuReplacePrefabs
    public class PrefabReplacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_NewPrefab;

        public GameObject NewPrefab => m_NewPrefab;
    }
}
