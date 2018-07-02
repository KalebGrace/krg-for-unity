using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [CreateAssetMenu(
        fileName = "NewKRGRasterAnimation.asset",
        menuName = "KRG Scriptable Object/Raster Animation",
        order = 123
    )]
    public class RasterAnimation : KRGAnimation {

#region serialized fields

        [HideInInspector]
        [SerializeField]
        int _serializedVersion;

		[Header("Raster Data")]

        [SerializeField]
        [FormerlySerializedAs("m_gifBytes")]
        TextAsset _gifBytes;

        [SerializeField]
        [FormerlySerializedAs("m_dimensions")]
        protected Vector2 _dimensions;

        [Header("Looping")]

        [SerializeField]
        [FormerlySerializedAs("m_loop")]
        bool _loop = true;

        [SerializeField]
        [Tooltip("Checked: How many ADDITIONAL times to play this animation. Unchecked: Loop indefinitely.")]
        [BoolObjectDisable(false, "Infinite Loop")]
        BoolInt _loopCount = new BoolInt(false, 1);

        [SerializeField]
        [Tooltip("Index of the frame sequence from which to start any loops.")]
        int _loopToSequence;

        [Header("Frame Sequences")]

        [SerializeField]
        [FormerlySerializedAs("m_frameSequences")]
        FrameSequence[] _frameSequences;

#endregion

#region private fields

        bool _hasPlayableFrameSequences;

#endregion

#region properties

        public virtual Vector2 dimensions { get { return _dimensions; } }

        public virtual int frameSequenceCount { get { return _frameSequences.Length; } }

        public virtual int frameSequenceCountMax { get { return 20; } }

        public virtual TextAsset gifBytes { get { return _gifBytes; } }

        public virtual bool hasPlayableFrameSequences { get { return _hasPlayableFrameSequences; } }

        public virtual int loopToSequence { get { return _loopToSequence; } }

#endregion

#region MonoBehaviour methods

        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        protected virtual void Awake() {
            UpdateSerializedVersion();
            Validate();
            if (_frameSequences != null) {
                CheckForPlayableFrameSequences();
            }
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        protected virtual void OnValidate() {
            UpdateSerializedVersion();
            Validate();
            if (_frameSequences != null) {
                for (int i = 0; i < _frameSequences.Length; i++) {
                    _frameSequences[i].OnValidate();
                }
                CheckForPlayableFrameSequences();
            }
        }

#endregion

#region public methods

        /// <summary>
        /// Does this loop? NOTE: The loop index passed in (starting at 0) should be incremented every loop.
        /// </summary>
        /// <returns><c>true</c>, if the raster animation is intended to loop, <c>false</c> otherwise.</returns>
        /// <param name="loopIndex">Loop index.</param>
        public bool DoesLoop(int loopIndex) {
            return _loop && (!_loopCount.boolValue || loopIndex < _loopCount.intValue);
        }

        public virtual string GetFrameSequenceName(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.name : "";
        }

        public virtual int GetFrameSequenceFromFrame(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrame : 0;
        }

        public virtual int GetFrameSequenceFromFrameMin(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrameMinValue + (fs.fromFrameMinInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceFromFrameMax(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrameMaxValue - (fs.fromFrameMaxInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceToFrame(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrame : 0;
        }

        public virtual int GetFrameSequenceToFrameMin(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrameMinValue + (fs.toFrameMinInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceToFrameMax(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrameMaxValue - (fs.toFrameMaxInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequencePlayCount(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCount : -1;
        }

        public virtual int GetFrameSequencePlayCountMin(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCountMinValue + (fs.playCountMinInclusive ? 0 : 1) : -1;
        }

        public virtual int GetFrameSequencePlayCountMax(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCountMaxValue - (fs.playCountMaxInclusive ? 0 : 1) : -1;
        }

        public virtual bool GetFrameSequenceDoesCallCode(int frameSequenceIndex) {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null && fs.doesCallCode;
        }

#endregion

#region protected methods

        protected virtual FrameSequence GetFrameSequence(int frameSequenceIndex) {
            if (_frameSequences.Length > frameSequenceIndex) {
                return _frameSequences[frameSequenceIndex];
            } else {
                G.U.Error("The frameSequenceIndex is out of bounds.", this, frameSequenceIndex);
                return null;
            }
        }

        protected virtual void CheckForPlayableFrameSequences() {
            if (_frameSequences.Length > frameSequenceCountMax) {
                G.U.Error("Frame sequence count is higher than maximum.",
                    this, _frameSequences.Length, frameSequenceCountMax);
            }
            FrameSequence fs;
            int min, max;
            for (int i = 0; i < _frameSequences.Length; i++) {
                fs = _frameSequences[i];
                min = fs.playCountMinValue;
                max = fs.playCountMaxValue;
                if (max > 1 || (max == 1 && fs.playCountMaxInclusive) || (min == 1 && fs.playCountMinInclusive)) {
                    _hasPlayableFrameSequences = true;
                    return;
                }
            }
            _hasPlayableFrameSequences = false;
        }

        protected virtual void UpdateSerializedVersion() {
            /*
            switch (_serializedVersion) {
                case 0:
                    //put code here to assign default values or update existing values
                    _serializedVersion = 1;
                    break;
            }
            */
        }

        protected virtual void Validate() {
            //validate _loopCount
            _loopCount.intValue = Mathf.Max(_loopCount.intValue, 1);
            //validate _loopToSequence
            int max = (_frameSequences == null || _frameSequences.Length == 0) ? 0 : _frameSequences.Length - 1;
            _loopToSequence = Mathf.Clamp(_loopToSequence, 0, max);
        }

#endregion

    }
}
