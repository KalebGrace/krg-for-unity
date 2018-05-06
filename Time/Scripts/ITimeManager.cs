using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface ITimeManager {

#region methods

        ITimeThread GetTimeThread(int timeThreadIndex, System.Enum defaultTimeThreadInstance);

        ITimeThread GetTimeThread(System.Enum timeThreadInstance);

        ITimeThread GetTimeThread(int timeThreadIndex);

#endregion

    }
}
