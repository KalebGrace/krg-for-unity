using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// A VisRect GameObject should have a RectTransform and this VisRect script.
    /// Use position just like a normal Transform (e.g. old "Center" GameObject).
    /// Rotation should be identity (0, 0, 0). Scale should be one (1, 1, 1).
    /// Keep all four anchors at 0.5. Set height and width to 1 to start.
    /// Using the RectTransform tool, bound the four visual sides of the sprite.
    /// Drag the blue circle to what should be considered the center.
    /// If adjusting any values numerically, switch to raw mode first.
    /// NOTE: This was done with Unity 5.6.2f1. Different versions may work differently.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VisRect : MonoBehaviour, IBodyComponent
    {
        // SERIALIZED FIELDS

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PRIVATE FIELDS

        private RectTransform _rt;

        // PROPERTIES

        public GameObjectBody Body => m_Body;

        // Basic VisRect transform world space position

        public Vector3 center { get { return _rt.position; } }

        // Rectangle top and bottom world space positions

        public Vector3 rectTop { get { return _rt.position.Add(y: _rt.rect.yMax); } }

        public Vector3 rectBottom { get { return _rt.position.Add(y: _rt.rect.yMin); } }

        // Rectangle top and bottom local positions / offsets

        public Vector3 OffsetTop => new Vector3(0, _rt.rect.yMax);

        public Vector3 OffsetBottom => new Vector3(0, _rt.rect.yMin);

        // INIT METHOD

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

        private void Awake()
        {
            _rt = this.Require<RectTransform>();
        }
    }
}