using UnityEngine;

namespace _0G.Legacy
{
    [CreateAssetMenu(
        fileName = "SomeBuff_Buff.asset",
        menuName = "0G Legacy Scriptable Object/Buff",
        order = 221
    )]
    public class BuffObject : ScriptableObject
    {
        public BuffData BuffData;

        public void Reset()
        {
            BuffData.Name = null;
            BuffData.BuffID = 0;
            BuffData.BuffStackID = 0;
            BuffData.HasDuration = true;
            BuffData.Duration = 300;
            BuffData.Effectors = null;
        }
    }
}