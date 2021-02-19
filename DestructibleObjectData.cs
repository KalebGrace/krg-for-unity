using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [CreateAssetMenu(
        fileName = "SomeObject_DestructibleObjectData.asset",
        menuName = "0G Legacy Scriptable Object/Destructible Object Data",
        order = 405
    )]
    public class DestructibleObjectData : ScriptableObject
    {
        //applicable time thread index
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

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
        PhysicMaterial _physicMaterial = default;

        [Header("Requires DOTween")]
        [SerializeField]
        [FormerlySerializedAs("m_doesFade")]
        bool _doesFade = true;

        //applicable time thread interface (from _timeThreadIndex)
        protected TimeThread _timeThread;

        public virtual TimeThread TimeThread
        {
            get
            {
#if UNITY_EDITOR
                SetTimeThread();
#else
                if (_timeThread == null) SetTimeThread();
#endif
                return _timeThread;
            }
        }

        public virtual bool DoesFade => _doesFade;

        public virtual float ExplosionForce => _explosionForce;

        public virtual float ExplosionRadius => _explosionRadius;

        public virtual float Lifetime => _lifetime;

        public virtual PhysicMaterial PhysicMaterial => _physicMaterial;

        public virtual int TimeThreadIndex => _timeThreadIndex;

        protected virtual void SetTimeThread()
        {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }
    }
}