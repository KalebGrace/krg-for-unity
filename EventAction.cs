using System;

namespace _0G.Legacy
{
    public struct EventAction
    {
        private event Action ActionHigh;
        private event Action ActionNormal;

        public void AddHigh(Action action)
        {
            if (ActionHigh != null)
            {
                G.U.Warn("There is already a high priority action for this event.");
            }
            ActionHigh += action;
        }

        public static EventAction operator +(EventAction eventAction, Action action)
        {
            eventAction.ActionNormal += action;
            return eventAction;
        }

        public static EventAction operator -(EventAction eventAction, Action action)
        {
            eventAction.ActionHigh -= action;
            eventAction.ActionNormal -= action;
            return eventAction;
        }

        public void Invoke()
        {
            ActionHigh?.Invoke();
            ActionNormal?.Invoke();
        }
    }

    public struct EventAction<T>
    {
        private event Action<T> ActionHigh;
        private event Action<T> ActionNormal;

        public void AddHigh(Action<T> action)
        {
            if (ActionHigh != null)
            {
                G.U.Warn("There is already a high priority action for this event.");
            }
            ActionHigh += action;
        }

        public static EventAction<T> operator +(EventAction<T> eventAction, Action<T> action)
        {
            eventAction.ActionNormal += action;
            return eventAction;
        }

        public static EventAction<T> operator -(EventAction<T> eventAction, Action<T> action)
        {
            eventAction.ActionHigh -= action;
            eventAction.ActionNormal -= action;
            return eventAction;
        }

        public void Invoke(T in1)
        {
            ActionHigh?.Invoke(in1);
            ActionNormal?.Invoke(in1);
        }
    }
}