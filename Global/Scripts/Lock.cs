using System.Collections.Generic;

namespace KRG
{
    public class Lock
    {
        public event LockHandler Locked;

        public delegate void LockHandler(bool isLocked, object lockingObject);

        private readonly List<object> m_Locks = new List<object>();

        public bool IsLocked => m_Locks.Count > 0;

        public void AddLock(object lockingObject)
        {
            G.U.Assert(lockingObject != null);

            m_Locks.Add(lockingObject);

            if (m_Locks.Count == 1)
            {
                Locked(true, lockingObject);
            }
        }

        public void RemoveLock(object lockingObject)
        {
            if (m_Locks.Contains(lockingObject))
            {
                m_Locks.Remove(lockingObject);
            }

            if (m_Locks.Count == 0)
            {
                Locked(false, lockingObject);
            }
        }
    }
}
