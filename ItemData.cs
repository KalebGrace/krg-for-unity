using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    /// <summary>
    /// Item data. Represents the item data, including pickup, effects, and physical GameObject.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SomeItem_ItemData.asset",
        menuName = "0G Legacy Scriptable Object/Item Data",
        order = 920
    )]
    public class ItemData : ScriptableObject
    {
        // CONSTANTS

        public const int VERSION = 2;

        // SERIALIZED FIELDS

        [SerializeField, HideInInspector]
        private int _serializedVersion;

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

        [SerializeField, Tooltip(
            "Show the soft currency panel upon collecting this item.")]
        protected bool m_ShowSoftCurrencyPanelOnCollect = default;

        [SerializeField, Tooltip("Play this sound effect upon collecting this item.")]
        [AudioEvent]
        protected string sfxFmodEventOnCollect = default;

        [Header("On Use/Equip")]

        public List<BuffObject> Buffs;

        [Header("Base Stat Effectors")]

        [SerializeField]
        protected List<Effector> effectors = new List<Effector>();

        // PROPERTIES

        public bool AutoOwnerUse => m_AutoOwnerUse;

        public string DisplayName => displayName;

        public string Instruction => instruction;

        public bool IsKeyItem => ItemType == (int) _0G.Legacy.ItemType.KeyItem;

        public int ItemID => m_ItemID;

        public int ItemType => m_ItemType;

        public bool ShowInfoPanelOnCollect => m_ShowInfoPanelOnCollect;

        public bool ShowTitleCardOnCollect => m_ShowTitleCardOnCollect;

        public bool ShowSoftCurrencyPanelOnCollect => m_ShowSoftCurrencyPanelOnCollect;

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake() // GAME BUILD only
        {
            UpdateSerializedVersion();
        }

        protected virtual void OnValidate() // UNITY EDITOR only
        {
            UpdateSerializedVersion();

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
            return G.obj.IsPlayerCharacter(other);
        }

        public virtual void Collect(Collider other, int instanceID)
        {
            Collect(other.GetComponent<GameObjectBody>(), instanceID);
        }
        public virtual void Collect(GameObjectBody owner, int instanceID)
        {
            if (owner == G.obj.FirstPlayerCharacter)
            {
                G.inv.AddItemInstanceCollected(instanceID);
                G.inv.AddItemQty(m_ItemID, 1);
            }

            if (!string.IsNullOrWhiteSpace(sfxFmodEventOnCollect))
            {
                G.audio.PlaySFX(sfxFmodEventOnCollect, owner.transform.position);
            }
        }

        public virtual void DoEffects(int condition, GameObjectBody self) { }

        #region VERSIONING
        private void UpdateSerializedVersion()
        {
            while (_serializedVersion < VERSION)
            {
                switch (_serializedVersion)
                {
                    case 0:
                        for (int i = 0; i < effectors.Count; ++i)
                        {
                            Effector e = effectors[i];
                            // update EffectorProperty value
                            switch (e.property)
                            {
                                case 1100: // HP
                                    e.property = (int) StatID.HP;
                                    break;
                                case 1200: // SP
                                    e.property = 104;
                                    break;
                                case 2100: // HPMax
                                    e.property = (int) StatID.HPMax;
                                    break;
                                case 2200: // SPMax
                                    e.property = 114;
                                    break;
                            }
                            effectors[i] = e;
                        }
                        break;
                    case 1:
                        for (int i = 0; i < effectors.Count; ++i)
                        {
                            Effector e = effectors[i];
                            // update EffectorProperty value
                            switch (e.property)
                            {
                                case 1401: // LP
                                    e.property = 105;
                                    break;
                                case 2401: // LPMax
                                    e.property = 115;
                                    break;
                            }
                            effectors[i] = e;
                        }
                        break;
                }
                ++_serializedVersion;
            }
        }
        #endregion
    }
}