using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    /// <summary>
    /// Item data. Represents the item data, including pickup, effects, and physical GameObject.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewKRGItemData.asset",
        menuName = "KRG Scriptable Object/Item Data",
        order = 123
    )]
    public class ItemData : ScriptableObject
    {
        [SerializeField, Tooltip("The name of the item, as displayed to the player.")]
        string displayName = default;

        [SerializeField, Tooltip("A prefab that has an Item component on its root GameObject. " +
        "This is used to instantiate a physical GameObject for this item in the world.")]
        Item _prefab = default;

        //will the item be picked up automatically? (makes this an "auto-collected" item)
        //and if so, how long after the item is instantiated will the item be picked up? (delay in seconds)
        [SerializeField]
        [LabelText("Auto-Collect (+Delay)")]
        [Tooltip("Will this be picked up automatically? And if so, with how much delay (in seconds)?")]
        [BoolObjectDisable(false)]
        BoolFloat _autoCollect = default;

        //
        //
        [Header("Effectors")]

        [SerializeField]
        protected List<Effector> effectors = new List<Effector>();

        //
        //
        //

        protected virtual void OnValidate()
        {
            if (displayName == null || string.IsNullOrEmpty(displayName.Trim()))
            {
                displayName = name;
            }
            _autoCollect.floatValue = Mathf.Max(0, _autoCollect.floatValue);
        }

        public Item SpawnFrom(ISpawn spawner)
        {
            if (_autoCollect.boolValue && _autoCollect.floatValue <= 0)
            {
                return InstaCollect(spawner);
            }
            else
            {
                return SpawnFoSho(spawner);
            }
        }

        protected virtual Item InstaCollect(ISpawn spawner)
        {
            return SpawnFoSho(spawner);
        }

        protected virtual Item SpawnFoSho(ISpawn spawner)
        {
            Transform parent = spawner.transform.parent;
            Vector3 position = spawner.centerTransform.position;

            Item item = Instantiate(_prefab, parent);
            item.transform.position = item.transform.localPosition + position;
            item.Init(this, spawner);

            return item;
        }
    }
}
