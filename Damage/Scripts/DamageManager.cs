using UnityEngine;

namespace KRG
{
    public class DamageManager : Manager
    {
        public override float priority { get { return 80; } }

        public override void Awake()
        {
        }

        /// <summary>
        /// Displays the damage value for the target.
        /// The value will be parented to the target,
        /// and then will be offloaded at the target's last position at the time the target is destroyed.
        /// This overload is the most commonly used one.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="damage">Damage.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd
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
        public void DisplayDamageValue(IEnd target, Transform anchor, int damage)
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
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <returns>HP Bar.</returns>
        public HPBar GetHPBar<T>(T target, VisRect visRect = null) where T : MonoBehaviour, IDamageable
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
        public HPBar GetHPBar(IDamageable target, Transform anchor, Vector3 offset)
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
