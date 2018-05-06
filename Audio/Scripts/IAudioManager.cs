using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IAudioManager {

#region methods

        void PauseGame();

        void UnpauseGame();

        void PlayMusic(
            string fmodEvent,
            float outgoingMusicFadeOutSeconds = AudioManager.musicFadeOutSecondsDefault
        );

        void StopMusic(float fadeOutSeconds = 0);

        void PlaySFX(string fmodEvent, Vector3? position = null);

#endregion

    }
}
