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

        private CanvasRenderer m_CanvasRenderer;

        private TimeTrigger m_FlickerTimeTrigger;

        private int m_FrameIndex;

        private AssetBundle m_IdleAnimAssetBundle;

        private RasterAnimation m_IdleRasterAnimation;

        private List<Texture2D> m_IdleTextureList;

        private Color m_ImageColor = Color.white;

        private int m_ImageIndex;

        private Material m_Material;

        private float m_TimeElapsed;

        // SHORTCUT PROPERTIES

        public GameObjectBody Body => m_Body;

        protected virtual float DeltaTime => Application.IsPlaying(this) ? TimeThread.deltaTime : Time.deltaTime;

        protected virtual GameObject GraphicGameObject => m_Body.Refs.GraphicGameObject;

        protected virtual bool IsTimePaused => Application.IsPlaying(this) ? TimeThread.isPaused : false;

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(TimeThreadInstance.Gameplay);

        private Material BaseSharedMaterial => m_Body.CharacterDossier?.GraphicData.BaseSharedMaterial;

        private float FrameRate => DEFAULT_SPRITE_FPS;

        private int ImageCount => m_IdleTextureList?.Count ?? 0;

        // STORAGE PROPERTIES

        public RasterAnimationInfo RasterAnimationInfo { get; set; } // mainly for debug purposes

        protected MeshSortingLayer MeshSortingLayer { get; private set; }

        // FLIP STUFF

        private bool m_IsFlippedX;

        public event System.Action<bool> FlippedX;

        public bool IsFlippedX
        {
            get
            {
                return m_IsFlippedX;
            }
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

        // RENDER PROPERTIES

        protected Renderer Renderer { get; private set; }

        public bool IsRendered
        {
            get
            {
                return Renderer?.enabled ?? false;
            }
            set
            {
                if (Renderer != null)
                {
                    Renderer.enabled = value;
                }
            }
        }

        // INIT METHOD

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

        protected virtual void Start()
        {
            G.U.Log("GraphicController Start for {0}.", name);
            Renderer = GraphicGameObject.Require<Renderer>();

            MeshSortingLayer = G.U.Guarantee<MeshSortingLayer>(GraphicGameObject);

            if (BaseSharedMaterial != null)
            {
                Renderer.sharedMaterial = BaseSharedMaterial;
            }

            if (Application.IsPlaying(this))
            {
                m_Material = Renderer.material; //instance material
            }
            else
            {
                m_Material = Renderer.sharedMaterial;
            }

            string idleAnimName = m_Body.CharacterDossier?.GraphicData.IdleRasterAnimationName;

            if (!string.IsNullOrEmpty(idleAnimName))
            {
                m_IdleAnimAssetBundle = G.obj.LoadCharacterAnimationAssetBundle(idleAnimName);
                m_IdleRasterAnimation = m_IdleAnimAssetBundle.LoadAsset<RasterAnimation>(idleAnimName);
                m_IdleTextureList = m_IdleRasterAnimation.FrameTextures;
            }

            ShowImage(m_ImageIndex);
        }

        protected virtual void Update()
        {
            if (IsTimePaused) return;

            float ts = m_TimeElapsed;
            float td = DeltaTime;
            float te = ts + td;

            int fs = m_FrameIndex;
            int fe = Mathf.FloorToInt(FrameRate * te);

            while (fs < fe)
            {
                m_FrameIndex = ++fs;
                FrameUpdate();
            }

            m_TimeElapsed = te;

            if (Application.IsPlaying(this))
            {
                ApplyImageColor();
            }
        }

        protected virtual void OnDestroy()
        {
            G.U.Log("GraphicController OnDestroy for {0}.", name);
            if (m_IdleAnimAssetBundle != null)
            {
                m_IdleAnimAssetBundle.Unload(true);
            }

            if (Application.IsPlaying(this))
            {
                DestroyImmediate(m_Material);
            }
        }

        // PRIVATE IMAGE METHODS

        private void FrameUpdate()
        {
            if (ImageCount == 0) return;

            m_ImageIndex = (m_ImageIndex + 1) % ImageCount;

            ShowImage(m_ImageIndex);
        }

        private void ShowImage(int imageIndex)
        {
            if (ImageCount == 0) return;

            m_Material.mainTexture = m_IdleTextureList[imageIndex];
        }

        // COLOR METHODS

        public void StartDamageColor(float seconds)
        {
#if NS_DG_TWEENING
            m_ImageColor = new Color(1, 0.2f, 0.2f);
            TimeThread.AddTween(DOTween
                .To(() => m_ImageColor, x => m_ImageColor = x, Color.white, seconds)
                .SetEase(Ease.OutSine)
            );
#else
            G.U.Err("This function requires DG.Tweening (DOTween).");
#endif
        }

        private void ApplyImageColor()
        {
            Renderer.sharedMaterial.color = m_ImageColor;
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
    }
}
