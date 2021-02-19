using UnityEngine;

namespace _0G.Legacy
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

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            if (G.inv.HasKeyItem(itemData))
            {
                // player already has this key item
                gameObject.Dispose();
                return;
            }

            if (G.inv.HasItemInstanceCollected(m_InstanceID))
            {
                // player already collected this instance of this item
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
            OnTriggerEnter(this, 0, other);
        }
        public virtual void OnTriggerEnter(MonoBehaviour source, int sourceID, Collider other)
        {
            if (this == null) return; // if we destroy this later, we don't want this running again
            if (source == null) return; // if for some reason the source was unreported, it's not a collect
            Collider sourceCollider = source.GetComponent<Collider>();
            if (sourceCollider == null) return; // again, if unreported, it's not a collect
            if (!sourceCollider.isTrigger) return; // we only want triggers; no bounding boxes
            if (!itemData.CanCollect(other, m_InstanceID)) return; // if you can't collect, don't collect
            itemData.Collect(other, m_InstanceID);
            gameObject.Dispose();
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit(this, 0, other);
        }
        public virtual void OnTriggerExit(MonoBehaviour source, int sourceID, Collider other) { }

        // CUSTOM METHODS

        public virtual void SpawnInit()
        {
            m_InstanceID = 0;
        }

        protected virtual void StartAnimateBody() { }

        protected virtual void EndAnimateBody() { }
    }
}