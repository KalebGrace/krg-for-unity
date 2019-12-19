using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

#if NS_FMOD
using FMOD.Studio;
using FMODUnity;
#endif

namespace KRG
{
    public class AudioManager : Manager, IOnDestroy
    {
        public override float priority => 60;

#if !NS_FMOD
        public struct EventInstance
        {
            public bool hasHandle() => false;
            public bool isValid() => false;
        }
#endif

        //TODO: put this in the config, then make "-1" trigger the default
        public const float MUSIC_FADE_OUT_SECONDS = 2;

        private const string P_IS_GAME_PAUSED = "isGamePaused";

        public event System.Action<string> MusicPlayed;

        private bool _isInitialized;

#if NS_DG_TWEENING
        private Tween _musicStopTween;
#endif

        private string _musicFmodEvent;

        private EventInstance _musicInstance;

        private EventInstance _musicStopInstance;

        private PersistentData<float> m_MasterVolume = new PersistentData<float>(
            Persist.PlayerPrefs, "audio.master_volume", 1);
        private PersistentData<float> m_MusicVolume = new PersistentData<float>(
            Persist.PlayerPrefs, "audio.music_volume", 1);
        private PersistentData<float> m_SFXVolume = new PersistentData<float>(
            Persist.PlayerPrefs, "audio.sfx_volume", 1);

        private float m_MusicStopVolume = 1;

        // PUBLIC PROPERTIES

        /// <summary>
        /// Get or set the master volume from 0 (0%) to 1 (100%), or higher.
        /// </summary>
        public float MasterVolume
        {
            get => m_MasterVolume.Value;
            set
            {
                m_MasterVolume.Value = value;
                UpdateMusicVolume();
                UpdateMusicStopVolume();
            }
        }

        /// <summary>
        /// Get or set the music volume from 0 (0%) to 1 (100%), or higher.
        /// </summary>
        public float MusicVolume
        {
            get => m_MusicVolume.Value;
            set
            {
                m_MusicVolume.Value = value;
                UpdateMusicVolume();
            }
        }

        /// <summary>
        /// Get or set the sound effect volume from 0 (0%) to 1 (100%), or higher.
        /// </summary>
        public float SFXVolume
        {
            get => m_SFXVolume.Value;
            set => m_SFXVolume.Value = value;
        }

        // PRIVATE PROPERTIES

        private float MusicStopVolume
        {
            get => m_MusicStopVolume;
            set
            {
                m_MusicStopVolume = value;
                UpdateMusicStopVolume();
            }
        }

        // METHODS

        public override void Awake()
        {
            if (_isInitialized)
            {
                G.U.Warn("AudioManager is already initialized.");
                return;
            }
            _isInitialized = true;
            // do nothing, for now
        }

        public virtual void OnDestroy()
        {
            StopMusic();
        }

        public void PauseGame()
        {
            if (_musicInstance.isValid())
            {
#if NS_FMOD
                _musicInstance.setParameterByName(P_IS_GAME_PAUSED, 1);
#endif
            }
        }

        public void UnpauseGame()
        {
            if (_musicInstance.isValid())
            {
#if NS_FMOD
                _musicInstance.setParameterByName(P_IS_GAME_PAUSED, 0);
#endif
            }
        }

        public void PlayMusic(string fmodEvent, float outgoingMusicFadeOutSeconds = MUSIC_FADE_OUT_SECONDS)
        {
#if NS_FMOD && !(UNITY_EDITOR && EDITOR_MUSIC_OFF)
            if (_musicFmodEvent == fmodEvent) return;
            StopMusic(outgoingMusicFadeOutSeconds);
            //TODO: if music was stopped, fade in new music to make a proper crossfade,
            //and consider using ease settings to make it an equal-power crossfade
            _musicFmodEvent = fmodEvent;
            _musicInstance = RuntimeManager.CreateInstance(fmodEvent);
            UpdateMusicVolume();
            _musicInstance.start();
#endif
            MusicPlayed?.Invoke(fmodEvent);
        }

        public void StopMusic(float fadeOutSeconds = 0)
        {
#if NS_FMOD && !(UNITY_EDITOR && EDITOR_MUSIC_OFF)
            if (!_musicInstance.hasHandle()) return;
#if NS_DG_TWEENING
            _musicStopTween?.Complete();
#endif
            _musicStopInstance = _musicInstance;
            _musicInstance.clearHandle();
            _musicFmodEvent = null;
            if (fadeOutSeconds > 0)
            {
#if NS_DG_TWEENING
                MusicStopVolume = 1;
                _musicStopTween = DOTween.To(
                    () => MusicStopVolume,
                    x => MusicStopVolume = x,
                    0,
                    fadeOutSeconds
                ).OnComplete(StopMusicComplete);
#else
                G.U.Err("Using StopMusic with fadeOutSeconds requires DG.Tweening (DOTween).");
#endif
            }
            else
            {
                MusicStopVolume = 0;
                StopMusicComplete();
            }
#endif
        }

        private void StopMusicComplete()
        {
#if NS_FMOD
            //TODO: determine if there's ever a time when we would want to use STOP_MODE.ALLOWFADEOUT
            //if so, it probably would not work with our tweening fade out procedure
            _musicStopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _musicStopInstance.release();
#endif
        }

        public EventInstance PlaySFX(string fmodEvent, Vector3? position = null)
        {
#if NS_FMOD
            if (string.IsNullOrWhiteSpace(fmodEvent)) return new EventInstance();
            EventInstance eventInstance = RuntimeManager.CreateInstance(fmodEvent);
            if (position.HasValue) eventInstance.set3DAttributes(position.Value.To3DAttributes());
            UpdateSFXVolume(eventInstance, fmodEvent);
            eventInstance.start();
            //TODO: is this released automatically when it's done playing, or do I need to do this?
            return eventInstance;
#else
            return new EventInstance();
#endif
        }

        private void UpdateMusicVolume()
        {
            if (_musicInstance.hasHandle())
            {
#if NS_FMOD
                _musicInstance.setVolume(MasterVolume * MusicVolume);
#endif
            }
        }

        private void UpdateMusicStopVolume()
        {
            if (_musicStopInstance.hasHandle())
            {
#if NS_FMOD
                _musicStopInstance.setVolume(MasterVolume * MusicVolume * MusicStopVolume);
#endif
            }
        }

        protected virtual void UpdateSFXVolume(EventInstance eventInstance, string fmodEvent)
        {
            if (eventInstance.hasHandle())
            {
#if NS_FMOD
                eventInstance.setVolume(MasterVolume * SFXVolume);
#endif
            }
        }
    }
}
