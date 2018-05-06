using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class RasterAnimationState {

#region delegates

        public delegate void Handler(RasterAnimationState ras);

#endregion

#region fields

        /// <summary>
        /// Occurs when a frame sequence starts, if it has "Does Call Code" set to true.
        /// </summary>
        event Handler _frameSequenceStartHandlers;

        /// <summary>
        /// Occurs when a frame sequence stops, if it has "Does Call Code" set to true.
        /// </summary>
        event Handler _frameSequenceStopHandlers;

        /// <summary>
        /// The raster animation scriptable object.
        /// </summary>
        RasterAnimation _rasterAnimation;

        /// <summary>
        /// The raster animation info (optional debugging tool).
        /// </summary>
        RasterAnimationInfo _rasterAnimationInfo;

        /// <summary>
        /// The current loop index. If the raster animation's "Loop" setting is unchecked, this will always be 0.
        /// </summary>
        int _loopIndex;

        /// <summary>
        /// The current frame sequence index (zero-based).
        /// </summary>
        int _frameSequenceIndex;

        /// <summary>
        /// The name of the frame sequence.
        /// </summary>
        protected string _frameSequenceName;

        /// <summary>
        /// The current frame sequence's From Frame (one-based; may be a cached random value).
        /// </summary>
        int _frameSequenceFromFrame;

        /// <summary>
        /// The current frame sequence's To Frame (one-based; may be a cached random value).
        /// </summary>
        int _frameSequenceToFrame;

        /// <summary>
        /// The current frame sequence's Play Count (may be a cached random value).
        /// </summary>
        int _frameSequencePlayCount;

        /// <summary>
        /// The current frame sequence's play index (zero-based).
        /// </summary>
        int _frameSequencePlayIndex;

        /// <summary>
        /// Whether the current frame sequence does call code.
        /// </summary>
        bool _frameSequenceDoesCallCode;

#endregion

#region properties

        public virtual int frameSequenceIndex { get { return _frameSequenceIndex; } }

        public virtual string frameSequenceName { get { return _frameSequenceName; } }

        public virtual int frameSequenceFromFrame { get { return _frameSequenceFromFrame; } }

        public virtual RasterAnimation rasterAnimation { get { return _rasterAnimation; } }

        public virtual RasterAnimationInfo rasterAnimationInfo { set { _rasterAnimationInfo = value; } }

#endregion

#region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KRG.RasterAnimationState"/> class.
        /// </summary>
        /// <param name="rasterAnimation">The raster animation.</param>
        /// <param name="frameSequenceStartHandler">An optional frame sequence start handler.</param>
        /// <param name="frameSequenceStopHandler">An optional frame sequence stop handler.</param>
        public RasterAnimationState(
            RasterAnimation rasterAnimation,
            Handler frameSequenceStartHandler = null,
            Handler frameSequenceStopHandler = null
        ) {
            _rasterAnimation = rasterAnimation;
            G.U.Require(_rasterAnimation, "Raster Animation", "this Raster Animation State");
            _frameSequenceStartHandlers = frameSequenceStartHandler;
            _frameSequenceStopHandlers = frameSequenceStopHandler;
            Reset();
        }

#endregion

#region public methods

        /// <summary>
        /// Advances the frame number. Currently this only supports moving forward by a single frame.
        /// </summary>
        /// <returns><c>true</c>, if the animation should continue playing, <c>false</c> otherwise.</returns>
        /// <param name="frameNumber">Frame number (one-based).</param>
        public virtual bool AdvanceFrame(ref int frameNumber) {
            if (frameNumber < _frameSequenceToFrame) {
                frameNumber++;
            } else if (_frameSequencePlayIndex < _frameSequencePlayCount - 1) {
                _frameSequencePlayIndex++;
                frameNumber = _frameSequenceFromFrame;
            } else if (_frameSequenceIndex < _rasterAnimation.frameSequenceCount - 1) {
                InvokeFrameSequenceStopHandlers();
                SetFrameSequence(_frameSequenceIndex + 1);
                frameNumber = _frameSequenceFromFrame;
            } else if (_rasterAnimation.DoesLoop(_loopIndex)) {
                InvokeFrameSequenceStopHandlers();
                _loopIndex++;
                SetFrameSequence(_rasterAnimation.loopToSequence);
                frameNumber = _frameSequenceFromFrame;
            } else {
                InvokeFrameSequenceStopHandlers();
                //the animation has finished playing
                return false;
            }
            RefreshRasterAnimationInfo(frameNumber);
            return true;
        }

        public void Reset() {
            SetFrameSequence(0);
        }

#endregion

#region protected methods

        protected virtual void RefreshRasterAnimationInfo(int frameNumber) {
            RasterAnimationInfo i = _rasterAnimationInfo;
            if (i == null) return;
            i.frameSequenceName = _frameSequenceName;
            i.frameSequenceFromFrameMin = _rasterAnimation.GetFrameSequenceFromFrameMin(_frameSequenceIndex);
            i.frameSequenceFromFrame = _frameSequenceFromFrame;
            i.frameSequenceFromFrameMax = _rasterAnimation.GetFrameSequenceFromFrameMax(_frameSequenceIndex);
            i.frameSequenceToFrameMin = _rasterAnimation.GetFrameSequenceToFrameMin(_frameSequenceIndex);
            i.frameSequenceToFrame = _frameSequenceToFrame;
            i.frameSequenceToFrameMax = _rasterAnimation.GetFrameSequenceToFrameMax(_frameSequenceIndex);
            i.frameSequencePlayCountMin = _rasterAnimation.GetFrameSequencePlayCountMin(_frameSequenceIndex);
            i.frameSequencePlayCount = _frameSequencePlayCount;
            i.frameSequencePlayCountMax = _rasterAnimation.GetFrameSequencePlayCountMax(_frameSequenceIndex);
            i.frameSequenceCount = _rasterAnimation.frameSequenceCount;
            i.frameSequenceIndex = _frameSequenceIndex;
            i.frameSequencePlayIndex = _frameSequencePlayIndex;
            i.frameNumber = frameNumber;
            i.Refresh();
        }

        /// <summary>
        /// Sets the frame sequence, or if not playable, advances to the next playable frame sequence.
        /// </summary>
        /// <param name="frameSequenceIndex">Frame sequence index.</param>
        protected virtual void SetFrameSequence(int frameSequenceIndex) {
            if (!_rasterAnimation.hasPlayableFrameSequences) {
                G.U.Error("This Raster Animation must have playable Frame Sequences.", this, _rasterAnimation);
                return;
            }
            int playCount, fsLoopCount = 0;
            while (true) {
                if (frameSequenceIndex >= _rasterAnimation.frameSequenceCount) {
                    frameSequenceIndex = 0;
                }
                playCount = _rasterAnimation.GetFrameSequencePlayCount(frameSequenceIndex);
                if (playCount > 0) {
                    break;
                }
                frameSequenceIndex++;
                fsLoopCount++;
                if (fsLoopCount >= _rasterAnimation.frameSequenceCountMax) {
                    G.U.Error("Stuck in an infinite loop.", this, _rasterAnimation);
                    return;
                }
            }
            _frameSequenceIndex = frameSequenceIndex;
            _frameSequenceName = _rasterAnimation.GetFrameSequenceName(frameSequenceIndex);
            _frameSequenceFromFrame = _rasterAnimation.GetFrameSequenceFromFrame(frameSequenceIndex);
            _frameSequenceToFrame = _rasterAnimation.GetFrameSequenceToFrame(frameSequenceIndex);
            _frameSequencePlayCount = playCount;
            _frameSequencePlayIndex = 0;
            _frameSequenceDoesCallCode = _rasterAnimation.GetFrameSequenceDoesCallCode(frameSequenceIndex);
            RefreshRasterAnimationInfo(_frameSequenceFromFrame);
            InvokeFrameSequenceStartHandlers();
        }

#endregion

#region handler methods

        public void AddFrameSequenceStartHandler(Handler frameSequenceStartHandler) {
            _frameSequenceStartHandlers += frameSequenceStartHandler;
        }

        public void AddFrameSequenceStopHandler(Handler frameSequenceStopHandler) {
            _frameSequenceStopHandlers += frameSequenceStopHandler;
        }

        public void RemoveFrameSequenceStartHandler(Handler frameSequenceStartHandler) {
            _frameSequenceStartHandlers -= frameSequenceStartHandler;
        }

        public void RemoveFrameSequenceStopHandler(Handler frameSequenceStopHandler) {
            _frameSequenceStopHandlers -= frameSequenceStopHandler;
        }

        void InvokeFrameSequenceStartHandlers() {
            if (_frameSequenceDoesCallCode && _frameSequenceStartHandlers != null) _frameSequenceStartHandlers(this);
        }

        void InvokeFrameSequenceStopHandlers() {
            if (_frameSequenceDoesCallCode && _frameSequenceStopHandlers != null) _frameSequenceStopHandlers(this);
        }

#endregion

    }
}
