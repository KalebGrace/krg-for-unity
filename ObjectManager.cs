using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _0G.Legacy
{
    public class ObjectManager : Manager, ILateUpdate, IOnDestroy
    {
        public override float priority => 50;

        public delegate void CharacterIDHandler(int characterID);
        public delegate void EnvironmentIDHandler(int environmentID);
        public delegate void GameObjectBodyHandler(GameObjectBody body);

        public event CharacterIDHandler CharacterIDAdded;
        public event CharacterIDHandler CharacterIDRemoved;
        public event EnvironmentIDHandler EnvironmentIDAdded;
        public event EnvironmentIDHandler EnvironmentIDRemoved;
        public event GameObjectBodyHandler PlayerCharacterAdded;
        public event GameObjectBodyHandler PlayerCharacterRemoved;

        private event GameObjectBodyHandler PlayerCharacterExists;

        // PUBLIC PROPERTIES

#if KRG_BUNDLE_VARIANT_HD
        public string AssetBundleVariant => "hd";
#else
        public string AssetBundleVariant => "sd";
#endif

        public Dictionary<int, int> CharacterCounts { get; } = new Dictionary<int, int>();

        public Dictionary<int, CharacterDossier> CharacterDossiers { get; } = new Dictionary<int, CharacterDossier>();
        public Dictionary<int, EnvironmentChart> EnvironmentCharts { get; } = new Dictionary<int, EnvironmentChart>();

        public GameObjectBody FirstPlayerCharacter => PlayerCharacters.Count > 0 ? PlayerCharacters[0] : null;

        public List<GameObjectBody> PlayerCharacters { get; } = new List<GameObjectBody>();

        public Dictionary<string, RasterAnimation> RasterAnimations { get; } = new Dictionary<string, RasterAnimation>();

        // MONOBEHAVIOUR METHODS

        public override void Awake()
        {
            // eject any editor bundles/assets from memory
            AssetBundle.UnloadAllAssetBundles(true);

            var anims = config.ExtraRasterAnimations;
            for (int i = 0; i < anims.Count; ++i)
            {
                var anim = anims[i];
                RasterAnimations.Add(anim.name, anim);
            }

            G.app.GameplaySceneStarted += OnGameplaySceneStarted;

            // instantiate KRGLoader child GameObjects from prefabs
            GameObject[] ps = config.autoInstancedPrefabs;
            for (int i = 0; i < ps.Length; ++i)
            {
                G.U.New(ps[i], transform);
            }
        }

        public void OnDestroy()
        {
            G.app.GameplaySceneStarted -= OnGameplaySceneStarted;
        }

        // EVENT HANDLER METHODS

        private void OnGameplaySceneStarted()
        {
            EnvironmentChart ec;
            bool doneUnloadingUnusedAssets = false;

            // unload all environment assets
            while (EnvironmentCharts.Count > 0)
            {
                int id = EnvironmentCharts.Keys.First();
                UnloadAssetPack<EnvironmentChart>(id);
                UnloadDocket<EnvironmentChart>(id);
            }

            foreach (var cc in CharacterCounts.OrderBy(cc => cc.Value))
            {
                if (cc.Value == 0)
                {
                    UnloadAssetPack<CharacterDossier>(cc.Key);
                    UnloadDocket<CharacterDossier>(cc.Key);
                    _ = CharacterCounts.Remove(cc.Key);
                }
                else
                {
                    if (!doneUnloadingUnusedAssets)
                    {
                        Resources.UnloadUnusedAssets();
                        doneUnloadingUnusedAssets = true;
                    }

                    LoadAssetPack<CharacterDossier>(cc.Key);
                }
            }

            // load environment assets
            int environmentID = G.env.CurrentEnvironmentID;
            ec = LoadDocket<EnvironmentChart>(environmentID);
            if (ec != null) LoadAssetPack<EnvironmentChart>(environmentID);
        }

        // MAIN PUBLIC METHODS

        public bool Register(GameObjectBody body)
        {
            switch (body.GameObjectType)
            {
                case GameObjectType.Character:
                    int id = body.CharacterID;
                    if (id == 0)
                    {
                        return false;
                    }
                    LoadCharacterDossier(body);
                    if (G.U.IsPlayMode(body) && G.config.IsSinglePlayerGame && body.IsPlayerCharacter && PlayerCharacters.Count > 0)
                    {
                        // a player character is already registered, so this is a duplicate
                        body.Dispose();
                        return false;
                    }
                    if (!CharacterCounts.ContainsKey(id))
                    {
                        CharacterCounts.Add(id, 0);
                    }
                    if (++CharacterCounts[id] == 1)
                    {
                        CharacterIDAdded?.Invoke(id);
                    }
                    if (body.IsPlayerCharacter)
                    {
                        PlayerCharacters.Add(body);
                        PlayerCharacterAdded?.Invoke(body);
                        if (PlayerCharacters.Count == 1) PlayerCharacterExists?.Invoke(body);
                    }
                    break;
            }
            return true;
        }

        public void Deregister(GameObjectBody body)
        {
            switch (body.GameObjectType)
            {
                case GameObjectType.Character:
                    int id = body.CharacterID;
                    if (id == 0)
                    {
                        return;
                    }
                    if (body.IsPlayerCharacter)
                    {
                        if (PlayerCharacters.Count == 1) PlayerCharacterExists?.Invoke(null);
                        PlayerCharacterRemoved?.Invoke(body);
                        PlayerCharacters.Remove(body);
                    }
                    if (G.U.IsEditMode() && !CharacterCounts.ContainsKey(id)) // verified
                    {
                        return;
                    }
                    if (--CharacterCounts[id] == 0)
                    {
                        CharacterIDRemoved?.Invoke(id);
                    }
                    // never unload the dossier until OnGameplaySceneStarted()
                    break;
            }
        }

        /// <summary>
        /// This is used to handle code where a player character must exist. Specifically, the FirstPlayerCharacter.
        /// Unlike with events, this method will call the handler automatically if a player character was already added.
        /// </summary>
        /// <param name="handler">The method to be called when the PC exists (GameObjectBody) or dies (null).</param>
        public void AddPlayerCharacterExistsHandler(GameObjectBodyHandler handler)
        {
            PlayerCharacterExists += handler;
            GameObjectBody pc = FirstPlayerCharacter;
            if (pc != null) handler(pc);
        }

        /// <summary>
        /// This removes the handler added in AddPlayerCharacterExistsHandler.
        /// </summary>
        /// <param name="handler">The method to be called when the PC exists (GameObjectBody) or dies (null).</param>
        public void RemovePlayerCharacterExistsHandler(GameObjectBodyHandler handler)
        {
            PlayerCharacterExists -= handler;
        }

        public GameObjectBody GetBody(Collider collider)
        {
            GameObjectBody body;
            body = collider.GetComponent<GameObjectBody>();
            if (body != null) return body;
            G.U.Todo("Body is null. Perform additional checks to get body.");
            return null;
        }
        public GameObjectBody GetBody(Collision collision)
        {
            return GetBody(collision.collider);
        }

        public bool IsPlayerCharacter(Collider collider)
        {
            return collider.CompareTag(CharacterTag.Player.ToString()) &&
                collider.gameObject.layer == Layer.PCBoundBox;
        }
        public bool IsPlayerCharacter(Collision collision)
        {
            return IsPlayerCharacter(collision.collider);
        }

        // ASSET BUNDLE METHODS

        private AssetBundle GetLoadedAssetBundle(string bundleName) =>
            AssetBundle.GetAllLoadedAssetBundles()
            .Where(ab => ab.name == bundleName)
            .SingleOrDefault();

        private AssetBundle LoadAssetBundle(string bundleName)
        {
            if (!string.IsNullOrWhiteSpace(AssetBundleVariant))
            {
                bundleName += "." + AssetBundleVariant;
            }

            AssetBundle assetBundle = GetLoadedAssetBundle(bundleName);

            if (assetBundle == null)
            {
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);

                if (!System.IO.File.Exists(path))
                {
                    G.U.Warn("AssetBundle {0} does not exist at path {1}.", bundleName, path);
                    return null;
                }

                assetBundle = AssetBundle.LoadFromFile(path);

                if (assetBundle == null)
                {
                    G.U.Err("Failed to load AssetBundle {0} at path {1}.", bundleName, path);
                }
            }

            return assetBundle;
        }

        private void UnloadAssetBundle(string bundleName)
        {
            AssetBundle assetBundle = GetLoadedAssetBundle(bundleName);

            if (assetBundle != null) assetBundle.Unload(true);
        }

        // DOCKET & ASSET PACK METHODS

        private T LoadDocket<T>(int id) where T : Docket
        {
            if (id == 0) return null;

            Docket dk;
            string bundleName;
            bool isCharacter = typeof(T) == typeof(CharacterDossier);

            // look in dictionary for existing docket; remove stale dockets
            if (isCharacter)
            {
                if (CharacterDossiers.ContainsKey(id))
                {
                    dk = CharacterDossiers[id];
                    if (dk != null) return (T)dk;
                    CharacterDossiers.Remove(id);
                }
                bundleName = CharacterDossier.GetBundleName(id);
            }
            else
            {
                if (EnvironmentCharts.ContainsKey(id))
                {
                    dk = EnvironmentCharts[id];
                    if (dk != null) return (T)dk;
                    EnvironmentCharts.Remove(id);
                }
                bundleName = EnvironmentChart.GetBundleName(id);
            }

            // load asset bundle
            AssetBundle assetBundle = LoadAssetBundle(bundleName);
            if (assetBundle == null)
            {
                G.U.Err("Failed to load AssetBundle for {0}ID {1}.", isCharacter ? "Character" : "Environment", id);
                return null;
            }

            // load docket(s)
            T[] dockets = assetBundle.LoadAllAssets<T>();
            if (dockets.Length == 0)
            {
                G.U.Err("Failed to load {0} dockets from AssetBundle {1}.", typeof(T).ToString(), bundleName);
                return null;
            }

            // there shall only be one
            dk = dockets[0];
            if (dockets.Length > 1)
            {
                G.U.Warn("Multiple {0} dockets in AssetBundle {1}. Falling back to {2}.",
                    typeof(T).ToString(), bundleName, dk.FileName);
            }

            // add to dictionary of loaded dockets; invoke event as applicable
            if (isCharacter)
            {
                CharacterDossier cd = (CharacterDossier)dk;
                CharacterDossiers.Add(id, cd);

                // we always need the Idle animation
                string idleAnimName = cd.GraphicData.IdleAnimationName;
                if (!string.IsNullOrWhiteSpace(idleAnimName))
                {
                    AddAnimation(assetBundle, idleAnimName);
                }

                // CharacterIDAdded is invoked in Register
            }
            else
            {
                EnvironmentChart ec = (EnvironmentChart)dk;
                EnvironmentCharts.Add(id, ec);
                EnvironmentIDAdded?.Invoke(id);
            }

            return (T)dk;
        }

        private void UnloadDocket<T>(int id) where T : Docket
        {
            if (id == 0) return;

            Docket dk;
            string bundleName;
            bool isCharacter = typeof(T) == typeof(CharacterDossier);

            // invoke event as applicable; look in dictionary for existing docket
            if (isCharacter)
            {
                // CharacterIDRemoved is invoked in Deregister
                dk = CharacterDossiers[id];
            }
            else
            {
                EnvironmentIDRemoved?.Invoke(id);
                dk = EnvironmentCharts[id];
            }

            // remove all remaining raster animations for this docket
            // NOTE: this includes default animations such as Idle
            string keyPrefix = dk.FileName + "_";
            List<string> keysToRemove = RasterAnimations
                .Where(pair => pair.Key.StartsWith(keyPrefix, System.StringComparison.Ordinal))
                .Select(pair => pair.Key)
                .ToList();
            for (int i = 0; i < keysToRemove.Count; ++i)
            {
                RasterAnimations.Remove(keysToRemove[i]);
            }

            // remove docket from dictionary; unload its bundle
            if (isCharacter)
            {
                CharacterDossiers.Remove(id);
                bundleName = CharacterDossier.GetBundleName(id);
            }
            else
            {
                EnvironmentCharts.Remove(id);
                bundleName = EnvironmentChart.GetBundleName(id);
            }
            UnloadAssetBundle(bundleName);
        }

        private void LoadAssetPack<T>(int id) where T : Docket
        {
            if (id == 0) return;

            bool isCharacter = typeof(T) == typeof(CharacterDossier);
            Docket dk = isCharacter ? (Docket)CharacterDossiers[id] : (Docket)EnvironmentCharts[id];

            // load the asset pack asset bundle
            AssetBundle ab = LoadAssetBundle(dk.AssetPackBundleName);

            if (ab == null) return;

            // add all of its animations to the dictionary of loaded animations
            if (isCharacter)
            {
                foreach (StateAnimation sa in ((CharacterDossier)dk).GraphicData.StateAnimations)
                {
                    AddAnimation(ab, sa.animationName);
                }
            }
            else
            {
                foreach (string animationName in ((EnvironmentChart)dk).AnimationNames)
                {
                    AddAnimation(ab, animationName);
                }
            }
        }

        private void UnloadAssetPack<T>(int id) where T : Docket
        {
            if (id == 0) return;

            bool isCharacter = typeof(T) == typeof(CharacterDossier);
            Docket dk = isCharacter ? (Docket)CharacterDossiers[id] : (Docket)EnvironmentCharts[id];

            // remove all remaining raster animations for this asset pack
            if (isCharacter)
            {
                foreach (StateAnimation sa in ((CharacterDossier)dk).GraphicData.StateAnimations)
                {
                    RasterAnimations.Remove(sa.animationName);
                }
            }
            else
            {
                foreach (string animationName in ((EnvironmentChart)dk).AnimationNames)
                {
                    RasterAnimations.Remove(animationName);
                }
            }

            // unload the asset pack asset bundle
            UnloadAssetBundle(dk.AssetPackBundleName);
        }

        // CHARACTER & ENVIRONMENT METHODS

        public CharacterDossier LoadCharacterDossier(int characterID) => LoadDocket<CharacterDossier>(characterID);
        public void UnloadCharacterDossier(int characterID) => UnloadDocket<CharacterDossier>(characterID);
        public EnvironmentChart LoadEnvironmentChart(int environmentID) => LoadDocket<EnvironmentChart>(environmentID);
        public void UnloadEnvironmentChart(int environmentID) => UnloadDocket<EnvironmentChart>(environmentID);

        private void LoadCharacterDossier(GameObjectBody body)
        {
            CharacterDossier cd = LoadDocket<CharacterDossier>(body.CharacterID);

            if (cd != null)
            {
                body.CharacterDossier = cd;
            }
        }

        /// <summary>
        /// LoadCharacterAssetPack will be called automatically for each
        /// character in a gameplay scene when the scene is started.
        /// Call this manually ONLY in the case where
        /// you need to spawn a character dynamically.
        /// </summary>
        public void LoadCharacterAssetPack(int characterID) => LoadAssetPack<CharacterDossier>(characterID);
        public void UnloadCharacterAssetPack(int characterID) => UnloadAssetPack<CharacterDossier>(characterID);
        public void LoadEnvironmentAssetPack(int environmentID) => LoadAssetPack<EnvironmentChart>(environmentID);
        public void UnloadEnvironmentAssetPack(int environmentID) => UnloadAssetPack<EnvironmentChart>(environmentID);

        // ANIMATION METHODS

        /// <summary>
        /// Use this only if you need a default EDITOR animation other than Idle.
        /// The animation must be in the same AssetBundle as the CharacterDossier.
        /// </summary>
        public void AddDefaultAnimation(int characterID, string animationName)
        {
            string bundleName = CharacterDossier.GetBundleName(characterID);

            AssetBundle assetBundle = LoadAssetBundle(bundleName);

            AddAnimation(assetBundle, animationName);
        }

        public void AddExtraAnimation(CharacterDossier characterDossier, string animationName)
        {
            string bundleName = characterDossier.AssetPackBundleName;

            AssetBundle assetBundle = LoadAssetBundle(bundleName);

            AddAnimation(assetBundle, animationName);
        }

        private void AddAnimation(AssetBundle assetBundle, string animationName)
        {
            RasterAnimation ra;

            // look in dictionary for existing animation; remove stale animations
            if (RasterAnimations.ContainsKey(animationName))
            {
                ra = RasterAnimations[animationName];
                if (ra != null) return;
                RasterAnimations.Remove(animationName);
            }

            // load animation and add to dictionary of loaded animations
            ra = assetBundle.LoadAsset<RasterAnimation>(animationName);
            RasterAnimations.Add(animationName, ra);
        }

        public void RemoveAnimation(string animationName)
        {
            if (RasterAnimations.ContainsKey(animationName))
            {
                RasterAnimations.Remove(animationName);
            }
        }

        // OLD SHIZ

        private event System.Action _destroyRequests;

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
                action = (System.Action) list[i];
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
        public static void AwakeSingletonComponent<T>(ISingletonComponent<T> instance) where T : Component, ISingletonComponent<T>
        {
            System.Type t = typeof(T);
            if (instance.singletonInstance == null)
            {
                //this is the FIRST instance; set it as the singleton instance
                T first = (T) instance;
                instance.singletonInstance = first;
                first.PersistNewScene(PersistNewSceneType.MoveToHierarchyRoot);
                G.U.Prevent(instance.isDuplicateInstance, "First instance marked as duplicate.", instance, t);
            }
            else
            {
                //this is a NEW instance
                T newIn = (T) instance;
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
                    newIn.PersistNewScene(PersistNewSceneType.MoveToHierarchyRoot);
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
                G.U.Err(
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
                    G.U.Warn("Instance {0} of type {1} not destroyed. Did you mean to do this?",
                        instance.name, instance.GetType());
                    break;
                default:
                    G.U.Err("Unsupported DestroyType {0} for instance {1} of type {2}.",
                        destroyType, instance.name, instance.GetType());
                    break;
            }
        }
    }
}