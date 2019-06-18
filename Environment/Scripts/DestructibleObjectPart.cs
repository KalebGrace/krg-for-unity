using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public class DestructibleObjectPart : MonoBehaviour, IDestructibleObjectData, IEnd, IExplodable {

        [SerializeField]
        [FormerlySerializedAs("m_data")]
        protected DestructibleObjectData _data;

        Collider _collider;
        Renderer _renderer;
        Rigidbody _rigidbody;

#region IDestructibleObjectData implementation

        DestructibleObjectData IDestructibleObjectData.data {
            get { return _data; }
            set { _data = value; }
        }

#endregion



        void Awake() {
            _collider = G.U.Require<Collider>(this);
            _renderer = G.U.Require<Renderer>(this);
            _rigidbody = G.U.Require<Rigidbody>(this);

            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _renderer.enabled = false;
        }

        void FixedUpdate() {
        //TODO: remove this code when all bugs are fixed
            if (float.IsNaN(transform.position.x) ||
                float.IsNaN(transform.position.y) ||
                float.IsNaN(transform.position.z) ||
                float.IsInfinity(transform.position.x) ||
                float.IsInfinity(transform.position.y) ||
                float.IsInfinity(transform.position.z)) {
                G.U.Log("Invalid position for DestructibleObjectPart.");
                Destroy(gameObject);
            }
        }



        End my_end = new End();

        public End end { get { return my_end; } }

        void OnDestroy()
        {
            my_end.Invoke();
        }



        public void Explode(Vector3 explosionPosition) {
            G.U.Require(_data, "Data");

            _rigidbody.isKinematic = false;
            _collider.sharedMaterial = _data.physicMaterial;
            _collider.enabled = true;
            _renderer.enabled = true;

            _rigidbody.AddExplosionForce(_data.explosionForce, explosionPosition, _data.explosionRadius);

#if NS_DG_TWEENING
            if (_data.doesFade) {
                _renderer.material.DOColor(new Color(0, 0, 0, 0), _data.lifetime);
            }
#endif

            //unparent from DestructibleObject, as that will be disposed after this method ends
            transform.parent = transform.parent.parent;

            //dispose of me later
            _data.timeThread.AddTrigger(_data.lifetime, Dispose);
        }

        void Dispose(TimeTrigger tt) {
            //this may already be ended at the end of a scene
            if (!my_end.wasInvoked) G.U.End(gameObject);
        }
    }
}
