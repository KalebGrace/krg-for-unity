using UnityEngine;
using System.Collections.Generic;

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
        [Header("Item Data")]

        [SerializeField, Tooltip("The name of the item, as displayed to the player.")]
        protected string displayName = default;

        [SerializeField, Tooltip("If this represents a key item, set it here.")]
        [Enum(typeof(KeyItem))]
        protected int m_KeyItem = default;

        [SerializeField, Tooltip("A prefab that has an Item component on its root GameObject. " +
            "This is used to instantiate a physical GameObject for this item in the world.")]
        public Item itemPrefab = default;

        //will the item be picked up automatically? (makes this an "auto-collected" item)
        //and if so, how long after the item is instantiated will the item be picked up? (delay in seconds)
        [SerializeField, Tooltip("Will this be picked up automatically? " +
            "And if so, with how much delay (in seconds)?")]
        [LabelText("Auto-Collect (+Delay)")]
        [BoolObjectDisable(false)]
        protected BoolFloat autoCollect = default;


        [Header("Effectors")]

        [SerializeField]
        protected List<Effector> effectors = new List<Effector>();


        // properties

        public bool HasEffectors => effectors != null && effectors.Count > 0;

        public bool IsKeyItem => m_KeyItem != 0;

        public int KeyItemIndex => m_KeyItem;


        // MonoBehaviour methods

        protected virtual void OnValidate()
        {
            if (displayName == null || displayName.Trim() == "")
            {
                displayName = name;
            }

            if (itemPrefab != null)
            {
                itemPrefab.itemData = this;
                //TODO: set as dirty?
            }

            autoCollect.floatValue = Mathf.Max(0, autoCollect.floatValue);
        }


        // custom methods

        public Item SpawnFrom(ISpawn spawner)
        {
            if (autoCollect.boolValue && autoCollect.floatValue <= 0)
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

            Item item = Instantiate(itemPrefab, parent);
            item.transform.position = item.transform.localPosition + position;
            item.Init(this, spawner);

            return item;
        }
    }
}
