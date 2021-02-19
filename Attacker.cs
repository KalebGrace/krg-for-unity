using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
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
    /// </summary>
    public class Attacker : MonoBehaviour, IBodyComponent
    {
        // DELEGATES

        public delegate void AttackStartHandler(Attack attack);
        public delegate void AttackEndHandler(Attack attack, bool isCompleted);

        // EVENTS

        public event AttackStartHandler AttackStarted;
        public event AttackEndHandler AttackEnded;

        // SERIALIZED FIELDS

        [SerializeField, FormerlySerializedAs("m_attackAbilities")]
        protected AttackAbility[] _attackAbilities;

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PRIVATE FIELDS

        private SortedDictionary<InputSignature, AttackAbilityUse> _availableAttacks =
            new SortedDictionary<InputSignature, AttackAbilityUse>(new InputSignatureComparer());

        private SortedDictionary<InputSignature, AttackAbilityUse> _availableAttacksBase =
            new SortedDictionary<InputSignature, AttackAbilityUse>(new InputSignatureComparer());

        private Dictionary<int, InputSignature> _inputEzKeySigMap = new Dictionary<int, InputSignature>();

        private AttackAbilityUse _queuedAttack;

        private bool m_IsAttackerAnimating;

        // STORAGE PROPERTIES

        public Attack CurrentAttack { get; private set; }

        // SHORTCUT PROPERTIES

        public GameObjectBody Body => m_Body;

        private GraphicController GraphicController => m_Body.Refs.GraphicController;

        // INIT METHOD

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake()
        {
            InitAvailableAttacks();
        }

        protected virtual void Update()
        {
            CheckInputAndTryAttack();
        }

        // MAIN METHODS

        private void InitAvailableAttacks()
        {
            for (int i = 0; i < _attackAbilities.Length; ++i)
            {
                var aa = _attackAbilities[i];
                var sig = aa.inputSignature;
                if (sig == null)
                {
                    G.U.Err("Missing input signature for {0}.", aa.name);
                }
                else if (_availableAttacks.ContainsKey(sig))
                {
                    G.U.Err("Duplicate input signature key {0} for {1} & {2}.",
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

        private void CheckInputAndTryAttack()
        {
            if (_queuedAttack != null)
            {
                return;
            }
            int tempComparerTestVariable = 999;
            InputSignature inputSig;
            AttackAbilityUse aaUse;
            foreach (KeyValuePair<InputSignature, AttackAbilityUse> kvPair in _availableAttacks)
            {
                inputSig = kvPair.Key;
                //begin temp InputSignatureComparer test
                G.U.Assert(inputSig.complexity <= tempComparerTestVariable);
                tempComparerTestVariable = inputSig.complexity;
                //end temp InputSignatureComparer test
                if (IsInputSignatureExecuted(inputSig))
                {
                    aaUse = kvPair.Value;
                    //allow derived class to check conditions
                    if (IsAttackAbilityUseAvailable(aaUse))
                    {
                        //if this new attack is allowed to interrupt the current one, try the attack right away
                        //NOTE: doesInterrupt defaults to true for base attacks (see InitAvailableAttacks -> aaUse)
                        //otherwise, queue the attack to be tried when the current attack ends
                        if (aaUse.doesInterrupt)
                        {
                            //try the attack; if successful, stop searching for attacks to try and just return
                            if (TryAttack(aaUse))
                            {
                                return;
                            }
                        }
                        else
                        {
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
            TryAttack(aaUse);
        }

        protected virtual bool IsInputSignatureExecuted(InputSignature inputSig)
        {
            return inputSig.IsExecuted(this);
        }

        protected virtual bool IsAttackAbilityUseAvailable(AttackAbilityUse aaUse)
        {
            return true;
        }

        private bool TryAttack(AttackAbilityUse aaUse)
        {
            Attack attack = aaUse.AttemptAttack();
            //if the attack attempt failed, return FALSE...
            if (attack == null) return false;
            //...otherwise, the attack attempt succeeded!

            //first, interrupt the current attack (if applicable)
            EndCurrentAttack(false);

            //now, set up the NEW current attack
            CurrentAttack = attack;
            attack.Destroyed += OnAttackDestroy;
            attack.damageDealtHandler = OnDamageDealt;
            UpdateAvailableAttacks(attack);
            //call optional derived functionality (high priority), then fire event as applicable
            OnAttackStart(attack);
            AttackStarted?.Invoke(attack);
            //set attacker animation
            m_IsAttackerAnimating = true;
            RasterAnimation attackerAnimation = attack.attackAbility.GetRandomAttackerRasterAnimation();
            GraphicController.SetAnimation(AnimationContext.Attack, attackerAnimation, OnAttackerAnimationEnd);
            //and since the attack attempt succeeded, return TRUE
            return true;
        }

        protected void EndCurrentAttack(bool isCompleted)
        {
            Attack attack = CurrentAttack;
            //if there is no current attack, return
            if (attack == null) return;

            //unset attacker animation
            if (m_IsAttackerAnimating)
            {
                m_IsAttackerAnimating = false;
                GraphicController.EndAnimation(AnimationContext.Attack);
            }
            //call optional derived functionality (high priority), then fire event as applicable
            OnAttackEnd(attack, isCompleted);
            AttackEnded?.Invoke(attack, isCompleted);
            //shut down the current attack
            attack.Destroyed -= OnAttackDestroy;
            if (attack != null && attack.attackAbility.isJoinedToAttacker)
            {
                attack.Dispose(isCompleted);
            }
            CurrentAttack = null;

            //we now have no current attack, so try the queued attack; if successful, return (otherwise, proceed)
            if (_queuedAttack != null)
            {
                var aaUse = _queuedAttack;
                _queuedAttack = null;
                if (TryAttack(aaUse))
                {
                    return;
                }
            }
            //we now have no current or queued attack, so revert back to our base dictionary of available attacks
            _availableAttacks.Clear();
            foreach (var inputSig in _availableAttacksBase.Keys)
            {
                _availableAttacks.Add(inputSig, _availableAttacksBase[inputSig]);
            }
        }

        private void UpdateAvailableAttacks(Attack attack)
        {
            _availableAttacks.Clear();
            AttackString[] strings = attack.attackAbility.attackStrings;
            for (int i = 0; i < strings.Length; ++i)
            {
                //TODO:
                //1.  Open and close string during specifically-defined frame/second intervals using
                //    TimeTriggers/callbacks; for now, we just open immediately and close on destroy.
                //2.  Generate all possible AttackAbilityUse objects at init and
                //    just add/remove them to/from _availableAttacks as needed.
                AttackString aString = strings[i];
                AttackAbility aa = aString.AttackAbility;
                AttackAbilityUse aaUse = new AttackAbilityUse(aa, this, aString.DoesInterrupt, attack);
                _availableAttacks.Add(aa.inputSignature, aaUse);
            }
        }

        private void OnAttackerAnimationEnd(GraphicController graphicController, bool isCompleted)
        {
            //check m_IsAttackerAnimating to avoid looping via GraphicController.EndAnimation/EndCurrentAttack
            if (m_IsAttackerAnimating)
            {
                m_IsAttackerAnimating = false;
                EndCurrentAttack(isCompleted);
            }
        }

        private void OnAttackDestroy(Attack attack)
        {
            G.U.Assert(attack == CurrentAttack);
            //the current attack has been destroyed by external forces, so end it
            EndCurrentAttack(attack.IsCompleted);
        }

        protected virtual void OnAttackStart(Attack attack) { }

        protected virtual void OnAttackEnd(Attack attack, bool isCompleted) { }

        protected virtual void OnDamageDealt(Attack attack, DamageTaker target) { }
    }
}