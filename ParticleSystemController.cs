using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    // IMPORTANT: consider using VFXBasePrefab instead

    public class ParticleSystemController : MonoBehaviour, IBodyComponent
    {
        // SERIALIZED FIELDS

        [SerializeField]
        [Tooltip("Automatically dispose of this body/object when the ParticleSystem is no longer alive.")]
        [FormerlySerializedAs("m_autoDispose"), FormerlySerializedAs("_autoDispose")]
        public bool AutoDispose = default;

        [SerializeField]
        private GameObjectBody m_Body = default;

        [Header("DON'T USE")]
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        // PROPERTIES

        public GameObjectBody Body => m_Body;

        public ParticleSystem ParticleSystem { get; private set; }

        protected virtual TimeThread TimeThread
        {
            get
            {
                if (m_Body)
                {
                    return m_Body.TimeThread;
                }
                else
                {
                    G.U.Warn("{0} is using deprecated particle system controller settings.", gameObject.name);
                    return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
                }
            }
        }

        // INITIALIZATION METHODS

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

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
            if (AutoDispose && !ParticleSystem.IsAlive())
            {
                if (m_Body != null)
                {
                    m_Body.Dispose();
                }
                else
                {
                    gameObject.Dispose();
                }
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