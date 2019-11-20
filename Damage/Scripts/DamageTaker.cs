using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    public abstract class DamageTaker : MonoBehaviour, IBodyComponent, IEnd, ISpawn
    {
        // STATIC EVENTS

        public static event Handler DamageDealt;

        // DELEGATES

        public delegate void Handler(
            DamageTaker damageTaker,
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        );

        // fields

        [SerializeField]
        [FormerlySerializedAs("m_damageProfile")]
        DamageProfile _damageProfile;

        [SerializeField]
        private GameObjectBody m_Body = default;

        event System.Action _endInvulnerabilityHandlers;

        protected AttackAbility _damageAttackAbility;
        protected Vector3 _damageAttackPositionCenter;
        protected Vector3 _damageHitPositionCenter;

        // ENEMY / BOSS current and maximum HP (hit points)

        protected float m_HP;
        protected float m_HPMax;

        //damage stuff
        TimeTrigger _invulnerabilityTimeTrigger;
        TimeTrigger _knockBackTimeTrigger;

        // properties: IDamagable implementation

        public virtual float HP
        {
            get => IsPlayerCharacter ? (G.inv.StatHP ?? HPMax) : m_HP;
            set
            {
                if (IsPlayerCharacter)
                {
                    G.inv.StatHP = value;
                }
                else
                {
                    m_HP = value;
                }
            }
        }

        public virtual float HPMin => _damageProfile.HPMin;

        public virtual float HPMax
        {
            get => IsPlayerCharacter ? (G.inv.StatHPMax ?? _damageProfile.HPMax) : m_HPMax;
            set
            {
                if (IsPlayerCharacter)
                {
                    G.inv.StatHPMax = value;
                }
                else
                {
                    m_HPMax = value;
                }
            }
        }

        // properties

        public virtual Transform centerTransform => m_Body.Refs.VisRect.transform;

        public virtual DamageProfile damageProfile
        {
            get
            {
                return _damageProfile;
            }
            set
            {
                _damageProfile = value;
                InitHP();
            }
        }

        public virtual bool IsKnockedBack => _knockBackTimeTrigger != null;

        public virtual bool IsKnockedOut => HP.Ap(HPMin);

        public virtual bool IsPlayerCharacter => m_Body.IsPlayerCharacter;

        public virtual Direction knockBackDirection { get; set; }

        public virtual float knockBackSpeed { get; private set; }

        protected virtual GraphicController GraphicController => m_Body.Refs.GraphicController;

        protected virtual Rigidbody Rigidbody => m_Body.Refs.Rigidbody;

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake()
        {
            G.U.Require(_damageProfile, "Damage Profile");

            InitHP();

            knockBackDirection = Direction.Unknown;
        }

        protected virtual void OnDestroy()
        {
            my_end.Invoke();
        }

        // END

        protected End my_end = new End();

        public End end { get { return my_end; } }

        public GameObjectBody Body => m_Body;

        // Primary Methods

        public bool Damage(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            _damageAttackAbility = attackAbility;
            _damageAttackPositionCenter = attackPositionCenter;
            _damageHitPositionCenter = hitPositionCenter;

            if (!CanBeDamaged()) return false;

            DealDamage(attackAbility);
            DamageDealt?.Invoke(this, attackAbility, attackPositionCenter, hitPositionCenter);
            DisplayDamageVFX(attackAbility, attackPositionCenter, hitPositionCenter);
            PlayDamageSFX();

            CheckCustomPreKOC(attackAbility, attackPositionCenter, hitPositionCenter);

            if (IsKnockedOut)
            {
                OnKnockedOut(attackPositionCenter);
                return true;
            }

            CheckCustom(attackAbility, attackPositionCenter);

            CheckInvulnerability(attackAbility);
            CheckKnockBack(attackAbility, attackPositionCenter);

            return true;
        }

        protected virtual bool CanBeDamaged()
        {
            //these could pop up at any time, so let's be safe
            return !my_end.wasInvoked && !IsKnockedOut && !IsInvulnerableTo(_damageAttackAbility);
        }

        protected virtual void DealDamage(AttackAbility attackAbility)
        {
            HP = Mathf.Clamp(HP - attackAbility.hpDamage, HPMin, HPMax);
        }

        protected virtual void DisplayDamageVFX(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            G.damage.DisplayDamageValue(this, Mathf.RoundToInt(attackAbility.hpDamage));
            GameObject p = attackAbility.hitVFXPrefab;
            if (p != null) Instantiate(p, hitPositionCenter, Quaternion.identity);
        }

        protected virtual void PlayDamageSFX()
        {
            string sfxFmodEvent = _damageProfile.sfxFmodEvent;
            if (!string.IsNullOrEmpty(sfxFmodEvent))
            {
                G.audio.PlaySFX(sfxFmodEvent, transform.position);
            }
        }

        /// <summary>
        /// Sets the HP to the minimum. Does not check isKnockedOut or call OnKnockedOut, etc.
        /// </summary>
        protected void SetHPEmpty()
        {
            HP = HPMin;
        }

        /// <summary>
        /// Sets the HP to the maximum defined by the damage profile (or whatever is defined in the HPMax property).
        /// </summary>
        protected void SetHPFull()
        {
            HP = HPMax;
        }

        public void SetNewHpMax(float newHpMax)
        {
            HPMax = newHpMax;
            HP = Mathf.Min(HP, newHpMax);
        }

        private void InitHP()
        {
            if (m_Body.IsPlayerCharacter)
            {
                if (!G.inv.StatHPMax.HasValue)
                {
                    G.inv.StatHPMax = _damageProfile.HPMax;
                }
                if (!G.inv.StatHP.HasValue)
                {
                    G.inv.StatHP = G.inv.StatHPMax;
                }
            }
            else
            {
                HP = HPMax = _damageProfile.HPMax;
            }
        }

        //TODO: this was added as part of the ItemLoot system and needs revision
        public void AddHP(float hp)
        {
            HP = Mathf.Clamp(HP + hp, HPMin, HPMax);
        }

        // Custom Methods

        protected virtual void CheckCustomPreKOC(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            //allow derived class to check conditions (before knock out check)
        }

        protected virtual void CheckCustom(AttackAbility attackAbility, Vector3 attackPositionCenter)
        {
            //allow derived class to check conditions (only if not KO'ed)
        }

        // INVULNERABILITY METHODS

        public void AddEndInvulnerabilityHandler(System.Action handler)
        {
            _endInvulnerabilityHandlers += handler;
        }

        public void RemoveEndInvulnerabilityHandler(System.Action handler)
        {
            _endInvulnerabilityHandlers -= handler;
        }

        protected virtual void CheckInvulnerability(AttackAbility attackAbility)
        {
            if (!attackAbility.causesInvulnerability || _damageProfile.invulnerabilityTime <= 0) return;
            BeginInvulnerability(attackAbility);
        }

        protected virtual void BeginInvulnerability(AttackAbility attackAbility)
        {
            _invulnerabilityTimeTrigger = _damageProfile.timeThread.AddTrigger(
                _damageProfile.invulnerabilityTime, EndInvulnerability);
            BeginInvulnerabilityVFX();
        }

        protected virtual void BeginInvulnerabilityVFX()
        {
            if (GraphicController != null)
            {
                GraphicController.SetDamageColor(_damageProfile.invulnerabilityTime);
                if (_damageProfile.invulnerabilityFlicker)
                {
                    GraphicController.SetFlicker();
                }
            }
        }

        protected virtual void EndInvulnerabilityVFX()
        {
            GraphicController?.EndFlicker();
        }

        protected virtual void EndInvulnerability(TimeTrigger tt)
        {
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

        // KnockBack Methods

        protected virtual void CheckKnockBack(AttackAbility attackAbility, Vector3 attackPositionCenter)
        {
            if (!attackAbility.causesKnockBack || _damageProfile.IsImmuneToKnockBack) return;

            AddKnockBackForce(attackAbility, attackPositionCenter);

            float knockBackTime;
            switch (attackAbility.knockBackTimeCalcMode)
            {
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
            switch (attackAbility.knockBackDistanceCalcMode)
            {
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

        protected virtual void AddKnockBackForce(AttackAbility attackAbility, Vector3 attackPositionCenter)
        {
            Vector3 force = attackAbility.KnockBackForceImpulse;
            if (force == Vector3.zero)
            {
                return;
            }
            if (Rigidbody == null)
            {
                G.U.Warn("{0} requires a rigidbody in order to add knock back force.", m_Body.name);
                return;
            }
            if (m_Body.CenterTransform.position.x < attackPositionCenter.x)
            {
                force.x *= -1f;
            }
            Rigidbody.AddForce(force, ForceMode.Impulse);
        }

        protected virtual void BeginKnockBack(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            float knockBackTime,
            float knockBackDistance
        )
        {
            //if already knocked back, stop that time trigger before starting the new one
            if (IsKnockedBack) _knockBackTimeTrigger.Dispose();

            knockBackSpeed = knockBackDistance / knockBackTime;
            _knockBackTimeTrigger = _damageProfile.timeThread.AddTrigger(knockBackTime, EndKnockBack);
            SetKnockBackDirection(attackPositionCenter);
        }

        protected abstract void SetKnockBackDirection(Vector3 attackPositionCenter);

        protected virtual void EndKnockBack(TimeTrigger tt)
        {
            _knockBackTimeTrigger = null;
        }

        // MISC

        protected virtual void OnKnockedOut(Vector3 attackPositionCenter)
        {
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
