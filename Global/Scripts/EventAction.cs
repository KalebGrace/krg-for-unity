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
                G.U.Warning("There is already a high priority action for this event.");
            }
            actionHigh += action;
        }

        public void Add(Action action)
        {
            actionNormal += action;
        }

        public void Remove(Action action)
        {
            actionHigh -= action;
            actionNormal -= action;
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
                G.U.Warning("There is already a high priority action for this event.");
            }
            actionHigh += action;
        }

        public void Add(Action<T> action)
        {
            actionNormal += action;
        }

        public void Remove(Action<T> action)
        {
            actionHigh -= action;
            actionNormal -= action;
        }

        public void Invoke(T in1)
        {
            if (actionHigh != null) actionHigh(in1);
            if (actionNormal != null) actionNormal(in1);
        }
    }
}
