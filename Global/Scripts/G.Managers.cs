#if !KRG_CUSTOM_G

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

	/// <summary>
	/// G.Managers.cs is a partial class of G (G.cs).
	/// This is the manager-oriented part of the G class.
	/// See G.cs for the proper class declaration and more info.
	/// 
	/// Create a class like this for your game (e.g. "G.ManagersMyGame.cs")
	/// and use the KRG_CUSTOM_G define symbol in order to customize Manager interfaces.
	/// NOTE: Also see the KRG_CUSTOM_G section in KRGConfig.
	/// </summary>
    partial class G {

#region instance fields (KRG)

        readonly AppManager _appManager = new AppManager();
        readonly AudioManager _audioManager = new AudioManager();
        readonly CameraManager _camManager = new CameraManager();
        readonly DamageManager _damageManager = new DamageManager();
        readonly ObjectManager _objManager = new ObjectManager();
        readonly TimeManager _timeManager = new TimeManager();
        readonly UIManager _uiManager = new UIManager();

#endregion

#region static properties (KRG)

        public static IAppManager app { get; private set; }

#pragma warning disable 0109
        //building the game in Unity 2017.2.0f3 will log a warning saying this "does not hide an inherited member"
        //however, this is incorrect, as it is actually hiding the deprecated Component.audio property
        public static new IAudioManager audio { get; private set; }
#pragma warning restore 0109

        public static ICameraManager cam { get; private set; }

        public static IDamageManager damage { get; private set; }

        public static IObjectManager obj { get; private set; }

        public static ITimeManager time { get; private set; }

        public static IUIManager ui { get; private set; }

#endregion

#region private & MonoBehaviour methods

        void SetManagerProperties() {
            //KRG
            app = _appManager;
            audio = _audioManager;
            cam = _camManager;
            damage = _damageManager;
            obj = _objManager;
            time = _timeManager;
            ui = _uiManager;
        }

        void AwakeManagers() {
            //KRG
            _appManager.Awake();
            _timeManager.Awake();
            _objManager.Awake();
            _audioManager.Awake();
        }

        void StartApp() {
        }

        void FixedUpdate() {
            //KRG
            _timeManager.FixedUpdate();
        }

        void LateUpdate() {
            //KRG
            _objManager.LateUpdate();
        }

        void OnApplicationQuit() {
            //KRG
            _appManager.OnApplicationQuit();
        }

        void DestroyManagers() {
            //KRG
            _audioManager.OnDestroy();
            _appManager.OnDestroy();
        }

#endregion

    }
}

#endif
