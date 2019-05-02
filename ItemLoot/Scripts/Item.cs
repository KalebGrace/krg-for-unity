using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

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

        [Header("VFX")]

        [SerializeField]
        protected Transform _animatingBody;


        protected ItemData _itemData;
        protected ISpawn _spawner;


        protected virtual void Start()
        {
            if (_animatingBody != null) StartAnimateBody();
        }

        protected virtual void OnDestroy()
        {
            if (_animatingBody != null) EndAnimateBody();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (OnCollect(other)) G.End(gameObject);
        }


        public virtual void Init(ItemData itemData, ISpawn spawner)
        {
            _itemData = itemData;
            _spawner = spawner;
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
