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
        public override float priority { get { return 60; } }

        //TODO: put this in the config, then make "-1" trigger the default
        public const float musicFadeOutSecondsDefault = 2;

        private const string _pIsGamePaused = "isGamePaused";

        public event System.Action<string> MusicPlayed;

        private bool _isInitialized;

#if NS_DG_TWEENING
        private Tween _musicStopTween;
#endif

#if NS_FMOD
        private string _musicFmodEvent;

        private EventInstance _musicInstance;

        private EventInstance _musicStopInstance;
#endif

        private float m_MasterVolume = 1;
        private float m_MusicVolume = 1;
        private float m_MusicStopVolume = 1;

        /// <summary>
        /// Set the master volume from 0 (0%) to 1 (100%).
        /// </summary>
        public float MasterVolume
        {
            get => m_MasterVolume;
            set
            {
                m_MasterVolume = value;
                UpdateMusicVolume();
                UpdateMusicStopVolume();
            }
        }

        /// <summary>
        /// Set the music volume from 0 (0%) to 1 (100%).
        /// </summary>
        public float MusicVolume
        {
            get => m_MusicVolume;
            set
            {
                m_MusicVolume = value;
                UpdateMusicVolume();
            }
        }

        /// <summary>
        /// Set the music stop volume from 0 (0%) to 1 (100%).
        /// </summary>
        private float MusicStopVolume
        {
            get => m_MusicStopVolume;
            set
            {
                m_MusicStopVolume = value;
                UpdateMusicStopVolume();
            }
        }

        /// <summary>
        /// Set the sound effect volume from 0 (0%) to 1 (100%).
        /// </summary>
        public float SFXVolume { get; set; } = 1;

        public override void Awake()
        {
            if (_isInitialized)
            {
                G.U.Warn("AudioManager is already initialized.");
                return;
            }
            _isInitialized = true;
            LoadVolumesFromDisk();
        }

        public virtual void OnDestroy()
        {
            StopMusic();
        }

        public void PauseGame()
        {
#if NS_FMOD
            if (_musicInstance.isValid())
            {
                _musicInstance.setParameterByName(_pIsGamePaused, 1);
            }
#endif
        }

        public void UnpauseGame()
        {
#if NS_FMOD
            if (_musicInstance.isValid())
            {
                _musicInstance.setParameterByName(_pIsGamePaused, 0);
            }
#endif
        }

        public void PlayMusic(string fmodEvent, float outgoingMusicFadeOutSeconds = musicFadeOutSecondsDefault)
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
            if (_musicStopTween != null)
            {
                _musicStopTween.Complete();
            }
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

        public void PlaySFX(string fmodEvent, Vector3? position = null)
        {
#if NS_FMOD
            if (string.IsNullOrWhiteSpace(fmodEvent)) return;
            EventInstance eventInstance = RuntimeManager.CreateInstance(fmodEvent);
            if (position.HasValue) eventInstance.set3DAttributes(position.Value.To3DAttributes());
            UpdateSFXVolume(eventInstance, fmodEvent);
            eventInstance.start();
            //TODO: is this released automatically when it's done playing, or do I need to do this?
#endif
        }

        protected virtual void LoadVolumesFromDisk()
        {
            //TODO: have a standard loading feature
        }

        public virtual void SaveVolumesToDisk()
        {
            //TODO: have a standard saving feature
        }

        private void UpdateMusicVolume()
        {
#if NS_FMOD
            if (_musicInstance.hasHandle())
            {
                _musicInstance.setVolume(MasterVolume * MusicVolume);
            }
#endif
        }

        private void UpdateMusicStopVolume()
        {
#if NS_FMOD
            if (_musicStopInstance.hasHandle())
            {
                _musicStopInstance.setVolume(MasterVolume * MusicVolume * MusicStopVolume);
            }
#endif
        }

#if NS_FMOD
        protected virtual void UpdateSFXVolume(EventInstance eventInstance, string fmodEvent)
        {
            if (eventInstance.hasHandle())
            {
                eventInstance.setVolume(MasterVolume * SFXVolume);
            }
        }
#endif
    }
}
