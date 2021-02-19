using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    public abstract class DamageTaker : MonoBehaviour, IBodyComponent, IDestroyedEvent<DamageTaker>, ISpawn
    {
        // STATIC EVENTS

        public static event Handler DamageDealt;

        // INSTANCE EVENTS

        public event System.Action<DamageTaker> Destroyed;
        public event StatChangedHandler StatChanged;
        public event StatInitializedHandler StatInitialized;

        // DELEGATES

        public delegate void Handler(
            DamageTaker damageTaker,
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        );
        public delegate void StatChangedHandler(
            DamageTaker damageTaker,
            int statID,
            float oldValue,
            float newValue
        );
        public delegate void StatInitializedHandler(
            DamageTaker damageTaker,
            int statID,
            float value
        );

        // FIELDS

        [SerializeField]
        [FormerlySerializedAs("m_damageProfile")]
        protected DamageProfile _damageProfile;

        [SerializeField]
        private GameObjectBody m_Body = default;

        protected Attacker _damageAttacker;
        protected AttackAbility _damageAttackAbility;
        protected Vector3 _damageAttackPositionCenter;
        protected Vector3 _damageHitPositionCenter;

        // ENEMY / BOSS current and maximum HP (hit points)
        protected float m_HP;
        protected float m_HPMax;

        // damage stuff
        protected TimeTrigger _invulnerabilityTimeTrigger;
        TimeTrigger _knockBackTimeTrigger;

        // PROPERTIES

        public virtual float HP
        {
            get => IsPlayerCharacter ? G.inv.GetStatVal(StatID.HP) : m_HP;
            set
            {
                float oldValue = HP;
                value = Mathf.Clamp(value, HPMin, HPMax);
                if (IsPlayerCharacter)
                {
                    G.inv.SetStatVal(StatID.HP, value);
                }
                else
                {
                    m_HP = value;
                }
                InvokeStatChanged((int)StatID.HP, oldValue, value);
                ResolveKnockedOut(oldValue, value);
            }
        }

        public virtual float HPMin => _damageProfile.HPMin;

        public virtual float HPMax
        {
            get => IsPlayerCharacter ? G.inv.GetStatVal(StatID.HPMax) : m_HPMax;
            set
            {
                float oldValue = HPMax;
                if (IsPlayerCharacter)
                {
                    G.inv.SetStatVal(StatID.HPMax, value);
                }
                else
                {
                    m_HPMax = value;
                }
                InvokeStatChanged((int)StatID.HPMax, oldValue, value);
            }
        }

        public GameObjectBody Body => m_Body;

        public Transform CenterTransform => m_Body.CenterTransform;

        public virtual DamageProfile damageProfile
        {
            get => _damageProfile;
            set
            {
                _damageProfile = value;
                if (G.U.IsPlayMode(this)) InitHP();
            }
        }

        public virtual bool IsKnockedBack => _knockBackTimeTrigger != null;

        public virtual bool IsKnockedOut => HP <= HPMin;

        public virtual bool IsPlayerCharacter => m_Body.IsPlayerCharacter;

        public virtual Direction knockBackDirection { get; set; } = Direction.Unknown;

        public virtual float knockBackSpeed { get; private set; }

        protected virtual GraphicController GraphicController => m_Body.Refs.GraphicController;

        protected virtual Rigidbody Rigidbody => m_Body.Refs.Rigidbody;

        GameObjectBody ISpawn.Invoker => _damageAttacker != null ? _damageAttacker.Body : null;

        // MONOBEHAVIOUR METHODS

        protected virtual void Start()
        {
            InitHP();
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        // PRIMARY METHODS

        public bool Damage(
            Attack attack,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            _damageAttacker = attack.attacker;
            _damageAttackAbility = attack.attackAbility;
            _damageAttackPositionCenter = attackPositionCenter;
            _damageHitPositionCenter = hitPositionCenter;

            return Damage();
        }

        public bool Damage(
            Attacker attacker,
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            _damageAttacker = attacker;
            _damageAttackAbility = attackAbility;
            _damageAttackPositionCenter = attackPositionCenter;
            _damageHitPositionCenter = hitPositionCenter;

            return Damage();
        }

        private bool Damage()
        {
            if (!CanBeDamagedBy(_damageAttacker, _damageAttackAbility)) return false;

            DealDamage(_damageAttackAbility);
            DamageDealt?.Invoke(this, _damageAttackAbility, _damageAttackPositionCenter, _damageHitPositionCenter);
            DisplayDamageVFX(_damageAttackAbility, _damageAttackPositionCenter, _damageHitPositionCenter);
            PlayDamageSFX();

            if (IsKnockedOut) return true;

            CheckInvulnerability(_damageAttackAbility);
            CheckKnockBack(_damageAttackAbility, _damageAttackPositionCenter);

            return true;
        }

        public virtual bool CanBeDamagedBy(Attacker attacker, AttackAbility attackAbility)
        {
            if (this == null) return false;
            if (IsKnockedOut) return false;

            if (_invulnerabilityTimeTrigger != null) return false;

            var vList = _damageProfile.attackVulnerabilities;
            int count = vList?.Count ?? 0;

            if (count > 0 && !vList.Contains(attackAbility)) return false;

            return true;
        }

        protected virtual void DealDamage(AttackAbility attackAbility)
        {
            HP -= attackAbility.hpDamage;
        }

        protected virtual void DisplayDamageVFX(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
            if (attackAbility.isDPSClone) return;

            string damage;
            if (attackAbility.hpDamage.Ap(0) && attackAbility.requiresDPSClone)
            {
                damage = "DPS";
            }
            else
            {
                damage = Mathf.RoundToInt(attackAbility.hpDamage).ToString();
            }
            G.damage.DisplayDamageValue(this, damage);

            GameObject p = attackAbility.hitVFXPrefab;
            if (p != null) Instantiate(p, hitPositionCenter, Quaternion.identity);
        }

        protected virtual void PlayDamageSFX()
        {
            if (_damageAttackAbility.isDPSClone) return;

            string sfxFmodEvent = _damageProfile.sfxFmodEvent;
            if (!string.IsNullOrEmpty(sfxFmodEvent))
            {
                G.audio.PlaySFX(sfxFmodEvent, transform.position);
            }
        }

        private void InitHP()
        {
            if (IsPlayerCharacter)
            {
                if (!G.inv.HasStatVal(StatID.HPMax))
                {
                    G.inv.SetStatVal(StatID.HPMax, _damageProfile.HPMax);
                }
                if (!G.inv.HasStatVal(StatID.HP))
                {
                    G.inv.SetStatVal(StatID.HP, G.inv.GetStatVal(StatID.HPMax));
                }
            }
            else
            {
                HP = HPMax = _damageProfile.HPMax;
            }
            InvokeStatInitialized((int)StatID.HPMax, HPMax);
            InvokeStatInitialized((int)StatID.HP, HP);
        }

        // KNOCKED OUT (KO) METHODS

        protected virtual void ResolveKnockedOut(float oldHP, float newHP)
        {
            if (oldHP > HPMin && newHP <= HPMin) OnKnockedOut();
        }

        protected virtual void OnKnockedOut()
        {
            var ld = _damageProfile.KnockedOutLoot;
            if (ld != null) ld.Drop(this);
            gameObject.Dispose();
        }

        // INVULNERABILITY METHODS

        protected virtual void CheckInvulnerability(AttackAbility attackAbility)
        {
            if (!attackAbility.causesInvulnerability || _damageProfile.invulnerabilityTime <= 0) return;
            BeginInvulnerability(attackAbility);
        }

        protected virtual void BeginInvulnerability(AttackAbility attackAbility)
        {
            _invulnerabilityTimeTrigger = _damageProfile.TimeThread.AddTrigger(
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
            _knockBackTimeTrigger = _damageProfile.TimeThread.AddTrigger(knockBackTime, EndKnockBack);
            SetKnockBackDirection(attackPositionCenter);
        }

        protected abstract void SetKnockBackDirection(Vector3 attackPositionCenter);

        protected virtual void EndKnockBack(TimeTrigger tt)
        {
            _knockBackTimeTrigger = null;
        }

        // MISC

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        protected void InvokeStatChanged(int statID, float oldValue, float newValue)
        {
            StatChanged?.Invoke(this, statID, oldValue, newValue);
        }

        protected void InvokeStatInitialized(int statID, float value)
        {
            StatInitialized?.Invoke(this, statID, value);
        }
    }
}