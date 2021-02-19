using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace _0G.Legacy
{
    /// <summary>
    /// G.Managers.cs is a partial class of G (G.cs).
    /// This is the manager-oriented part of the G class.
    /// See G.cs for the proper class declaration and more info.
    /// 
    /// Create a class like this for your game (e.g. "G.ManagersMyGame.cs")
    /// and use the KRG_CUSTOM_G define symbol in order to customize Manager accessors.
    /// NOTE: Also see the KRG_CUSTOM_G section in KRGConfig.
    /// </summary>
    partial class G
    {
#if !KRG_CUSTOM_G

        public static readonly AppManager app = new AppManager();
#pragma warning disable 0109
        //building the game in Unity 2017.2.0f3 will log a warning saying this "does not hide an inherited member"
        //however, this is incorrect, as it is actually hiding the deprecated Component.audio property
        public static readonly new AudioManager audio = new AudioManager();
#pragma warning restore 0109
        public static readonly CameraManager cam = new CameraManager();
        public static readonly DamageManager damage = new DamageManager();
        public static readonly DOTweenManager dotween = new DOTweenManager();
        public static readonly EnvironmentManager env = new EnvironmentManager();
        public static readonly GraphicsManager gfx = new GraphicsManager();
        public static readonly InventoryManager inv = new InventoryManager();
        public static readonly ObjectManager obj = new ObjectManager();
        public static readonly SaveManager save = new SaveManager();
        public static readonly TimeManager time = new TimeManager();
        public static readonly UIManager ui = new UIManager();

#endif

        readonly List<Manager> m_Managers = new List<Manager>();

        readonly SortedList<float, IStart> m_ManagerEventsStart = new SortedList<float, IStart>();

        readonly SortedList<float, IFixedUpdate> m_ManagerEventsFixedUpdate = new SortedList<float, IFixedUpdate>();

        readonly SortedList<float, IUpdate> m_ManagerEventsUpdate = new SortedList<float, IUpdate>();

        readonly SortedList<float, ILateUpdate> m_ManagerEventsLateUpdate = new SortedList<float, ILateUpdate>();

        readonly SortedList<float, IOnApplicationQuit> m_ManagerEventsOnApplicationQuit = new SortedList<float, IOnApplicationQuit>();

        readonly SortedList<float, IOnDestroy> m_ManagerEventsOnDestroy = new SortedList<float, IOnDestroy>();

        void InitManagers()
        {
            var fields = typeof(G).GetFields(BindingFlags.Public | BindingFlags.Static);

            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].GetValue(null) is Manager m) m_Managers.Add(m);
            }

            m_Managers.Sort(
                (x, y) =>
                {
                    IAwake x_a = x;
                    IAwake y_a = y;
                    return x_a.priority.CompareTo(y_a.priority);
                }
            );

            foreach (IAwake m in m_Managers)
            {
                m.Awake();
            }

            foreach (Manager m in m_Managers)
            {
                if (m is IStart st) m_ManagerEventsStart.Add(st.priority, st);

                if (m is IFixedUpdate fx) m_ManagerEventsFixedUpdate.Add(fx.priority, fx);

                if (m is IUpdate up) m_ManagerEventsUpdate.Add(up.priority, up);

                if (m is ILateUpdate lu) m_ManagerEventsLateUpdate.Add(lu.priority, lu);

                if (m is IOnApplicationQuit aq) m_ManagerEventsOnApplicationQuit.Add(aq.priority, aq);

                if (m is IOnDestroy ds) m_ManagerEventsOnDestroy.Add(ds.priority, ds);
            }

            app.StartApp();
        }

        // TRUE MONOBEHAVIOUR METHODS

        void Start()
        {
            foreach (var m in m_ManagerEventsStart.Values) m.Start();
        }

        void FixedUpdate()
        {
            foreach (var m in m_ManagerEventsFixedUpdate.Values) m.FixedUpdate();
        }

        void Update()
        {
            foreach (var m in m_ManagerEventsUpdate.Values) m.Update();
        }

        void LateUpdate()
        {
            foreach (var m in m_ManagerEventsLateUpdate.Values) m.LateUpdate();
        }

        void OnApplicationQuit()
        {
            foreach (var m in m_ManagerEventsOnApplicationQuit.Values) m.OnApplicationQuit();
        }

        // PRIVATE _0G.Legacy METHODS

        void DestroyManagers()
        {
            var reversedList = m_ManagerEventsOnDestroy.Values.Reverse();

            foreach (var m in reversedList) m.OnDestroy();
        }

        // PUBLIC _0G.Legacy METHODS

        public void InvokeManagers<T>(System.Action<T> action)
        {
            if (!typeof(T).IsInterface)
            {
                U.Err("InvokeManagers is intended for use with interfaces only. {0} is not an interface.", typeof(T));
                return;
            }

            for (int i = 0; i < m_Managers.Count; ++i)
            {
                if (m_Managers[i] is T t)
                {
                    action(t);
                }
            }
        }
    }
}