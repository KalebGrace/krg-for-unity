using System.Collections.Generic;

namespace _0G.Legacy
{
    public abstract class InfiniteStateMachine<TOwner, TLogic>
        where TOwner : IStateOwner
    where TLogic : IStateLogic<TOwner>
    {
        // STATIC STATE LOGIC OBJECTS

        //>> NEEDS TO BE ADDED TO DERIVED CLASS <<
        /*
        private static readonly FooState s_fooState = new FooState();
        */
        //...where FooState : IStateLogic

        // FIELDS

        protected readonly int _stateCount;
        protected readonly TOwner _stateOwner;

        readonly TimeTrigger[] _lifetimeTriggers;
        readonly List<int> _lockSources = new List<int>();
        readonly List<int> _lockTargets = new List<int>();
        readonly bool[] _states;

        // PROPERTIES

        protected int? pendingAddition { get; private set; }

        // CONSTRUCTOR

        protected InfiniteStateMachine(TOwner stateOwner, int stateCount)
        {
            _stateOwner = stateOwner;
            _stateCount = stateCount;
            _states = new bool[stateCount];
            _lifetimeTriggers = new TimeTrigger[stateCount];
        }

        // PUBLIC METHODS

        //>> NEEDS TO BE CALLED DURING UPDATE <<
        public virtual void Update()
        {
            for (int i = 0; i < _stateCount; i++)
            {
                if (!_states[i]) continue;
                TLogic stateLogic = GetStateLogic(i);
                if (!IsVerified(stateLogic)) continue;
                stateLogic.Update(_stateOwner);
            }
        }

        // PROTECTED METHODS

        /// <summary>
        /// Add the specified state (w/ option to set lifetime, and option to lock states).
        /// If the time thread is null and the lifetime is 0, the lifetime will be infinite.
        /// </summary>
        /// <param name="stateIndex">Index of the state to add.</param>
        /// <param name="stateTimeThread">State time thread.</param>
        /// <param name="stateLifetime">State lifetime in seconds.</param>
        /// <param name="statesToLock">States to lock.</param>
        /// <param name="removeOnLock">If set to <c>true</c> remove states on lock.</param>
        protected void Add(
            int stateIndex,
            TimeThread stateTimeThread = null,
            float stateLifetime = 0,
            int[] statesToLock = null,
            bool removeOnLock = true
        )
        {
            if (!CanAdd(stateIndex)) return;
            if (!G.U.Prevent(stateLifetime < 0,
                    "stateLifetime must be 0 or greater.")) return;
            if (!G.U.Prevent(stateLifetime > 0 && stateTimeThread == null,
                    "If a lifetime is provided, a time thread must also be provided.")) return;
            TLogic stateLogic = GetStateLogic(stateIndex);
            if (!IsVerified(stateLogic)) return;
            //cache the pending addition so it can be viewed by callbacks (e.g. those on removed locked states)
            if (pendingAddition.HasValue) G.U.Warn(
                "State {0} is being added before the pending addition of state {1} has completed.",
                stateIndex, pendingAddition.Value);
            pendingAddition = stateIndex;
            //add locks as applicable
            if (statesToLock != null)
            {
                _ = Lock(stateIndex, statesToLock, removeOnLock);
            }
            //remove (prior) lifetime trigger from the state, if it exists
            RemoveLifetimeTrigger(stateIndex);
            //add (new) lifetime trigger to the state as applicable
            if (stateLifetime > 0)
            {
                TimeTrigger tt = stateTimeThread.AddTrigger(stateLifetime, OnLifetimeExpired, true);
                tt.intData = stateIndex;
                _lifetimeTriggers[stateIndex] = tt;
            }
            //add or renew the state based on its current value
            if (!_states[stateIndex])
            {
                //this state doesn't exist; add it, then call external methods
                _states[stateIndex] = true;
                OnChangedAnteEvent(stateIndex, true);
                stateLogic.OnAdded(_stateOwner);
                OnChangedPostEvent(stateIndex, true);
            }
            else
            {
                //this state already exists; renew it (via an external method)
                stateLogic.OnRenewed(_stateOwner);
            }
            //clear the pending addition cache
            pendingAddition = null;
            //if the lifetime is zero, but a time thread is provided, this state is a facade; remove it
            if (stateLifetime.Ap(0) && stateTimeThread != null) Remove(stateIndex);
        }

        protected bool CanAdd(int stateIndex)
        {
            return IsVerified(stateIndex) && !_lockTargets.Contains(stateIndex);
        }

        protected bool Has(int stateIndex)
        {
            return IsVerified(stateIndex) && _states[stateIndex];
        }

        protected void Remove(int stateIndex)
        {
            if (!Has(stateIndex) || _lockTargets.Contains(stateIndex)) return;
            TLogic stateLogic = GetStateLogic(stateIndex);
            if (!IsVerified(stateLogic)) return;
            //remove locks as applicable
            for (int i = 0; i < _lockSources.Count; i++)
            {
                if (_lockSources[i] == stateIndex)
                {
                    _lockSources.RemoveAt(i);
                    _lockTargets.RemoveAt(i);
                    i--;
                }
            }
            //remove lifetime trigger from the state, if it exists
            RemoveLifetimeTrigger(stateIndex);
            //remove the state, then call external methods
            _states[stateIndex] = false;
            OnChangedAnteEvent(stateIndex, false);
            stateLogic.OnRemoved(_stateOwner);
            OnChangedPostEvent(stateIndex, false);
        }

        protected void SetLifetimeInfinite(int stateIndex)
        {
            if (IsVerified(stateIndex)) RemoveLifetimeTrigger(stateIndex);
        }

        /// <summary>
        /// Lock the specified states.
        /// </summary>
        /// <param name="lockingStateIndex">Index of the locking state.</param>
        /// <param name="statesToLock">States to lock.</param>
        /// <param name="removeOnLock">If set to <c>true</c> remove states on lock.</param>
        /// <returns>True, if everything completed successfully.</returns>
        protected bool Lock(
            int lockingStateIndex,
            int[] statesToLock,
            bool removeOnLock
        )
        {
            if (pendingAddition != lockingStateIndex && !Has(lockingStateIndex)) return false;
            for (int i = 0; i < statesToLock.Length; ++i)
            {
                int lok = statesToLock[i];
                if (!G.U.Prevent(lockingStateIndex == lok, "A state cannot lock itself.")) continue;
                //remove the locked state as applicable
                if (removeOnLock) Remove(lok);
                //now lock said state
                _lockSources.Add(lockingStateIndex);
                _lockTargets.Add(lok);
            }
            return true;
        }

        // ABSTRACT METHODS

        protected abstract TLogic GetStateLogic(int stateIndex);

        protected abstract void OnChangedAnteEvent(int stateIndex, bool stateValue);

        protected abstract void OnChangedPostEvent(int stateIndex, bool stateValue);

        // PRIVATE METHODS

        bool IsVerified(int stateIndex)
        {
            if (!G.U.Prevent(stateIndex < 0,
                    "stateIndex must be 0 or greater.")) return false;
            if (!G.U.Prevent(stateIndex >= _stateCount,
                    "stateIndex must be less than the state count.")) return false;
            return true;
        }

        static bool IsVerified(TLogic stateLogic)
        {
            if (!G.U.Prevent(stateLogic.Equals(default(TLogic)),
                    "stateLogic must not be null/default.")) return false;
            return true;
        }

        void OnLifetimeExpired(TimeTrigger tt)
        {
            int stateIndex = tt.intData;
            _lifetimeTriggers[stateIndex] = null;
            Remove(stateIndex);
        }

        void RemoveLifetimeTrigger(int stateIndex)
        {
            TimeTrigger tt = _lifetimeTriggers[stateIndex];
            _lifetimeTriggers[stateIndex] = null;
            if (tt != null) tt.Dispose();
        }
    }
}