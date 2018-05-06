using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface ICameraManager {

#region properties

        /// <summary>
        /// Gets the active camera (typically the main camera).
        /// </summary>
        /// <value>The active camera.</value>
        Camera camera { get; }

#endregion

#region methods

        /// <summary>
        /// Shake the active camera for the specified duration.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        void Shake(float duration);

#endregion

    }
}
