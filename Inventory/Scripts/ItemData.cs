using UnityEngine;
using UnityEngine.Serialization;
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
        // STATIC EVENTS

        public static event System.Action<ItemData, GameObjectBody> ItemCollected;

        // SERIALIZED FIELDS

        [Header("Item Data")]

        [SerializeField, Tooltip("The name of the item, as displayed to the player.")]
        protected string displayName = default;

        [SerializeField, Tooltip("Instruction for item use, as displayed to the player.")]
        protected string instruction = default;

        [SerializeField, Enum(typeof(ItemID)), FormerlySerializedAs("m_KeyItem")]
        protected int m_ItemID = default;

        [SerializeField, Enum(typeof(ItemType))]
        protected int m_ItemType = default;

        [Header("On Spawn")]

        [SerializeField, Tooltip("The prefab of the item to spawn.")]
        protected GameObject m_ItemPrefab = default;

        [SerializeField, Tooltip(
            "Instead of spawning, the player that caused this item to spawn will collect it automatically.")]
        protected bool m_AutoPlayerCollect = default;

        [Header("On Collect")]

        [SerializeField, Tooltip(
            "Consumables: Instead of this item going to the inventory, the owner will consume it automatically. -- " +
            "Equippables: The item will go to the inventory and the owner will equip it automatically.")]
        protected bool m_AutoOwnerUse = default;

        [SerializeField, Tooltip(
            "Show the small item info panel upon collecting this item.")]
        protected bool m_ShowInfoPanelOnCollect = true;

        [SerializeField, FormerlySerializedAs("showCardOnAcquire"), Tooltip(
            "Show the large item title card upon collecting this item.")]
        protected bool m_ShowTitleCardOnCollect = default;

        [SerializeField, Tooltip("Play this sound effect upon collecting this item.")]
        [AudioEvent]
        protected string sfxFmodEventOnCollect = default;

        [Header("Effectors")]

        [SerializeField]
        protected List<Effector> effectors = new List<Effector>();

        // PROPERTIES

        public string DisplayName => displayName;

        public string Instruction => instruction;

        public bool IsKeyItem => ItemType == (int)KRG.ItemType.KeyItem;

        public int ItemID => m_ItemID;

        public int ItemType => m_ItemType;

        public bool ShowInfoPanelOnCollect => m_ShowInfoPanelOnCollect;

        public bool ShowTitleCardOnCollect => m_ShowTitleCardOnCollect;

        // MONOBEHAVIOUR METHODS

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = name;
            }
        }

        // CUSTOM METHODS

        public Item SpawnFrom(ISpawn spawner)
        {
            GameObjectBody invoker = spawner.Invoker;

            if (m_AutoPlayerCollect && invoker != null && invoker.IsPlayerCharacter)
            {
                Collect(invoker, 0);
                return null;
            }
            else if (m_ItemPrefab == null)
            {
                G.U.Err("Attempted to spawn item with no prefab.", this, spawner);
                return null;
            }
            else
            {
                Transform parent = spawner.transform.parent;
                Vector3 position = spawner.CenterTransform.position;

                GameObject itemInstance = Instantiate(m_ItemPrefab, parent);
                itemInstance.transform.position = itemInstance.transform.localPosition + position;

                Item item = itemInstance.GetComponent<Item>();
                if (item != null) item.SpawnInit();
                return item;
            }
        }

        public virtual bool CanCollect(Collider other, int instanceID)
        {
            return other.gameObject.tag == "Player";
        }

        public virtual void Collect(Collider other, int instanceID)
        {
            Collect(other.GetComponent<GameObjectBody>(), instanceID);
        }
        public virtual void Collect(GameObjectBody owner, int instanceID)
        {
            AddCollectionRecord(owner, instanceID);

            switch (m_ItemType)
            {
                case (int)OSH.ItemType.Consumable:
                    if (m_AutoOwnerUse)
                    {
                        DoEffects((int)EffectorCondition.Use, owner);
                    }
                    else
                    {
                        AddToInventory(owner, instanceID);
                    }
                    break;
                case (int)OSH.ItemType.Equipment:

                    AddToInventory(owner, instanceID);

                    if (m_AutoOwnerUse)
                    {
                        DoEffects((int)EffectorCondition.Equip, owner);
                    }
                    break;
                default:

                    AddToInventory(owner, instanceID);

                    break;
            }

            if (!string.IsNullOrWhiteSpace(sfxFmodEventOnCollect))
            {
                G.audio.PlaySFX(sfxFmodEventOnCollect, owner.transform.position);
            }

            ItemCollected?.Invoke(this, owner);
        }

        private void AddCollectionRecord(GameObjectBody owner, int instanceID)
        {
            if (owner == G.obj.FirstPlayerCharacter)
            {
                G.inv.AddItemInstanceCollected(instanceID);
            }
        }

        private void AddToInventory(GameObjectBody owner, int instanceID)
        {
            if (owner == G.obj.FirstPlayerCharacter)
            {
                G.inv.AddItemQty(m_ItemID, 1);
            }
        }

        protected virtual void DoEffects(int condition, GameObjectBody self) { }
    }
}
