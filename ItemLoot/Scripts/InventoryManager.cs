using System.Collections.Generic;

namespace KRG
{
    public class InventoryManager : Manager, ISave
    {
        public override float priority => 130;


        // FIELDS & DELEGATES

        readonly List<int> m_AcquiredItems = new List<int>();

        readonly Dictionary<int, ItemAcquiredHandler> m_ItemAcquiredHandlers
           = new Dictionary<int, ItemAcquiredHandler>();

        public delegate void ItemAcquiredHandler(int acquiredItem);


        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
            ResetContents();
        }


        // PUBLIC METHODS

        public void Acquire(int item)
        {
            if (!Has(item))
            {
                m_AcquiredItems.Add(item);

                if (m_ItemAcquiredHandlers.ContainsKey(item))
                {
                    m_ItemAcquiredHandlers[item]?.Invoke(item);
                    m_ItemAcquiredHandlers.Remove(item);
                }

                G.save.SaveCheckpoint();
            }
            else
            {
                G.U.Warning("Already has key item {0}.", item);
            }
        }

        public bool Has(int item)
        {
            return m_AcquiredItems.Contains(item);
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
            if (m_ItemAcquiredHandlers.ContainsKey(item) )
            {
                m_ItemAcquiredHandlers[item] -= handler;
            }
        }

        public void ResetContents()
        {
            m_AcquiredItems.Clear();
        }


        // ISAVE METHODS

        public virtual void SaveTo(ref SaveFile sf)
        {
            sf.acquiredItems = m_AcquiredItems.ToArray();
        }

        public virtual void LoadFrom(SaveFile sf)
        {
            ResetContents();

            if (sf.acquiredItems != null) m_AcquiredItems.AddRange(sf.acquiredItems);
        }
    }
}
