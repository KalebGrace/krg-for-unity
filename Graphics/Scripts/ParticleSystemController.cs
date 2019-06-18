using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    //TODO: add ExecuteInEditMode or remove G.isInEditMode checks
    public abstract class ParticleSystemController : MonoBehaviour {

        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        [SerializeField]
        [FormerlySerializedAs("m_autoDispose")]
        bool _autoDispose = true;

        //TODO: do something with this or remove it
        [SerializeField]
        SpriteMask _particleMask;

        ParticleSystem _particleSystem;

        protected virtual ITimeThread timeThread {
            get {
                return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
            }
        }

        void Awake() {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            G.U.Require(_particleSystem, "Particle System", "this Component or its children");
        }

        void Start() {
            if (!G.isInEditMode) return;
            timeThread.AddPauseHandler(OnPause);
            timeThread.AddUnpauseHandler(OnUnpause);
        }

        void Update() {
            if (!G.isInEditMode) return;
            if (_autoDispose && !_particleSystem.IsAlive()) {
                G.U.End(gameObject);
            }
        }

        void OnDestroy() {
            if (!G.isInEditMode) return;
            timeThread.RemovePauseHandler(OnPause);
            timeThread.RemoveUnpauseHandler(OnUnpause);
        }

        void OnPause() {
            _particleSystem.Pause(true);
        }

        void OnUnpause() {
            _particleSystem.Play(true);
        }
    }
}
