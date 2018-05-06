using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [System.Serializable]
    public sealed class FrameSequence {

        const int _version = 1;

        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_serializedVersion")]
        int _serializedVersion;

        [SerializeField]
        [Tooltip("An optional name you may give this frame sequence.")]
        [FormerlySerializedAs("m_name")]
        string _name;

        [SerializeField]
        [FormerlySerializedAs("m_fromFrame")]
        RangeInt _fromFrame = new RangeInt();

        [SerializeField]
        [FormerlySerializedAs("m_toFrame")]
        RangeInt _toFrame = new RangeInt();

        [SerializeField]
        [FormerlySerializedAs("m_playCount")]
        RangeInt _playCount = new RangeInt();

        [SerializeField]
        [Tooltip("Denotes that this does call code upon starting this frame sequence.")]
        [FormerlySerializedAs("m_doesCallCode")]
        bool _doesCallCode;

        //[System.Obsolete("Use _fromFrame instead.")]
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_from")]
        int _from;

        //[System.Obsolete("Use _toFrame instead.")]
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_to")]
        int _to;


        public string name { get { return _name; } }

        public int fromFrame { get { return _fromFrame.randomValue; } }

        public bool fromFrameMinInclusive { get { return _fromFrame.minInclusive; } }

        public bool fromFrameMaxInclusive { get { return _fromFrame.maxInclusive; } }

        public int fromFrameMinValue { get { return _fromFrame.minValue; } }

        public int fromFrameMaxValue { get { return _fromFrame.maxValue; } }

        public int toFrame { get { return _toFrame.randomValue; } }

        public bool toFrameMinInclusive { get { return _toFrame.minInclusive; } }

        public bool toFrameMaxInclusive { get { return _toFrame.maxInclusive; } }

        public int toFrameMinValue { get { return _toFrame.minValue; } }

        public int toFrameMaxValue { get { return _toFrame.maxValue; } }

        public int playCount { get { return _playCount.randomValue; } }

        public bool playCountMinInclusive { get { return _playCount.minInclusive; } }

        public bool playCountMaxInclusive { get { return _playCount.maxInclusive; } }

        public int playCountMinValue { get { return _playCount.minValue; } }

        public int playCountMaxValue { get { return _playCount.maxValue; } }

        public bool doesCallCode { get { return _doesCallCode; } }


        public void OnValidate() {
            //init version / migrate deprecated values
            if (_serializedVersion < _version) {
                //initialization
                if (_playCount.minValue == 0 && _playCount.maxValue == 0) {
                    _playCount.minValue = 1;
                }
                //version 0
                if (_from > 0) {
                    _fromFrame.minValue = _from;
                    _fromFrame.maxValue = _from;
                    _from = 0;
                }
                if (_to > 0) {
                    _toFrame.minValue = _to;
                    _toFrame.maxValue = _to;
                    _to = 0;
                }
                //version 1
                _serializedVersion = _version;
            }
            //real validation
            _fromFrame.minValue = Mathf.Max(1, _fromFrame.minValue);
            _toFrame.minValue = Mathf.Max(_fromFrame.maxValue, _toFrame.minValue);
            _playCount.minValue = Mathf.Max(0, _playCount.minValue);
        }
    }
}
