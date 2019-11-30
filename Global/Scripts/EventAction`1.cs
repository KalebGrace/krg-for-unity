using System;

namespace KRG
{
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

        public void Invoke(T arg)
        {
            ActionHigh?.Invoke(arg);
            ActionNormal?.Invoke(arg);
        }
    }
}
