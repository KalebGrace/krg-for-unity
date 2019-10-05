using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    public abstract class DamageTaker : MonoBehaviour, IBodyComponent, IDamageable, IEnd, ISpawn {

#region fields

        [SerializeField]
        [FormerlySerializedAs("m_damageProfile")]
        DamageProfile _damageProfile;

        [SerializeField]
        GameObjectBody m_Body;

        event System.Action _endInvulnerabilityHandlers;

        protected AttackAbility _damageAttackAbility;
        protected Vector3 _damageAttackPositionCenter;
        protected Vector3 _damageHitPositionCenter;

        //current & maximum HP (hit points)
        float _hp;
        float _hpMaxNew;

        //damage stuff
        TimeTrigger _invulnerabilityTimeTrigger;
        TimeTrigger _knockBackTimeTrigger;

#endregion

#region properties: IDamagable implementation

        public virtual float hp { get { return _hp; } }

        public virtual float hpMin { get { return _damageProfile.hpMin; } }

        public virtual float hpMax { get { return _hpMaxNew; } }

#endregion

#region properties

        public virtual Transform centerTransform => m_Body.Refs.VisRect.transform;

        public virtual DamageProfile damageProfile {
            get {
                return _damageProfile;
            }
            set {
                _damageProfile = value;
                InitHP();
            }
        }

        public virtual bool isKnockedBack { get { return _knockBackTimeTrigger != null; } }

        public virtual bool isKnockedOut { get { return _hp.Ap(hpMin); } }

        public virtual Direction knockBackDirection { get; set; }

        public virtual float knockBackSpeed { get; private set; }

        private GraphicController GraphicController => m_Body.Refs.GraphicController;

#endregion



        protected virtual void Awake() {
            G.U.Require(_damageProfile, "Damage Profile");

            InitHP();

            knockBackDirection = Direction.Unknown;
        }



        End my_end = new End();

        public End end { get { return my_end; } }

        public GameObjectBody Body => m_Body;

        protected virtual void OnDestroy()
        {
            my_end.Invoke();
        }



#region Primary Methods

        public bool Damage(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        ) {
            _damageAttackAbility = attackAbility;
            _damageAttackPositionCenter = attackPositionCenter;
            _damageHitPositionCenter = hitPositionCenter;

            if (!CanBeDamaged()) return false;

            DealDamage(attackAbility);
            DisplayDamageVFX(attackAbility, attackPositionCenter, hitPositionCenter);
            PlayDamageSFX();

            CheckCustomPreKOC(attackAbility, attackPositionCenter, hitPositionCenter);

            if (isKnockedOut) {
                OnKnockedOut(attackPositionCenter);
                return true;
            }

            CheckCustom(attackAbility, attackPositionCenter);

            CheckInvulnerability(attackAbility);
            CheckKnockBack(attackAbility, attackPositionCenter);

            return true;
        }

        protected virtual bool CanBeDamaged() {
            //these could pop up at any time, so let's be safe
            return !my_end.wasInvoked && !isKnockedOut && !IsInvulnerableTo(_damageAttackAbility);
        }

        protected virtual void DealDamage(AttackAbility attackAbility) {
            _hp = Mathf.Clamp(_hp - attackAbility.hpDamage, hpMin, hpMax);
        }

        protected virtual void DisplayDamageVFX(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        ) {
            G.damage.DisplayDamageValue(this, Mathf.RoundToInt(attackAbility.hpDamage));
            GameObject p = attackAbility.hitVFXPrefab;
            if (p != null) Instantiate(p, hitPositionCenter, Quaternion.identity);
        }

        protected virtual void PlayDamageSFX() {
            string sfxFmodEvent = _damageProfile.sfxFmodEvent;
            if (!string.IsNullOrEmpty(sfxFmodEvent)) {
                G.audio.PlaySFX(sfxFmodEvent, transform.position);
            }
        }

        /// <summary>
        /// Sets the HP to the minimum. Does not check isKnockedOut or call OnKnockedOut, etc.
        /// </summary>
        protected void SetHPEmpty() {
            _hp = hpMin;
        }

        /// <summary>
        /// Sets the HP to the maximum defined by the damage profile (or whatever is defined in the hpMax property).
        /// </summary>
        protected void SetHPFull() {
            _hp = hpMax;
        }

        public void SetNewHpMax(float newHpMax)
        {
            //TODO: this should all be reworked to keep track of persistent HP max increasing items
            _hpMaxNew = newHpMax;
            _hp = Mathf.Min(_hp, newHpMax);
        }

        private void InitHP()
        {
            //TODO: this should all be reworked to keep track of persistent HP max increasing items
            _hpMaxNew = _damageProfile.hpMax;
            SetHPFull();
        }

        //TODO: this was added as part of the ItemLoot system and needs revision
        public void AddHP(float hp) {
            _hp = Mathf.Clamp(_hp + hp, hpMin, hpMax);
        }

#endregion

#region Custom Methods

        protected virtual void CheckCustomPreKOC(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        ) {
            //allow derived class to check conditions (before knock out check)
        }

        protected virtual void CheckCustom(AttackAbility attackAbility, Vector3 attackPositionCenter) {
            //allow derived class to check conditions (only if not KO'ed)
        }

#endregion

#region Invulnerability/Invulnerable Methods

        public void AddEndInvulnerabilityHandler(System.Action handler) {
            _endInvulnerabilityHandlers += handler;
        }

        public void RemoveEndInvulnerabilityHandler(System.Action handler) {
            _endInvulnerabilityHandlers -= handler;
        }

        protected virtual void CheckInvulnerability(AttackAbility attackAbility) {
            if (!attackAbility.causesInvulnerability || _damageProfile.invulnerabilityTime <= 0) return;
            BeginInvulnerability(attackAbility);
        }

        protected virtual void BeginInvulnerability(AttackAbility attackAbility) {
            _invulnerabilityTimeTrigger = _damageProfile.timeThread.AddTrigger(
                _damageProfile.invulnerabilityTime, EndInvulnerability);
            BeginInvulnerabilityVFX();
        }

        protected virtual void BeginInvulnerabilityVFX() {
            if (GraphicController != null) {
                GraphicController.StartDamageColor(_damageProfile.invulnerabilityTime);
                if (_damageProfile.invulnerabilityFlicker) {
                    GraphicController.StartFlicker(20f);
                }
            }
        }

        protected virtual void EndInvulnerabilityVFX() {
            GraphicController?.StopFlicker();
        }

        protected virtual void EndInvulnerability(TimeTrigger tt) {
            _invulnerabilityTimeTrigger = null;
            EndInvulnerabilityVFX();
            ObjectManager.InvokeEventActions(ref _endInvulnerabilityHandlers);
        }

        public virtual bool IsInvulnerableTo(AttackAbility attackAbility)
        {
            if (_invulnerabilityTimeTrigger != null) return true;

            var vList = _damageProfile.attackVulnerabilities;
            int count = vList?.Count ?? 0;

            if (count > 0 && !vList.Contains(attackAbility)) return true;

            return false;
        }

#endregion

#region KnockBack Methods

        protected virtual void CheckKnockBack(AttackAbility attackAbility, Vector3 attackPositionCenter) {
            if (!attackAbility.causesKnockBack) return;

            float knockBackTime;
            switch (attackAbility.knockBackTimeCalcMode) {
                case KnockBackCalcMode.Override:
                    knockBackTime = attackAbility.knockBackTime;
                    break;
                case KnockBackCalcMode.Multiply:
                    knockBackTime = attackAbility.knockBackTime * _damageProfile.knockBackTime;
                    break;
                default:
                    G.U.Unsupported(this, attackAbility.knockBackTimeCalcMode);
                    return;
            }

            if (knockBackTime <= 0) return;

            float knockBackDistance;
            switch (attackAbility.knockBackDistanceCalcMode) {
                case KnockBackCalcMode.Override:
                    knockBackDistance = attackAbility.knockBackDistance;
                    break;
                case KnockBackCalcMode.Multiply:
                    knockBackDistance = attackAbility.knockBackDistance * _damageProfile.knockBackDistance;
                    break;
                default:
                    G.U.Unsupported(this, attackAbility.knockBackDistanceCalcMode);
                    return;
            }

            if (knockBackDistance.Ap(0)) return;

            BeginKnockBack(attackAbility, attackPositionCenter, knockBackTime, knockBackDistance);
        }

        protected virtual void BeginKnockBack(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            float knockBackTime,
            float knockBackDistance
        ) {
            //if already knocked back, stop that time trigger before starting the new one
            if (isKnockedBack) _knockBackTimeTrigger.Dispose();

            knockBackSpeed = knockBackDistance / knockBackTime;
            _knockBackTimeTrigger = _damageProfile.timeThread.AddTrigger(knockBackTime, EndKnockBack);
            SetKnockBackDirection(attackPositionCenter);
        }

        protected abstract void SetKnockBackDirection(Vector3 attackPositionCenter);

        protected virtual void EndKnockBack(TimeTrigger tt) {
            _knockBackTimeTrigger = null;
        }

#endregion

        protected virtual void OnKnockedOut(Vector3 attackPositionCenter) {
            var ld = _damageProfile.knockedOutLoot;
            if (ld != null) ld.Drop(this);
            gameObject.Dispose();
        }

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }
    }
}
