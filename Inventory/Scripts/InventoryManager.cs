using System.Collections.Generic;
using System.Linq;

namespace KRG
{
    public class InventoryManager : Manager, ISave, IOnDestroy
    {
        public override float priority => 130;

        public StateData? StateData { get; set; }


        // fields, delegates, events : ITEMS

        readonly Dictionary<int, ItemAcquiredHandler> m_ItemAcquiredHandlers
           = new Dictionary<int, ItemAcquiredHandler>();

        readonly Dictionary<int, ItemData> m_ItemDataDictionary
           = new Dictionary<int, ItemData>();

        public delegate void ItemAcquiredHandler(int acquiredItem);

        public event ItemAcquiredHandler KeyItemAcquired;


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


        // PUBLIC METHODS

        public void Acquire(int item)
        {
            if (!Has(item))
            {
                StateData.Value.Items.Add(item, 1);

                if (m_ItemAcquiredHandlers.ContainsKey(item))
                {
                    m_ItemAcquiredHandlers[item]?.Invoke(item);
                    m_ItemAcquiredHandlers.Remove(item);
                }

                KeyItemAcquired?.Invoke(item);

                G.save.SaveCheckpoint();
            }
            else
            {
                G.U.Warn("Already has key item {0}.", item);
            }
        }

        public ItemData GetItemData(int item)
        {
            G.U.Assert(m_ItemDataDictionary.ContainsKey(item), "Set item {0} ref in KRGConfig.", item);

            return m_ItemDataDictionary[item];
        }

        public bool Has(int item)
        {
            if (StateData.Value.Items.ContainsKey(item))
            {
                return StateData.Value.Items[item] >= 1;
            }

            return false;
        }

        /// <summary>
        /// Adds the handler to be invoked upon acquiring the key item with this index.
        /// If the key item is already acquired, it will invoke the handler immediately.
        /// </summary>
        /// <param name="item">Key item index.</param>
        /// <param name="handler">Handler.</param>
        public void AddItemAcquiredHandler(int item, ItemAcquiredHandler handler)
        {
            if (!Has(item))
            {
                if (m_ItemAcquiredHandlers.ContainsKey(item))
                {
                    m_ItemAcquiredHandlers[item] += handler;
                }
                else
                {
                    m_ItemAcquiredHandlers.Add(item, handler);
                }
            }
            else
            {
                handler?.Invoke(item);
            }
        }

        public void RemoveItemAcquiredHandler(int item, ItemAcquiredHandler handler)
        {
            if (m_ItemAcquiredHandlers.ContainsKey(item))
            {
                m_ItemAcquiredHandlers[item] -= handler;
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

        [System.Obsolete]
        public float GetStatVal(int statID)
        {
            return StateData.Value.Stats.ContainsKey(statID) ? StateData.Value.Stats[statID] : 0;
        }

        [System.Obsolete]
        public bool HasStatVal(int statID)
        {
            return StateData.Value.Stats.ContainsKey(statID);
        }

        [System.Obsolete]
        public void SetStatVal(int statID, float value)
        {
            StateData.Value.Stats[statID] = value;
        }


        // ISAVE METHODS

        public virtual void OnSaving(ref SaveFile sf)
        {
            sf.items = StateData.Value.Items;
            sf.acquiredItems = null;

            AutoMapSaveDataRequested?.Invoke();

            sf.autoMaps = m_AutoMaps.Values.ToArray();
        }

        public virtual void OnLoading(SaveFile sf)
        {
            Dictionary<int, float> itemDict = StateData.Value.Items;
            itemDict.Clear();
            if (sf.acquiredItems != null && sf.acquiredItems.Length > 0)
            {
                for (int i = 0; i < sf.acquiredItems.Length; ++i)
                {
                    int itemID = sf.acquiredItems[i];
                    itemDict.Add(itemID, 1);
                }
            }
            else if (sf.items != null)
            {
                foreach (KeyValuePair<int, float> pair in sf.items)
                {
                    itemDict.Add(pair.Key, pair.Value);
                }
            }

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

                m_ItemDataDictionary.Add(id.KeyItemIndex, id);
            }
        }
    }
}
