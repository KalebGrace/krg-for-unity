using System.Collections.Generic;

namespace KRG
{
    public class Lock
    {
        public event LockHandler Locked;

        public delegate void LockHandler(bool isLocked, object lockingObject, Options options);

        public struct Options
        {
            public float AnimateTime;
        }

        private readonly List<object> m_Locks = new List<object>();

        public bool IsLocked => m_Locks.Count > 0;

        public void AddLock(object lockingObject, Options options = new Options())
        {
            G.U.Assert(lockingObject != null);

            if (m_Locks.Contains(lockingObject)) return;

            m_Locks.Add(lockingObject);

            if (m_Locks.Count == 1)
            {
                Locked?.Invoke(true, lockingObject, options);
            }
        }

        public void RemoveLock(object lockingObject, Options options = new Options())
        {
            G.U.Assert(lockingObject != null);

            if (!m_Locks.Contains(lockingObject)) return;

            m_Locks.Remove(lockingObject);

            if (m_Locks.Count == 0)
            {
                Locked?.Invoke(false, lockingObject, options);
            }
        }
    }
}
