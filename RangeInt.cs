using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public class RangeInt : RangeObject
    {
        [SerializeField]
        [FormerlySerializedAs("m_minValue")]
        int _minValue;
        [SerializeField]
        [FormerlySerializedAs("m_maxValue")]
        int _maxValue;

        public int minValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnValidate();
            }
        }

        public int maxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnValidate();
            }
        }

        public int randomValue
        {
            get
            {
                //
                int min, max, r;
                //
                min = _minValue + (_minInclusive ? 0 : 1);
                max = _maxValue + (_maxInclusive ? 1 : 0);
                r = Random.Range(min, max); //as of Unity 5.6.2f1, this is said to be (inclusive, EXclusive)
                //
                //
                //
                //
                return r;
            }
        }

        public RangeInt()
        { //will call base constructor with ResetInclusives()
            ResetValues();
        }

        public RangeInt(int minValue, int maxValue)
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
            bool same = _minValue == _maxValue;
            bool near = _minValue < int.MaxValue && _minValue + 1 == _maxValue;
            if (same || near)
            {
                _minInclusive = true;
                _maxInclusive = true;
            }
        }

        public void Inclusivize()
        {
            if (!minInclusive)
            {
                minInclusive = true;
                ++minValue;
            }
            if (!maxInclusive)
            {
                maxInclusive = true;
                --maxValue;
            }
        }
    }
}