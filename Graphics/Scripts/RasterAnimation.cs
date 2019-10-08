using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Serialization;

#if KRG_X_ODIN
using Sirenix.OdinInspector;
#endif

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

        [Header("Raster Data")]

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        public RasterAnimationData Data;

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        [SerializeField]
        [FormerlySerializedAs("m_gifBytes")]
        TextAsset _gifBytes = default;

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        [SerializeField]
        protected string m_GifName = default;

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        [SerializeField]
        protected float m_SecondsPerFrame = default;

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        [SerializeField]
        protected Vector2Int m_Dimensions = default;

#if KRG_X_ODIN
        [PropertyOrder(-10)]
#endif
        [SerializeField]
        protected List<Texture2D> m_FrameTextures = new List<Texture2D>();

        //--

        [Header("Looping")]

#if KRG_X_ODIN
        [PropertyOrder(10)]
#endif
        [SerializeField]
        [FormerlySerializedAs("m_loop")]
        bool _loop = true;

#if KRG_X_ODIN
        [PropertyOrder(10)]
#endif
        [SerializeField]
        [Tooltip("Checked: How many ADDITIONAL times to play this animation. Unchecked: Loop indefinitely.")]
        [BoolObjectDisable(false, "Infinite Loop")]
        BoolInt _loopCount = new BoolInt(false, 1);

#if KRG_X_ODIN
        [PropertyOrder(10)]
#endif
        [SerializeField]
        [Tooltip("Index of the frame sequence from which to start any loops.")]
        int _loopToSequence = default;

        //--

        [Header("Frame Sequences")]

#if KRG_X_ODIN
        [PropertyOrder(20)]
#endif
        [SerializeField]
        [FormerlySerializedAs("m_frameSequences")]
        FrameSequence[] _frameSequences = default;



        bool _hasPlayableFrameSequences;



        public virtual Vector2 Dimensions => m_Dimensions;

        public virtual float FrameRate => m_SecondsPerFrame > 0 ? 1f / m_SecondsPerFrame : DEFAULT_SPRITE_FPS;

        public virtual int frameSequenceCount { get { return _frameSequences.Length; } }

        public virtual int frameSequenceCountMax { get { return 20; } }

        public virtual List<Texture2D> FrameTextures => m_FrameTextures;

        public virtual TextAsset gifBytes { get { return _gifBytes; } }

        public virtual bool hasPlayableFrameSequences { get { return _hasPlayableFrameSequences; } }

        public virtual int loopToSequence { get { return _loopToSequence; } }



        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        protected virtual void Awake()
        {
            UpdateSerializedVersion();
            Validate();
            if (_frameSequences != null)
            {
                CheckForPlayableFrameSequences();
            }
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        protected virtual void OnValidate()
        {
            UpdateSerializedVersion();
            Validate();
            if (_frameSequences != null)
            {
                for (int i = 0; i < _frameSequences.Length; i++)
                {
                    _frameSequences[i].OnValidate();
                }
                CheckForPlayableFrameSequences();
            }
        }



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
            return fs != null ? fs.name : "";
        }

        public ReadOnlyCollection<int> GetFrameSequenceFrameList(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.frameList : null;
        }

        public virtual int GetFrameSequenceFromFrame(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrame : 0;
        }

        public virtual int GetFrameSequenceFromFrameMin(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrameMinValue + (fs.fromFrameMinInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceFromFrameMax(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.fromFrameMaxValue - (fs.fromFrameMaxInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceToFrame(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrame : 0;
        }

        public virtual int GetFrameSequenceToFrameMin(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrameMinValue + (fs.toFrameMinInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequenceToFrameMax(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.toFrameMaxValue - (fs.toFrameMaxInclusive ? 0 : 1) : 0;
        }

        public virtual int GetFrameSequencePlayCount(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCount : -1;
        }

        public virtual int GetFrameSequencePlayCountMin(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCountMinValue + (fs.playCountMinInclusive ? 0 : 1) : -1;
        }

        public virtual int GetFrameSequencePlayCountMax(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null ? fs.playCountMaxValue - (fs.playCountMaxInclusive ? 0 : 1) : -1;
        }

        public virtual bool GetFrameSequenceDoesCallCode(int frameSequenceIndex)
        {
            FrameSequence fs = GetFrameSequence(frameSequenceIndex);
            return fs != null && fs.doesCallCode;
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
                min = fs.playCountMinValue;
                max = fs.playCountMaxValue;
                if (max > 1 || (max == 1 && fs.playCountMaxInclusive) || (min == 1 && fs.playCountMinInclusive))
                {
                    _hasPlayableFrameSequences = true;
                    return;
                }
            }
            _hasPlayableFrameSequences = false;
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

        protected virtual void Validate()
        {
            //validate _loopCount
            _loopCount.intValue = Mathf.Max(_loopCount.intValue, 1);
            //validate _loopToSequence
            int max = (_frameSequences == null || _frameSequences.Length == 0) ? 0 : _frameSequences.Length - 1;
            _loopToSequence = Mathf.Clamp(_loopToSequence, 0, max);
        }
    }
}
