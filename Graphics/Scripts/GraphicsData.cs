using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [CreateAssetMenu(
        fileName = "NewKRGGraphicsData.asset",
        menuName = "KRG Scriptable Object/Graphics Data",
        order = 123
    )]
    public class GraphicsData : ScriptableObject {

#region serialized fields

        //applicable time thread index
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

#endregion

#region protected fields

        //applicable time thread interface (from _timeThreadIndex)
        protected ITimeThread _timeThread;

#endregion

#region properties

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

        public virtual int timeThreadIndex { get { return _timeThreadIndex; } }

#endregion

#region methods

        protected virtual void SetTimeThread() {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }

#endregion

    }
}
