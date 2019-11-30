using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public sealed class AbilityOwner : MonoBehaviour, IBodyComponent
    {
        // LIMITATIONS

        // only one ability usable at a time

        // SERIALIZED FIELDS

        [Header("Non-Character Data")]

        [SerializeField]
        private List<Ability> m_Abilities;

        // NON-SERIALIZED FIELDS

        [System.NonSerialized]
        public EventAction<AbilityObject> AbilityStarted;
        [System.NonSerialized]
        public EventAction<AbilityObject> AbilityStopped;

        private Dictionary<int, AbilityState> m_AbilityStates = new Dictionary<int, AbilityState>();

        private AbilityObject m_CurrentAbilityObject;

        private SortedDictionary<InputSignature, AbilityState> m_InputAbilities =
            new SortedDictionary<InputSignature, AbilityState>(new InputSignatureComparer());

        private bool m_IsOwnerAnimating;

        private AbilityState m_QueuedAbility;

        // PROPERTIES

        public List<Ability> Abilities { get; private set; }

        public List<InputSignature> AI_Inputs { get; } = new List<InputSignature>();

        public GameObjectBody Body { get; private set; }

        private GraphicController GraphicController => Body.Refs.GraphicController;

        // INIT METHODS

        public void InitBody(GameObjectBody body)
        {
            Body = body;

            if (body.IsCharacter)
            {
                InitAbilities(Body.CharacterDossier.Abilities);
            }
            else
            {
                InitAbilities(m_Abilities);
            }
        }

        private void InitAbilities(List<Ability> abilities)
        {
            Abilities = abilities;

            m_InputAbilities.Clear();

            for (int i = 0; i < abilities.Count; ++i)
            {
                Ability ability = abilities[i];
                InputSignature sig = ability.InputSignature;

                if (sig == null)
                {
                    G.U.Err("Missing input signature for {0}.", ability.name);
                }
                else if (m_InputAbilities.ContainsKey(sig))
                {
                    G.U.Err("Duplicate input signature key {0} for {1} & {2}.",
                        sig, ability.name, m_InputAbilities[sig].Ability.name);
                }
                else
                {
                    AbilityState abil = new AbilityState(ability, this);
                    m_AbilityStates.Add(ability.AbilityID, abil);
                    m_InputAbilities.Add(sig, abil);
                }
            }
        }

        // MONOBEHAVIOUR METHODS

        private void Update()
        {
            CheckInputAndTryAbility();
        }

        // MAIN PUBLIC & PRIVATE METHODS

        private void CheckInputAndTryAbility()
        {
            if (m_QueuedAbility != null) return;

            foreach (KeyValuePair<InputSignature, AbilityState> pair in m_InputAbilities)
            {
                if (pair.Key.IsInputExecuted(this))
                {
                    AbilityState abil = pair.Value;

                    if (abil.IsAbilityAvailable)
                    {
                        if (abil.DoesInterrupt(m_CurrentAbilityObject))
                        {
                            if (TryAbility(abil)) return;
                        }
                        else
                        {
                            m_QueuedAbility = abil;
                            return;
                        }
                    }
                }
            }
        }

        public void TryAbility(int abilityID)
        {
            TryAbility(m_AbilityStates[abilityID]);
        }

        private bool TryAbility(AbilityState abilityState)
        {
            AbilityObject obj = abilityState.TryUse();

            if (obj == null) return false; // attempt failed

            // if reaching this point, ability use succeeded, creating a new object
            // interrupt the current ability (if applicable)

            EndCurrentAbility(false);

            m_CurrentAbilityObject = obj;
            obj.Destroyed += OnAbilityObjectDestroy;

            foreach (AbilityState abil in m_InputAbilities.Values)
            {
                abil.TryChain(obj);
            }

            AbilityStarted.Invoke(obj);

            m_IsOwnerAnimating = true;
            RasterAnimation ownerAnimation = abilityState.Ability.GetRandomOwnerAnimation();
            GraphicController.SetAnimation(AnimationContext.Ability, ownerAnimation, OnOwnerAnimationEnd);

            return true;
        }

        public void EndCurrentAbility(bool isCompleted)
        {
            AbilityObject obj = m_CurrentAbilityObject;

            if (obj == null) return;

            if (m_IsOwnerAnimating)
            {
                m_IsOwnerAnimating = false;
                GraphicController.EndAnimation(AnimationContext.Ability);
            }

            obj.Deactivate(isCompleted);

            AbilityStopped.Invoke(obj);

            obj.Destroyed -= OnAbilityObjectDestroy;

            if (obj.AbilityState.Ability.IsJoinedToOwner)
            {
                obj.Dispose(isCompleted);
            }
            m_CurrentAbilityObject = null;

            // no current ability
            // try the queued ability

            if (m_QueuedAbility != null)
            {
                AbilityState abil = m_QueuedAbility;
                m_QueuedAbility = null;
                if (TryAbility(abil)) return;
            }

            // no current or queued ability
            // revert abilities to default

            foreach (AbilityState abil in m_InputAbilities.Values)
            {
                abil.IsOutsideOfChainScope = false;
            }
        }

        // PRIVATE HANDLER METHODS

        private void OnAbilityObjectDestroy(AbilityObject obj)
        {
            G.U.Assert(obj == m_CurrentAbilityObject);

            // the object was destroyed by external forces
            // end the current ability now

            EndCurrentAbility(obj.IsCompleted);
        }

        private void OnOwnerAnimationEnd(GraphicController graphicController, bool isCompleted)
        {
            // check m_IsOwnerAnimating to avoid looping via GraphicController.EndAnimation or EndCurrentAbility

            if (m_IsOwnerAnimating)
            {
                m_IsOwnerAnimating = false;
                EndCurrentAbility(isCompleted);
            }
        }
    }
}
