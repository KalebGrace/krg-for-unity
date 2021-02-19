using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public class BoolFloat : BoolObject
    {
        [SerializeField, FormerlySerializedAs("m_float")]
        float _float;

        public float floatValue { get { return _float; } set { _float = value; } }

        public BoolFloat(bool boolValue, float floatValue) : base(boolValue)
        {
            _float = floatValue;
        }
    }
}