using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Attacker: Attacker
    /// 1. Attacker allows a game object to generate "attacks" from the supplied attack abilities.
    /// 2. Attacker is to be added to a game object as a script/component*, and then assigned attack abilities
    /// (references to scriptable objects instanced from AttackAbility). An attack is generated during an Update
    /// whenever an assigned ability's input signature is executed (see AttackAbility._inputSignature and
    /// InputSignature.isExecuted [to be implemented in a per-project derived class]). That said, there are certain
    /// conditions that can deter generation of the attack (e.g. attack rate and attack limit).
    /// 3. Attacker is a key component of the Attack system, and is used in conjunction with the following classes:
    /// Attack, AttackAbility, AttackAbilityUse, AttackString, AttackTarget, and KnockBackCalcMode.
    /// 4. *Attacker is abstract and must have a per-project derived class created;
    /// the derived class itself must be added to a game object as a script/component.
    /// Last Refactor: 1.00.003 / 2018-07-15
    /// </summary>
    public abstract class Attacker : MonoBehaviour {

#region FIELDS: SERIALIZED

        [SerializeField, FormerlySerializedAs("m_attackAbilities")]
        protected AttackAbility[] _attackAbilities;

#endregion

#region FIELDS: PRIVATE

        SortedDictionary<InputSignature, List<AttackAbilityUse>> _availableAttacks =
            new SortedDictionary<InputSignature, List<AttackAbilityUse>>(new InputSignatureComparer());

        AttackAbilityUse _queuedString;

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
            List<AttackAbilityUse> list;
            for (int i = 0; i < _attackAbilities.Length; i++) {
                aa = _attackAbilities[i];
                aaUse = new AttackAbilityUse(aa, this);
                list = new List<AttackAbilityUse>(1) { aaUse };
                //TODO: this assumes every attack ability has a unique input signature
                //but this may need to be checked and enforced with a message on error
                _availableAttacks.Add(aa.inputSignature, list);
            }
        }

        void CheckInputAndTryAttack() {
            if (_queuedString != null) {
                return;
            }
            int tempComparerTestVariable = 999;
            InputSignature inputSig;
            List<AttackAbilityUse> list;
            AttackAbilityUse aaUse;
            foreach (KeyValuePair<InputSignature, List<AttackAbilityUse>> kvPair in _availableAttacks) {
                inputSig = kvPair.Key;
                //begin temp InputSignatureComparer test
                G.U.Assert(inputSig.complexity <= tempComparerTestVariable);
                tempComparerTestVariable = inputSig.complexity;
                //end temp InputSignatureComparer test
                if (IsInputSignatureExecuted(inputSig)) {
                    list = kvPair.Value;
                    //start with most recently added item and work backwards (like a stack, but with greater control)
                    for (int i = list.Count - 1; i >= 0; i--) {
                        aaUse = list[i];
                        //allow derived class to check conditions
                        if (IsAttackAbilityUseAvailable(aaUse)) {
                            if (aaUse.doesInterrupt) {
                                if (TryAttack(aaUse)) {
                                    return;
                                }
                            } else {
                                _queuedString = aaUse;
                                return;
                            }
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
            if (attack != null) {
                //TODO: maybe kill the aaUse.originAttack here if it cancels current attack;
                //but we also have to think of the possibility of queued attacks
                attack.damageDealtHandler = OnDamageDealt;
                LinkStrings(attack);
                OnAttack(attack);
                return true;
            }
            return false;
        }

        void LinkStrings(Attack attack) {
            AttackString[] strings = attack.attackAbility.attackStrings;
            AttackString aString;
            for (int i = 0; i < strings.Length; i++) {
                aString = strings[i];
                //TODO: open and close string during specifically-defined frame/second intervals using
                //TimeTriggers/callbacks; for now, we just open immediately and close on destroy
                OpenString(attack, aString);
                attack.end.actions += () => CloseString(attack, aString);
            }
        }

        void OpenString(Attack originAttack, AttackString aString) {
            //disable the origin attack's ability use
            List<AttackAbilityUse> list;
            AttackAbilityUse aaUse;
            bool isOriginDisabled = false;
            foreach (KeyValuePair<InputSignature, List<AttackAbilityUse>> kvPair in _availableAttacks) {
                list = kvPair.Value;
                //start with most recently added item and work backwards (like a stack, but with greater control)
                for (int i = list.Count - 1; i >= 0; i--) {
                    aaUse = list[i];
                    if (aaUse.attackAbility == originAttack.attackAbility) {
                        aaUse.isEnabled = false;
                        isOriginDisabled = true;
                        break;
                    }
                }
                if (isOriginDisabled) break;
            }
            if (!isOriginDisabled) {
                G.U.Error("Origin attack's ability use not disabled!");
            }
            //open the string
            AttackAbility aa = aString.attackAbility;
            InputSignature inputSig = aa.inputSignature;
            if (!_availableAttacks.ContainsKey(inputSig)) {
                _availableAttacks.Add(inputSig, new List<AttackAbilityUse>(1));
            }
            aaUse = new AttackAbilityUse(aa, this, aString.doesInterrupt, originAttack);
            _availableAttacks[inputSig].Add(aaUse);
        }

        void CloseString(Attack originAttack, AttackString aString) { //TODO: why is aString unused?
            //close the string & re-enable the origin attack's ability use
            List<AttackAbilityUse> list;
            AttackAbilityUse aaUse;
            InputSignature inputSigToRemove = null;
            bool isStringClosed = false;
            bool isOriginEnabled = false;
            foreach (KeyValuePair<InputSignature, List<AttackAbilityUse>> kvPair in _availableAttacks) {
                list = kvPair.Value;
                //start with most recently added item and work backwards (like a stack, but with greater control)
                for (int i = list.Count - 1; i >= 0; i--) {
                    aaUse = list[i];
                    if (!isStringClosed && aaUse.originAttack == originAttack) {
                        if (_queuedString == aaUse) {
                            TryAttack(aaUse);
                            _queuedString = null;
                        }
                        if (list.Count > 1) {
                            list.RemoveAt(i);
                        } else {
                            inputSigToRemove = kvPair.Key;
                        }
                        isStringClosed = true;
                    } else if (!isOriginEnabled && aaUse.attackAbility == originAttack.attackAbility) {
                        aaUse.isEnabled = true;
                        isOriginEnabled = true;
                    }
                    if (isStringClosed && isOriginEnabled) break;
                }
                if (isStringClosed && isOriginEnabled) break;
            }
            if (inputSigToRemove != null) {
                _availableAttacks.Remove(inputSigToRemove);
            } else if (!isStringClosed) {
                G.U.Error("No string closed!");
            }
            //it's possible for the origin attack's ability use to be closed before it can be re-enabled,
            //so don't throw an error if isOriginEnabled is false
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
