using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    public abstract class DestructibleObject<TPart> : MonoBehaviour, IExplodable where TPart : IExplodable
    {
        [SerializeField]
        [FormerlySerializedAs("m_partData")]
        protected DestructibleObjectData _partData = default;
        [SerializeField]
        Collider _externalCollider = default;
        [SerializeField]
        Renderer _externalRenderer = default;

        Vector3 _explosionPosition;

        public virtual void Explode(Vector3 explosionPosition)
        {
            _explosionPosition = explosionPosition;

            //disable collider/renderer as applicable
            var col = _externalCollider ?? GetComponent<Collider>();
            var ren = _externalRenderer ?? GetComponent<Renderer>();
            if (col != null) col.enabled = false;
            if (ren != null) ren.enabled = false;

            //explode all parts (and set data as applicable)
            TPart[] parts = GetComponentsInChildren<TPart>();
            for (int i = 0; i < parts.Length; i++)
            {
                TPart aPart = parts[i];
                if (aPart is IDestructibleObjectData idod)
                {
                    idod.data = _partData;
                }
                aPart.Explode(_explosionPosition);
            }

            gameObject.Dispose();
        }
    }
}