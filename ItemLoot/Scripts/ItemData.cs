using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Item data. Represents the item data, including pickup, effects, and physical GameObject.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewKRGItemData.asset",
        menuName = "KRG Scriptable Object/Item Data",
        order = 123
    )]
    public class ItemData : ScriptableObject {

#region serialized fields

        [SerializeField, Tooltip("The name of the item.")]
        string _name;

        [SerializeField, Tooltip("A prefab that has an Item component on its root GameObject. " +
        "This is used to instantiate a physical GameObject for this item in the world.")]
        Item _prefab;

        //will the item be picked up automatically? (makes this an "auto-collected" item)
        //and if so, how long after the item is instantiated will the item be picked up? (delay in seconds)
        [SerializeField]
        [LabelText("Auto-Collect (+Delay)")]
        [Tooltip("Will this be picked up automatically? And if so, with how much delay (in seconds)?")]
        [BoolObjectDisable(false)]
        BoolFloat _autoCollect;

        [Header("HP (Hit Points)")]

        [SerializeField]
        float _hp;

        [SerializeField]
        float _hpMax;

#endregion

#region properties

        public float hp { get { return _hp; } }

        public float hpMax { get { return _hpMax; } }

        public string itemName { get { return _name; } }

#endregion

#region MonoBehaviour methods

        protected virtual void OnValidate() {
            if (string.IsNullOrEmpty(_name.Trim())) {
                _name = name;
            }
            _autoCollect.floatValue = Mathf.Max(0, _autoCollect.floatValue);
        }

#endregion

#region public methods

        public Item Spawn(Vector3 position, Transform parent) {
            var item = Instantiate(_prefab, position, Quaternion.identity, parent);
            item.Init(this);
            return item;
        }

#endregion

    }
}
