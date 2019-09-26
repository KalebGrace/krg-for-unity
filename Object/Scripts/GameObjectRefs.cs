using UnityEngine;

namespace KRG
{
    [System.Serializable]
    public struct GameObjectRefs
    {
        public Animator Animator;

        public GameObject GraphicGameObject;

        public GraphicController GraphicController;

        [Tooltip("The VisRect GameObject should be positioned at the visual center of the character."
            + " By contrast, the root GameObject should be positioned under the character's feet.")]
        public VisRect VisRect;

        public Collider Collider;

        public Rigidbody Rigidbody;

        public Collider Hitbox;

        public Collider Hurtbox;

        public Attacker Attacker;

        public DamageTaker DamageTaker;
    }
}
