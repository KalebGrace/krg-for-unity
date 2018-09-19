using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class ItemSpawner : MonoBehaviour {

        [SerializeField]
        ItemData _item;

        void Start() {
            if (_item != null) {
                _item.Spawn(transform.position, transform.parent);
            }
            G.End(gameObject);
        }
    }
}
