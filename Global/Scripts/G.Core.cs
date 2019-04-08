using UnityEngine;

namespace KRG
{
    /// <summary>
    /// G.Core.cs is a partial class of G (G.cs).
    /// This is the core part of the G class.
    /// See G.cs for the proper class declaration and more info.
    /// </summary>
    partial class G
    {
        /// <summary>
        /// The cached config (KRGConfig).
        /// </summary>
        KRGConfig m_Config;

        /// <summary>
        /// Gets the config (KRGConfig). Runtime only. Will return null if either the G instance or the config is null.
        /// </summary>
        /// <value>The config.</value>
        public static KRGConfig config { get { return instance != null ? instance.LoadConfig() : null; } }

        /// <summary>
        /// Gets a value indicating whether Unity is in edit mode.
        /// This cannot be accessed from AppManager because that is only created at runtime.
        /// </summary>
        /// <value><c>true</c> if Unity is in edit mode; otherwise, <c>false</c>.</value>
        public static bool isInEditMode { get { return Application.isEditor && !Application.isPlaying; } }

        /// <summary>
        /// Gets the KRG version.
        /// </summary>
        /// <value>The KRG version.</value>
        public static string krgVersion { get { return AppManager.krgVersion; } }

        // TRUE MONOBEHAVIOUR METHODS

        protected override void Awake()
        {
            //singleton stuff
            base.Awake();
            //if this has already been run (e.g. new scene loaded), bail out
            if (isDuplicateInstance) return;
            //ensure the config is cached (and if not found, log an error)
            LoadConfig();
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

        // MORE KRG METHODS

        KRGConfig LoadConfig()
        {
            //cache the config if not done already
            if (m_Config == null)
            {
                m_Config = Resources.Load<KRGConfig>(KRGConfig.resourcePath);
                //if the config is still null, log an error
                if (m_Config == null)
                {
                    G.Err("Please ensure a KRGConfig is located in a Resources folder."
                    + " You may use the KRG menu to create a new one if needed.");
                }
            }
            //return the cached config
            return m_Config;
        }
    }
}
