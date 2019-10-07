using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    [ExecuteAlways]
    public class GraphicController : MonoBehaviour, IBodyComponent
    {
        // CONSTANTS

        public const float DEFAULT_SPRITE_FPS = 20;

        // SERIALIZED FIELDS

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PRIVATE FIELDS

        private int m_AnimationFrameIndex;

        private int m_AnimationFrameListIndex;

        private int m_AnimationImageIndex;

        private List<Texture2D> m_AnimationTextureList;

        private float m_AnimationTimeElapsed;

        private AttackAbility m_AttackerAttackAbility;

        private CanvasRenderer m_CanvasRenderer;

        private TimeTrigger m_FlickerTimeTrigger;

        private Material m_Material;

        private RasterAnimationState m_RasterAnimationState;

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

        public RasterAnimation CurrentRasterAnimation { get; private set; }

        public RasterAnimationInfo RasterAnimationInfo { get; set; } // mainly for debug purposes

        protected MeshSortingLayer MeshSortingLayer { get; private set; }

        protected Renderer Renderer { get; private set; }

        // STANDARD PROPERTIES

        public bool IsRendered
        {
            get => Renderer?.enabled ?? false;
            set
            {
                if (Renderer != null)
                {
                    Renderer.enabled = value;
                }
            }
        }

        // SHORTCUT PROPERTIES

        public GameObjectBody Body => m_Body;

        protected virtual float DeltaTime => G.U.IsPlayMode(this) ? TimeThread.deltaTime : Time.deltaTime;

        protected virtual GameObject GraphicGameObject => m_Body.Refs.GraphicGameObject;

        protected virtual bool IsTimePaused => G.U.IsPlayMode(this) ? TimeThread.isPaused : false;

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(TimeThreadInstance.Field);

        private float AnimationFrameRate => DEFAULT_SPRITE_FPS;

        private int AnimationImageCount => m_AnimationTextureList?.Count ?? 0;

        private Material BaseSharedMaterial => m_Body.CharacterDossier?.GraphicData.BaseSharedMaterial;

        private GameObjectType GameObjectType => m_Body.GameObjectType;

        private string IdleAnimationName => m_Body.CharacterDossier?.GraphicData.IdleAnimationName;

        // FLIP STUFF

        private bool m_IsFlippedX;

        public event System.Action<bool> FlippedX;

        public bool IsFlippedX
        {
            get => m_IsFlippedX;
            set
            {
                if (m_IsFlippedX != value)
                {
                    m_IsFlippedX = value;

                    if (GraphicGameObject != null)
                    {
                        Transform tf = GraphicGameObject.transform;
                        tf.localPosition = tf.localPosition.Multiply(x: -1);
                        tf.localScale = tf.localScale.Multiply(x: -1);
                    }

                    FlippedX?.Invoke(value);
                }
            }
        }

        public void FlipX()
        {
            IsFlippedX = !m_IsFlippedX;
        }

        // INIT METHOD

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

        protected virtual void Start()
        {
            Renderer = GraphicGameObject.Require<Renderer>();

            MeshSortingLayer = G.U.Guarantee<MeshSortingLayer>(GraphicGameObject);

            if (BaseSharedMaterial != null)
            {
                Renderer.sharedMaterial = BaseSharedMaterial;
            }

            if (G.U.IsPlayMode(this))
            {
                m_Material = Renderer.material; //instance material
            }
            else
            {
                m_Material = Renderer.sharedMaterial;
            }

            switch (GameObjectType)
            {
                case GameObjectType.None:
                    break;
                case GameObjectType.Character:
                    InitCharacter();
                    break;
                case GameObjectType.Attack:
                    InitAttack();
                    break;
                default:
                    G.U.Err("Unsupported GameObjectType {0}.", GameObjectType);
                    break;
            }
        }

        protected virtual void Update()
        {
            if (IsTimePaused || G.U.IsEditMode(this)) return;

            m_AnimationTimeElapsed += DeltaTime;

            int newFrameIndex = Mathf.FloorToInt(AnimationFrameRate * m_AnimationTimeElapsed);

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

        private void InitCharacter()
        {
            AddCharacterStateHandlers();

            if (!string.IsNullOrEmpty(IdleAnimationName))
            {
                SetAnimation(IdleAnimationName);
            }
            else
            {
                RefreshGraphic();
            }
        }

        private void InitAttack()
        {
            Attack a = this.Require<Attack>();
            a.attacker.Body.Refs.GraphicController.SetAttackerAttackAbility(a.attackAbility);
        }

        private void RefreshGraphic()
        {
            if (m_Material == null) return;

            if (AnimationImageCount > m_AnimationImageIndex)
            {
                m_Material.mainTexture = m_AnimationTextureList[m_AnimationImageIndex];
            }

            m_Material.color = ImageColor;
        }

        // ANIMATION METHODS

        public void SetAnimation(string animationName)
        {
            SetAnimation(G.obj.RasterAnimations[animationName]);
        }

        public void SetAnimation(RasterAnimation rasterAnimation)
        {
            m_AnimationFrameIndex = 0;
            m_AnimationFrameListIndex = 0;
            m_AnimationImageIndex = 0;
            m_AnimationTextureList = rasterAnimation.FrameTextures;
            m_AnimationTimeElapsed = 0;

            CurrentRasterAnimation = rasterAnimation;

            if (G.U.IsPlayMode(this))
            {
                m_RasterAnimationState = new RasterAnimationState(
                    rasterAnimation, OnFrameSequenceStart, OnFrameSequenceStop);
                m_RasterAnimationState.rasterAnimationInfo = RasterAnimationInfo;
                m_AnimationImageIndex = m_RasterAnimationState.frameSequenceFromFrame - 1; // 1-based -> 0-based
            }

            RefreshGraphic();
        }

        public void SetAttackerAttackAbility(AttackAbility attackAbility)
        {
            m_AttackerAttackAbility = attackAbility;
            SetAnimation(attackAbility.GetRandomAttackerRasterAnimation());
        }

        public void PlayContextualAnimation(RasterAnimation ra, System.Action onCompleteCallback)
        {
            G.U.Todo("Play contextual animation.");
        }

        // MATERIAL METHODS

        public void SetSharedMaterial(Material sharedMaterial)
        {
            Renderer.sharedMaterial = sharedMaterial;

            if (G.U.IsPlayMode(this))
            {
                m_Material = Renderer.material; //instance material
            }
            else
            {
                m_Material = Renderer.sharedMaterial;
            }

            RefreshGraphic();
        }

        // COLOR METHODS

        public void StartDamageColor(float seconds)
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

        // FLICKER METHODS

        public void StartFlicker(float flickerRate = 30f)
        {
            if (m_FlickerTimeTrigger != null)
            {
                m_FlickerTimeTrigger.Dispose();
            }
            m_FlickerTimeTrigger = TimeThread.AddTrigger(1f / flickerRate, DoFlicker);
        }

        public void StopFlicker()
        {
            if (m_FlickerTimeTrigger != null)
            {
                m_FlickerTimeTrigger.Dispose();
                m_FlickerTimeTrigger = null;
            }
            IsRendered = true;
        }

        private void DoFlicker(TimeTrigger tt)
        {
            IsRendered = !IsRendered;
            tt.Proceed();
        }

        // IMAGE INDEX / FRAME SEQUENCE METHODS

        private void AdvanceImageIndex()
        {
            //m_AnimationImageIndex = (m_AnimationImageIndex + 1) % AnimationImageCount;

            // GraphicController uses zero-based image index (m_AnimationImageIndex)
            // RasterAnimation uses one-based frame number (frameNumber)

            int frameNumber;

            if (m_RasterAnimationState.frameSequenceHasFrameList)
            {
                if (!m_RasterAnimationState.AdvanceFrame(ref m_AnimationFrameListIndex, out frameNumber))
                {
                    OnAnimationStop();
                    return;
                }
            }
            else
            {
                frameNumber = m_AnimationImageIndex + 1;
                if (!m_RasterAnimationState.AdvanceFrame(ref frameNumber))
                {
                    OnAnimationStop();
                    return;
                }
            }

            m_AnimationImageIndex = frameNumber - 1;
        }

        private void OnFrameSequenceStart(RasterAnimationState ras)
        {

        }

        private void OnFrameSequenceStop(RasterAnimationState ras)
        {

        }

        private void OnAnimationStop()
        {
            m_AttackerAttackAbility = null;
            OnCharacterStateChange(0, false);
        }

        // CHARACTER STATE METHODS

        private void AddCharacterStateHandlers()
        {
            if (G.U.IsEditMode(this)) return;

            CharacterDossier cd = m_Body.CharacterDossier;

            if (cd == null) return;

            List<StateAnimation> stateAnimations = cd.GraphicData.StateAnimations;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                Body.Refs.StateOwner.AddStateHandler(sa.state, OnCharacterStateChange);
            }
        }

        private void RemoveCharacterStateHandlers()
        {
            if (G.U.IsEditMode(this)) return;

            CharacterDossier cd = m_Body.CharacterDossier;

            if (cd == null) return;

            List<StateAnimation> stateAnimations = cd.GraphicData.StateAnimations;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                Body.Refs.StateOwner.RemoveStateHandler(sa.state, OnCharacterStateChange);
            }
        }

        private void OnCharacterStateChange(ulong state, bool value)
        {
            CharacterDossier cd = m_Body.CharacterDossier;

            if (cd == null) return;

            if (m_AttackerAttackAbility != null) return;

            List<StateAnimation> stateAnimations = cd.GraphicData.StateAnimations;

            string animationName = IdleAnimationName;

            for (int i = 0; i < stateAnimations.Count; ++i)
            {
                StateAnimation sa = stateAnimations[i];

                if (m_Body.Refs.StateOwner.HasState(sa.state) || (value && state == sa.state))
                {
                    animationName = sa.animationName;
                    break;
                }
            }

            if (animationName != CurrentRasterAnimation?.name)
            {
                SetAnimation(animationName);
            }
        }
    }
}
