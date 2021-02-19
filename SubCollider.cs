using UnityEngine;

namespace _0G.Legacy
{
    public sealed class SubCollider : MonoBehaviour, ICollider
    {
        [SerializeField, Enum(typeof(AlphaBravo))]
        private int m_Identifier = default;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter(this, m_Identifier, other);
        }
        public void OnTriggerEnter(MonoBehaviour source, int sourceID, Collider other)
        {
            Transform parentTF = transform.parent;
            if (parentTF == null) return;
            ICollider parentIC = parentTF.GetComponent<ICollider>();
            if (parentIC == null) return;
            parentIC.OnTriggerEnter(source, sourceID, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit(this, m_Identifier, other);
        }
        public void OnTriggerExit(MonoBehaviour source, int sourceID, Collider other)
        {
            Transform parentTF = transform.parent;
            if (parentTF == null) return;
            ICollider parentIC = parentTF.GetComponent<ICollider>();
            if (parentIC == null) return;
            parentIC.OnTriggerExit(source, sourceID, other);
        }
    }
}