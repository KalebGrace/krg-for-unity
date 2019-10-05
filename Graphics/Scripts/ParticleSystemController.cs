using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    public abstract class ParticleSystemController : MonoBehaviour
    {
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        [SerializeField]
        [FormerlySerializedAs("m_autoDispose")]
        private bool _autoDispose = true;

        //TODO: do something with this or remove it
        [SerializeField]
        private SpriteMask _particleMask;

        private ParticleSystem _particleSystem;

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);

        private void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            G.U.Require(_particleSystem, "Particle System", "this Component or its children");
        }

        private void Start()
        {
            TimeThread.AddPauseHandler(OnPause);
            TimeThread.AddUnpauseHandler(OnUnpause);
        }

        private void Update()
        {
            if (_autoDispose && !_particleSystem.IsAlive())
            {
                gameObject.Dispose();
            }
        }

        private void OnDestroy()
        {
            TimeThread.RemovePauseHandler(OnPause);
            TimeThread.RemoveUnpauseHandler(OnUnpause);
        }

        private void OnPause()
        {
            _particleSystem.Pause(true);
        }

        private void OnUnpause()
        {
            _particleSystem.Play(true);
        }
    }
}
