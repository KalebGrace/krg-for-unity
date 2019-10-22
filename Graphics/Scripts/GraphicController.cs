using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    [ExecuteAlways]
    public class GraphicController : MonoBehaviour, IBodyComponent
    {
        // DELEGATES

        public delegate void AnimationEndHandler(GraphicController graphicController, bool isCompleted);

        // EVENTS

        public event AnimationEndHandler AnimationEnded;
        public event RasterAnimationState.Handler FrameSequenceStarted;
        public event RasterAnimationState.Handler FrameSequenceStopped;

        // SERIALIZED FIELDS

        [SerializeField, Tooltip("Optional priority standalone animation.")]
        private RasterAnimation m_StandaloneAnimation = default;

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PRIVATE FIELDS

        private AnimationEndHandler m_AnimationCallback;

        private AnimationContext m_AnimationContext;

        private int m_AnimationFrameIndex;

        private int m_AnimationFrameListIndex;

        private int m_AnimationImageIndex;

        private List<Texture2D> m_AnimationTextureList;

        private float m_AnimationTimeElapsed;

        private CanvasRenderer m_CanvasRenderer;

        private TimeTrigger m_FlickerTimeTrigger;

        private Material m_Material;

        private RasterAnimationState m_RasterAnimationState;

        private RawImage m_RawImage;

        private Renderer m_Renderer;

        // COMPOUND PROPERTIES

        private Color m_ImageColor = Color.white;

        public Color ImageColor
        {
            get => m_ImageColor;
            set
            {
                m_ImageColor = value;
                RefreshGraphic();
            }
        }

        // STORAGE PROPERTIES

        protected MeshSortingLayer MeshSortingLayer { get; private set; }

        protected RasterAnimation RasterAnimation { get; private set; }

        // STANDARD PROPERTIES

        public bool IsRendered
        {
            get
            {
                // NOTE: null-coalescing operator does not work properly for this
                if (m_Renderer != null)
                {
                    return m_Renderer.enabled;
                }
                return false;
            }
            set
            {
                if (m_Renderer != null)
                {
                    m_Renderer.enabled = value;
                }
            }
        }

        // SHORTCUT PROPERTIES

        public GameObjectBody Body => m_Body;

        protected virtual GameObject GraphicGameObject => m_Body?.Refs.GraphicGameObject ?? gameObject;

        protected virtual bool IsTimePaused => TimeThread.isPaused;

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(TimeThreadInstance.Field);

        private int AnimationImageCount => m_AnimationTextureList?.Count ?? 0;

        private Material BaseSharedMaterial => CharacterDossier?.GraphicData.BaseSharedMaterial;

        private CharacterDossier CharacterDossier => m_Body.CharacterDossier;

        private string IdleAnimationName => CharacterDossier?.GraphicData.IdleAnimationName;

        // MONOBEHAVIOUR METHODS

        protected virtual void Start()
        {
            InitRenderer();
            InitMaterial();
            InitRawImage();
            InitMeshSort();

            InitStandaloneAnimation();

            if (m_Body != null && m_Body.IsCharacter)
            {
                InitCharacter();
            }
        }

        protected virtual void Update()
        {
            if (G.U.IsEditMode(this) || RasterAnimation == null || IsTimePaused) return;

            m_AnimationTimeElapsed += TimeThread.deltaTime;

            int newFrameIndex = Mathf.FloorToInt(RasterAnimation.FrameRate * m_AnimationTimeElapsed);

            while (m_AnimationFrameIndex < newFrameIndex)
            {
                ++m_AnimationFrameIndex;

                if (AnimationImageCount > 0)
                {
                    AdvanceImageIndex();
                }
            }

            RefreshGraphic();
        }

        protected virtual void OnDestroy()
        {
            RemoveCharacterStateHandlers();

            if (G.U.IsPlayMode(this))
            {
                DestroyImmediate(m_Material);
            }
        }

        // INITIALIZATION METHODS

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        private void InitRenderer()
        {
            m_Renderer = GraphicGameObject.GetComponent<Renderer>();
            if (m_Renderer == null || m_Body == null || BaseSharedMaterial == null) return;
            m_Renderer.sharedMaterial = BaseSharedMaterial;
        }

        private void InitMaterial()
        {
            if (m_Renderer == null) return;
            m_Material = G.U.IsEditMode(this) ? m_Renderer.sharedMaterial : m_Renderer.material; // instance
        }

        private void InitRawImage()
        {
            m_RawImage = GraphicGameObject.GetComponent<RawImage>();
        }

        private void InitMeshSort()
        {
            if (GraphicGameObject.GetComponent<MeshFilter>() == null) return;
            MeshSortingLayer = G.U.Guarantee<MeshSortingLayer>(GraphicGameObject);
        }

        private void InitStandaloneAnimation()
        {
            if (m_StandaloneAnimation == null) return;
            SetAnimation(AnimationContext.Priority, m_StandaloneAnimation);
        }

        private void InitCharacter()
        {
            if (m_AnimationContext == AnimationContext.None && !string.IsNullOrWhiteSpace(IdleAnimationName))
            {
                SetAnimation(AnimationContext.Idle, IdleAnimationName);
            }
            AddCharacterStateHandlers();
        }

        // MAIN METHODS

        public void RefreshGraphic()
        {
            if (AnimationImageCount == 0) return;

            int i = Mathf.Min(m_AnimationImageIndex, AnimationImageCount - 1);
            Texture texture = m_AnimationTextureList[i];

            if (m_RawImage != null)
            {
                m_RawImage.texture = texture;
                m_RawImage.color = ImageColor;
            }
            else if (m_Material != null)
            {
                m_Material.mainTexture = texture;
                m_Material.color = ImageColor;
            }
        }

        public void SetAnimation(AnimationContext context, string animationName, AnimationEndHandler callback = null)
        {
            if (G.obj.RasterAnimations.ContainsKey(animationName))
            {
                SetAnimation(context, G.obj.RasterAnimations[animationName], callback);
            }
            else
            {
                G.U.Err("Unable to find animation {0}.", animationName);
            }
        }

        public void SetAnimation(AnimationContext context, RasterAnimation rasterAnimation, AnimationEndHandler callback = null)
        {
            // handle an interrupted animation
            if (m_AnimationContext != AnimationContext.None)
            {
                OnAnimationEnd(false, false);
            }

            m_AnimationCallback = callback;
            m_AnimationContext = context;
            m_AnimationFrameIndex = 0;
            m_AnimationFrameListIndex = 0;
            m_AnimationImageIndex = 0;
            m_AnimationTextureList = rasterAnimation.FrameTextures;
            m_AnimationTimeElapsed = 0;
            RasterAnimation = rasterAnimation;

            if (G.U.IsPlayMode(this))
            {
                m_RasterAnimationState = new RasterAnimationState(
                    rasterAnimation, OnFrameSequenceStart, OnFrameSequenceStop);
                m_AnimationImageIndex = m_RasterAnimationState.frameSequenceFromFrame - 1; // 1-based -> 0-based
            }

            OnAnimationSet();

            RefreshGraphic();
        }

        public void EndAnimation(AnimationContext context)
        {
            // handle an interrupted animation
            if (m_AnimationContext == context)
            {
                OnAnimationEnd(false);
            }
            else
            {
                G.U.Err("Attempting to end animation context {0}, but current context is {1}.",
                    context, m_AnimationContext);
            }
        }

        private void FlipXInternal()
        {
            Transform tf = GraphicGameObject.transform;
            tf.localPosition = tf.localPosition.Multiply(x: -1);
            tf.localScale = tf.localScale.Multiply(x: -1);
        }

        public void SetSharedMaterial(Material sharedMaterial)
        {
            m_Renderer.sharedMaterial = sharedMaterial;
            m_Material = G.U.IsEditMode(this) ? m_Renderer.sharedMaterial : m_Renderer.material; // instance
            RefreshGraphic();
        }

        public void SetDamageColor(float seconds)
        {
#if NS_DG_TWEENING
            ImageColor = new Color(1, 0.2f, 0.2f);
            TimeThread.AddTween(DOTween
                .To(() => ImageColor, x => ImageColor = x, Color.white, seconds)
                .SetEase(Ease.OutSine)
            );
#else
            G.U.Err("This function requires DG.Tweening (DOTween).");
#endif
        }

        public void SetFlicker(float flickerRate = 20)
        {
            if (m_FlickerTimeTrigger != null)
            {
                m_FlickerTimeTrigger.Dispose();
            }
            m_FlickerTimeTrigger = TimeThread.AddTrigger(1f / flickerRate, DoFlicker);
        }

        private void DoFlicker(TimeTrigger tt)
        {
            IsRendered = !IsRendered;
            tt.Proceed();
        }

        public void EndFlicker()
        {
            if (m_FlickerTimeTrigger != null)
            {
                m_FlickerTimeTrigger.Dispose();
                m_FlickerTimeTrigger = null;
            }
            IsRendered = true;
        }

        // IMAGE INDEX / FRAME SEQUENCE METHODS

        public void AdvanceImageIndex()
        {
            // GraphicController uses zero-based image index (m_AnimationImageIndex)
            // RasterAnimation uses one-based frame number (frameNumber)

            int frameNumber;

            if (m_RasterAnimationState.frameSequenceHasFrameList)
            {
                if (!m_RasterAnimationState.AdvanceFrame(ref m_AnimationFrameListIndex, out frameNumber))
                {
                    OnAnimationEnd(true);
                    return;
                }
            }
            else
            {
                frameNumber = m_AnimationImageIndex + 1;
                if (!m_RasterAnimationState.AdvanceFrame(ref frameNumber))
                {
                    OnAnimationEnd(true);
                    return;
                }
            }

            m_AnimationImageIndex = frameNumber - 1;
        }

        private void OnFrameSequenceStart(RasterAnimationState state)
        {
            FrameSequenceStarted?.Invoke(state);
        }

        private void OnFrameSequenceStop(RasterAnimationState state)
        {
            FrameSequenceStopped?.Invoke(state);
        }

        protected virtual void OnAnimationSet() { }

        private void OnAnimationEnd(bool isCompleted, bool reassessState = true)
        {
            m_AnimationContext = AnimationContext.None;

            // invoke main callback and fire event as applicable
            m_AnimationCallback?.Invoke(this, isCompleted);
            m_AnimationCallback = null;
            AnimationEnded?.Invoke(this, isCompleted);

            // if no new animation set by callback/event, reassess state as applicable
            if (m_AnimationContext == AnimationContext.None && reassessState)
            {
                OnCharacterStateChange(0, false);
            }
        }

        // CHARACTER STATE METHODS

        private void AddCharacterStateHandlers()
        {
            if (G.U.IsEditMode(this)) return;

            if (m_Body == null || CharacterDossier == null) return;

            List<StateAnimation> stateAnimations = CharacterDossier.GraphicData.StateAnimations;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                m_Body.Refs.StateOwner.AddStateHandler(sa.state, OnCharacterStateChange);
            }
        }

        private void RemoveCharacterStateHandlers()
        {
            if (G.U.IsEditMode(this)) return;

            if (m_Body == null || CharacterDossier == null) return;

            List<StateAnimation> stateAnimations = CharacterDossier.GraphicData.StateAnimations;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                m_Body.Refs.StateOwner.RemoveStateHandler(sa.state, OnCharacterStateChange);
            }
        }

        protected virtual void OnCharacterStateChange(ulong state, bool value)
        {
            // ignore state change if currently playing a higher priority animation
            if (m_AnimationContext > AnimationContext.CharacterState) return;

            if (m_Body == null || CharacterDossier == null) return;

            List<StateAnimation> stateAnimations = CharacterDossier.GraphicData.StateAnimations;

            AnimationContext context = AnimationContext.Idle;
            string animationName = IdleAnimationName;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                if (m_Body.Refs.StateOwner.HasState(sa.state) || (value && state == sa.state))
                {
                    context = AnimationContext.CharacterState;
                    animationName = sa.animationName;
                    break;
                }
            }

            if (animationName != RasterAnimation?.name)
            {
                SetAnimation(context, animationName);
            }
        }

        // EDITOR METHODS

        /// <summary>
        /// Intended only for specialized editor use, such as an animation preview.
        /// </summary>
        public void ClearMaterialTexture()
        {
            m_Material.mainTexture = null;
        }

        /// <summary>
        /// Intended only for specialized editor use, such as an animation preview.
        /// </summary>
        public void ResetStandaloneAnimation()
        {
            if (m_StandaloneAnimation == null) return;
            SetAnimation(m_AnimationContext, m_StandaloneAnimation);
            m_RasterAnimationState = new RasterAnimationState(m_StandaloneAnimation);
            m_AnimationImageIndex = m_RasterAnimationState.frameSequenceFromFrame - 1; // 1-based -> 0-based
        }
    }
}
