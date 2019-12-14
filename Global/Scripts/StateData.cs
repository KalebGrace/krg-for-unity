using System.Collections.Generic;

namespace KRG
{
    public struct StateData
    {
        public Dictionary<int, float> Items;
        public Dictionary<int, float> Stats;

        public float GetItemQty(int itemID)
        {
            return Items.ContainsKey(itemID) ? Items[itemID] : 0;
        }
        public float GetItemQty(ItemID itemID)
        {
            return GetItemQty((int)itemID);
        }

        public float GetStatVal(int statID)
        {
            return Stats.ContainsKey(statID) ? Stats[statID] : 0;
        }
        public float GetStatVal(StatID statID)
        {
            return GetStatVal((int)statID);
        }

        public bool HasItemQty(int itemID)
        {
            return Items.ContainsKey(itemID);
        }
        public bool HasItemQty(ItemID itemID)
        {
            return HasItemQty((int)itemID);
        }

        public bool HasStatVal(int statID)
        {
            return Stats.ContainsKey(statID);
        }
        public bool HasStatVal(StatID statID)
        {
            return HasStatVal((int)statID);
        }

        public void SetItemQty(int itemID, float quantity)
        {
            Items[itemID] = quantity;
        }
        public void SetItemQty(ItemID itemID, float quantity)
        {
            SetItemQty((int)itemID, quantity);
        }

        public void SetStatVal(int statID, float value)
        {
            Stats[statID] = value;
        }
        public void SetStatVal(StatID statID, float value)
        {
            SetStatVal((int)statID, value);
        }
    }
}
