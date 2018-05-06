using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [CreateAssetMenu(
        fileName = "NewKRGDestructibleObjectData.asset",
        menuName = "KRG Scriptable Object/Destructible Object Data",
        order = 123
    )]
    public class DestructibleObjectData : ScriptableObject {

        //values listed below are simply DEFAULTS; check inspector for actual values

        //applicable time thread index
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        [SerializeField]
        [FormerlySerializedAs("m_lifetime")]
        float _lifetime = 3;

        [SerializeField]
        [FormerlySerializedAs("m_explosionForce")]
        float _explosionForce = 500;

        [SerializeField]
        [FormerlySerializedAs("m_explosionRadius")]
        float _explosionRadius = 5;

        [SerializeField]
        [FormerlySerializedAs("m_physicMaterial")]
        PhysicMaterial _physicMaterial;

        [Header("Requires DOTween")]
        [SerializeField]
        [FormerlySerializedAs("m_doesFade")]
        bool _doesFade = true;


        //applicable time thread interface (from _timeThreadIndex)
        protected ITimeThread _timeThread;


        public virtual ITimeThread timeThread {
            get {
#if UNITY_EDITOR
                SetTimeThread();
#else
                if (_timeThread == null) SetTimeThread();
#endif
                return _timeThread;
            }
        }


        public virtual bool doesFade { get { return _doesFade; } }

        public virtual float explosionForce { get { return _explosionForce; } }

        public virtual float explosionRadius { get { return _explosionRadius; } }

        public virtual float lifetime { get { return _lifetime; } }

        public virtual PhysicMaterial physicMaterial { get { return _physicMaterial; } }

        public virtual int timeThreadIndex { get { return _timeThreadIndex; } }


        protected virtual void SetTimeThread() {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }
    }
}
