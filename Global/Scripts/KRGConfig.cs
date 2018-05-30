using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public sealed partial class KRGConfig : ScriptableObject {

#region krg

		//If you get an error stating `KRG.KRGConfig' does not contain a definition for `resourcePath',
		//create a KRGConfig.MyGame.cs file containing a partial class KRGConfig with the following constants in it:

#if !KRG_CUSTOM_G

        public const string assetPath = "Assets/Resources/KRGConfig.asset";

        public const string resourcePath = "KRGConfig";

#endif

        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_serializedVersion")]
        int _serializedVersion;

        [Header("KRG: King's Royal Gold")]

        [SerializeField]
        [FormerlySerializedAs("m_references")]
        KRGReferences _krgReferences;

        public KRGReferences krgReferences { get { return _krgReferences; } }

#endregion

#region damage

        [Header("Damage (KRG)")]

        [SerializeField]
        DamageValue _damageValuePrefab;
        [SerializeField]
        HPBar _hpBarPrefab;

        public DamageValue damageValuePrefab { get { return _damageValuePrefab; } }

        public HPBar hpBarPrefab { get { return _hpBarPrefab; } }

#endregion

#region dotween

        [Header("DOTween (KRG)")]

        [SerializeField]
        bool _doTweenUseInitSettings;
        [SerializeField]
        bool _doTweenRecycleAllByDefault;
        [SerializeField]
        bool _doTweenUseSafeMode;
#if NS_DG_TWEENING
        [SerializeField]
        LogBehaviour _doTweenLogBehaviour = LogBehaviour.Default;
#endif

        public bool doTweenUseInitSettings { get { return _doTweenUseInitSettings; } }

        public bool doTweenRecycleAllByDefault { get { return _doTweenRecycleAllByDefault; } }

        public bool doTweenUseSafeMode { get { return _doTweenUseSafeMode; } }

#if NS_DG_TWEENING
        public LogBehaviour doTweenLogBehaviour { get { return _doTweenLogBehaviour; } }
#endif

#endregion

#region object

        [Header("Object (KRG)")]

        [SerializeField]
        [Tooltip("Add prefabs here to have them automatically instantiated as child GameObjects of KRGLoader."
        + " As children of KRGLoader, they will persist across scenes for the lifetime of the application.")]
        GameObject[] _autoInstancedPrefabs;

        public GameObject[] autoInstancedPrefabs { get { return (GameObject[])_autoInstancedPrefabs.Clone(); } }

#endregion

#region time

        [Header("Time (KRG)")]

        [SerializeField]
        string _timeThreadInstanceEnum = "KRG.TimeThreadInstance";

        public string timeThreadInstanceEnum { get { return _timeThreadInstanceEnum; } }

#endregion

#region MonoBehaviour methods

        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        void Awake() {
            UpdateSerializedVersion();
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        void OnValidate() {
            UpdateSerializedVersion();
        }

#endregion

#region private methods

        void UpdateSerializedVersion() {
            /*
            switch (_serializedVersion) {
                case 0:
                    //put code here to assign default values or update existing values
                    _serializedVersion = 1;
                    break;
            }
            */
        }

#endregion

    }
}
