using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KRG
{
    public class RasterAnimationState
    {
        // EVENTS

        /// <summary>
        /// Occurs when a frame sequence starts.
        /// </summary>
        public event RasterAnimationHandler FrameSequenceStarted;

        /// <summary>
        /// Occurs when a frame sequence stops.
        /// </summary>
        public event RasterAnimationHandler FrameSequenceStopped;

        /// <summary>
        /// Occurs when a frame sequence starts or after the play index is incremented.
        /// </summary>
        public event RasterAnimationHandler FrameSequencePlayLoopStarted;

        /// <summary>
        /// Occurs when a frame sequence stops or before the play index is incremented.
        /// </summary>
        public event RasterAnimationHandler FrameSequencePlayLoopStopped;

        // FIELDS

        /// <summary>
        /// The raster animation scriptable object.
        /// </summary>
        private RasterAnimation _rasterAnimation;

        /// <summary>
        /// The current loop index. If the raster animation's "Loop" setting is unchecked, this will always be 0.
        /// </summary>
        private int _loopIndex;

        /// <summary>
        /// The current frame sequence index (zero-based).
        /// </summary>
        private int _frameSequenceIndex;

        /// <summary>
        /// The name of the current frame sequence.
        /// </summary>
        protected string _frameSequenceName;

        /// <summary>
        /// The ordered list of frames to play in the current frame sequence.
        /// </summary>
        private ReadOnlyCollection<int> _frameSequenceFrameList;

        /// <summary>
        /// The current frame sequence's play count (may be a cached random value).
        /// </summary>
        private int _frameSequencePlayCount;

        /// <summary>
        /// The current frame sequence's play index (zero-based).
        /// </summary>
        private int _frameSequencePlayIndex;

        private RasterAnimationOptions m_Options;

        // PROPERTIES

        public virtual int frameSequenceIndex => _frameSequenceIndex;

        public virtual string frameSequenceName => _frameSequenceName;

        public virtual int frameSequenceFromFrame => _frameSequenceFrameList[0];

        public virtual RasterAnimation rasterAnimation => _rasterAnimation;

        public List<int> FrameSequencePreActions { get; private set; }

        public AudioPlayStyle FrameSequenceAudioPlayStyle { get; private set; }

        public string FrameSequenceAudioEvent { get; private set; }

        // CONSTRUCTOR

        public RasterAnimationState(RasterAnimation rasterAnimation, RasterAnimationOptions options)
        {
            _rasterAnimation = rasterAnimation;
            G.U.Require(_rasterAnimation, "Raster Animation", "this Raster Animation State");
            m_Options = options;
            FrameSequenceStarted = options.FrameSequenceStartHandler;
            FrameSequenceStopped = options.FrameSequenceStopHandler;
            FrameSequencePlayLoopStarted = options.FrameSequencePlayLoopStartHandler;
            FrameSequencePlayLoopStopped = options.FrameSequencePlayLoopStopHandler;
            Reset();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Advances the frame number.
        /// </summary>
        /// <returns><c>true</c>, if the animation should continue playing, <c>false</c> otherwise.</returns>
        /// <param name="frameListIndex">Frame list index.</param>
        /// <param name="frameNumber">Frame number (one-based).</param>
        public virtual bool AdvanceFrame(ref int frameListIndex, out int frameNumber)
        {
            frameNumber = 0;
            if (frameListIndex < _frameSequenceFrameList.Count - 1)
            {
                frameNumber = _frameSequenceFrameList[++frameListIndex];
            }
            else if (_frameSequencePlayIndex < _frameSequencePlayCount - 1)
            {
                FrameSequencePlayLoopStopped?.Invoke(this);
                ++_frameSequencePlayIndex;
                frameListIndex = 0;
                frameNumber = _frameSequenceFrameList[0];
                FrameSequencePlayLoopStarted?.Invoke(this);
            }
            else if (_frameSequenceIndex < _rasterAnimation.frameSequenceCount - 1)
            {
                InvokeFrameSequenceStopHandlers();
                SetFrameSequence(_frameSequenceIndex + 1);
                frameListIndex = 0;
                frameNumber = _frameSequenceFrameList[0];
            }
            else if (_rasterAnimation.DoesLoop(_loopIndex))
            {
                InvokeFrameSequenceStopHandlers();
                ++_loopIndex;
                SetFrameSequence(_rasterAnimation.loopToSequence);
                frameListIndex = 0;
                frameNumber = _frameSequenceFrameList[0];
            }
            else
            {
                InvokeFrameSequenceStopHandlers();
                //the animation has finished playing
                return false;
            }
            return true;
        }

        /// <summary>
        /// Advances the frame sequence.
        /// NOTE: This is all copied from AdvanceFrame(...)
        /// </summary>
        /// <returns><c>true</c>, if the animation should continue playing, <c>false</c> otherwise.</returns>
        /// <param name="frameListIndex">Frame list index.</param>
        /// <param name="frameNumber">Frame number (one-based).</param>
        public virtual bool AdvanceFrameSequence(ref int frameListIndex, out int frameNumber)
        {
            frameNumber = 0;
            if (_frameSequenceIndex < _rasterAnimation.frameSequenceCount - 1)
            {
                InvokeFrameSequenceStopHandlers();
                SetFrameSequence(_frameSequenceIndex + 1);
                frameListIndex = 0;
                frameNumber = _frameSequenceFrameList[0];
            }
            else if (_rasterAnimation.DoesLoop(_loopIndex))
            {
                InvokeFrameSequenceStopHandlers();
                ++_loopIndex;
                SetFrameSequence(_rasterAnimation.loopToSequence);
                frameListIndex = 0;
                frameNumber = _frameSequenceFrameList[0];
            }
            else
            {
                InvokeFrameSequenceStopHandlers();
                //the animation has finished playing
                return false;
            }
            return true;
        }

        public void Reset()
        {
            SetFrameSequence(0);
        }

        // PROTECTED METHODS

        /// <summary>
        /// Sets the frame sequence, or if not playable, advances to the next playable frame sequence.
        /// </summary>
        /// <param name="frameSequenceIndex">Frame sequence index.</param>
        protected virtual void SetFrameSequence(int frameSequenceIndex)
        {
            if (!_rasterAnimation.hasPlayableFrameSequences)
            {
                G.U.Err("This Raster Animation must have playable Frame Sequences.", this, _rasterAnimation);
                return;
            }
            int playCount, fsLoopCount = 0;
            while (true)
            {
                if (frameSequenceIndex >= _rasterAnimation.frameSequenceCount)
                {
                    frameSequenceIndex = 0;
                }
                playCount = _rasterAnimation.GetFrameSequencePlayCount(frameSequenceIndex);
                if (playCount > 0)
                {
                    break;
                }
                frameSequenceIndex++;
                fsLoopCount++;
                if (fsLoopCount >= _rasterAnimation.frameSequenceCountMax)
                {
                    G.U.Err("Stuck in an infinite loop.", this, _rasterAnimation);
                    return;
                }
            }
            if (playCount >= FrameSequence.INFINITE_PLAY_COUNT)
            {
                playCount = m_Options.IgnoreInfiniteLoops ? 1 : int.MaxValue;
            }
            _frameSequenceIndex = frameSequenceIndex;
            _frameSequenceName = _rasterAnimation.GetFrameSequenceName(frameSequenceIndex);
            _frameSequenceFrameList = _rasterAnimation.GetFrameSequenceFrameList(frameSequenceIndex);
            _frameSequencePlayCount = playCount;
            _frameSequencePlayIndex = 0;
            FrameSequencePreActions = _rasterAnimation.GetFrameSequencePreActions(frameSequenceIndex);
            FrameSequenceAudioPlayStyle = _rasterAnimation.GetFrameSequenceAudioPlayStyle(frameSequenceIndex);
            FrameSequenceAudioEvent = _rasterAnimation.GetFrameSequenceAudioEvent(frameSequenceIndex);
            InvokeFrameSequenceStartHandlers();
        }

        // PRIVATE METHODS

        private void InvokeFrameSequenceStartHandlers()
        {
            FrameSequenceStarted?.Invoke(this);
            FrameSequencePlayLoopStarted?.Invoke(this);
        }

        private void InvokeFrameSequenceStopHandlers()
        {
            FrameSequencePlayLoopStopped?.Invoke(this);
            FrameSequenceStopped?.Invoke(this);
        }
    }
}