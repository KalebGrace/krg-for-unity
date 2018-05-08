using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IDamageManager {

#region methods

        void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd;

        void DisplayDamageValue(IEnd target, Transform anchor, int damage);

        HPBar GetHPBar<T>(T target) where T : MonoBehaviour, IDamageable;

#endregion

    }
}
