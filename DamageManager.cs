using UnityEngine;

namespace _0G.Legacy
{
    public class DamageManager : Manager
    {
        public override float priority => 80;

        public override void Awake() { }

        /// <summary>
        /// Displays the damage value for the target.
        /// The value will be parented to the target,
        /// and then will be offloaded at the target's last position at the time the target is destroyed.
        /// This overload is the most commonly used one.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="damage">Damage.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DisplayDamageValue(DamageTaker target, string damage)
        {
            DisplayDamageValue(target, target.transform, damage);
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
        public void DisplayDamageValue<T>(IDestroyedEvent<T> target, Transform anchor, string damage)
        {
            G.U.New(config.damageValuePrefab, anchor).Init(target, damage);
        }

        /// <summary>
        /// Gets (or creates) the HP Bar for the target.
        /// The HP Bar will be parented to the target.
        /// This overload is the most commonly used one.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="visRect">Optional VisRect, used for precise automatic positioning.</param>
        /// <returns>HP Bar.</returns>
        public HPBar GetHPBar(DamageTaker target, VisRect visRect = null)
        {
            if (visRect == null)
            {
                return GetHPBar(target, target.transform, Vector3.up);
            }

            return GetHPBar(target, visRect.transform, visRect.OffsetTop.Add(y: 0.1f));
        }

        /// <summary>
        /// Gets (or creates) the HP Bar for the target.
        /// The HP Bar will be parented to the anchor (as specified).
        /// This overload is useful when you need to attach the HP Bar to a sub-object of a target.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="anchor">Anchor (parent Transform).</param>
        /// <param name="offset">Positional offset. When in doubt, use Vector3.up.</param>
        /// <returns>HP Bar.</returns>
        public HPBar GetHPBar(DamageTaker target, Transform anchor, Vector3 offset)
        {
            var hpBar = anchor.GetComponentInChildren<HPBar>(true);
            if (hpBar == null)
            {
                hpBar = G.U.New(config.hpBarPrefab, anchor);
                hpBar.transform.localPosition = offset;
                hpBar.Init(target);
            }
            return hpBar;
        }
    }
}