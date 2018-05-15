using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class Item : MonoBehaviour {

#region protected fields

        protected ItemData _itemData;

#endregion

#region MonoBehaviour methods

        protected virtual void OnTriggerEnter(Collider other) {
            if (OnCollect(other)) G.End(gameObject);
        }

#endregion

#region public methods

        public virtual void Init(ItemData itemData) {
            _itemData = itemData;
        }

        public virtual bool OnCollect(Collider other) {
            //TODO: differentiate between inventory items and instant-use items
            return OnUse(other);
        }

        public virtual bool OnUse(Collider other) {
            //TODO: differentiate between the character, etc. using the item
            if (other.gameObject.tag != "Player") return false;
            var dt = other.GetComponent<DamageTaker>();
            if (dt != null && _itemData.hp > 0) dt.AddHP(_itemData.hp);
            return true;
        }

#endregion

    }
}
