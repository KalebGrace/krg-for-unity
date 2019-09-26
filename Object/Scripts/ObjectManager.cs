using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRG
{
    public class ObjectManager : Manager, ILateUpdate, IOnDestroy
    {
        public override float priority => 50;

        public delegate void CharacterIDHandler(int id);

        public delegate void GameObjectBodyHandler(GameObjectBody body);

        public event CharacterIDHandler CharacterIDAdded;
        public event CharacterIDHandler CharacterIDRemoved;

        public event GameObjectBodyHandler PlayerCharacterAdded;
        public event GameObjectBodyHandler PlayerCharacterRemoved;

        private event GameObjectBodyHandler PlayerCharacterExists;

        // PUBLIC PROPERTIES

        public Dictionary<int, int> CharacterCounts { get; } = new Dictionary<int, int>();

        public Dictionary<int, CharacterDossier> CharacterDossiers { get; } = new Dictionary<int, CharacterDossier>();

        public GameObjectBody FirstPlayerCharacter => PlayerCharacters.Count > 0 ? PlayerCharacters[0] : null;

        public List<GameObjectBody> PlayerCharacters { get; } = new List<GameObjectBody>();

        // MONOBEHAVIOUR METHODS

        public override void Awake()
        {
            G.U.Todo("Need reset code or Reset method.");

            G.app.GameplaySceneStarted += OnGameplaySceneStarted;

            //instantiate KRGLoader child GameObjects from prefabs
            GameObject[] ps = config.autoInstancedPrefabs;
            for (int i = 0; i < ps.Length; i++)
            {
                G.U.New(ps[i], transform);
            }
        }

        public void OnDestroy()
        {
            G.app.GameplaySceneStarted -= OnGameplaySceneStarted;
        }

        // PUBLIC METHODS

        public void Register(GameObjectBody body)
        {
            switch (body.GameObjectType)
            {
                case GameObjectType.Character:
                    int id = body.CharacterID;
                    if (id == 0)
                    {
                        return;
                    }
                    if (!CharacterCounts.ContainsKey(id))
                    {
                        CharacterCounts.Add(id, 0);
                    }
                    if (++CharacterCounts[id] == 1)
                    {
                        LoadCharacterDossier(id);
                        CharacterIDAdded?.Invoke(id);
                    }
                    if (body.IsPlayerCharacter)
                    {
                        PlayerCharacters.Add(body);
                        PlayerCharacterAdded?.Invoke(body);
                        if (PlayerCharacters.Count == 1) PlayerCharacterExists?.Invoke(body);
                    }
                    G.U.Log("Registered {0} (character ID {1}).", body.name, id);
                    break;
            }
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
                    if (G.U.IsEditMode() && !CharacterCounts.ContainsKey(id))
                    {
                        G.U.Log("SPOOKED YA!");
                        return;
                    }
                    if (--CharacterCounts[id] == 0)
                    {
                        CharacterIDRemoved?.Invoke(id);
                        UnloadCharacterDossier(id);
                    }
                    G.U.Log("Deregistered {0} (character ID {1}).", body.name, id);
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
            return collider.CompareTag(CharacterTag.Player.ToString())
                && collider.gameObject.layer == Layer.PCBoundBox;
        }
        public bool IsPlayerCharacter(Collision collision)
        {
            return IsPlayerCharacter(collision.collider);
        }

        public AssetBundle LoadAssetBundle(string bundleName)
        {
            AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles()
                   .Where(ab => ab.name == bundleName)
                   .SingleOrDefault();

            if (assetBundle == null)
            {
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);

                assetBundle = AssetBundle.LoadFromFile(path);

                if (assetBundle == null)
                {
                    G.U.Err("Failed to load AssetBundle {0} at path {1}.", bundleName, path);
                }
            }

            return assetBundle;
        }

        public AssetBundle LoadCharacterAnimationAssetBundle(string animationName)
        {
            string bundleName = animationName.Replace("_RasterAnimation", "").ToLower();

            return LoadAssetBundle(bundleName);
        }

        // PRIVATE METHODS

        private void OnGameplaySceneStarted()
        {
            bool doneUnloading = false;

            foreach (var cc in CharacterCounts.OrderBy(cc => cc.Value))
            {
                if (cc.Value == 0)
                {
                    UnloadCharacterAssetPack(cc.Key);
                    _ = CharacterCounts.Remove(cc.Key);
                }
                else
                {
                    if (!doneUnloading)
                    {
                        Resources.UnloadUnusedAssets();
                        doneUnloading = true;
                    }

                    LoadCharacterAssetPack(cc.Key);
                }
            }
        }

        // CHARACTER

        private void LoadCharacterDossier(int characterID)
        {
            if (characterID == 0)
            {
                return;
            }

            string bundleName = "_c" + characterID.ToString("D5");

            AssetBundle assetBundle = LoadAssetBundle(bundleName);

            if (assetBundle == null)
            {
                G.U.Err("Failed to load AssetBundle for character ID {0}.", characterID);
                return;
            }

            CharacterDossier[] dossiers = assetBundle.LoadAllAssets<CharacterDossier>();

            if (dossiers.Length == 0)
            {
                G.U.Err("Failed to load CharacterDossiers from AssetBundle {0}.", bundleName);
                return;
            }

            CharacterDossier cd = dossiers[0];

            if (dossiers.Length > 1)
            {
                G.U.Warn("Multiple CharacterDossiers in AssetBundle {0}. Falling back to {1}.",
                    bundleName, cd.FileName);
            }

            CharacterDossiers.Add(characterID, cd);
        }

        private void UnloadCharacterDossier(int characterID)
        {
            CharacterDossiers.Remove(characterID);

            string bundleName = "_c" + characterID.ToString("D5");

            AssetBundle.GetAllLoadedAssetBundles()
                   .Where(ab => ab.name == bundleName)
                   .SingleOrDefault()?.Unload(true);
        }

        private void LoadCharacterAssetPack(int characterID)
        {
            G.U.Todo("Load all animation/etc bundles for character ID {0}.", characterID);
        }

        private void UnloadCharacterAssetPack(int characterID)
        {
            G.U.Todo("Unload all animation/etc bundles for character ID {0}.", characterID);
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
                first.PersistNewScene(PersistNewSceneType.MoveToHierarchyRoot);
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
