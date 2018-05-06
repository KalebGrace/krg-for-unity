using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IDamageManager {

#region methods

        void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd;

#endregion

    }
}
