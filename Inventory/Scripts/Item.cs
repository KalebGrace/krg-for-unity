using UnityEngine;

namespace KRG
{
    public class Item : MonoBehaviour, ICollider
    {
        // SERIALIZED FIELDS

        [Header("Item")]

        [SerializeField]
        protected ItemData itemData = default;

        [SerializeField]
        protected int m_InstanceID = default;

        [Header("Visual Effects")]

        [SerializeField]
        protected Transform animatingBody = default;

        // PROPERTIES

        public int InstanceID => m_InstanceID;

        // MONOBEHAVIOUR METHODS

        protected virtual void OnValidate()
        {
            if (m_InstanceID == 0)
            {
                m_InstanceID = GetInstanceID();
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            if (G.inv.HasKeyItem(itemData))
            {
                // player already has this key item
                gameObject.Dispose();
                return;
            }

            if (animatingBody != null) StartAnimateBody();
        }

        protected virtual void OnDestroy()
        {
            if (animatingBody != null) EndAnimateBody();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter(this, other);
        }
        public virtual void OnTriggerEnter(MonoBehaviour source, Collider other)
        {
            if (this != null && source != null)
            {
                Collider itemCollider = source.GetComponent<Collider>();
                if (itemCollider != null && itemCollider.isTrigger && itemData.CanBeCollectedBy(other))
                {
                    itemData.Collect(other);
                    gameObject.Dispose();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit(this, other);
        }
        public virtual void OnTriggerExit(MonoBehaviour source, Collider other) { }

        // CUSTOM METHODS

        protected virtual void StartAnimateBody() { }

        protected virtual void EndAnimateBody() { }
    }
}
