using UnityEngine;

namespace _0G.Legacy
{
    public class AudioOnCollide : MonoBehaviour
    {
        //sound effect FMOD event string
        [AudioEvent]
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