using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public class BoolInt : BoolObject
    {
        [SerializeField, FormerlySerializedAs("m_int")]
        int _int;

        public int intValue { get { return _int; } set { _int = value; } }

        public BoolInt(bool boolValue, int intValue) : base(boolValue)
        {
            _int = intValue;
        }
    }
}