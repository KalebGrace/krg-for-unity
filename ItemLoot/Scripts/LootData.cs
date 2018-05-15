using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Loot data. Represents the conditions and probability of generating an item.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewKRGLootData.asset",
        menuName = "KRG Scriptable Object/Loot Data",
        order = 123
    )]
    public class LootData : ScriptableObject {

#region serialized fields

        //[SerializeField]
        //float _probabilityScale = 100;

        //list of LootItems that can be generated:
        //▲ top item is most commonly generated
        //▼ bottom item is most rarely generated
        //NOTE: the probability of generating items can also be equal,
        //in which case the order is largely irrelevant
        [SerializeField, Tooltip("These are all the possible items that can be generated.")]
        List<LootItem> _items = new List<LootItem>();

#endregion

#region public methods

        public LootItem[] GetItemArray() {
            return _items.ToArray();
        }

        public virtual ItemData RollItem() {
            if (_items == null) {
                G.Err(this, "No items are available for the {0} loot data.", name);
                return null;
            }
            var idx = Random.Range(0, _items.Count);
            var li = _items[idx];
            return li.item;
        }

#endregion

    }
}
