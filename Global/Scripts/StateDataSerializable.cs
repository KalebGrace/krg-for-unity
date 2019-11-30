using System.Collections.Generic;

namespace KRG
{
    [System.Serializable]
    public struct StateDataSerializable
    {
        public List<Item> Items;
        public List<Stat> Stats;

        [System.Serializable]
        public struct Item
        {
            [Enum(typeof(ItemID))]
            public int ItemID;
            public float Quantity;
        }

        [System.Serializable]
        public struct Stat
        {
            [Enum(typeof(StatID))]
            public int StatID;
            public float Value;
        }

        public StateData Deserialize()
        {
            StateData sd = new StateData();
            if (Items != null && Items.Count > 0)
            {
                sd.Items = new Dictionary<int, float>();
                for (int i = 0; i < Items.Count; ++i)
                {
                    Item item = Items[i];
                    sd.Items.Add(item.ItemID, item.Quantity);
                }
            }
            if (Stats != null && Stats.Count > 0)
            {
                sd.Stats = new Dictionary<int, float>();
                for (int i = 0; i < Stats.Count; ++i)
                {
                    Stat stat = Stats[i];
                    sd.Stats.Add(stat.StatID, stat.Value);
                }
            }
            return sd;
        }
    }
}
