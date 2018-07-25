using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Attacker: Attacker
    /// 1.  Attacker allows a game object to generate "attacks" from the supplied attack abilities.
    /// 2.  Attacker is to be added to a game object as a script/component*, and then assigned attack abilities
    ///     (references to scriptable objects instanced from AttackAbility). An attack is generated during an Update
    ///     whenever an assigned ability's input signature is executed (see AttackAbility._inputSignature and
    ///     InputSignature.isExecuted [to be implemented in a per-project derived class]). That said, there are certain
    ///     conditions that can deter generation of the attack (e.g. attack rate and attack limit).
    /// 3.  Attacker is a key component of the Attack system, and is used in conjunction with the following classes:
    ///     Attack, AttackAbility, AttackAbilityUse, AttackString, AttackTarget, and KnockBackCalcMode.
    /// 4.*-Attacker is abstract and must have a per-project derived class created;
    ///     the derived class itself must be added to a game object as a script/component.
    /// Last Refactor: 1.00.003 / 2018-07-15
    /// </summary>
    public abstract class Attacker : MonoBehaviour {

#region FIELDS: SERIALIZED

        [SerializeField, FormerlySerializedAs("m_attackAbilities")]
        protected AttackAbility[] _attackAbilities;

#endregion

#region FIELDS: PRIVATE

        SortedDictionary<InputSignature, AttackAbilityUse> _availableAttacks =
            new SortedDictionary<InputSignature, AttackAbilityUse>(new InputSignatureComparer());

        SortedDictionary<InputSignature, AttackAbilityUse> _availableAttacksBase =
            new SortedDictionary<InputSignature, AttackAbilityUse>(new InputSignatureComparer());

        Attack _currentAttack;

        AttackAbilityUse _queuedAttack;

#endregion

#region PROPERTIES

        public abstract bool isFlippedX { get; }

        public abstract bool isPlayerCharacter { get; }

#endregion

#region METHODS: MonoBehaviour

        protected virtual void Awake() {
            InitAvailableAttacks();
        }

        protected virtual void Update() {
            CheckInputAndTryAttack();
        }

#endregion

#region METHODS: PROTECTED & PRIVATE

        void InitAvailableAttacks() {
            AttackAbility aa;
            AttackAbilityUse aaUse;
            for (int i = 0; i < _attackAbilities.Length; ++i) {
                aa = _attackAbilities[i];
                aaUse = new AttackAbilityUse(aa, this);
                //TODO: this assumes every attack ability has a unique input signature
                //but this may need to be checked and enforced with a message on error
                _availableAttacks.Add(aa.inputSignature, aaUse);
                _availableAttacksBase.Add(aa.inputSignature, aaUse);
            }
        }

        void CheckInputAndTryAttack() {
            if (_queuedAttack != null) {
                return;
            }
            int tempComparerTestVariable = 999;
            InputSignature inputSig;
            AttackAbilityUse aaUse;
            foreach (KeyValuePair<InputSignature, AttackAbilityUse> kvPair in _availableAttacks) {
                inputSig = kvPair.Key;
                //begin temp InputSignatureComparer test
                G.U.Assert(inputSig.complexity <= tempComparerTestVariable);
                tempComparerTestVariable = inputSig.complexity;
                //end temp InputSignatureComparer test
                if (IsInputSignatureExecuted(inputSig)) {
                    aaUse = kvPair.Value;
                    //allow derived class to check conditions
                    if (IsAttackAbilityUseAvailable(aaUse)) {
                        //if this new attack is allowed to interrupt the current one, try the attack right away
                        //NOTE: doesInterrupt defaults to true for base attacks (see InitAvailableAttacks -> aaUse)
                        //otherwise, queue the attack to be tried when the current attack ends
                        if (aaUse.doesInterrupt) {
                            //try the attack; if successful, stop searching for attacks to try and just return
                            if (TryAttack(aaUse)) {
                                return;
                            }
                        } else {
                            _queuedAttack = aaUse;
                            return;
                        }
                    }
                }
            }
        }

        protected virtual bool IsInputSignatureExecuted(InputSignature inputSig) {
            return inputSig.isExecuted;
        }

        protected virtual bool IsAttackAbilityUseAvailable(AttackAbilityUse aaUse) {
            return true;
        }

        bool TryAttack(AttackAbilityUse aaUse) {
            Attack attack = aaUse.AttemptAttack();
            //if the attack attempt failed, return FALSE (otherwise, proceed)
            if (attack == null) {
                return false;
            }
            //the attack attempt succeeded!
            //first off, if we interrupted an attack (_currentAttack), remove its end callback
            if (_currentAttack != null) {
                _currentAttack.end.actions -= OnAttackEnd;
            }
            //now, set up the NEW current attack
            _currentAttack = attack;
            attack.end.actions += OnAttackEnd;
            attack.damageDealtHandler = OnDamageDealt;
            UpdateAvailableAttacks(attack);
            OnAttack(attack);
            //and since the attack attempt succeeded, return TRUE
            return true;
        }

        void UpdateAvailableAttacks(Attack attack) {
            _availableAttacks.Clear();
            var strings = attack.attackAbility.attackStrings;
            AttackString aString;
            AttackAbility aa;
            AttackAbilityUse aaUse;
            for (int i = 0; i < strings.Length; ++i) {
                //TODO:
                //1.  Open and close string during specifically-defined frame/second intervals using
                //    TimeTriggers/callbacks; for now, we just open immediately and close on destroy.
                //2.  Generate all possible AttackAbilityUse objects at init and
                //    just add/remove them to/from _availableAttacks as needed.
                aString = strings[i];
                aa = aString.attackAbility;
                aaUse = new AttackAbilityUse(aa, this, aString.doesInterrupt, attack);
                _availableAttacks.Add(aa.inputSignature, aaUse);
            }
        }

        void OnAttackEnd() {
            _currentAttack = null;
            //the current attack has ended, so try the queued attack; if successful, return (otherwise, proceed)
            if (_queuedAttack != null) {
                var aaUse = _queuedAttack;
                _queuedAttack = null;
                if (TryAttack(aaUse)) {
                    return;
                }
            }
            //we now have no current or queued attack, so revert back to our base dictionary of available attacks
            _availableAttacks.Clear();
            foreach (var inputSig in _availableAttacksBase.Keys) {
                _availableAttacks.Add(inputSig, _availableAttacksBase[inputSig]);
            }
        }

        protected virtual void OnAttack(Attack attack) {
            //can override with character state, graphics controller, and/or other code
        }

        protected virtual void OnDamageDealt(Attack attack, DamageTaker target) {
            //can override with character state, graphics controller, and/or other code
        }

#endregion

    }
}
