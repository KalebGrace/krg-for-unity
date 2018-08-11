using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class CameraManager : Manager, ICameraManager {

#region IManager implementation

        public override void Awake() {
        }

#endregion

#region ICameraManager implementation: properties

        public virtual Camera camera { get { return Camera.main; } }

#endregion

#region ICameraManager implementation: methods

        public virtual void Shake(float duration) {
            G.U.Warning("Shake(...) is not implemented.");
        }

#endregion
        
    }
}
