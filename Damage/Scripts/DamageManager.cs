using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// SCRIPT EXECUTION ORDER: #08
    /// </summary>
    public class DamageManager : Manager, IDamageManager {

#region IDamageManager implementation: methods

        /// <summary>
        /// Displays the damage value for the target.
        /// The value will be parented to the target,
        /// and then will be offloaded at the target's last position at the time the target is destroyed.
        /// This overload is the most commonly used one.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="damage">Damage.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd {
            G.New(config.damageValuePrefab, target.transform).Init(target, damage);
        }

        /// <summary>
        /// Displays the damage value for the target.
        /// The value will be parented to the anchor (as specified),
        /// and then will be offloaded at the anchor's last position at the time the target is destroyed.
        /// This overload is useful when you need to attach the damage value to a sub-object of a target.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="anchor">Anchor (parent Transform).</param>
        /// <param name="damage">Damage.</param>
        public void DisplayDamageValue(IEnd target, Transform anchor, int damage) {
            G.New(config.damageValuePrefab, anchor).Init(target, damage);
        }

#endregion

    }
}
