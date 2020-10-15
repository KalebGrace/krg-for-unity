using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRG
{
    public class InventoryManager : Manager, ISave, IOnDestroy
    {
        public override float priority => 130;

        // DELEGATES & EVENTS

        public delegate void ItemAcquiredHandler(ItemData itemData, GameObjectBody owner, bool isNewlyAcquired);

        public event System.Action AutoMapSaveDataRequested;
        public event System.Action AutoMapSaveDataProvided;
        public event ItemAcquiredHandler ItemAcquired;

        // FIELDS

        readonly Dictionary<int, ItemAcquiredHandler> m_KeyItemAcquiredHandlers = new Dictionary<int, ItemAcquiredHandler>();

        readonly Dictionary<int, ItemData> m_ItemDataDictionary = new Dictionary<int, ItemData>();

        private List<int> m_ItemInstancesCollected = new List<int>();

        private Dictionary<int, float> m_Items = new Dictionary<int, float>();

        private Dictionary<int, float> m_Stats = new Dictionary<int, float>();

        readonly Dictionary<int, AutoMapSaveData> m_AutoMaps = new Dictionary<int, AutoMapSaveData>();

        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
            G.save.Subscribe(this);

            BuildItemDataDictionary();
        }

        public void OnDestroy()
        {
            G.save.Unsubscribe(this);
        }

        // ITEM MODIFYING METHODS

        public void AddItemInstanceCollected(int instanceID)
        {
            if (instanceID != 0)
            {
                m_ItemInstancesCollected.Add(instanceID);
            }
        }

        public void AddItemQty(int itemID, float quantity, float defaultQuantity = 0)
        {
            float oldQuantity = GetItemQty(itemID, defaultQuantity);
            float newQuantity = quantity + oldQuantity;
            ChangeItemQty(itemID, oldQuantity, newQuantity);
        }

        public void SetItemQty(int itemID, float quantity, float defaultQuantity = 0)
        {
            float oldQuantity = GetItemQty(itemID, defaultQuantity);
            float newQuantity = quantity;
            ChangeItemQty(itemID, oldQuantity, newQuantity);
        }

        private void ChangeItemQty(int itemID, float oldQuantity, float newQuantity)
        {
            ItemData itemData = GetItemData(itemID);

            if (itemData == null)
            {
                G.U.Warn("Missing item data.");
            }

            bool hasKeyItem = itemData != null && itemData.IsKeyItem && newQuantity >= 1;

            if (hasKeyItem)
            {
                newQuantity = 1;
            }

            GameObjectBody owner = G.obj.FirstPlayerCharacter;

            if (itemData.AutoOwnerUse)
            {
                int dq = Mathf.FloorToInt(newQuantity - oldQuantity);

                switch (itemData.ItemType)
                {
                    case (int)ItemType.Consumable:
                        for (int i = 0; i < dq; ++i)
                        {
                            itemData.DoEffects((int)EffectorCondition.Use, owner);
                        }
                        break;
                    case (int)ItemType.Equipment:
                        m_Items[itemID] = newQuantity;
                        for (int i = 0; i < dq; ++i)
                        {
                            itemData.DoEffects((int)EffectorCondition.Equip, owner);
                        }
                        break;
                    default:
                        m_Items[itemID] = newQuantity;
                        break;
                }
            }
            else
            {
                m_Items[itemID] = newQuantity;
            }

            if (newQuantity > oldQuantity)
            {
                ItemAcquired?.Invoke(itemData, owner, true);

                if (hasKeyItem)
                {
                    if (m_KeyItemAcquiredHandlers.ContainsKey(itemID))
                    {
                        m_KeyItemAcquiredHandlers[itemID]?.Invoke(itemData, owner, true);
                        m_KeyItemAcquiredHandlers.Remove(itemID);
                    }

                    G.save.SaveCheckpoint();
                }
            }
        }

        // PUBLIC ITEM QUANTITY METHODS

        public float GetItemQty(ItemID itemID, float defaultQuantity = 0)
        {
            return GetItemQty((int) itemID, defaultQuantity);
        }

        public bool HasItemQty(ItemID itemID)
        {
            return HasItemQty((int) itemID);
        }

        // PUBLIC STAT VALUE METHODS

        public void AddStatVal(StatID statID, float value, float defaultValue = 0)
        {
            AddStatVal((int) statID, value, defaultValue);
        }

        public float GetStatVal(StatID statID, float defaultValue = 0)
        {
            return GetStatVal((int) statID, defaultValue);
        }

        public bool HasStatVal(StatID statID)
        {
            return HasStatVal((int) statID);
        }

        public void SetStatVal(StatID statID, float value)
        {
            SetStatVal((int) statID, value);
        }

        // PUBLIC KEY ITEM METHODS

        public bool HasKeyItem(ItemID itemID)
        {
            return HasKeyItem(GetItemData((int) itemID));
        }
        public bool HasKeyItem(ItemData itemData)
        {
            return itemData != null &&
                itemData.IsKeyItem &&
                m_Items.ContainsKey(itemData.ItemID) &&
                m_Items[itemData.ItemID] >= 1;
        }

        // PUBLIC ITEM INSTANCE COLLECTED METHODS

        public bool HasItemInstanceCollected(int instanceID)
        {
            return instanceID != 0 && m_ItemInstancesCollected.Contains(instanceID);
        }

        // MAIN METHODS (PROTECTED & PRIVATE)

        protected float GetItemQty(int itemID, float defaultQuantity = 0)
        {
            return m_Items.ContainsKey(itemID) ? m_Items[itemID] : defaultQuantity;
        }

        protected bool HasItemQty(int itemID)
        {
            return m_Items.ContainsKey(itemID);
        }

        protected bool HasKeyItem(int itemID)
        {
            return HasKeyItem(GetItemData(itemID));
        }

        protected void AddStatVal(int statID, float value, float defaultValue = 0)
        {
            float oldValue = GetItemQty(statID, defaultValue);
            m_Stats[statID] = value + oldValue;
        }

        protected float GetStatVal(int statID, float defaultValue = 0)
        {
            return m_Stats.ContainsKey(statID) ? m_Stats[statID] : defaultValue;
        }

        protected bool HasStatVal(int statID)
        {
            return m_Stats.ContainsKey(statID);
        }

        protected void SetStatVal(int statID, float value)
        {
            m_Stats[statID] = value;
        }

        // MAIN METHODS 3

        protected ItemData GetItemData(int itemID)
        {
            return m_ItemDataDictionary.ContainsKey(itemID) ? m_ItemDataDictionary[itemID] : null;
        }

        /// <summary>
        /// Adds the handler to be invoked upon acquiring the key item with this index.
        /// If the key item is already acquired, it will invoke the handler immediately.
        /// </summary>
        /// <param name="itemID">Key item ID.</param>
        /// <param name="handler">Handler.</param>
        public void AddKeyItemAcquiredHandler(int itemID, ItemAcquiredHandler handler)
        {
            GameObjectBody owner = G.obj.FirstPlayerCharacter;

            if (HasKeyItem(itemID))
            {
                handler?.Invoke(GetItemData(itemID), owner, false);
            }
            else
            {
                if (m_KeyItemAcquiredHandlers.ContainsKey(itemID))
                {
                    m_KeyItemAcquiredHandlers[itemID] += handler;
                }
                else
                {
                    m_KeyItemAcquiredHandlers.Add(itemID, handler);
                }
            }
        }

        public void RemoveKeyItemAcquiredHandler(int itemID, ItemAcquiredHandler handler)
        {
            if (m_KeyItemAcquiredHandlers.ContainsKey(itemID))
            {
                m_KeyItemAcquiredHandlers[itemID] -= handler;
            }
        }

        public AutoMapSaveData GetAutoMapSaveData(AutoMap autoMap)
        {
            AutoMapSaveData saveData;

            int gameplaySceneId = G.app.GameplaySceneId;

            var cb = autoMap.tilemap.cellBounds;

            var width = cb.size.x;
            var height = cb.size.y;

            if (!m_AutoMaps.ContainsKey(gameplaySceneId))
            {
                saveData = new AutoMapSaveData(gameplaySceneId, width, height);

                m_AutoMaps.Add(gameplaySceneId, saveData);
            }
            else
            {
                saveData = m_AutoMaps[gameplaySceneId];

                saveData.UpdateDimensions(width, height);
            }

            return saveData;
        }

        public void SetAutoMapSaveData(AutoMapSaveData saveData)
        {
            m_AutoMaps[saveData.gameplaySceneId] = saveData;
        }

        public void RestorePlayerHealth()
        {
            if (HasStatVal((int) StatID.HP))
            {
                float cur = GetStatVal((int) StatID.HP);
                float max = GetStatVal((int) StatID.HPMax, 1);
                if (cur < max)
                {
                    SetStatVal((int) StatID.HP, max);
                }
            }
        }

        // ISAVE METHODS

        public virtual void OnSaving(ref SaveFile sf)
        {
            sf.itemInstancesCollected = new List<int>(m_ItemInstancesCollected);
            sf.items = new Dictionary<int, float>(m_Items);
            sf.stats = new Dictionary<int, float>(m_Stats);

            AutoMapSaveDataRequested?.Invoke();

            sf.autoMaps = m_AutoMaps.Values.ToArray();
        }

        public virtual void OnLoading(SaveFile sf)
        {
            m_ItemInstancesCollected = new List<int>(sf.itemInstancesCollected);
            m_Items = new Dictionary<int, float>(sf.items);
            m_Stats = new Dictionary<int, float>(sf.stats);

            m_AutoMaps.Clear();
            if (sf.autoMaps != null)
            {
                for (int i = 0; i < sf.autoMaps.Length; ++i)
                {
                    AutoMapSaveData map = sf.autoMaps[i];
                    m_AutoMaps.Add(map.gameplaySceneId, map);
                }
                AutoMapSaveDataProvided?.Invoke();
            }
        }

        // PRIVATE METHODS

        private void BuildItemDataDictionary()
        {
            var refs = config.ItemDataReferences;
            if (refs != null)
            {
                for (int i = 0; i < refs.Count; ++i)
                {
                    ItemData itemData = refs[i];
                    int id = itemData.ItemID;
                    if (id != (int) ItemID.None)
                    {
                        m_ItemDataDictionary.Add(id, itemData);
                    }
                }
            }
        }
    }
}