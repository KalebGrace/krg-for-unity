using UnityEngine;

namespace _0G.Legacy
{
    public class Follow : MonoBehaviour, IBodyComponent
    {
        // SERIALIZED FIELDS

        [Tooltip("Follow this transform in world space. If no reference is assigned upon enabling, " +
            "it will auto-assign the current parent, and then reparent to the root, as applicable.")]
        public Transform TransformToFollow = default;

        [Tooltip("Automatically dispose of this body/object when the transform to follow becomes null (or is never set).")]
        public bool AutoDispose = default;

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PRIVATE FIELDS

        private Vector3 m_Offset;

        // PROPERTIES

        public GameObjectBody Body => m_Body;

        // INITIALIZATION METHODS

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

        private void OnEnable()
        {
            Transform me = m_Body != null ? m_Body.transform : transform;
            m_Offset = me.localPosition;
            if (TransformToFollow == null && me.parent != null)
            {
                TransformToFollow = me.parent;
                me.SetParent(null);
            }
        }

        private void LateUpdate()
        {
            if (TransformToFollow != null)
            {
                Transform me = m_Body != null ? m_Body.transform : transform;
                me.position = TransformToFollow.position + m_Offset;
            }
            else if (AutoDispose)
            {
                if (m_Body != null)
                {
                    m_Body.Dispose();
                }
                else
                {
                    gameObject.Dispose();
                }
                enabled = false;
            }
        }
    }
}