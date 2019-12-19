#if NS_DG_TWEENING
using DG.Tweening;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    public class DestructibleObjectPart : MonoBehaviour, IDestroyedEvent<DestructibleObjectPart>, IDestructibleObjectData, IExplodable
    {
        public event System.Action<DestructibleObjectPart> Destroyed;

        [SerializeField]
        [FormerlySerializedAs("m_data")]
        protected DestructibleObjectData _data;

        private Collider _collider;
        private Renderer _renderer;
        private Rigidbody _rigidbody;

        DestructibleObjectData IDestructibleObjectData.data { get => _data; set => _data = value; }

        private void Awake()
        {
            _collider = this.Require<Collider>();
            _renderer = this.Require<Renderer>();
            _rigidbody = this.Require<Rigidbody>();

            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _renderer.enabled = false;
        }

        private void FixedUpdate()
        {
            //TODO: remove this code when all bugs are fixed
            if (float.IsNaN(transform.position.x) ||
                float.IsNaN(transform.position.y) ||
                float.IsNaN(transform.position.z) ||
                float.IsInfinity(transform.position.x) ||
                float.IsInfinity(transform.position.y) ||
                float.IsInfinity(transform.position.z))
            {
                G.U.Log("Invalid position for DestructibleObjectPart.");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        public void Explode(Vector3 explosionPosition)
        {
            G.U.Require(_data, "Data");

            _rigidbody.isKinematic = false;
            _collider.sharedMaterial = _data.physicMaterial;
            _collider.enabled = true;
            _renderer.enabled = true;

            _rigidbody.AddExplosionForce(_data.explosionForce, explosionPosition, _data.explosionRadius);

#if NS_DG_TWEENING
            if (_data.doesFade)
            {
                _renderer.material.DOColor(new Color(0, 0, 0, 0), _data.lifetime);
            }
#endif

            //unparent from DestructibleObject, as that will be disposed after this method ends
            //null is safer to use than an ancestor, as an ancestor may be destroyed
            transform.parent = null;

            //dispose of me later
            _data.timeThread.AddTrigger(_data.lifetime, Dispose);
        }

        private void Dispose(TimeTrigger tt)
        {
            //this may already be ended at the end of a scene
            if (this != null) gameObject.Dispose();
        }
    }
}
