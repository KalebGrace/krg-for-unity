using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [System.Obsolete]
    public abstract class DestructibleObject<TPart> : MonoBehaviour, IExplodable where TPart : IExplodable {

        [SerializeField]
        [FormerlySerializedAs("m_partData")]
        protected DestructibleObjectData _partData = default;
        [SerializeField]
        Collider _externalCollider = default;
        [SerializeField]
        Renderer _externalRenderer = default;

        Vector3 _explosionPosition;

#region IExplodable implementation

        public virtual void Explode(Vector3 explosionPosition) {
            _explosionPosition = explosionPosition;
            gameObject.Dispose();
        }

#endregion

        void OnDestroy() {
            //spawn object(s) on destroy (optional), but only if app isn't quitting 
            if (!G.app.isQuitting) {
                SpawnOnDestroy();
            }

            //disable collider/renderer as applicable
            var col = _externalCollider ?? GetComponent<Collider>();
            var ren = _externalRenderer ?? GetComponent<Renderer>();
            if (col != null) col.enabled = false;
            if (ren != null) ren.enabled = false;

            //explode all parts (and set data as applicable), but only if app isn't quitting 
            if (!G.app.isQuitting) {
                TPart[] parts = GetComponentsInChildren<TPart>();
                TPart aPart;
                IDestructibleObjectData idod;
                for (int i = 0; i < parts.Length; i++) {
                    aPart = parts[i];
                    idod = (IDestructibleObjectData)aPart;
                    if (idod != null) idod.data = _partData;
                    aPart.Explode(_explosionPosition);
                }
            }
        }

        /// <summary>
        /// Override this method if you wish to spawn object(s) on destroy.
        /// This will automatically be skipped when the app is quitting.
        /// </summary>
        protected virtual void SpawnOnDestroy() {
        }
    }
}
