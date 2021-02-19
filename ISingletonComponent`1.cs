using UnityEngine;

namespace _0G.Legacy
{
    public interface ISingletonComponent<T> where T : Component
    {
        #region Add To Class

        /*
        static event System.Action _instantiateHandlers;

        /// <summary>
        /// The singleton instance. This property will not use lazy initialization to create a singleton instance,
        /// and thus it will return null if it has not yet been created, or it has been destroyed.
        /// Use AddInstantiateHandler to add a callback to be made when the instance is created.
        /// </summary>
        public static MyType instance { get; private set; }

        MyType ISingletonComponent<MyType>.singletonInstance {
            get { return instance; } set { instance = value; } }

        SingletonType ISingletonComponent<MyType>.singletonType {
            get { return SingletonType.SingletonFirstOnly; } }

        DestroyType ISingletonComponent<MyType>.duplicateDestroyType { get; set; }

        bool ISingletonComponent<MyType>.isDuplicateInstance { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a duplicate instance.
        /// </summary>
        /// <value><c>true</c> if it is a duplicate instance; <c>false</c> if it is the singleton instance.</value>
        bool isDuplicateInstance {
            get { return ((ISingletonComponent<MyType>)this).isDuplicateInstance; } }

        void ISingletonComponent<MyType>.OnIsDuplicateInstance() { }

        public static void AddInstantiateHandler(System.Action handler) { _instantiateHandlers += handler;
            if (instance != null) handler(); }

        public static void RemoveInstantiateHandler(System.Action handler) { _instantiateHandlers -= handler; }

        void Awake() { ObjectManager.AwakeSingletonComponent(this); if (!isDuplicateInstance) {
            BaseCallAndOrOtherCode(); ObjectManager.InvokeEventActions(ref _instantiateHandlers); } }

        void OnDestroy() { if (!isDuplicateInstance) {
            BaseCallAndOrOtherCode(); } ObjectManager.OnDestroySingletonComponent(this); }
         */

        #endregion

        #region Back With Static Property

        T singletonInstance { get; set; }

        #endregion

        #region Back With Constant Value

        SingletonType singletonType { get; }

        #endregion

        #region Back With Member Properties

        /// <summary>
        /// Gets or sets the DestroyType of the duplicate instance.
        /// </summary>
        /// <value>The DestroyType of the duplicate instance.</value>
        DestroyType duplicateDestroyType { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a duplicate instance.
        /// </summary>
        /// <value><c>true</c> if it is a duplicate instance; <c>false</c> if it is the singleton instance.</value>
        bool isDuplicateInstance { get; set; }

        #endregion

        #region Back With Member Method

        /// <summary>
        /// Raises the event for when this is a duplicate instance.
        /// In this method, data can be transferred and duplicateDestroyType can be changed as needed.
        /// This duplicate instance will be destroyed immediately afterwards.
        /// NOTE: singletonType can be used to determine if the singleton is the first instance or the newest instance.
        /// </summary>
        void OnIsDuplicateInstance();

        #endregion
    }
}