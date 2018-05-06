using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    [ExecuteInEditMode]
    public abstract class GraphicsController : MonoBehaviour {

        //values listed below are simply DEFAULTS; check inspector for actual values
        [SerializeField]
        [FormerlySerializedAs("m_graphicsData")]
        protected GraphicsData _graphicsData;
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;
        [SerializeField]
        [FormerlySerializedAs("m_spriteGraphic")]
        protected GameObject _spriteGraphic;
        [SerializeField]
        [FormerlySerializedAs("m_visibilityCondition")]
        protected VisibilityCondition _visibilityCondition;

        CanvasRenderer _canvasRenderer;
        TimeTrigger _flickerTimeTrigger;
        //the "image blend" terminology comes from GameMaker: Studio
        Color _imageBlend = Color.white;
        bool _isFlippedX;
        protected MeshSortingLayer _meshSortingLayer;
        protected Renderer _spriteRenderer;
        protected Transform _transform;

        public event System.Action flipXEvents;

        bool isVisibilityOK {
            get {
#if DEBUG_VISIBILITY
                return true;
#else
                return _visibilityCondition != VisibilityCondition.DebugOnly;
#endif
            }
        }

        //_isFlippedX should only be referenced by this isFlippedX property
        public bool isFlippedX {
            get {
                return _isFlippedX;
            }
            private set {
                if (_isFlippedX != value) {
                    _isFlippedX = value;
                    if (_spriteGraphic != null) {
                        Transform tf = _spriteGraphic.transform;
                        tf.localPosition = tf.localPosition.Multiply(x: -1);
                        tf.localScale = tf.localScale.Multiply(x: -1);
                    }
                    if (flipXEvents != null) flipXEvents();
                }
            }
        }

        public bool isRendered {
            get {
                return _canvasRenderer != null || (_spriteRenderer != null && _spriteRenderer.enabled);
            }
            set {
                if (_spriteRenderer != null) {
                    _spriteRenderer.enabled = value && isVisibilityOK;
                }
            }
        }

        public RasterAnimationInfo rasterAnimationInfo { get; set; }

        public virtual ITimeThread timeThread {
            get {
                return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
            }
        }

#region MonoBehaviour methods

        protected virtual void Awake() {
            _transform = transform;
        }

        protected virtual void Start() {
            InitSpriteGraphic();
        }

        protected virtual void Update() {
            if (G.isInEditMode) {
                Start();
                return;
            }

            if (timeThread.isPaused) return;

            ApplyImageBlend();
        }

#endregion

#region custom methods: public, protected, private

        public void FlipX() {
            isFlippedX = !isFlippedX;
        }

        public void StartDamageColor(float seconds) {
#if NS_DG_TWEENING
            _imageBlend = new Color(1, 0.2f, 0.2f);
            timeThread.AddTween(DOTween
                .To(() => _imageBlend, x => _imageBlend = x, Color.white, seconds)
                .SetEase(Ease.OutSine)
            );
#else
            G.U.Error("This function requires DG.Tweening (DOTween).");
#endif
        }

        protected void InitSpriteGraphic() {
            if (_spriteGraphic == null) return;
            _canvasRenderer = GetComponent<CanvasRenderer>();
            if (_canvasRenderer == null) {
                _spriteRenderer = G.U.Require<Renderer>(_spriteGraphic);
            }
            _meshSortingLayer = G.U.Guarantee<MeshSortingLayer>(_spriteGraphic);
            //the following must be done to refresh it with the current value of isVisibilityOK
            isRendered = isRendered;
        }

        void ApplyImageBlend() {
            if (_spriteRenderer != null) {
                _spriteRenderer.sharedMaterial.color = _imageBlend;
            }
        }

#endregion

#region flicker methods: public, private

        public void StartFlicker(float flickerRate = 30) {
            if (_flickerTimeTrigger != null) {
                _flickerTimeTrigger.Dispose();
            }
            _flickerTimeTrigger = timeThread.AddTrigger(1f / flickerRate, DoFlicker);
        }

        public void StopFlicker() {
            if (_flickerTimeTrigger != null) {
                _flickerTimeTrigger.Dispose();
                _flickerTimeTrigger = null;
            }
            isRendered = true;
        }

        void DoFlicker(TimeTrigger tt) {
            isRendered = !isRendered;
            tt.Proceed();
        }

#endregion

    }
}
