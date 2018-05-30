using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

	/// <summary>
	/// G.Core.cs is a partial class of G (G.cs).
	/// This is the core part of the G class.
	/// See G.cs for the proper class declaration and more info.
	/// </summary>
    partial class G {

#region private static fields

        /// <summary>
        /// The cached config (KRGConfig).
        /// </summary>
        static KRGConfig _config;

#endregion

#region public static properties

        /// <summary>
        /// Gets the config (KRGConfig). Runtime only. Will return null if either the G instance or the config is null.
        /// </summary>
        /// <value>The config.</value>
        public static KRGConfig config { get { return instance != null ? instance.GetConfig() : null; } }

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

#endregion

#region MonoBehaviour methods

        protected override void Awake() {
            //singleton stuff
            base.Awake();
            //if this has already been run (e.g. new scene loaded), bail out
            if (isDuplicateInstance) return;
            //ensure the config is set (and if not, log an error)
            GetConfig();
            //cache manager interfaces in publicly accessible properties
            SetManagerProperties();
            //awake the managers
            AwakeManagers();
            //call an app start method of your choosing (if you wish)
            StartApp();
        }

        protected override void OnDestroy() {
            //singleton stuff
            base.OnDestroy();
            //just like in Awake(), don't do anything if this is a duplicate
            if (isDuplicateInstance) return;
            //destroy the managers
            DestroyManagers();
            //unload the cached config
            if (_config != null) Resources.UnloadAsset(_config);
        }

#endregion

#region public methods

        public KRGConfig GetConfig() {
            //cache the config if not done already
            if (_config == null) {
                _config = Resources.Load<KRGConfig>(KRGConfig.resourcePath);
                //if the config is still null, log an error
                if (_config == null) {
                    G.Err("Please ensure a KRGConfig is located in a Resources folder."
                    + " You may use the KRG menu to create a new one if needed.");
                }
            }
            //return the cached config
            return _config;
        }

#endregion
        
    }
}
