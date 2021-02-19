using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// G.Core.cs is a partial class of G (G.cs).
    /// This is the core part of the G class.
    /// See G.cs for the proper class declaration and more info.
    /// </summary>
    partial class G
    {
        /// <summary>
        /// The version of G.
        /// </summary>
        public const int version = 40;

        /// <summary>
        /// Fires during G's Awake method.
        /// </summary>
        private static event System.Action Awoken;

        /// <summary>
        /// The cached config (KRGConfig).
        /// </summary>
        private static KRGConfig m_Config;

        /// <summary>
        /// Is G Awake?
        /// </summary>
        private static bool m_IsAwake;

        /// <summary>
        /// Gets the config (KRGConfig).
        /// </summary>
        /// <value>The config.</value>
        public static KRGConfig config => LoadConfig();

        // TRUE MONOBEHAVIOUR METHODS

        protected override void Awake()
        {
            //singleton stuff
            base.Awake();
            //if this has already been run (e.g. new scene loaded), bail out
            if (isDuplicateInstance) return;
            //ensure the config is cached (and if not found, log an error)
            LoadConfig();
            //get woke
            m_IsAwake = true;
            Awoken?.Invoke();
            Awoken = null;
            //initialize and awaken the managers
            InitManagers();
        }

        protected override void OnDestroy()
        {
            //singleton stuff
            base.OnDestroy();
            //just like in Awake(), don't do anything if this is a duplicate
            if (isDuplicateInstance) return;
            //destroy the managers
            DestroyManagers();
            //unload the cached config
            if (m_Config != null) Resources.UnloadAsset(m_Config);
        }

        // _0G.Legacy METHODS

        private static KRGConfig LoadConfig()
        {
            //cache the config if not done already
            if (m_Config == null)
            {
                m_Config = Resources.Load<KRGConfig>(KRGConfig.RESOURCE_PATH);
                //if the config is still null, log an error
                if (m_Config == null)
                {
                    G.U.Err("Please ensure a KRGConfig is located in a Resources folder." +
                        " You may use the 0G > Legacy menu to create a new one if needed.");
                }
            }
            //return the cached config
            return m_Config;
        }

        public static void DoPlayerPrefsAction(System.Action action)
        {
            if (m_IsAwake)
            {
                action();
            }
            else
            {
                Awoken += action;
            }
        }
    }
}