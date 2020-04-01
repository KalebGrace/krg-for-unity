using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    public abstract class DamageTaker : MonoBehaviour, IBodyComponent, IDestroyedEvent<DamageTaker>, ISpawn
    {
        // STATIC EVENTS

        public static event Handler DamageDealt;

        // INSTANCE EVENTS

        public event System.Action<DamageTaker> Destroyed;

        // DELEGATES

        public delegate void Handler(
            DamageTaker damageTaker,
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        );

        // FIELDS

        [SerializeField]
        [FormerlySerializedAs("m_damageProfile")]
        DamageProfile _damageProfile;

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
        TimeTrigger _invulnerabilityTimeTrigger;
        TimeTrigger _knockBackTimeTrigger;

        // PROPERTIES

        public virtual float HP
        {
            get => IsPlayerCharacter ? G.inv.GetStatVal(StatID.HP) : m_HP;
            set
            {
                value = Mathf.Clamp(value, HPMin, HPMax);
                if (IsPlayerCharacter)
                {
                    G.inv.SetStatVal(StatID.HP, value);
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
            get => IsPlayerCharacter ? G.inv.GetStatVal(StatID.HPMax) : m_HPMax;
            set
            {
                if (IsPlayerCharacter)
                {
                    G.inv.SetStatVal(StatID.HPMax, value);
                }
                else
                {
                    m_HPMax = value;
                }
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
                InitHP();
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
            _damageAttackAbility = attack.attackAbility;
            _damageAttacker = attack.attacker;
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
            if (!CanBeDamaged()) return false;

            DealDamage(_damageAttackAbility);
            DamageDealt?.Invoke(this, _damageAttackAbility, _damageAttackPositionCenter, _damageHitPositionCenter);
            DisplayDamageVFX(_damageAttackAbility, _damageAttackPositionCenter, _damageHitPositionCenter);
            PlayDamageSFX();

            CheckCustomPreKOC(_damageAttackAbility, _damageAttackPositionCenter, _damageHitPositionCenter);

            if (IsKnockedOut)
            {
                OnKnockedOut(_damageAttackPositionCenter);
                return true;
            }

            CheckCustom(_damageAttackAbility, _damageAttackPositionCenter);

            CheckInvulnerability(_damageAttackAbility);
            CheckKnockBack(_damageAttackAbility, _damageAttackPositionCenter);

            return true;
        }

        protected virtual bool CanBeDamaged()
        {
            //these could pop up at any time, so let's be safe
            return this != null && !IsKnockedOut && !IsInvulnerableTo(_damageAttackAbility);
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
        }

        // CUSTOM METHODS

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
