using UnityEngine;

namespace KRG
{
    [RequireComponent(typeof(Collider))]
    public abstract class ColliderController : MonoBehaviour, IBodyComponent
    {
        [SerializeField]
        private GameObjectBody m_Body;

        private Collider m_Collider;

        public GameObjectBody Body => m_Body;

        public Vector3 Center => ((BoxCollider)m_Collider).center;

        public Vector3 Size => ((BoxCollider)m_Collider).size;

        public bool Enabled
        {
            get => m_Collider.enabled;
            set => m_Collider.enabled = value;
        }

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        private void Start()
        {
            G.U.Assert(gameObject.layer != Layer.Default, "This GameObject must exist on a hitbox/hurtbox Layer.");

            m_Collider = this.Require<Collider>();

            // We always want to have a Rigidbody on this (hitbox/hurtbox) GameObject in order to exclude
            // its Collider from the parent (bounding box) GameObject's Rigidbody when doing SweepTestAll.

            var rb = G.U.Guarantee<Rigidbody>(this);
            rb.isKinematic = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 p = transform.position;
            KRGGizmos.DrawCrosshairXY(p, 0.25f);
        }
    }
}
