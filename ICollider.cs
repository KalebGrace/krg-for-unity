using UnityEngine;

namespace KRG
{
    public interface ICollider
    {
        void OnTriggerEnter(MonoBehaviour source, int sourceID, Collider other);

        void OnTriggerExit(MonoBehaviour source, int sourceID, Collider other);
    }
}