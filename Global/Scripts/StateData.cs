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
        public float GetStatVal(int statID)
        {
            return Stats.ContainsKey(statID) ? Stats[statID] : 0;
        }

        public bool HasItemQty(int itemID)
        {
            return Items.ContainsKey(itemID);
        }
        public bool HasStatVal(int statID)
        {
            return Stats.ContainsKey(statID);
        }

        public void SetItemQty(int itemID, float quantity)
        {
            Items[itemID] = quantity;
        }
        public void SetStatVal(int statID, float value)
        {
            Stats[statID] = value;
        }
    }
}
