using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_UGIF
using uGIF;
#endif

namespace KRG
{
    [CreateAssetMenu(
        fileName = "NewKRGRasterAnimation.asset",
        menuName = "KRG Scriptable Object/Raster Animation",
        order = 123
    )]
    public class RasterAnimation : AnimationData
    {
        // CONSTANTS

        public const float DEFAULT_SPRITE_FPS = 20;

        // SERIALIZED FIELDS

        [Header("Custom Data")]

        [OrderAttribute(-10)]
        public RasterAnimationData Data;

        //--

        [Header("Essentials")]

        [OrderAttribute(-10)]
        [SerializeField]
        protected float m_SecondsPerFrame = default;

        [OrderAttribute(-10)]
        [SerializeField]
        protected Vector2Int m_Dimensions = default;

        [OrderAttribute(-10)]
        [SerializeField]
        protected List<Texture2D> m_FrameTextures = new List<Texture2D>();

        //--

        [Header("GIF Import")]

        [OrderAttribute(-10)]
        [SerializeField]
        [FormerlySerializedAs("m_gifBytes")]
        public TextAsset _gifBytes = default;

        [OrderAttribute(-10)]
        [SerializeField]
        protected string m_GifName = default;

        //--

        [Header("Looping")]

        [OrderAttribute(10)]
        [SerializeField]
        [FormerlySerializedAs("m_loop")]
        bool _loop = true;

        [OrderAttribute(10)]
        [SerializeField]
        [Tooltip("Checked: How many ADDITIONAL times to play this animation. Unchecked: Loop indefinitely.")]
        [BoolObjectDisable(false, "Infinite Loop")]
        BoolInt _loopCount = new BoolInt(false, 1);

        [OrderAttribute(10)]
        [SerializeField]
        [Tooltip("Index of the frame sequence from which to start any loops.")]
        int _loopToSequence = default;

        //--

        [Header("Frame Sequences")]

        [Order(15), SerializeField, TextArea(6, 24)]
        protected string m_FrameParagraph = default;

        [Order(15), SerializeField]
        private bool m_FrameParagraphParse = default;

        [OrderAttribute(20)]
        [SerializeField]
        [FormerlySerializedAs("m_frameSequences")]
        protected FrameSequence[] _frameSequences = default;

        // PROPERTIES

        public virtual Vector2 Dimensions => m_Dimensions;

        public virtual float FrameRate => m_SecondsPerFrame > 0 ? 1f / m_SecondsPerFrame : DEFAULT_SPRITE_FPS;

        public virtual int frameSequenceCount { get { return _frameSequences.Length; } }

        public virtual int frameSequenceCountMax { get { return 20; } }

        public virtual List<Texture2D> FrameTextures => m_FrameTextures;

        public virtual TextAsset gifBytes { get { return _gifBytes; } }

        public virtual bool hasPlayableFrameSequences { get; private set; }

        public virtual int loopToSequence { get { return _loopToSequence; } }

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnValidate()
        {
            Init();
        }

        // PRIVATE METHODS

        private void Init()
        {
            UpdateSerializedVersion();

            // validate _loopCount
            _loopCount.intValue = Mathf.Max(_loopCount.intValue, 1);

            // validate _loopToSequence
            int max = (_frameSequences == null || _frameSequences.Length == 0) ? 0 : _frameSequences.Length - 1;
            _loopToSequence = Mathf.Clamp(_loopToSequence, 0, max);

            if (m_FrameParagraphParse)
            {
                m_FrameParagraphParse = false;
                ParseFrameParagraph();
            }

            if (_frameSequences != null)
            {
                for (int i = 0; i < _frameSequences.Length; i++)
                {
                    _frameSequences[i].OnValidate();
                }
                CheckForPlayableFrameSequences();
            }
        }

        // OTHER METHODS

        public bool IsGifReplacedWithFrameTextures => !string.IsNullOrEmpty(m_GifName);

#if NS_UGIF
        public void ReplaceGifWithFrameTextures(Gif gif, List<Texture2D> frameTextures)
        {
            m_GifName = _gifBytes.name;
            m_SecondsPerFrame = gif.delay;
            m_Dimensions = new Vector2Int(gif.width, gif.height);
            m_FrameTextures = frameTextures;
        }
#endif

        /// <summary>
        /// Does this loop? NOTE: The loop index passed in (starting at 0) should be incremented every loop.
        /// </summary>
        /// <returns><c>true</c>, if the raster animation is intended to loop, <c>false</c> otherwise.</returns>
        /// <param name="loopIndex">Loop index.</param>
        public bool DoesLoop(int loopIndex)
        {
            return _loop && (!_loopCount.boolValue || loopIndex < _loopCount.intValue);
        }

        public virtual string GetFrameSequenceName(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.Name : "";
        }

        public ReadOnlyCollection<int> GetFrameSequenceFrameList(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.FrameList : null;
        }

        public virtual int GetFrameSequencePlayCount(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.PlayCount : -1;
        }

        public virtual int GetFrameSequencePlayCountMin(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.PlayCountMinValue + (fs.PlayCountMinInclusive ? 0 : 1) : -1;
        }

        public virtual int GetFrameSequencePlayCountMax(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.PlayCountMaxValue - (fs.PlayCountMaxInclusive ? 0 : 1) : -1;
        }

        public virtual List<int> GetFrameSequencePreActions(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs?.PreSequenceActions;
        }

        public virtual AudioPlayStyle GetFrameSequenceAudioPlayStyle(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs?.AudioPlayStyle ?? AudioPlayStyle.None;
        }

        public virtual string GetFrameSequenceAudioEvent(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs?.AudioEvent;
        }



        protected virtual FrameSequence GetFrameSequence(int frameSequenceIndex)
        {
            if (_frameSequences.Length > frameSequenceIndex)
            {
                return _frameSequences[frameSequenceIndex];
            }
            else
            {
                G.U.Err("The frameSequenceIndex is out of bounds.", this, frameSequenceIndex);
                return null;
            }
        }

        protected virtual void CheckForPlayableFrameSequences()
        {
            if (_frameSequences.Length > frameSequenceCountMax)
            {
                G.U.Err("Frame sequence count is higher than maximum.",
                    this, _frameSequences.Length, frameSequenceCountMax);
            }
            FrameSequence fs;
            int min, max;
            for (int i = 0; i < _frameSequences.Length; i++)
            {
                fs = _frameSequences[i];
                min = fs.PlayCountMinValue;
                max = fs.PlayCountMaxValue;
                if (max > 1 || (max == 1 && fs.PlayCountMaxInclusive) || (min == 1 && fs.PlayCountMinInclusive))
                {
                    hasPlayableFrameSequences = true;
                    return;
                }
            }
            hasPlayableFrameSequences = false;
        }

        protected virtual void UpdateSerializedVersion()
        {
            /*
            switch (_serializedVersion) {
                case 0:
                    //put code here to assign default values or update existing values
                    _serializedVersion = 1;
                    break;
            }
            */
        }

        private void ParseFrameParagraph()
        {
            if (string.IsNullOrEmpty(m_FrameParagraph)) return;

            m_FrameParagraph = m_FrameParagraph.Trim();

            if (m_FrameParagraph == "") return;

            if (_frameSequences == null)
            {
                _frameSequences = new FrameSequence[0];
            }

            char[] sep = { '\r', '\n' };
            string[] lines = m_FrameParagraph.Split(sep, 24);
            FrameSequence[] newArray = new FrameSequence[_frameSequences.Length + lines.Length];

            _frameSequences.CopyTo(newArray, _frameSequences.Length);

            for (int i = 0; i < lines.Length; ++i)
            {
                FrameSequence fs = new FrameSequence();
                fs.Name = lines[i];
                newArray[i + _frameSequences.Length] = fs;
            }

            _frameSequences = newArray;
            m_FrameParagraph = "";
        }
    }
}
