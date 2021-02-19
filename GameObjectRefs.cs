using UnityEngine;

namespace _0G.Legacy
{
    [System.Serializable]
    public struct GameObjectRefs
    {
        public Animator Animator;

        public StateOwnerBase StateOwner;

        public GameObject GraphicGameObject;

        public GraphicController GraphicController;

        [Tooltip("The VisRect GameObject should be positioned at the visual center (e.g. character torso)." +
            " By contrast, the root GameObject should be positioned at the visual base (e.g. character soles).")]
        public VisRect VisRect;

        [Tooltip("The main bounding box for the game object.")]
        public Collider Collider;

        public Rigidbody Rigidbody;

        public Hitbox Hitbox;

        public Hurtbox Hurtbox;

        public Attacker Attacker;

        public DamageTaker DamageTaker;

        public void AutoAssign(GameObjectBody body)
        {
            // automatically assign references, but only in edit mode, not play mode

            if (G.U.IsPlayMode(body)) return;

            // assign game object references

            void AssignGameObject(ref GameObject myRef)
            {
                if (myRef == default)
                {
                    myRef = body.gameObject;
                }
            }

            AssignGameObject(ref GraphicGameObject);

            // assign Unity component references

            void AssignUnityComponent<T>(ref T myRef) where T : Component
            {
                if (myRef == default)
                {
                    myRef = body.GetComponent<T>();
                }
            }

            AssignUnityComponent(ref Animator);
            AssignUnityComponent(ref Collider);
            AssignUnityComponent(ref Rigidbody);

            // get all IBodyComponents, including inactive ones, then assign references

            IBodyComponent[] components = body.GetComponentsInChildren<IBodyComponent>(true);

            foreach (IBodyComponent c in components)
            {
                if (c.Body == null)
                {
                    c.InitBody(body);
                }

                if (c.Body != body)
                {
                    continue; // don't reference components assigned to other bodies
                }

                // assign IBodyComponent references as applicable

                void AssignIBodyComponent<T>(ref T myRef) where T : IBodyComponent
                {
                    if (myRef == null && c is T newRef)
                    {
                        myRef = newRef;
                    }
                }

                AssignIBodyComponent(ref StateOwner);
                AssignIBodyComponent(ref GraphicController);
                AssignIBodyComponent(ref VisRect);
                AssignIBodyComponent(ref Hitbox);
                AssignIBodyComponent(ref Hurtbox);
                AssignIBodyComponent(ref Attacker);
                AssignIBodyComponent(ref DamageTaker);
            }
        }
    }
}