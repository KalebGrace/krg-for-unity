using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if KRG_X_UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

namespace _0G.Legacy
{
    public class AppManager : Manager, IOnApplicationQuit, IOnDestroy
    {
        public override float priority => 10;

        /* example for changing priorities of different methods
         *
         * NOTE: the following is actually no longer needed,
         * since OnDestroy priority is now reversed by default
        
        float IOnDestroy.priority => priorityOnDestroy;

        protected virtual float priorityOnDestroy => 1000;

        */

        public virtual void StartApp()
        {
#if KRG_X_UNITY_ANALYTICS
            _ = AnalyticsEvent.GameStart();
#endif
        }

        /// <summary>
        /// The default master scene name.
        /// </summary>
        public const string masterSceneNameDefault = "MasterScene";

        /// <summary>
        /// The default master scene path (inside the Assets folder).
        /// </summary>
        public const string masterScenePathDefault =
            "!_" + masterSceneNameDefault + "/" + masterSceneNameDefault + ".unity";

        /// <summary>
        /// The asynchronous scene loading progress value at which scene activation becomes available.
        /// </summary>
        protected const float _activationProgress = 0.9f;

        public event System.Action GameplaySceneStarted;

        /// <summary>
        /// The name of the scene that is intended to be active.
        /// NOTE 1: This may not necessarily be active at this moment.
        /// NOTE 2: GetSceneByName can't get a Scene that isn't loaded yet, so a string is used for identification.
        /// </summary>
        protected string _activeSceneName;

        /// <summary>
        /// The scene activation events.
        /// </summary>
        protected Dictionary<string, System.Action> _sceneActivationEvents = new Dictionary<string, System.Action>();

        /// <summary>
        /// A reference to all the scene controllers for currently loaded scenes.
        /// </summary>
        protected List<SceneController> _sceneControllers = new List<SceneController>();

        // MAIN PROPERTIES

        public virtual int GameplaySceneId
        {
            get
            {
                if (System.Enum.TryParse(_activeSceneName, out EnvironmentID sn))
                {
                    return (int) sn;
                }
                G.U.Err("Active scene {0} does not exist in SceneName enum.", _activeSceneName);
                return 0;
            }
            set
            {
                if (value != 0)
                {
                    EnvironmentID sn = (EnvironmentID) value;
                    _activeSceneName = sn.ToString();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="AppManager"/>
        /// is in the Unity Editor while running a single scene (and the App State is None).
        /// </summary>
        /// <value><c>true</c> if is in single scene editor; otherwise, <c>false</c>.</value>
        public virtual bool isInSingleSceneEditor { get; protected set; }

        public virtual bool isQuitting { get; private set; }

        public virtual string masterSceneName { get { return masterSceneNameDefault; } }

        // METHODS

        public override void Awake()
        {
            isQuitting = false;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /// <summary>
        /// Adds a scene activation listener.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="listener">Listener.</param>
        public virtual void AddSceneActivationListener(string sceneName, System.Action listener)
        {
            if (_sceneActivationEvents.ContainsKey(sceneName))
            {
                _sceneActivationEvents[sceneName] += listener;
            }
            else
            {
                _sceneActivationEvents.Add(sceneName, listener);
            }
        }

        /// <summary>
        /// Adds a scene controller.
        /// </summary>
        /// <param name="sceneController">Scene controller.</param>
        public virtual void AddSceneController(SceneController sceneController)
        {
            if (!_sceneControllers.Contains(sceneController))
            {
                _sceneControllers.Add(sceneController);
            }
            else
            {
                G.U.Err("The AppManager's SceneController list already contains the {0} SceneController!",
                    sceneController.sceneName);
            }
        }

        /// <summary>
        /// Gets a scene controller.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public virtual SceneController GetSceneController(string sceneName)
        {
            for (int i = 0; i < _sceneControllers.Count; ++i)
            {
                SceneController sc = _sceneControllers[i];
                // if the scene was unloaded or the scene controller was destroyed for any reason, remove it and continue
                if (sc == null)
                {
                    _sceneControllers.RemoveAt(i--);
                    continue;
                }
                // if this is the desired scene...
                if (sceneName == sc.sceneName)
                {
                    return sc;
                }
            }
            G.U.Err("No scene controller found for the {0} scene.", sceneName);
            return null;
        }

        /// <summary>
        /// Removes a scene activation listener.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="listener">Listener.</param>
        public virtual void RemoveSceneActivationListener(string sceneName, System.Action listener)
        {
            if (_sceneActivationEvents.ContainsKey(sceneName))
            {
                _sceneActivationEvents[sceneName] -= listener;
                if (_sceneActivationEvents[sceneName] == null)
                {
                    _sceneActivationEvents.Remove(sceneName);
                }
            }
        }

        /// <summary>
        /// Quits the application. The point of no return.
        /// </summary>
        public virtual void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// This is a bit of a hack.
        /// </summary>
        public virtual void ResetGameplaySceneId()
        {
            _activeSceneName = null;
        }

        /// <summary>
        /// Calls the OnSceneActive method on the SceneController for the active scene.
        /// </summary>
        protected void CallOnSceneActive()
        {
            for (int i = 0; i < _sceneControllers.Count; ++i)
            {
                SceneController sc = _sceneControllers[i];
                // if the scene was unloaded or the scene controller was destroyed for any reason, remove it and continue
                if (sc == null)
                {
                    _sceneControllers.RemoveAt(i--);
                    continue;
                }
                string sceneName = sc.sceneName;
                // if this is the active scene...
                if (sceneName == _activeSceneName)
                {
                    SendLevelStartAnalytics(sceneName);
                    if (sc.SceneType == SceneType.Gameplay)
                    {
                        InvokeGameplaySceneStarted();
                    }
                    // call OnSceneActive
                    sc.OnSceneActive();
                    // call events
                    if (_sceneActivationEvents.ContainsKey(sceneName))
                    {
                        _sceneActivationEvents[sceneName]?.Invoke();
                    }
                    return;
                }
            }
            G.U.Err("No scene controller found for the {0} scene.", _activeSceneName);
        }

        /// <summary>
        /// Loads the scene (additively and asynchronously).
        /// </summary>
        /// <returns>The asynchronous loading operation.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="makeActive">If set to <c>true</c> make the scene active.</param>
        public AsyncOperation LoadScene(string sceneName, bool makeActive = true)
        {
            if (makeActive) _activeSceneName = sceneName;
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Method that is called when a scene is loaded.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="mode">Mode.</param>
        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_activeSceneName == scene.name)
            {
                //the scene we intended to be active has been loaded; let's officially set it as active
                G.U.Assert(SceneManager.SetActiveScene(scene));
                CallOnSceneActive();
            }
        }

        /// <summary>
        /// Unloads the scene (asynchronously).
        /// </summary>
        /// <returns>The asynchronous unloading operation.</returns>
        /// <param name="sceneName">Scene name.</param>
        public AsyncOperation UnloadScene(string sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// Method that is called when a scene is unloaded.
        /// </summary>
        /// <param name="scene">Scene.</param>
        protected void OnSceneUnloaded(Scene scene)
        {
            if (_activeSceneName == scene.name)
            {
                //our active scene has unloaded; let's revert to the master scene for now
                _activeSceneName = null;
                G.U.Assert(SceneManager.SetActiveScene(SceneManager.GetSceneByName(masterSceneName)));
            }
        }

        protected virtual void InvokeGameplaySceneStarted()
        {
            GameplaySceneStarted?.Invoke();
        }

        protected virtual void SendLevelStartAnalytics(string sceneName)
        {
#if KRG_X_UNITY_ANALYTICS
            _ = AnalyticsEvent.LevelStart(sceneName);
#endif
        }
    }
}