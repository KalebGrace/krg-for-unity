using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    public abstract class Character : MonoBehaviour, IStateOwner {


        /// <summary>
        /// A VisRect GameObject is an advanced form of the old "Center" GameObject.
        /// It serves the same purpose, but also has added functionality.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("m_visRect")]
        protected VisRect _visRect;

        /// <summary>
        /// DEPRECATED:
        /// A child "Center" GameObject that's positioned at the visual center of the character.
        /// By contrast, the current GameObject should be positioned under the character's feet.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("m_center")]
        protected GameObject _center;


        GraphicsController _graphicsController;

        protected Transform _transform;


        public GraphicsController graphicsController { get { return _graphicsController; } }

        public abstract CharacterType Type { get; }

        public VisRect visRect { get { return _visRect; } }


        protected virtual void Awake() {
            if (_visRect != null) {
                if (_center != null) {
                    G.U.Warning("When a VisRect is assigned to this Component, "
                    + "a manual \"Center\" GameObject assignment is unnecessary, and will be ignored.");
                }
                _center = _visRect.gameObject;
            } else {
                G.U.Require(_center, "VisRect (or \"Center\" GameObject)");
            }
            _graphicsController = this.Require<GraphicsController>();
            _transform = transform;

#if DEBUG_VISIBILITY
            KRGReferences refs = G.config.krgReferences;
            Instantiate(refs.characterDebugTextPrefab, _visRect.transform).Init(this);
            _graphicsController.rasterAnimationInfo = Instantiate(refs.rasterAnimationInfoPrefab, _visRect.transform);
#endif
        }
    }
}
