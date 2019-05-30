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

        public enum AttackState { STARTED, INTERRUPTED, COMPLETED }

        public event AttackEventHandler attackStateChanged;

        public delegate void AttackEventHandler(Attack attack, AttackState state);

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

        Dictionary<int, InputSignature> _inputEzKeySigMap = new Dictionary<int, InputSignature>();

        AttackAbilityUse _queuedAttack;

#endregion

#region PROPERTIES

        public Character Character { get; protected set; }

        public GraphicsController GraphicsController { get; protected set; }

        public abstract bool isFlippedX { get; }

        public abstract bool isPlayerCharacter { get; }

#endregion

#region METHODS: MonoBehaviour

        protected virtual void Awake()
        {
            InitProperties();

            InitAvailableAttacks();
        }

        protected virtual void Update()
        {
            CheckInputAndTryAttack();
        }

#endregion

#region METHODS: PROTECTED & PRIVATE

        protected virtual void InitProperties()
        {
            GraphicsController = GetComponent<GraphicsController>();
            Character = GraphicsController?.GetComponent<Character>();
        }

        void InitAvailableAttacks()
        {
            for (int i = 0; i < _attackAbilities.Length; ++i)
            {
                var aa = _attackAbilities[i];
                var sig = aa.inputSignature;
                if (sig == null)
                {
                    G.Err("Missing input signature for {0}.", aa.name);
                }
                else if (_availableAttacks.ContainsKey(sig))
                {
                    G.Err("Duplicate input signature key {0} for {1} & {2}.",
                        sig, aa.name, _availableAttacks[sig].attackAbility.name);
                }
                else
                {
                    var aaUse = new AttackAbilityUse(aa, this);
                    _availableAttacks.Add(sig, aaUse);
                    _availableAttacksBase.Add(sig, aaUse);
                    if (sig.hasEzKey) _inputEzKeySigMap.Add(sig.ezKey, sig);
                }
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
                            if (_TryAttack(aaUse)) {
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

        protected virtual void AttackViaInputEzKey(int ezKey)
        {
            var sig = _inputEzKeySigMap[ezKey];
            var aaUse = _availableAttacksBase[sig];
            _TryAttack(aaUse);
        }

        protected virtual bool IsInputSignatureExecuted(InputSignature inputSig)
        {
            return inputSig.IsExecuted(this);
        }

        protected virtual bool IsAttackAbilityUseAvailable(AttackAbilityUse aaUse)
        {
            return true;
        }

        bool _TryAttack(AttackAbilityUse aaUse)
        {
            Attack attack = aaUse.AttemptAttack();
            //if the attack attempt failed, return FALSE (otherwise, proceed)
            if (attack == null) return false;
            //the attack attempt succeeded!

            //first, interrupt the current attack (if applicable)
            InterruptCurrentAttack();
            //now, set up the NEW current attack
            _currentAttack = attack;
            attack.end.actions += _OnAttackCompleted;
            attack.damageDealtHandler = OnDamageDealt;
            UpdateAvailableAttacks(attack);
            OnAttack(attack);
            if (attackStateChanged != null) attackStateChanged(attack, AttackState.STARTED);
            //and since the attack attempt succeeded, return TRUE
            return true;
        }

        void InterruptCurrentAttack()
        {
            if (_currentAttack == null) return;
            //remove the end callback
            _currentAttack.end.actions -= _OnAttackCompleted;
            //fire the state changed event
            if (attackStateChanged != null) attackStateChanged(_currentAttack, AttackState.INTERRUPTED);
            //set null
            _currentAttack = null;
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

        void _OnAttackCompleted() {
            if (attackStateChanged != null) attackStateChanged(_currentAttack, AttackState.COMPLETED);
            _currentAttack = null;
            //the current attack has ended, so try the queued attack; if successful, return (otherwise, proceed)
            if (_queuedAttack != null) {
                var aaUse = _queuedAttack;
                _queuedAttack = null;
                if (_TryAttack(aaUse)) {
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
