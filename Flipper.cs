using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _0G.Legacy
{
    [ExecuteAlways]
    public class Flipper : MonoBehaviour, IFacingDirection
    {
        [SerializeField, Enum(typeof(FacingDirection))]
        private int m_FacingDirection = default;

        [SerializeField]
        private bool m_FlipLocalPosition = default;
        [SerializeField]
        private bool m_FlipLocalRotation = default;
        [SerializeField]
        private bool m_FlipLocalScale = default;
        [SerializeField]
        private bool m_FlipCollider = default;

        [SerializeField]
        private GameObjectBody m_Body = default;

        public GameObjectBody Body => m_Body;

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        private void Awake()
        {
            if (G.U.IsPlayMode(this) && m_Body != null)
            {
                m_Body.FacingDirectionChanged += ((IFacingDirection) this).OnFacingDirectionChange;
            }
        }

        private void OnDestroy()
        {
            if (G.U.IsPlayMode(this) && m_Body != null)
            {
                m_Body.FacingDirectionChanged -= ((IFacingDirection) this).OnFacingDirectionChange;
            }
        }

        void IFacingDirection.OnFacingDirectionChange(Direction oldDirection, Direction newDirection)
        {
#if UNITY_EDITOR
            // IMPORTANT! this is needed to properly mark the new facing direction as dirty
            // (the actual undo part does not work properly, and should be avoided)
            Undo.RecordObject(this, "Changed Facing Direction");
#endif
            // while oldDirection is provided, we want to check our own data instead
            if (newDirection.IsOpposite((Direction) m_FacingDirection))
            {
                Flip();
            }
            m_FacingDirection = (int) newDirection;
        }

        private void Flip()
        {
            // TODO: support additional axes
            if (m_FlipLocalPosition)
            {
                transform.localPosition = transform.localPosition.Multiply(x: -1);
            }
            if (m_FlipLocalRotation)
            {
                Vector3 v = transform.localEulerAngles;
                v.y = (v.y + 180) % 360;
                transform.localEulerAngles = v;
            }
            if (m_FlipLocalScale)
            {
                transform.localScale = transform.localScale.Multiply(x: -1);
            }
            if (m_FlipCollider)
            {
                BoxCollider bc = GetComponent<BoxCollider>();
                if (bc != null)
                {
                    bc.center = bc.center.Multiply(x: -1);
                }
                SphereCollider sc = GetComponent<SphereCollider>();
                if (sc != null)
                {
                    sc.center = sc.center.Multiply(x: -1);
                }
            }
        }
    }
}