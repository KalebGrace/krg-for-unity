using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    // DEPRECATED (try using VFXBasePrefab instead)
    public abstract class ParticleSystemController : MonoBehaviour
    {
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

        [SerializeField]
        [Tooltip("When the ParticleSystem is no longer alive, dispose of this GameObject.")]
        [FormerlySerializedAs("m_autoDispose")]
        private bool _autoDispose = true;

        public ParticleSystem ParticleSystem { get; private set; }

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);

        private void Awake()
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            TimeThread.AddPauseHandler(OnPause);
            TimeThread.AddUnpauseHandler(OnUnpause);
        }

        private void Update()
        {
            if (_autoDispose && !ParticleSystem.IsAlive())
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
            ParticleSystem.Pause(true);
        }

        private void OnUnpause()
        {
            ParticleSystem.Play(true);
        }
    }
}