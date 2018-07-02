using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Attack ability use.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    public sealed class AttackAbilityUse {

#region private fields

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

#region properties

        public AttackAbility attackAbility { get { return _attackAbility; } }

        public bool doesInterrupt { get { return _doesInterrupt; } }

        public bool isEnabled { get { return _isEnabled; } set { _isEnabled = value; } }

        public Attack originAttack { get { return _originAttack; } }

#endregion

#region constructors

        public AttackAbilityUse(
            AttackAbility attackAbility,
            Attacker attacker,
            bool doesInterrupt = true,
            Attack originAttack = null
        ) {
            _attackAbility = attackAbility;
            _attacker = attacker;
            _doesInterrupt = doesInterrupt;
            _originAttack = originAttack;

            G.U.Assert(_attackAbility.attackPrefab != null,
                string.Format("Attack Prefab must be set on {0} (AttackAbility).", attackAbility.name));

            _attacks = new List<Attack>(_attackAbility.attackLimit);
        }

#endregion

#region public methods

        public Attack AttemptAttack() {
            if (_isAttackReady && isEnabled && _attacks.Count < _attackAbility.attackLimit) {
                //TODO: add more _if_ conditions
                //E.G. SecS: obj_pc.object.gmx "//Firing (Shooting)."
                _isAttackReady = false;
                //TODO: also set m_isAttackReady or isEnabled to false on related attack abilites
                //E.G. SecS: blocking resets firing, and disables its trigger too
                _attackAbility.timeThread.AddTrigger(_attackAbility.attackRateSec, ReadyAttack);
                return Attack();
            } else {
                return null;
            }
        }

#endregion

#region private methods

        Attack Attack() {
            //instantiate attack GameObject using transform options
            Transform akerTF = _attacker.transform;
            Attack a = G.New(_attackAbility.attackPrefab, _attackAbility.isJoinedToAttacker ? akerTF : null);
            Transform attaTF = a.transform;
            attaTF.position = akerTF.position;
            attaTF.rotation = akerTF.rotation;

            //finish setting up attack
            a.Init(_attackAbility, _attacker);

            //track attack
            _attacks.Add(a);
            a.end.actions += () => _attacks.Remove(a);

            //return attack for external management
            return a;
        }

        void ReadyAttack(TimeTrigger tt) {
            _isAttackReady = true;
        }

#endregion

    }
}
