using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif
#if NS_FMOD
using FMOD.Studio;
using FMODUnity;
#endif

namespace KRG {

    /// <summary>
    /// SCRIPT EXECUTION ORDER: #06 (after FMODUnity)
    /// </summary>
    public class AudioManager : Manager, IAudioManager {

        //TODO: put this in the config, then make "-1" trigger the default
        public const float musicFadeOutSecondsDefault = 2;

        const string _pIsGamePaused = "isGamePaused";

        bool _isInitialized;
        
#if NS_DG_TWEENING
        Tween _musicStopTween;
#endif

#if NS_FMOD
        string _musicFmodEvent;
        
        EventInstance _musicInstance;
        
        EventInstance _musicStopInstance;
#endif

#if NS_FMOD
        float musicStopVolume {
            get {
                float volume, finalvolume;
                _musicStopInstance.getVolume(out volume, out finalvolume);
                return volume;
            }
            set {
                _musicStopInstance.setVolume(value);
            }
        }
#endif

#region MonoBehaviour methods

        public virtual void Awake() {
            if (_isInitialized) {
                G.U.Warning("AudioManager is already initialized.");
                return;
            }
            _isInitialized = true;
            //nothing for now
        }

        public virtual void OnDestroy() {
            StopMusic();
        }

#endregion

#region Pause Game Methods

        public void PauseGame() {
#if NS_FMOD
            if (_musicInstance.isValid()) {
                ParameterInstance isGamePaused;
                _musicInstance.getParameter(_pIsGamePaused, out isGamePaused);
                isGamePaused.setValue(1);
            }
#endif
        }

        public void UnpauseGame() {
#if NS_FMOD
            if (_musicInstance.isValid()) {
                ParameterInstance isGamePaused;
                _musicInstance.getParameter(_pIsGamePaused, out isGamePaused);
                isGamePaused.setValue(0);
            }
#endif
        }

#endregion

#region Music Methods

        public void PlayMusic(string fmodEvent, float outgoingMusicFadeOutSeconds = musicFadeOutSecondsDefault) {
#if NS_FMOD && !(UNITY_EDITOR && EDITOR_MUSIC_OFF)
            if (_musicFmodEvent == fmodEvent) {
                return;
            }
            StopMusic(outgoingMusicFadeOutSeconds);
            //TODO: if music was stopped, fade in new music to make a proper crossfade,
            //and consider using ease settings to make it an equal-power crossfade
            _musicFmodEvent = fmodEvent;
            _musicInstance = RuntimeManager.CreateInstance(fmodEvent);
            _musicInstance.start();
#endif
        }

        public void StopMusic(float fadeOutSeconds = 0) {
#if NS_FMOD && !(UNITY_EDITOR && EDITOR_MUSIC_OFF)
            if (!_musicInstance.hasHandle()) return;
            #if NS_DG_TWEENING
            if (_musicStopTween != null) {
                _musicStopTween.Complete();
            }
            #endif
            _musicStopInstance = _musicInstance;
            _musicInstance.clearHandle();
            _musicFmodEvent = null;
            if (fadeOutSeconds > 0) {
                #if NS_DG_TWEENING
                _musicStopTween = DOTween.To(
                    () => musicStopVolume,
                    x => musicStopVolume = x,
                    0,
                    fadeOutSeconds
                ).OnComplete(StopMusicComplete);
                #else
                G.U.Error("Using StopMusic with fadeOutSeconds requires DG.Tweening (DOTween).");
                #endif
            } else {
                musicStopVolume = 0;
                StopMusicComplete();
            }
#endif
        }

        void StopMusicComplete() {
#if NS_FMOD
            //TODO: determine if there's ever a time when we would want to use STOP_MODE.ALLOWFADEOUT
            //if so, it probably would not work with our tweening fade out procedure
            _musicStopInstance.stop(STOP_MODE.IMMEDIATE);
            _musicStopInstance.release();
#endif
        }

#endregion

#region SFX Methods

        public void PlaySFX(string fmodEvent, Vector3? position = null) {
#if NS_FMOD
            var eventInstance = RuntimeManager.CreateInstance(fmodEvent);
            if (position.HasValue) eventInstance.set3DAttributes(position.Value.To3DAttributes());
            eventInstance.start();
            //TODO: is this released automatically when it's done playing, or do I need to do this?
#endif
        }

#endregion

    }
}
