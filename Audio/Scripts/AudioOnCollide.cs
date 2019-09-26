using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class AudioOnCollide : MonoBehaviour
    {
        //sound effect FMOD event string
#if NS_FMOD
        [FMODUnity.EventRef]
#endif
        [SerializeField]
        string _sfxFmodEvent = default;

        private void OnCollisionEnter(Collision collision)
        {
            if (G.obj.IsPlayerCharacter(collision))
            {
                G.audio.PlaySFX(_sfxFmodEvent, transform.position);
            }
        }
    }
}
