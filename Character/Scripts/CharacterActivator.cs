using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Character activator. Intended to be placed on a child GameObject of a Character GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CharacterActivator : MonoBehaviour {

#region constants

        const string _activatorTag = "Player";

#endregion

#region fields

        Character _character;
        Collider _collider;

#endregion

#region MonoBehaviour methods

        void Awake() {
            _collider = G.U.Require<Collider>(this);
            G.U.Assert(_collider.isTrigger);
            _character = G.U.Require<Character>(transform.parent);
            Activate(false);
        }

        void OnTriggerEnter(Collider other) {
            if (other.tag == _activatorTag) Activate(true);
        }

        void OnTriggerExit(Collider other) {
            if (other.tag == _activatorTag) Activate(false);
        }

#endregion

#region private methods

        void Activate(bool value) {
            _character.enabled = value;
            //currently there is an issue with the collider affecting AI
            //so we are simply going to destroy this GameObject after activation
            //TODO: find a proper solution; maybe use layers and the collision matrix?
            if (value) G.End(gameObject);
        }

#endregion

    }
}
