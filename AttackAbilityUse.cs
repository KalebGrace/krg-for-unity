using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// AttackAbilityUse: Attack Ability Use
    /// 1.  AttackAbilityUse is somewhat of a "wrapper" class for AttackAbility, and is mainly used to track
    ///     runtime information about the attacks that have been generated from an AttackAbility,
    ///     as well as determining when new attacks from that ability are allowed to be generated.
    /// 2.  AttackAbilityUse should generally only be instanced by the Attacker class.
    /// 3.  AttackAbilityUse is part of the Attack system, and is used in conjunction with the following classes:
    ///     Attack, AttackAbility, Attacker, AttackString, AttackTarget, and KnockBackCalcMode.
    /// 4.  AttackAbilityUse is sealed and currently has no extensibility.
    /// </summary>
    public sealed class AttackAbilityUse
    {
        #region FIELDS

        //attack ability scriptable object
        AttackAbility _attackAbility;

        //attacker component, i.e. source of attack
        Attacker _attacker;

        //list of attack object instances
        List<Attack> _attacks;

        //does this have the right to interrupt the current attack?
        bool _doesInterrupt = true;

        //is attack ready (available) to use?
        bool _isAttackReady = true;

        //is use of this attack ability currently enabled?
        bool _isEnabled = true;

        //attack that opened up the use of this new ability
        Attack _originAttack;

        #endregion

        #region PROPERTIES

        public AttackAbility attackAbility { get { return _attackAbility; } }

        public bool doesInterrupt { get { return _doesInterrupt; } }

        //TODO: no longer used; candidate for removal
        public bool isEnabled { get { return _isEnabled; } set { _isEnabled = value; } }

        //TODO: no longer used; candidate for removal
        public Attack originAttack { get { return _originAttack; } }

        #endregion

        #region CONSTRUCTORS

        public AttackAbilityUse(
            AttackAbility attackAbility,
            Attacker attacker,
            bool doesInterrupt = true,
            Attack originAttack = null
        )
        {
            _attackAbility = attackAbility;
            _attacker = attacker;
            _doesInterrupt = doesInterrupt;
            _originAttack = originAttack;

            G.U.Assert(_attackAbility.attackPrefab != null,
                string.Format("Attack Prefab must be set on {0} (AttackAbility).", attackAbility.name));

            _attacks = new List<Attack>(_attackAbility.attackLimit);
        }

        #endregion

        #region METHODS : PUBLIC

        public Attack AttemptAttack()
        {
            if (_isAttackReady && _isEnabled && _attacks.Count < _attackAbility.attackLimit)
            {
                //TODO: add more _if_ conditions
                //E.G. SecS: obj_pc.object.gmx "//Firing (Shooting)."
                _isAttackReady = false;
                //TODO: also set _isAttackReady or _isEnabled to false on related attack abilites
                //E.G. SecS: blocking resets firing, and disables its trigger too
                _attackAbility.TimeThread.AddTrigger(_attackAbility.attackRateSec, ReadyAttack);
                return Attack();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region METHODS : PRIVATE

        Attack Attack()
        {
            //instantiate attack GameObject using transform options
            Transform akerTF = _attacker.transform;
            Attack a = G.U.New(_attackAbility.attackPrefab, _attackAbility.isJoinedToAttacker ? akerTF : null);
            Transform attaTF = a.transform;
            attaTF.position = akerTF.position;
            attaTF.rotation = akerTF.rotation;

            //track attack
            _attacks.Add(a);
            a.Destroyed += RemoveAttack;

            //finish setting up attack
            a.Init(_attackAbility, _attacker);

            //return attack for external management
            return a;
        }

        private void ReadyAttack(TimeTrigger tt)
        {
            _isAttackReady = true;
        }

        private void RemoveAttack(Attack a)
        {
            _attacks.Remove(a);
        }

        #endregion
    }
}