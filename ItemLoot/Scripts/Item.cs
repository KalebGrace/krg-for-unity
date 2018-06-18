using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public class Item : MonoBehaviour {

#region serialized fields

        [Header("VFX")]

        [SerializeField]
        protected Transform _animatingBody;

#endregion

#region protected fields

        protected ItemData _itemData;

#endregion

#region MonoBehaviour methods

        protected virtual void Start() {
            AnimateBody();
        }

        protected virtual void OnTriggerEnter(Collider other) {
            if (OnCollect(other)) G.End(gameObject);
        }

#endregion

#region public methods

        public virtual void Init(ItemData itemData) {
            _itemData = itemData;
        }

        public virtual bool OnCollect(Collider other) {
            //TODO: differentiate between inventory items and instant-use items
            return OnUse(other);
        }

        public virtual bool OnUse(Collider other) {
            //TODO: differentiate between the character, etc. using the item
            if (other.gameObject.tag != "Player") return false;
            var dt = other.GetComponent<DamageTaker>();
            if (dt != null && _itemData.hp > 0) dt.AddHP(_itemData.hp);
            return true;
        }

#endregion

#region protected methods

        protected virtual void AnimateBody() {
            if (_animatingBody == null) return;
#if NS_DG_TWEENING
            const float duration = 2;
            const float diameter = 0.2f;
            var endValue = _animatingBody.localPosition + Vector3.up * diameter;
            _animatingBody.DOLocalMove(endValue, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
#endif
        }

#endregion

    }
}
