using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class ItemSpawner : MonoBehaviour, ISpawn
    {
        [SerializeField]
        ItemData _item = default;

        public Transform centerTransform => transform;

        void Start()
        {
            if (_item != null)
            {
                _item.SpawnFrom(this);
            }

            gameObject.Dispose();
        }
    }
}
