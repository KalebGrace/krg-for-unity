using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [RequireComponent(typeof(BoxCollider))]
    public abstract class HitBox : MonoBehaviour {

        [SerializeField]
        [FormerlySerializedAs("m_damageTaker")]
        DamageTaker _damageTaker;

        public DamageTaker damageTaker { get { return _damageTaker; } }

        void Awake() {
            G.U.Assert(gameObject.layer != Layer.Default, "This GameObject must exist on a hit box Layer.");

            G.U.Require<BoxCollider>(this);

            //we always want to have a Rigidbody on this (hit box) GameObject in order to exclude its
            //BoxCollider from the parent (bounding box) GameObject's Rigidbody when doing SweepTestAll
            var rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            G.U.Require(_damageTaker, "Damage Taker");
            //this hit box should already be placed on the "VisRect" GameObject of the thing taking damage
            _damageTaker.centerTransform = transform;
        }

        void OnDrawGizmos() { //runs in edit mode, so don't rely upon actions done in Awake
            Gizmos.color = Color.cyan;
            Vector3 p = transform.position;
            KRGGizmos.DrawCrosshairXY(p, 0.25f);
            var boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null) Gizmos.DrawWireCube(p + boxCollider.center, boxCollider.size);
        }
    }
}
