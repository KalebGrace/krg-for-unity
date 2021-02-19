using UnityEngine;

namespace _0G.Legacy
{
    public class BlockedMovementSensor
    {
        public System.Action<float> BlockedMovement;

        private float m_BlockedMovementTime;
        private float m_BlockedMovementTimeLimit;
        private float m_BlockedMovementTolerance;
        private Vector3 m_LastPosition;
        private TimeThread m_TimeThread;

        public bool IsBlocked { get; private set; }

        public BlockedMovementSensor(
            float blockedMovementTimeLimit,
            float blockedMovementTolerance,
            Vector3 initialPosition,
            TimeThread timeThread)
        {
            m_BlockedMovementTimeLimit = blockedMovementTimeLimit;
            m_BlockedMovementTolerance = blockedMovementTolerance;
            m_LastPosition = initialPosition;
            m_TimeThread = timeThread;
        }

        public void Update(bool canMove, Vector3 currentPosition)
        {
            if (canMove && m_LastPosition.Ap(currentPosition, m_BlockedMovementTolerance))
            {
                m_BlockedMovementTime += m_TimeThread.deltaTime;
                if (m_BlockedMovementTime >= m_BlockedMovementTimeLimit)
                {
                    IsBlocked = true;
                    BlockedMovement?.Invoke(m_BlockedMovementTime);
                    m_BlockedMovementTime = 0;
                }
            }
            else
            {
                m_BlockedMovementTime = 0;
            }

            m_LastPosition = currentPosition;
        }
    }
}