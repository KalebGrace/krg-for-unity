using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    [System.Serializable]
    public struct BuffData
    {
        [Header("Buff")]

        public string Name;

        [Enum(typeof(BuffID))]
        public int BuffID;

        [Enum(typeof(BuffStackID))]
        public int BuffStackID;

        [Header("Duration")]

        public bool HasDuration;
        public float Duration;

        [Header("Effectors")]

        public List<Effector> Effectors;
    }
}