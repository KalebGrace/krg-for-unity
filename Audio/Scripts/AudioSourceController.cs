using UnityEngine;

namespace KRG
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        // SERIALIZED FIELDS

        public bool IsMusic = false;
        public bool IsSFX = true;

        // PRIVATE FIELDS

        private AudioSource m_AudioSource;
        private float m_SourceVolume;

        // PROPERTIES

        public float SourceVolume
        {
            get => m_SourceVolume;
            set
            {
                // in case this is called prior to Awake...
                if (m_AudioSource == null)
                {
                    m_AudioSource = GetComponent<AudioSource>();
                }
                m_SourceVolume = value;
                OnVolumeChanged();
            }
        }

        // MONOBEHAVIOUR METHODS

        private void Awake()
        {
            // if this wasn't already done prior to Awake...
            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
                m_SourceVolume = m_AudioSource.volume;
                OnVolumeChanged();
            }
            G.audio.MasterVolumeChanged += OnVolumeChanged;
            G.audio.MusicVolumeChanged += OnVolumeChanged;
            G.audio.SFXVolumeChanged += OnVolumeChanged;
        }

        private void OnDestroy()
        {
            G.audio.SFXVolumeChanged -= OnVolumeChanged;
            G.audio.MusicVolumeChanged -= OnVolumeChanged;
            G.audio.MasterVolumeChanged -= OnVolumeChanged;
        }

        // PRIVATE METHODS

        private void OnVolumeChanged()
        {
            float volume = m_SourceVolume * G.audio.MasterVolume;
            if (IsMusic)
            {
                volume *= G.audio.MusicVolume * G.config.MusicVolumeScale;
            }
            if (IsSFX)
            {
                volume *= G.audio.SFXVolume * G.config.SFXVolumeScale;
            }
            m_AudioSource.volume = volume;
        }
    }
}