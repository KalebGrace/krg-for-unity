using UnityEngine;

#if KRG_X_FUNGUS
using Fungus;
#endif

namespace _0G.Legacy
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

#if KRG_X_FUNGUS
        private WriterAudio m_WriterAudio;
#endif

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

#if KRG_X_FUNGUS
            m_WriterAudio = GetComponent<WriterAudio>();
            if (m_WriterAudio != null)
            {
                m_WriterAudio.SourceVolumeChanged += SetSourceVolume;
            }
#endif

            G.audio.MasterVolumeChanged += OnVolumeChanged;
            G.audio.MusicVolumeChanged += OnVolumeChanged;
            G.audio.SFXVolumeChanged += OnVolumeChanged;
        }

        private void OnDestroy()
        {
            G.audio.SFXVolumeChanged -= OnVolumeChanged;
            G.audio.MusicVolumeChanged -= OnVolumeChanged;
            G.audio.MasterVolumeChanged -= OnVolumeChanged;

#if KRG_X_FUNGUS
            if (m_WriterAudio != null)
            {
                m_WriterAudio.SourceVolumeChanged -= SetSourceVolume;
            }
#endif
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

#if KRG_X_FUNGUS
        private void SetSourceVolume(float value) => SourceVolume = value;
#endif
    }
}