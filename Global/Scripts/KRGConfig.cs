using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    public sealed partial class KRGConfig : ScriptableObject
    {
        //If you get an error stating `KRG.KRGConfig' does not contain a definition for `RESOURCE_PATH',
        //create a KRGConfig.MyGame.cs file containing a partial class KRGConfig with the following constants in it:

#if !KRG_CUSTOM_G

        public const string ASSET_PATH = "Assets/Resources/KRGConfig.asset";

        public const string RESOURCE_PATH = "KRGConfig";

#endif

        [Header("Global (KRG)")]

        [SerializeField]
        string _applicationNamespace = "MyGame";

        public string ApplicationNamespace => _applicationNamespace;

        [Header("Audio (KRG)")]

        [SerializeField]
        float _musicVolumeScale = 1;

        public float MusicVolumeScale => _musicVolumeScale;

        [Header("Damage (KRG)")]

        [SerializeField]
        DamageValue _damageValuePrefab = default;
        [SerializeField]
        HPBar _hpBarPrefab = default;

        public DamageValue damageValuePrefab { get { return _damageValuePrefab; } }

        public HPBar hpBarPrefab { get { return _hpBarPrefab; } }

        [Header("DOTween (KRG)")]

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

        [Header("Inventory (KRG)")]

        public List<ItemData> ItemDataReferences = default;

        [SerializeField]
        AutoMapPaletteData _autoMapPaletteData = default;

        public AutoMapPaletteData AutoMapPaletteData => _autoMapPaletteData;

        [Header("Object (KRG)")]

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

        [Header("Time (KRG)")]

        [SerializeField]
        string _timeThreadInstanceEnum = "KRG.TimeThreadInstance";

        public string timeThreadInstanceEnum { get { return _timeThreadInstanceEnum; } }

        private void Awake() // GAME BUILD only
        {
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugText");
        }
    }
}