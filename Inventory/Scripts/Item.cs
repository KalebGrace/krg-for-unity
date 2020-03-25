using UnityEngine;

namespace KRG
{
    public class Item : MonoBehaviour, ICollider
    {
        // STATIC EVENTS

        public static event System.Action<Item, Collider> ItemCollected;

        //TODO: later
        /*
         * MAKE SURE that an item has a (non-serialized) property called "owner" that it defaults to if auto-collected.
         * Also, the default of this default should be the player character.
         * When an Item is spawned via Loot, this should be a constructor parameter.
        */

        // SERIALIZED FIELDS

        [Header("Item")]

        public ItemData itemData = default;

        [SerializeField]
        protected int m_ID = default;

        [Header("Visual Effects")]

        [SerializeField]
        protected Transform animatingBody = default;

        // NON-SERIALIZED FIELDS

        protected ISpawn spawner;

        // PROPERTIES

        public int ID => m_ID;

        // MONOBEHAVIOUR METHODS

        protected virtual void OnValidate()
        {
            if (m_ID == 0)
            {
                m_ID = GetInstanceID();
            }

            if (itemData != null)
            {
                itemData.itemPrefab = this;
                //TODO: set as dirty?
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            if (itemData.IsKeyItem && G.inv.HasKeyItem(itemData.KeyItemID))
            {
                // player already has this key item
                gameObject.Dispose();
                return;
            }

            if (animatingBody != null) StartAnimateBody();
        }

        protected virtual void OnDestroy()
        {
            if (animatingBody != null) EndAnimateBody();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter(this, other);
        }
        public virtual void OnTriggerEnter(MonoBehaviour source, Collider other)
        {
            if (source.GetComponent<Collider>().isTrigger && OnCollect(other))
            {
                ItemCollected?.Invoke(this, other);
                if (!string.IsNullOrWhiteSpace(itemData.sfxFmodEventOnCollect))
                {
                    G.audio.PlaySFX(itemData.sfxFmodEventOnCollect, transform.position);
                }
                gameObject.Dispose();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit(this, other);
        }
        public virtual void OnTriggerExit(MonoBehaviour source, Collider other) { }

        // CUSTOM METHODS

        public virtual void Init(ItemData itemData, ISpawn spawner)
        {
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

        protected virtual void StartAnimateBody() { }

        protected virtual void EndAnimateBody() { }
    }
}
