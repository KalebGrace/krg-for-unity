using UnityEngine;

namespace KRG
{
    public abstract class Character : MonoBehaviour, IStateOwner
    {
        /// <summary>
        /// The child VisRect GameObject should be positioned at the visual center of the character.
        /// By contrast, the root Character GameObject should be positioned under the character's feet.
        /// </summary>
        [SerializeField]
        protected VisRect _visRect;

        [System.NonSerialized]
        protected Transform _transform;

        public GraphicsController GraphicsController { get; private set; }

        public abstract CharacterType Type { get; }

        public VisRect VisRect => _visRect;

        protected virtual void Awake()
        {
            _transform = transform;
            G.U.Require(_visRect, "VisRect GameObject");
            GraphicsController = this.Require<GraphicsController>();

#if DEBUG_VISIBILITY
            Transform center = _visRect.transform;
            KRGReferences refs = G.config.krgReferences;
            Instantiate(refs.characterDebugTextPrefab, center).Init(this);
            GraphicsController.rasterAnimationInfo = Instantiate(refs.rasterAnimationInfoPrefab, center);
#endif
        }
    }
}
