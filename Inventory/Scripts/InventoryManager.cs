using System.Collections.Generic;
using System.Linq;

namespace KRG
{
    public class InventoryManager : Manager, ISave, IOnDestroy
    {
        public override float priority => 130;


        // fields, delegates, events : ITEMS & STATS

        readonly Dictionary<int, ItemAcquiredHandler> m_KeyItemAcquiredHandlers
           = new Dictionary<int, ItemAcquiredHandler>();

        readonly Dictionary<int, ItemData> m_ItemDataDictionary
           = new Dictionary<int, ItemData>();

        private Dictionary<int, float> m_Items
          = new Dictionary<int, float>();

        private Dictionary<int, float> m_Stats
          = new Dictionary<int, float>();

        public delegate void ItemAcquiredHandler(int itemID, bool isNewlyAcquired);

        public event ItemAcquiredHandler ItemAcquired;


        // fields & events : AUTOMAPS

        readonly Dictionary<int, AutoMapSaveData> m_AutoMaps
           = new Dictionary<int, AutoMapSaveData>();

        public event System.Action AutoMapSaveDataRequested;
        public event System.Action AutoMapSaveDataProvided;


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


        // MAIN METHODS (PUBLIC)

        public void AddItemQty(ItemID itemID, float quantity, float defaultQuantity = 0)
        {
            AddItemQty((int)itemID, quantity, defaultQuantity);
        }

        public float GetItemQty(ItemID itemID, float defaultQuantity = 0)
        {
            return GetItemQty((int)itemID, defaultQuantity);
        }

        public bool HasItemQty(ItemID itemID)
        {
            return HasItemQty((int)itemID);
        }

        public void SetItemQty(ItemID itemID, float quantity)
        {
            SetItemQty((int)itemID, quantity);
        }

        public bool HasKeyItem(ItemID itemID)
        {
            return HasKeyItem((int)itemID);
        }

        public void AddStatVal(StatID statID, float value, float defaultValue = 0)
        {
            AddStatVal((int)statID, value, defaultValue);
        }

        public float GetStatVal(StatID statID, float defaultValue = 0)
        {
            return GetStatVal((int)statID, defaultValue);
        }

        public bool HasStatVal(StatID statID)
        {
            return HasStatVal((int)statID);
        }

        public void SetStatVal(StatID statID, float value)
        {
            SetStatVal((int)statID, value);
        }


        // MAIN METHODS (PROTECTED)

        protected void AddItemQty(int itemID, float quantity, float defaultQuantity = 0)
        {
            float oldQuantity = GetItemQty(itemID, defaultQuantity);
            float newQuantity = quantity + oldQuantity;
            ChangeItemQty(itemID, oldQuantity, newQuantity);
        }

        protected float GetItemQty(int itemID, float defaultQuantity = 0)
        {
            return m_Items.ContainsKey(itemID) ? m_Items[itemID] : defaultQuantity;
        }

        protected bool HasItemQty(int itemID)
        {
            return m_Items.ContainsKey(itemID);
        }

        protected void SetItemQty(int itemID, float quantity)
        {
            float oldQuantity = GetItemQty(itemID);
            float newQuantity = quantity;
            ChangeItemQty(itemID, oldQuantity, newQuantity);
        }

        public bool HasKeyItem(int itemID)
        {
            return m_Items.ContainsKey(itemID) && m_Items[itemID] >= 1;
        }

        private void ChangeItemQty(int itemID, float oldQuantity, float newQuantity)
        {
            ItemData itemData = GetItemData(itemID);

            bool hasKeyItem = itemData != null && itemData.IsKeyItem && newQuantity >= 1;

            if (hasKeyItem)
            {
                newQuantity = 1;
            }

            m_Items[itemID] = newQuantity;

            if (newQuantity > oldQuantity)
            {
                ItemAcquired?.Invoke(itemID, true);

                if (hasKeyItem)
                {
                    if (m_KeyItemAcquiredHandlers.ContainsKey(itemID))
                    {
                        m_KeyItemAcquiredHandlers[itemID]?.Invoke(itemID, true);
                        m_KeyItemAcquiredHandlers.Remove(itemID);
                    }

                    G.save.SaveCheckpoint();
                }
            }
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

        public ItemData GetItemData(int itemID)
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
            if (HasKeyItem(itemID))
            {
                handler?.Invoke(itemID, false);
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
            if (HasStatVal((int)StatID.HP))
            {
                float cur = GetStatVal((int)StatID.HP);
                float max = GetStatVal((int)StatID.HPMax, 1);
                if (cur < max)
                {
                    SetStatVal((int)StatID.HP, max);
                }
            }
        }


        // ISAVE METHODS

        public virtual void OnSaving(ref SaveFile sf)
        {
            sf.items = new Dictionary<int, float>(m_Items);
            sf.stats = new Dictionary<int, float>(m_Stats);

            AutoMapSaveDataRequested?.Invoke();

            sf.autoMaps = m_AutoMaps.Values.ToArray();
        }

        public virtual void OnLoading(SaveFile sf)
        {
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
            var refs = config.KeyItemDataReferences;

            for (int i = 0; i < refs.Length; ++i)
            {
                ItemData id = refs[i];

                m_ItemDataDictionary.Add(id.KeyItemID, id);
            }
        }
    }
}
