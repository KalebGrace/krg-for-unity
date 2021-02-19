using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [System.Serializable]
    public abstract class RangeObject
    {
        [SerializeField]
        [FormerlySerializedAs("m_minInclusive")]
        protected bool _minInclusive;
        [SerializeField]
        [FormerlySerializedAs("m_maxInclusive")]
        protected bool _maxInclusive;

        public bool minInclusive
        {
            get { return _minInclusive; }
            set
            {
                _minInclusive = value;
                OnValidate();
            }
        }

        public bool maxInclusive
        {
            get { return _maxInclusive; }
            set
            {
                _maxInclusive = value;
                OnValidate();
            }
        }

        protected RangeObject()
        {
            ResetInclusives();
        }

        public virtual void Reset()
        {
            ResetInclusives();
        }

        void ResetInclusives()
        { //do not make this virtual
            _minInclusive = true;
            _maxInclusive = true;
        }

        protected abstract void OnValidate();
    }
}