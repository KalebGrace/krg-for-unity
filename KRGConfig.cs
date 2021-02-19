using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public sealed partial class KRGConfig : ScriptableObject
    {
        //If you get an error stating `_0G.Legacy.KRGConfig' does not contain a definition for `RESOURCE_PATH',
        //create a KRGConfig.MyGame.cs file containing a partial class KRGConfig with the following constants in it:

#if !KRG_CUSTOM_G
        public const string ASSET_PATH = "Assets/Resources/KRGConfig.asset";
        public const string RESOURCE_PATH = "KRGConfig";
#endif

        [Header("Global (0G Legacy)")]

        [SerializeField]
        string _applicationNamespace = "MyGame";

        public string ApplicationNamespace => _applicationNamespace;

        [Header("Audio (0G Legacy)")]

        [SerializeField]
        float _musicVolumeScale = 1;

        public float MusicVolumeScale => _musicVolumeScale;

        [SerializeField]
        float _sfxVolumeScale = 1;

        public float SFXVolumeScale => _sfxVolumeScale;

        [Header("Damage (0G Legacy)")]

        [SerializeField]
        DamageValue _damageValuePrefab = default;
        [SerializeField]
        HPBar _hpBarPrefab = default;

        public DamageValue damageValuePrefab { get { return _damageValuePrefab; } }

        public HPBar hpBarPrefab { get { return _hpBarPrefab; } }

        [Header("DOTween (0G Legacy)")]

        [SerializeField]
        bool _doTweenUseInitSettings = default;
        [SerializeField]
        bool _doTweenRecycleAllByDefault = default;
        [SerializeField]
        bool _doTweenUseSafeMode = default;
#if NS_DG_TWEENING
        [SerializeField]
        LogBehaviour _doTweenLogBehaviour = LogBehaviour.Default;
#endif

        public CharacterDebugText characterDebugTextPrefab { get; private set; }

        public bool doTweenUseInitSettings { get { return _doTweenUseInitSettings; } }

        public bool doTweenRecycleAllByDefault { get { return _doTweenRecycleAllByDefault; } }

        public bool doTweenUseSafeMode { get { return _doTweenUseSafeMode; } }

#if NS_DG_TWEENING
        public LogBehaviour doTweenLogBehaviour { get { return _doTweenLogBehaviour; } }
#endif

        [Header("Inventory (0G Legacy)")]

        public List<ItemData> ItemDataReferences = default;

        [SerializeField]
        AutoMapPaletteData _autoMapPaletteData = default;

        public AutoMapPaletteData AutoMapPaletteData => _autoMapPaletteData;

        [Header("Object (0G Legacy)")]

        [SerializeField]
        [Tooltip("Is there to be only a single player character at any time in this game?")]
        bool _isSinglePlayerGame = default;

        public bool IsSinglePlayerGame => _isSinglePlayerGame;

        [SerializeField]
        [Tooltip("Add prefabs here to have them automatically instantiated as child GameObjects of KRGLoader." +
            " As children of KRGLoader, they will persist across scenes for the lifetime of the application.")]
        GameObject[] _autoInstancedPrefabs = default;

        public GameObject[] autoInstancedPrefabs { get { return (GameObject[]) _autoInstancedPrefabs.Clone(); } }

        public List<RasterAnimation> ExtraRasterAnimations = default;

        [Header("Time (0G Legacy)")]

        [SerializeField]
        string _timeThreadInstanceEnum = "_0G.Legacy.TimeThreadInstance";

        public string timeThreadInstanceEnum { get { return _timeThreadInstanceEnum; } }

        private void Awake() // GAME BUILD only
        {
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugText");
        }
    }
}