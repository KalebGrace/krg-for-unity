using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public abstract class BoolObject
    {
        [SerializeField, FormerlySerializedAs("m_bool")]
        bool _bool;

        public bool boolValue { get { return _bool; } set { _bool = value; } }

        protected BoolObject(bool boolValue)
        {
            _bool = boolValue;
        }
    }
}