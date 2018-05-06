using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// SCRIPT EXECUTION ORDER: #08
    /// </summary>
    public class DamageManager : Manager, IDamageManager {

#region IDamageManager implementation: methods

        public void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd {
            G.New(config.damageValuePrefab, target.transform).Init(target, damage);
        }

#endregion

    }
}
