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

        /*
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_serializedVersion")]
        int _serializedVersion = default;
        */


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

        public RasterAnimationInfo rasterAnimationInfoPrefab { get; private set; }

        public bool doTweenUseInitSettings { get { return _doTweenUseInitSettings; } }

        public bool doTweenRecycleAllByDefault { get { return _doTweenRecycleAllByDefault; } }

        public bool doTweenUseSafeMode { get { return _doTweenUseSafeMode; } }

#if NS_DG_TWEENING
        public LogBehaviour doTweenLogBehaviour { get { return _doTweenLogBehaviour; } }
#endif


        [Header("Inventory (KRG)")]

        [SerializeField]
        ItemData[] _keyItemDataReferences = default;

        public ItemData[] KeyItemDataReferences => (ItemData[])_keyItemDataReferences.Clone();

        [SerializeField]
        AutoMapPaletteData _autoMapPaletteData = default;

        public AutoMapPaletteData AutoMapPaletteData => _autoMapPaletteData;


        [Header("Object (KRG)")]

        [SerializeField]
        [Tooltip("Add prefabs here to have them automatically instantiated as child GameObjects of KRGLoader."
        + " As children of KRGLoader, they will persist across scenes for the lifetime of the application.")]
        GameObject[] _autoInstancedPrefabs = default;

        public GameObject[] autoInstancedPrefabs { get { return (GameObject[])_autoInstancedPrefabs.Clone(); } }


        [Header("Time (KRG)")]

        [SerializeField]
        string _timeThreadInstanceEnum = "KRG.TimeThreadInstance";

        public string timeThreadInstanceEnum { get { return _timeThreadInstanceEnum; } }


        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        void Awake()
        {
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugText");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfo");

            UpdateSerializedVersion();
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        void OnValidate()
        {
            UpdateSerializedVersion();
        }


        void UpdateSerializedVersion()
        {
            /*
            switch (_serializedVersion) {
                case 0:
                    //put code here to assign default values or update existing values
                    _serializedVersion = 1;
                    break;
            }
            */
        }
    }
}
