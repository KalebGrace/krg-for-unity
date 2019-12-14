using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class AbilityObject : MonoBehaviour, IBodyComponent, IDestroyedEvent<AbilityObject>
    {
        // EVENTS

        public event System.Action<AbilityObject> Destroyed;

        // SERIALIZED FIELDS

        [SerializeField]
        private GameObjectBody m_Body = default;

        // PROPERTIES

        public AbilityState AbilityState { get; protected set; }

        public GameObjectBody Body => m_Body;

        protected Hitbox Hitbox => m_Body.Refs.Hitbox;

        protected Hurtbox Hurtbox => m_Body.Refs.Hurtbox;

        public bool IsCompleted { get; protected set; }

        public bool IsDisposed { get; protected set; }

        protected List<Hurtbox> Targets { get; } = new List<Hurtbox>();

        protected virtual ITimeThread TimeThread => G.time.GetTimeThread(TimeThreadInstance.Field);

        protected Vector3 Velocity { get; set; }

        // MONOBEHAVIOUR METHODS

        protected virtual void Awake()
        {
            if (Hitbox != null)
            {
                Hitbox.TriggerEntered += OnHitboxTriggerEnter;
                Hitbox.TriggerExited += OnHitboxTriggerExit;
            }

            if (Hurtbox != null)
            {
                // TODO: apply hurtbox where needed

                Hurtbox.Enabled = false;
            }
        }

        protected virtual void OnDestroy()
        {
            ExitTargets();

            if (Hitbox != null)
            {
                Hitbox.TriggerExited -= OnHitboxTriggerExit;
                Hitbox.TriggerEntered -= OnHitboxTriggerEnter;
            }

            Destroyed?.Invoke(this);
        }

        protected virtual void Update()
        {
            Transform tf = m_Body.transform;

            if (!TimeThread.isPaused && Velocity != Vector3.zero)
            {
                tf.Translate(Velocity * TimeThread.deltaTime, Space.World);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Transform tf = m_Body.transform;
            Vector3 p = tf.position;

            Gizmos.color = Color.red;
            KRGGizmos.DrawCrosshairXY(p, 0.25f);

            if (Hitbox != null)
            {
                if (!Hitbox.Enabled)
                {
                    Gizmos.color = new Color(1, 0.5f, 0);
                }
                Vector3 bcCenter = Hitbox.Center;
                if (tf.localEulerAngles.y.Ap(180)) // hacky, but necessary
                {
                    bcCenter = bcCenter.Multiply(x: -1);
                }
                Gizmos.DrawWireCube(p + bcCenter, Hitbox.Size);
            }
        }

        // MAIN PUBLIC METHODS

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        public virtual void InitAbility(AbilityState abil)
        {
            AbilityState = abil;

            TimeThread.AddTrigger(abil.Ability.ObjectActivationDelay, OnActivate, true);

            m_Body.gameObject.SetActive(false);
        }

        [System.Obsolete]
        public virtual void Deactivate(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }

        public virtual void Dispose(bool isCompleted)
        {
            if (this != null && !IsDisposed)
            {
                IsCompleted = isCompleted;
                IsDisposed = true;
                m_Body.Dispose();
            }
        }

        // MAIN PROTECTED METHODS

        protected virtual void OnActivate(TimeTrigger tt)
        {
            m_Body.gameObject.SetActive(true);

            m_Body.FacingDirection = AbilityState.AbilityOwner.Body.FacingDirection;
            float signX = m_Body.FacingDirection == Direction.Left ? -1 : 1;
            Velocity = AbilityState.Ability.Velocity.Multiply(signX);

            PlayObjectActivationSound();

            BoolFloat lifetime = AbilityState.Ability.Lifetime;
            if (lifetime.boolValue)
            {
                TimeThread.AddTrigger(lifetime.floatValue, OnLifetimeEnd, true);
            }
        }

        protected virtual void OnLifetimeEnd(TimeTrigger tt)
        {
            Dispose(true);
        }

        protected void OnHitboxTriggerEnter(Collider other)
        {
            Hurtbox target = other.GetComponent<Hurtbox>();
            if (target == null) return;
            Targets.Add(target);
            OnTargetEnter(target, other);
        }

        protected void OnHitboxTriggerExit(Collider other)
        {
            Hurtbox target = other.GetComponent<Hurtbox>();
            if (target == null) return;
            Targets.Remove(target);
            OnTargetExit(target, other);
        }

        protected virtual void OnTargetEnter(Hurtbox target, Collider targetCollider)
        {
            ApplyEffectors((int)EffectorCondition.OnTargetEnter, target.Body);
        }

        protected virtual void OnTargetExit(Hurtbox target, Collider targetCollider)
        {
            ApplyEffectors((int)EffectorCondition.OnTargetExit, target.Body);
        }

        protected virtual void ExitTargets()
        {
            while (Targets.Count > 0)
            {
                Hurtbox target = Targets[Targets.Count - 1];
                Targets.RemoveAt(Targets.Count - 1);
                if (target != null)
                {
                    OnTargetExit(target, null);
                }
            }
        }

        protected virtual void ApplyEffectors(int condition, GameObjectBody target)
        {
            if (condition == 0)
            {
                G.U.Err("No effector condition provided.");
                return;
            }

            List<Effector> effectors = AbilityState.Ability.Effectors;
            for (int i = 0; i < effectors.Count; ++i)
            {
                Effector e = effectors[i];

                if (condition != e.condition) continue;

                GameObjectBody subject;
                switch ((EffectorSubject)e.subject)
                {
                    case EffectorSubject.Self:
                        subject = AbilityState.AbilityOwner.Body;
                        break;
                    case EffectorSubject.Target:
                        subject = target;
                        break;
                    default:
                        G.U.Unsupported(this, (EffectorSubject)e.subject);
                        continue;
                }

                ApplyEffectorToSubject(e, subject);
            }
        }

        protected virtual void ApplyEffectorToSubject(Effector e, GameObjectBody subject)
        {
            G.U.Err("Unsupported for underived ability objects.");
        }

        protected virtual void PlayObjectActivationSound()
        {
            G.audio.PlaySFX(AbilityState.Ability.ObjectActivationSound, m_Body.transform.position);
        }
    }
}



/*

private void OnHitboxTriggerEnter(Collider other)
{
...
    Vector3 attackPositionCenter = m_Body.CenterTransform.position;
    Vector3 hitPositionCenter = other.ClosestPoint(attackPositionCenter);
...
}

public class AttackTarget
{
private const string _infiniteLoopError = "To prevent an infinite loop, "
                                  + "damage has ceased for the remainder of this collision. "
                                  + "Consider doing one or more of the following: "
                                  + "1. Set AttackAbility Hp Damage Rate to true. "
                                  + "2. Set AttackAbility Max Hits Per Target to true. "
                                  + "3. Set AttackAbility Causes Invulnerability to true, "
                                  + "and DamageProfile Invulnerability Time to a positive value.";

// private fields

private readonly AttackAbility _attackAbility;
private Vector3 _attackPositionCenter;
private System.Action _damageDealtCallback;
private TimeTrigger _damageTimeTrigger;
private int _hitCount;
private Vector3 _hitPositionCenter;
private bool _isDelayedDueToInvulnerability;
private bool _isDelayedDueToTimeThreadPause;

// properties

private bool isHitLimitReached
{
    get
    {
        //has the "hit" count reached the maximum number of "hits" (i.e. damage method calls)?
        return _attackAbility.hasMaxHitsPerTarget && _hitCount >= _attackAbility.maxHitsPerTarget;
    }
}

public bool isInProgress { get; private set; }

public DamageTaker target { get; private set; }

// methods 1 - Public Methods

public void StartTakingDamage(Vector3 attackPositionCenter, Vector3 hitPositionCenter)
{
    G.U.Assert(!isInProgress,
        "StartTakingDamage was called, but this AttackTarget has already started taking damage.");
    isInProgress = true;
    if (!isHitLimitReached) CheckForDelay(StartTakingDamageForReal);
}

public void StopTakingDamage()
{
    G.U.Assert(isInProgress,
        "StopTakingDamage was called, but this AttackTarget has already stopped taking damage.");
    isInProgress = false;
    //
    if (!isHitLimitReached) CheckForDelayCallbackRemoval(StopTakingDamageForReal);
}

// methods 2 - Check For, And Handle, Delays

private void CheckForDelay(System.Action onNoDelay)
{
    if (target.IsInvulnerableTo(_attackAbility))
    {
        _isDelayedDueToInvulnerability = true;
        target.AddEndInvulnerabilityHandler(DelayCallback);
    }
    else if (_attackAbility.timeThread.isPaused)
    {
        _isDelayedDueToTimeThreadPause = true;
        _attackAbility.timeThread.AddUnpauseHandler(DelayCallback);
    }
    else
    {
        onNoDelay();
    }
}

private void CheckForDelayCallbackRemoval(System.Action onNoRemoval)
{
    if (_isDelayedDueToInvulnerability)
    {
        _isDelayedDueToInvulnerability = false;
        target.RemoveEndInvulnerabilityHandler(DelayCallback);
    }
    else if (_isDelayedDueToTimeThreadPause)
    {
        _isDelayedDueToTimeThreadPause = false;
        _attackAbility.timeThread.RemoveUnpauseHandler(DelayCallback);
    }
    else
    {
        onNoRemoval();
    }
}

private void DelayCallback()
{
    CheckForDelayCallbackRemoval(DelayCallbackRemovalError);
    CheckForDelay(StartTakingDamageForReal); //check for any possible additional delays
}

private void DelayCallbackRemovalError()
{
    G.U.Err("DelayCallback was made with no delay flags set to true.");
}

// methods 3 - Start & Stop Damage For Real

private void StartTakingDamageForReal()
{
    if (_damageTimeTrigger != null)
    {
        _attackAbility.timeThread.LinkTrigger(_damageTimeTrigger);
    }
    else
    {
        Damage();
        if (!target.end.wasInvoked && !target.IsKnockedOut && !isHitLimitReached)
        {
            if (_attackAbility.hasHPDamageRate)
            {
                _damageTimeTrigger = _attackAbility.timeThread.AddTrigger(
                    _attackAbility.hpDamageRateSec, Damage);
                _damageTimeTrigger.doesMultiFire = true;
            }
            else
            {
                CheckForDelay(DamageInfiniteLoopCheck);
            }
        }
    }
}

private void StopTakingDamageForReal()
{
    if (_damageTimeTrigger != null)
    {
        G.U.Assert(_attackAbility.timeThread.UnlinkTrigger(_damageTimeTrigger));
    }
}

// methods 4 - The Actual Damage

private void Damage()
{
    bool isHit = target.Damage(_attackAbility);
    if (isHit)
    {
        _hitCount++;
        _damageDealtCallback();
    }
}

private void Damage(TimeTrigger tt)
{
    Damage();
    if (!target.end.wasInvoked && !target.IsKnockedOut && !isHitLimitReached)
    {
        tt.Proceed();
    }
}

private void DamageInfiniteLoopCheck()
{
    if (_attackAbility.hasMaxHitsPerTarget)
    {
        StartTakingDamageForReal();
    }
    else
    {
        G.U.Err(_infiniteLoopError);
    }
}

using UnityEngine.Serialization;

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

        event System.Action _endInvulnerabilityHandlers;

        //damage stuff
        TimeTrigger _invulnerabilityTimeTrigger;
        TimeTrigger _knockBackTimeTrigger;

        // properties

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
            InitHP();

            knockBackDirection = Direction.Unknown;
        }

        // Primary Methods

        public bool Damage(
            AttackAbility attackAbility,
            Vector3 attackPositionCenter,
            Vector3 hitPositionCenter
        )
        {
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

public class DamageManager : Manager
{
    /// <summary>
    /// Displays the damage value for the target.
    /// The value will be parented to the target,
    /// and then will be offloaded at the target's last position at the time the target is destroyed.
    /// This overload is the most commonly used one.
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="damage">Damage.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public void DisplayDamageValue<T>(T target, int damage) where T : MonoBehaviour, IEnd
    {
        DisplayDamageValue(target, target.transform, damage);
    }

    /// <summary>
    /// Displays the damage value for the target.
    /// The value will be parented to the anchor (as specified),
    /// and then will be offloaded at the anchor's last position at the time the target is destroyed.
    /// This overload is useful when you need to attach the damage value to a sub-object of a target.
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="anchor">Anchor (parent Transform).</param>
    /// <param name="damage">Damage.</param>
    public void DisplayDamageValue(IEnd target, Transform anchor, int damage)
    {
        G.U.New(config.damageValuePrefab, anchor).Init(target, damage);
    }

    /// <summary>
    /// Gets (or creates) the HP Bar for the target.
    /// The HP Bar will be parented to the target.
    /// This overload is the most commonly used one.
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="visRect">Optional VisRect, used for precise automatic positioning.</param>
    /// <returns>HP Bar.</returns>
    public HPBar GetHPBar(DamageTaker target, VisRect visRect = null)
    {
        if (visRect == null)
        {
            return GetHPBar(target, target.transform, Vector3.up);
        }

        return GetHPBar(target, visRect.transform, visRect.OffsetTop.Add(y: 0.1f));
    }

    /// <summary>
    /// Gets (or creates) the HP Bar for the target.
    /// The HP Bar will be parented to the anchor (as specified).
    /// This overload is useful when you need to attach the HP Bar to a sub-object of a target.
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="anchor">Anchor (parent Transform).</param>
    /// <param name="offset">Positional offset. When in doubt, use Vector3.up.</param>
    /// <returns>HP Bar.</returns>
    public HPBar GetHPBar(DamageTaker target, Transform anchor, Vector3 offset)
    {
        var hpBar = anchor.GetComponentInChildren<HPBar>(true);
        if (hpBar == null)
        {
            hpBar = G.U.New(config.hpBarPrefab, anchor);
            hpBar.transform.localPosition = offset;
            hpBar.Init(target);
        }
        return hpBar;
    }
}

 */
