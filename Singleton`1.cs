namespace _0G.Legacy
{
    /// <summary>
    /// Singleton base class using a variation of the Static Initialization implementation strategy.
    /// https://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        /// <summary>
        /// The instance.
        /// </summary>
        static readonly T _instance = new T();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T instance { get { return _instance; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton`1"/> class.
        /// </summary>
        protected Singleton()
        {
            if (_instance != null)
            {
                string s = string.Format(
                    "An instance of {0} already exists at {0}.instance. " +
                    "That's what \"Singleton\" means. You can't create another.",
                    typeof(T));
                throw new System.Exception(s);
            }
        }
    }
}