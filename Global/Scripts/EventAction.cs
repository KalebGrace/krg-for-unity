using System;

namespace KRG
{
    public struct EventAction
    {
        event Action actionHigh;
        event Action actionNormal;

        public void AddHigh(Action action)
        {
            if (actionHigh != null)
            {
                G.U.Warn("There is already a high priority action for this event.");
            }
            actionHigh += action;
        }

        public static EventAction operator + (EventAction eventAction, Action action)
        {
            eventAction.actionNormal += action;
            return eventAction;
        }

        public static EventAction operator - (EventAction eventAction, Action action)
        {
            eventAction.actionHigh -= action;
            eventAction.actionNormal -= action;
            return eventAction;
        }

        public void Invoke()
        {
            if (actionHigh != null) actionHigh();
            if (actionNormal != null) actionNormal();
        }
    }

    public struct EventAction<T>
    {
        event Action<T> actionHigh;
        event Action<T> actionNormal;

        public void AddHigh(Action<T> action)
        {
            if (actionHigh != null)
            {
                G.U.Warn("There is already a high priority action for this event.");
            }
            actionHigh += action;
        }

        public static EventAction<T> operator + (EventAction<T> eventAction, Action<T> action)
        {
            eventAction.actionNormal += action;
            return eventAction;
        }

        public static EventAction<T> operator - (EventAction<T> eventAction, Action<T> action)
        {
            eventAction.actionHigh -= action;
            eventAction.actionNormal -= action;
            return eventAction;
        }

        public void Invoke(T in1)
        {
            if (actionHigh != null) actionHigh(in1);
            if (actionNormal != null) actionNormal(in1);
        }
    }
}
