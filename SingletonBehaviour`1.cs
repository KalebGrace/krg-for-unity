using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// Singleton MonoBehaviour base class. To be concretely derived exactly one level.
    /// Otherwise, consider implementing ISingletonComponent<T>.
    /// 
    /// To avoid race conditions, only built-in Unity methods
    /// and ObjectManager static methods will be used in this base class.
    /// NOTE: Using ObjectManager's static methods will not trigger
    /// Unity to instantiate it (as such, Awake won't be called).
    /// </summary>
    public abstract class SingletonBehaviour<T> : MonoBehaviour, ISingletonComponent<T>
        where T : SingletonBehaviour<T>
        {
            #region static properties & methods

            /// <summary>
            /// The singleton instance.
            /// This property will not use lazy initialization to create a singleton instance,
            /// and thus it will return null if it has not yet been created, or it has been destroyed.
            /// Use instanceLazy if lazy initialization is preferred.
            /// </summary>
            public static T instance { get; private set; }

            /// <summary>
            /// Gets the singleton instance, assuming it has not been destroyed.
            /// If the singleton instance has not yet been created, this will use lazy initialization to create it first.
            /// If the singleton instance has been destroyed, errors will be logged and a new instance will be returned.
            /// </summary>
            public static T instanceLazy { get { return instance ?? CreateGameObject(); } }

            /// <summary>
            /// Creates a game object with a component of this type.
            /// This component will be the singleton instance if singletonType is not None.
            /// </summary>
            /// <returns>The new component attached to this new game object.</returns>
            public static T CreateGameObject()
            {
                return ObjectManager.CreateGameObject(instance);
            }

            #endregion

            #region ISingletonComponent<T> implementation

            T ISingletonComponent<T>.singletonInstance { get { return instance; } set { instance = value; } }

            DestroyType ISingletonComponent<T>.duplicateDestroyType { get; set; }

            bool ISingletonComponent<T>.isDuplicateInstance { get; set; }

            void ISingletonComponent<T>.OnIsDuplicateInstance()
            {
                OnIsDuplicateInstance();
            }

            #endregion

            #region instance properties & methods

            /// <summary>
            /// Gets the type of the singleton used by the derived class. (Can be overriden by said class.)
            /// </summary>
            public virtual SingletonType singletonType { get { return SingletonType.SingletonFirstOnly; } }

            /// <summary>
            /// Gets a value indicating whether this <see cref="SingletonBehaviour`1"/> is a duplicate instance.
            /// </summary>
            /// <value><c>true</c> if it is a duplicate instance; <c>false</c> if it is the singleton instance.</value>
            protected bool isDuplicateInstance { get { return ((ISingletonComponent<T>) this).isDuplicateInstance; } }

            protected virtual void Awake()
            {
                ObjectManager.AwakeSingletonComponent(this);
            }

            protected virtual void OnDestroy()
            {
                ObjectManager.OnDestroySingletonComponent(this);
            }

            protected virtual void OnIsDuplicateInstance() { }

            #endregion

        }
}