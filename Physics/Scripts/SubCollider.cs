using UnityEngine;

namespace KRG
{
    public sealed class SubCollider : MonoBehaviour, ICollider
    {
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter(this, other);
        }
        public void OnTriggerEnter(MonoBehaviour source, Collider other)
        {
            Transform parentTF = transform.parent;
            if (parentTF == null) return;
            ICollider parentIC = parentTF.GetComponent<ICollider>();
            if (parentIC == null) return;
            parentIC.OnTriggerEnter(source, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit(this, other);
        }
        public void OnTriggerExit(MonoBehaviour source, Collider other)
        {
            Transform parentTF = transform.parent;
            if (parentTF == null) return;
            ICollider parentIC = parentTF.GetComponent<ICollider>();
            if (parentIC == null) return;
            parentIC.OnTriggerExit(source, other);
        }
    }
}
