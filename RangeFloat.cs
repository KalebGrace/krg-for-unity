using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public class RangeFloat : RangeObject
    {
        [SerializeField]
        [FormerlySerializedAs("m_minValue")]
        float _minValue;
        [SerializeField]
        [FormerlySerializedAs("m_maxValue")]
        float _maxValue;

        public float minValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnValidate();
            }
        }

        public float maxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnValidate();
            }
        }

        public float randomValue
        {
            get
            {
                bool ok = false;
                float min, max, r = 0;
                while (!ok)
                {
                    min = _minValue;
                    max = _maxValue;
                    r = Random.Range(min, max); //as of Unity 5.6.2f1, this is said to be (inclusive, inclusive)
                    ok = true;
                    ok &= _minInclusive || !Mathf.Approximately(r, _minValue);
                    ok &= _maxInclusive || !Mathf.Approximately(r, _maxValue);
                }
                return r;
            }
        }

        public RangeFloat()
        { //will call base constructor with ResetInclusives()
            ResetValues();
        }

        public RangeFloat(float minValue, float maxValue)
        { //will call base constructor with ResetInclusives()
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override void Reset()
        {
            base.Reset();
            ResetValues();
        }

        void ResetValues()
        { //do not make this virtual
            _minValue = 0;
            _maxValue = 0;
        }

        protected override void OnValidate()
        {
            _maxValue = Mathf.Max(_minValue, _maxValue);
            bool same = Mathf.Approximately(_minValue, _maxValue);
            //
            if (same)
            {
                _minInclusive = true;
                _maxInclusive = true;
            }
        }
    }
}