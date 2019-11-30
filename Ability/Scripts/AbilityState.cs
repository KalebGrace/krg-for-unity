using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public sealed class AbilityState
    {
        // PROPERTIES

        public Ability Ability { get; private set; }

        public AbilityOwner AbilityOwner { get; private set; }

        public bool IsAbilityAvailable => Ability.IsAvailable(this);

        public bool IsOutsideOfChainScope { get; set; }

        public bool IsReady { get; set; } = true;

        private List<AbilityObject> AbilityObjects { get; set; } = new List<AbilityObject>();

        // CONSTRUCTOR

        public AbilityState(Ability ability, AbilityOwner owner)
		{
			Ability = ability;
            AbilityOwner = owner;
        }

        // PUBLIC METHODS

        /// <summary>
        /// Does this ability interrupt the ability from the provided object?
        /// </summary>
        public bool DoesInterrupt(AbilityObject obj)
        {
            if (obj == null) return true;

            List<AbilityChain> chains = obj.AbilityState.Ability.ChainedAbilities;
            if (chains != null)
            {
                for (int i = 0; i < chains.Count; ++i)
                {
                    AbilityChain c = chains[i];
                    if (c.Ability == Ability)
                    {
                        return c.DoesInterrupt;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Try to see if this ability is within the chain scope of the ability from the provided object.
        /// </summary>
        public void TryChain(AbilityObject obj)
        {
            List<AbilityChain> chains = obj.AbilityState.Ability.ChainedAbilities;
            if (chains != null)
            {
                for (int i = 0; i < chains.Count; ++i)
                {
                    AbilityChain c = chains[i];
                    if (c.Ability == Ability)
                    {
                        IsOutsideOfChainScope = false;
                        return;
                    }
                }
            }
            IsOutsideOfChainScope = true;
        }

        /// <summary>
        /// Try to use this ability, and return an object if successful.
        /// </summary>
        public AbilityObject TryUse()
        {
            if (IsReady)
            {
                if (!Ability.ObjectLimit.boolValue || AbilityObjects.Count < Ability.ObjectLimit.intValue)
                {
                    Ability.ConsumeResources(this);

                    Ability.PlayUseSound(this);

                    Transform ownerTF = AbilityOwner.Body.transform;

                    AbilityObject obj = G.U.New(Ability.ObjectPrefab, Ability.IsJoinedToOwner ? ownerTF : null);

                    Transform objTF = obj.transform;
                    objTF.position = ownerTF.position;
                    objTF.rotation = ownerTF.rotation;

                    AbilityObjects.Add(obj);
                    obj.Destroyed += RemoveObject;

                    obj.InitAbility(this);

                    return obj;
                }
            }
            return null;
        }

        // PRIVATE METHODS

        private void RemoveObject(AbilityObject obj)
        {
            AbilityObjects.Remove(obj);
        }
    }
}
