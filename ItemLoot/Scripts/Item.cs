using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    public class Item : MonoBehaviour
    {
        [Header("VFX")]

        [SerializeField]
        protected Transform _animatingBody;


        protected ItemData _itemData;


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


        public virtual void Init(ItemData itemData)
        {
            _itemData = itemData;
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
            var dt = other.GetComponent<DamageTaker>();
            if (dt != null && _itemData.hp > 0) dt.AddHP(_itemData.hp);
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
