namespace _0G.Legacy
{
    public class TimeManager : Manager, IFixedUpdate, IUpdate
    {
        public override float priority => 40;

        const string _timeThreadInstanceInfo =
            "Furthermore, the following must be adhered to: " +
            "Every enum entry must have a unique integer value. " +
            "The entry with value -1 does not map to an actual time thread; " +
            "it is used solely to indicate no time thread preference. " +
            "The entry with value 0 maps to the \"Application\" time thread, " +
            "which will always be UNSCALED and UNPAUSED. " +
            "The entry with value 1 maps to the \"Gameplay\" time thread, " +
            "which is affected by timeScale and can be paused/unpaused. " +
            "Entries with value 2 or greater are optional; " +
            "they map to additional time threads " +
            "that are also affected by timeScale and can be paused/unpaused.";

        //TODO: FIX
#pragma warning disable 0414
        static readonly TimeThreadFacade _timeThreadFacade = new TimeThreadFacade();
#pragma warning restore 0414

        bool _isInitialized;
        int _threadCount;
        TimeThread[] _threads;

        /// <summary>
        /// Initialize the TimeManager's TimeThread instances.
        /// This must be done by KRGLoader before FixedUpdate occurs.
        /// </summary>
        public override void Awake()
        {
            if (_isInitialized)
            {
                G.U.Warn("TimeManager is already initialized.");
                return;
            }
            _isInitialized = true;
            var eg = new EnumGeneric(config.timeThreadInstanceEnum);
            int len = eg.length;
            if (len < 3)
            {
                G.U.Err("There must be at least three time thread instance enum entries. " +
                    _timeThreadInstanceInfo);
                len = 3;
            }
            else if (!eg.HasSequentialValues(-1, 1))
            {
                G.U.Err("Time thread instance integer values must start at -1 and increment by 1. " +
                    _timeThreadInstanceInfo);
            }
            _threadCount = len - 1; //ignore "UseDefault"
            _threads = new TimeThread[_threadCount];
            for (int i = 0; i < _threadCount; i++)
            {
                _threads[i] = new TimeThread(i);
            }
        }

        public virtual void FixedUpdate()
        {
            for (int i = 0; i < _threadCount; i++)
            {
                _threads[i].FixedUpdate();
            }
        }

        public virtual void Update() { }

        public TimeThread GetTimeThread(int timeThreadIndex, System.Enum defaultTimeThreadInstance)
        {
            if (timeThreadIndex == (int) TimeThreadInstance.UseDefault)
            {
                return GetTimeThread(defaultTimeThreadInstance);
            }
            else
            {
                return GetTimeThread(timeThreadIndex);
            }
        }

        public TimeThread GetTimeThread(System.Enum timeThreadInstance)
        {
            //TODO: consider utilizing the EnumGeneric created in Init
            return GetTimeThread(System.Convert.ToInt32(timeThreadInstance));
        }

        public TimeThread GetTimeThread(int timeThreadIndex)
        {
            //TODO: FIX
            /*
            if (G.app.isQuitting && instance == null) {
                //if the app is shutting down and the TimeManager has been destroyed,
                //return a TimeThreadFacade instead
                return _timeThreadFacade;
            }
            var tm = instance;
            */
            var tm = this; //TEMP FIX
            if (timeThreadIndex < 0 || timeThreadIndex >= tm._threadCount)
            {
                G.U.Err(
                    "The specified time thread index must be 0 to {0}, but {1} was provided.",
                    tm._threadCount - 1,
                    timeThreadIndex
                );
                return null;
            }
            return tm._threads[timeThreadIndex];
        }
    }
}