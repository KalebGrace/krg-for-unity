namespace _0G.Legacy
{
    public interface IMonoBehaviour
    {
        /// <summary>
        /// Awake this instance. Called at the beginning, whether enabled or disabled.
        /// </summary>
        void Awake();

        /// <summary>
        /// Start this instance. Called after Awake, but only if enabled.
        /// </summary>
        void Start();

        /// <summary>
        /// Update this instance. Called at intervals defined by the time thread, but only if enabled.
        /// </summary>
        void Update();

        /// <summary>
        /// Raises the destroy event. Called at the extinction, whether enabled or disabled.
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// Gets a value indicating whether this <see cref="IMonoBehaviour"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool enabled { get; }
    }
}