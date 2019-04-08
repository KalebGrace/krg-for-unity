using System.Collections.Generic;

namespace KRG
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
        readonly List<Manager> m_Managers = new List<Manager>();

        readonly SortedList<float, IFixedUpdate> m_ManagerEventsFixedUpdate
        = new SortedList<float, IFixedUpdate>();

        readonly SortedList<float, ILateUpdate> m_ManagerEventsLateUpdate
        = new SortedList<float, ILateUpdate>();

        readonly SortedList<float, IOnApplicationQuit> m_ManagerEventsOnApplicationQuit
        = new SortedList<float, IOnApplicationQuit>();

        readonly SortedList<float, IOnDestroy> m_ManagerEventsOnDestroy
        = new SortedList<float, IOnDestroy>();

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
        public static readonly ObjectManager obj = new ObjectManager();
        public static readonly TimeManager time = new TimeManager();
        public static readonly UIManager ui = new UIManager();

        void InitManagers()
        {
            m_Managers.Add(app);
            m_Managers.Add(audio);
            m_Managers.Add(cam);
            m_Managers.Add(damage);
            m_Managers.Add(dotween);
            m_Managers.Add(obj);
            m_Managers.Add(time);
            m_Managers.Add(ui);

            InitManagersInternal();
        }

#endif

        void InitManagersInternal()
        {
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
                var fx = m as IFixedUpdate;
                if (fx != null) m_ManagerEventsFixedUpdate.Add(fx.priority, fx);

                var lt = m as ILateUpdate;
                if (lt != null) m_ManagerEventsLateUpdate.Add(lt.priority, lt);

                var aq = m as IOnApplicationQuit;
                if (aq != null) m_ManagerEventsOnApplicationQuit.Add(aq.priority, aq);

                var ds = m as IOnDestroy;
                if (ds != null) m_ManagerEventsOnDestroy.Add(ds.priority, ds);
            }

            app.StartApp();
        }

        // TRUE MONOBEHAVIOUR METHODS

        void FixedUpdate()
        {
            foreach (var x in m_ManagerEventsFixedUpdate.Values) x.FixedUpdate();
        }

        void LateUpdate()
        {
            foreach (var x in m_ManagerEventsLateUpdate.Values) x.LateUpdate();
        }

        void OnApplicationQuit()
        {
            foreach (var x in m_ManagerEventsOnApplicationQuit.Values) x.OnApplicationQuit();
        }

        // MORE KRG METHODS

        void DestroyManagers()
        {
            foreach (var x in m_ManagerEventsOnDestroy.Values) x.OnDestroy();
        }
    }
}
