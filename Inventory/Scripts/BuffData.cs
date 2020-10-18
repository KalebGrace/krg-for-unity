using System.Collections.Generic;

namespace KRG
{
    [System.Serializable]
    public struct BuffData
    {
        public bool HasDuration;
        public float Duration;
        public List<Effector> Effectors;
    }
}