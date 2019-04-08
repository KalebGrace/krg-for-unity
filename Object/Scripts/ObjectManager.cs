using UnityEngine;

namespace KRG
{
    //TODO: most this class should be a static helper class for KRGLoader,
    //as a lot of Singleton & KRGBehaviour stuff relies upon it.
    public class ObjectManager : Manager, ILateUpdate
    {
        public override float priority { get { return 50; } }

        //TODO: maybe just make this static in KRGBehaviour?
        event System.Action _destroyRequests;

        public override void Awake()
        {
            //instantiate KRGLoader child GameObjects from prefabs
            GameObject[] ps = config.autoInstancedPrefabs;
            for (int i = 0; i < ps.Length; i++)
            {
                G.New(ps[i], transform);
            }
        }

        public virtual void LateUpdate()
        {
            InvokeEventActions(ref _destroyRequests, true);
        }

        public void AddDestroyRequest(System.Action request)
        {
            _destroyRequests += request;
        }

        public static void InvokeEventActions(ref System.Action eventActions, bool clearEventActionsAfter = false)
        {
            if (eventActions == null)
            {
                return;
            }
            System.Delegate[] list = eventActions.GetInvocationList();
            System.Action action;
            for (int i = 0; i < list.Length; i++)
            {
                action = (System.Action)list[i];
                if (action.Target == null || !action.Target.Equals(null))
                {
                    //This is either a static method OR an instance method with a valid target.
                    action.Invoke();
                }
                else
                {
                    //This is an instance method with an invalid target, so remove it.
                    eventActions -= action;
                }
            }
            if (clearEventActionsAfter)
            {
                eventActions = null;
            }
        }

        /// <summary>
        /// Awake as the singleton instance for this class.
        /// This is necessary when a GameObject either pre-exists or is created in the scene with this Component on it.
        /// </summary>
        public static void AwakeSingletonComponent<T>(ISingletonComponent<T> instance
        ) where T : Component, ISingletonComponent<T>
        {
            System.Type t = typeof(T);
            if (instance.singletonInstance == null)
            {
                //this is the FIRST instance; set it as the singleton instance
                T first = (T)instance;
                instance.singletonInstance = first;
                first.PersistNewScene();
                G.U.Prevent(instance.isDuplicateInstance, "First instance marked as duplicate.", instance, t);
            }
            else
            {
                //this is a NEW instance
                T newIn = (T)instance;
                G.U.Prevent(instance.singletonInstance == newIn, "Singleton instance awoke twice.", instance, t);
                if (instance.isDuplicateInstance || instance.singletonType == SingletonType.SingletonFirstOnly)
                {
                    //either this was already marked as duplicate, or it we deduce it to be a duplicate,
                    //since the singleton type says only the first instance can be the singleton;
                    //mark the NEW instance as a duplicate and destroy it
                    newIn.isDuplicateInstance = true;
                    newIn.duplicateDestroyType = DestroyType.GameObjectNormal; //default
                    newIn.OnIsDuplicateInstance();
                    DestroyIn(newIn, newIn.duplicateDestroyType);
                }
                else if (instance.singletonType == SingletonType.SingletonAlwaysNew)
                {
                    //since the singleton type says to always use a new instance as the singleton...
                    //the current singleton instance is an OLD instance
                    T oldIn = instance.singletonInstance;
                    //set the NEW instance as the new singleton instance
                    instance.singletonInstance = newIn;
                    newIn.PersistNewScene();
                    //mark the OLD instance as a duplicate and destroy it
                    oldIn.isDuplicateInstance = true;
                    oldIn.duplicateDestroyType = DestroyType.GameObjectNormal; //default
                    oldIn.OnIsDuplicateInstance();
                    DestroyIn(oldIn, oldIn.duplicateDestroyType);
                }
                else
                {
                    G.U.Unsupported(instance, instance.singletonType);
                }
            }
        }

        /// <summary>
        /// Creates a game object with a component of this type.
        /// This component will be the singleton instance if the right conditions are met in AwakeSingletonComponent.
        /// </summary>
        /// <returns>The new component attached to this new game object.</returns>
        public static T CreateGameObject<T>(ISingletonComponent<T> instance) where T : Component
        {
            if (instance == null || instance.singletonType == SingletonType.SingletonAlwaysNew)
            {
                var go = new GameObject(typeof(T).ToString());
                return go.AddComponent<T>();
                //this added component will immediately Awake,
                //and AwakeSingletonComponent will be called if properly implemented
            }
            else
            {
                G.U.Error(
                    "A singleton instance already exists for {0}. Use {0}.instance to get it.", typeof(T));
                return null;
            }
        }

        public static void OnDestroySingletonComponent<T>(ISingletonComponent<T> instance) where T : Component
        {
            if (!instance.isDuplicateInstance)
            {
                instance.singletonInstance = null;
            }
        }

        /// <summary>
        /// Destroy the specified instance with the specified destroyType.
        /// This code was previously used in "G.U.Destroy(...)" as well as other places.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="destroyType">Destroy type.</param>
        static void DestroyIn(Component instance, DestroyType destroyType)
        {
            switch (destroyType)
            {
                case DestroyType.ComponentImmediate:
                    UnityEngine.Object.DestroyImmediate(instance);
                    break;
                case DestroyType.ComponentNormal:
                    UnityEngine.Object.Destroy(instance);
                    break;
                case DestroyType.GameObjectImmediate:
                    UnityEngine.Object.DestroyImmediate(instance.gameObject);
                    break;
                case DestroyType.GameObjectNormal:
                    UnityEngine.Object.Destroy(instance.gameObject);
                    break;
                case DestroyType.None:
                    G.U.Warning("Instance {0} of type {1} not destroyed. Did you mean to do this?",
                        instance.name, instance.GetType());
                    break;
                default:
                    G.U.Error("Unsupported DestroyType {0} for instance {1} of type {2}.",
                        destroyType, instance.name, instance.GetType());
                    break;
            }
        }
    }
}
