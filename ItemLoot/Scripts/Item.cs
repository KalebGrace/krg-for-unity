using UnityEngine;

namespace KRG
{
    public class Item : MonoBehaviour
    {
        //TODO: later
        /*
         * MAKE SURE that an item has a (non-serialized) property called "owner" that it defaults to if auto-collected.
         * Also, the default of this default should be the player character.
         * When an Item is spawned via Loot, this should be a constructor parameter.
        */

        [Header("Item")]

        [SerializeField]
        public ItemData itemData = default;


        [Header("Visual Effects")]

        [SerializeField]
        protected Transform animatingBody = default;


        // non-serialized fields

        protected ISpawn spawner;


        // MonoBehaviour methods

        protected virtual void OnValidate()
        {
            if (itemData != null)
            {
                itemData.itemPrefab = this;
                //TODO: set as dirty?
            }
        }

        protected virtual void Start()
        {
            if (animatingBody != null) StartAnimateBody();
        }

        protected virtual void OnDestroy()
        {
            if (animatingBody != null) EndAnimateBody();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (OnCollect(other)) G.End(gameObject);
        }


        // custom methods

        public virtual void Init(ItemData itemData, ISpawn spawner)
        {
            if (this.itemData != null)
            {
                G.U.Warning("m_ItemData already contains {0}. Overwriting with {1}.", this.itemData, itemData);
            }

            this.itemData = itemData;
            this.spawner = spawner;
        }

        public virtual bool OnCollect(Collider other)
        {
            //TODO: differentiate between inventory items and instant-use items
            return OnUse(other);
        }

        public virtual bool OnUse(Collider other)
        {
            //TODO: differentiate between the character, etc. using the item
            if (other.gameObject.tag != "Player") return false;
            //TODO: do effectors as appropriate
            return true;
        }

        protected virtual void StartAnimateBody()
        {
        }

        protected virtual void EndAnimateBody()
        {
        }
    }
}
