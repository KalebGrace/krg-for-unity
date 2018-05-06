using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IObjectManager {

#region methods

        void AddDestroyRequest(System.Action request);

#endregion

    }
}
