using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Time thread instance.
    /// See TimeManager for usage and restrictions.
    /// </summary>
    public enum TimeThreadInstance {

        //no time thread preference; use the script's default time thread instead
        UseDefault = -1,

        //always unscaled; never paused
        Application = 0,

        //should be used for functionality that can be paused, slowed, or sped up
        Gameplay = 1,
    }
}
