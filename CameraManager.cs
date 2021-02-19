using UnityEngine;

namespace _0G.Legacy
{
    public class CameraManager : Manager
    {
        public override float priority { get { return 90; } }

        public override void Awake() { }

        /// <summary>
        /// Gets the active camera (typically the main camera).
        /// </summary>
        /// <value>The active camera.</value>
        public virtual Camera camera { get { return Camera.main; } }

        /// <summary>
        /// Shake the active camera for the specified duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        public virtual void Shake(float duration)
        {
            G.U.Warn("Shake(...) is not implemented.");
        }
    }
}