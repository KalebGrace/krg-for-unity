using UnityEngine;

namespace _0G.Legacy
{
    [RequireComponent(typeof(Collider))]
    public abstract class ColliderController : MonoBehaviour, IBodyComponent
    {
        // EVENTS & DELEGATES

        public event System.Action<Collider> TriggerEntered;
        public event System.Action<Collider> TriggerExited;

        // SERIALIZED FIELDS

        [SerializeField]
        private GameObjectBody m_Body = default;

        // COMPOUND PROPERTIES

        private Collider m_Collider;

        protected Collider Collider
        {
            get
            {
                if (m_Collider == null)
                {
                    m_Collider = this.Require<Collider>();
                }
                return m_Collider;
            }
        }

        // STANDARD PROPERTIES

        public bool Enabled
        {
            get => Collider.enabled;
            set => Collider.enabled = value;
        }

        // SHORTCUT PROPERTIES

        public GameObjectBody Body => m_Body;

        public Vector3 Center => ((BoxCollider) Collider).center;

        public Vector3 Size => ((BoxCollider) Collider).size;

        // METHODS

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        private void Start()
        {
            G.U.Assert(gameObject.layer != Layer.Default,
                "{0}'s {1} GameObject must exist on a hitbox/hurtbox Layer.",
                m_Body.name, gameObject.name);

            G.U.Assert(Collider.isTrigger,
                "{0}'s {1} Collider Component must be a trigger.",
                m_Body.name, gameObject.name);

            // We always want to have a Rigidbody on this (hitbox/hurtbox) GameObject in order to exclude
            // its Collider from the parent (bounding box) GameObject's Rigidbody when doing SweepTestAll.

            var rb = G.U.Guarantee<Rigidbody>(this);
            rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEntered?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExited?.Invoke(other);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 p = transform.position;
            KRGGizmos.DrawCrosshairXY(p, 0.25f);
        }
    }
}