using UnityEngine;

namespace KRG
{
    public interface ICollider
    {
        void OnTriggerEnter(MonoBehaviour source, Collider other);

        void OnTriggerExit(MonoBehaviour source, Collider other);
    }
}
