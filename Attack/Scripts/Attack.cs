using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Attack: Attack
    /// 1.  Attack allows a game object, specifically in prefab form, to be used as an "attack".
    /// 2.  Attack is to be added to a game object as a script/component*. The following must also be done:
    ///   a.  Attack must have a derived class specifically for this project (e.g. AttackMyGame).
    ///   b.  GraphicsController must also have a derived class for this project (e.g. GraphicsControllerMyGame).
    ///   c.  Any game object with the Attack component must exist on an "Attack" layer.
    ///   d.  Any game object with the Attack component must have its box collider set as a trigger.
    /// 3.  Attack is a key component of the Attack system, and is used in conjunction with the following classes:
    ///     AttackAbility, AttackAbilityUse, Attacker, AttackString, AttackTarget, and KnockBackCalcMode.
    /// 4.*-Attacker is abstract and must have a per-project derived class created (as mentioned in 2a);
    ///     the derived class itself must be added to a game object as a script/component.
    /// </summary>
    [RequireComponent(typeof(GraphicsController))]
    public abstract class Attack : MonoBehaviour, IEnd {

        public BoxCollider hitbox;

        public BoxCollider hurtbox;

        [Header("Optional Standalone Attack Ability")]

        [SerializeField, FormerlySerializedAs("m_attackAbility")]
        AttackAbility _attackAbility = default;

        [Header("Transform")]

        [SerializeField]
        List<Transform> _flippableTransforms = default;



#region FIELDS: PRIVATE

        Attacker _attacker;
        List<AttackTarget> _attackTargets = new List<AttackTarget>();
        BoxCollider _boxCollider;
        GraphicsController _graphicsController;
        bool _isFlippedX;
        bool _isInitialized;
        bool _isPlayerCharacterAttacker;
        ITimeThread _timeThread;
        Transform _transform;
        Vector2 _velocity;

#endregion

#region PROPERTIES: PUBLIC

        public virtual AttackAbility attackAbility { get { return _attackAbility; } }

        public virtual Attacker attacker { get { return _attacker; } }

        public virtual DamageDealtHandler damageDealtHandler { get; set; }

        public virtual bool isPlayerCharacterAttacker { get { return _isPlayerCharacterAttacker; } }

#endregion



        protected virtual void Awake()
        {
            _transform = transform;

            G.U.Assert(gameObject.layer != Layer.Default, "This GameObject must exist on an attack Layer.");

            if (hitbox != null)
            {
                this.Forbid<BoxCollider>();
                _boxCollider = gameObject.AddComponent<BoxCollider>();
                _boxCollider.center = hitbox.center;
                _boxCollider.size = hitbox.size;
                _boxCollider.isTrigger = true;
            }
            else
            {
                _boxCollider = this.Require<BoxCollider>();
            }
            G.U.Assert(_boxCollider.isTrigger, "The BoxCollider Component must be a trigger.");

            //TODO: apply hurtbox where needed
            if (hurtbox) hurtbox.enabled = false;

            _graphicsController = this.Require<GraphicsController>();

            my_end.actions += ForceOnTriggerExit;
        }

        protected virtual void Start()
        {
            //if the attack is spawned normally, through AttackAbility, Init should have already been called by now
            //however, if it hasn't, and there is an Optional Standalone Attack Ability assigned, use this ability
            if (!_isInitialized)
            {
                if (_attackAbility != null)
                {
                    InitInternal();
                }
                else
                {
                    G.U.Warning("If this Attack exists on its own, " +
                    "it should probably have a Standalone Attack Ability.");
                }
            }
        }

        protected virtual void Update()
        {
            if (!_timeThread.isPaused && _velocity != Vector2.zero)
            {
                transform.Translate(_velocity * _timeThread.deltaTime);
            }
        }

        void OnTriggerEnter(Collider other) {
            HitBox hitbox = other.GetComponent<HitBox>();
            if (hitbox == null) return;
            DamageTaker target = hitbox.damageTaker;
            if (target.end.wasInvoked) return;
            AttackTarget at = _attackTargets.Find(x => x.target == target);
            if (at == null) {
                G.U.Assert(damageDealtHandler != null, "The damageDealtHandler must be set before collision occurs.");
                at = new AttackTarget(_attackAbility, target, () => damageDealtHandler(this, target));
                _attackTargets.Add(at);
            }
            //TODO: get specifically defined center rather than simply the base position
            Vector3 apc = _transform.position;
            at.StartTakingDamage(apc, other.ClosestPoint(apc));
        }

        void OnTriggerExit(Collider other) {
            HitBox hitbox = other.GetComponent<HitBox>();
            if (hitbox == null) return;
            DamageTaker target = hitbox.damageTaker;
            if (target.end.wasInvoked) return;
            AttackTarget at = _attackTargets.Find(x => x.target == target);
            G.U.Assert(at != null, string.Format(
                "Target \"{0}\" exited the trigger of Attack \"{1}\", but was not found in m_attackTargets.",
                other.name, name));
            at.StopTakingDamage();
        }

        void OnDrawGizmos() { //runs in edit mode, so don't rely upon actions done in Awake
            Gizmos.color = Color.red;
            Vector3 p = transform.position;
            KRGGizmos.DrawCrosshairXY(p, 0.25f);
            var boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null) Gizmos.DrawWireCube(p + boxCollider.center, boxCollider.size);
        }



        End my_end = new End();

        public End end { get { return my_end; } }

        protected virtual void OnDestroy()
        {
            my_end.Invoke();

            ForceReleaseTargets();
        }



#region METHODS: PUBLIC, PROTECTED, & PRIVATE

        public void Init(AttackAbility attackAbility, Attacker attacker) {
            if (_isInitialized) {
                G.U.Err("Init has already been called.");
                return;
            }
            _attackAbility = attackAbility;
            _attacker = attacker;
            _isFlippedX = attacker.isFlippedX;
            _isPlayerCharacterAttacker = attacker.isPlayerCharacter;
            InitInternal();
        }

        [System.Obsolete("Use Init(AttackAbility attackAbility, Attacker attacker) instead.")]
        public void Init(AttackAbility attackAbility, bool isFlippedX, bool isPlayerCharacterAttacker) {
            if (_isInitialized) {
                G.U.Err("Init has already been called.");
                return;
            }
            _attackAbility = attackAbility;
            _isFlippedX = isFlippedX;
            _isPlayerCharacterAttacker = isPlayerCharacterAttacker;
            InitInternal();
        }

        protected virtual void InitInternal() {
            _isInitialized = true;

            _timeThread = _attackAbility.timeThread;

            if (_attackAbility.hasAttackLifetime) {
                //TODO: dispose callback causes optional delay for related attacks
                //E.G. SecS: stopping block makes the fire/shot m_attackRateSec restart at 1.5x:
                //alarm[e_pc_alarm.fire_ready] = global.framerate / fire_rate * 1.5;
                _timeThread.AddTrigger(_attackAbility.attackLifetime, Dispose);
            }

            if (_isFlippedX) { //flip horizontal (x-axis)
                _boxCollider.center = _boxCollider.center.Multiply(x: -1);
                _graphicsController.FlipX();
                if (_flippableTransforms != null) {
                    Transform tf;
                    for (int i = 0; i < _flippableTransforms.Count; i++) {
                        tf = _flippableTransforms[i];
                        tf.localScale = tf.localScale.Multiply(x: -1);
                    }
                }
            }

            float travelSpeed = _attackAbility.travelSpeed;
            if (!travelSpeed.Ap(0))
            {
                float flipX = _isFlippedX ? -1 : 1;
                _velocity = new Vector2(travelSpeed * flipX, 0);
                //TODO: support Y dimension
            }

            PlayAttackSFX();
        }

        protected virtual void PlayAttackSFX() {
            string sfxFmodEvent = _attackAbility.sfxFmodEvent;
            if (!string.IsNullOrEmpty(sfxFmodEvent)) {
                G.audio.PlaySFX(sfxFmodEvent, _transform.position);
            }
        }

        void Dispose(TimeTrigger tt) {
            if (this != null) { //this may be null if e.g. this is joined to an attacker that was destroyed
                gameObject.Dispose();
            }
        }

        /// <summary>
        /// OnTriggerExit is often _randomly_ not called when the attack is destroyed.
        /// This method is called before destroying to ensure OnTriggerExit is called.
        /// TODO: This sometimes does not work properly, and may be obsolete now that ForceReleaseTargets exists.
        /// </summary>
        void ForceOnTriggerExit() {
            _transform.Translate(-1000, -1000, -1000);
        }

        /// <summary>
        /// If the PC and an enemy hit each other at same time, and the enemy's attack is cancelled -- thus calling the
        /// G.End(...) method on this attack -- all while in the same frame... ForceOnTriggerExit will not actually
        /// force OnTriggerExit to be called. This may be because OnTriggerEnter and OnTriggerExit can't both be called
        /// in the same frame. Whatever the case, this method ensures that all targets are released from further damage.
        /// NOTE: This must be called from OnDestroy in order to work properly.
        /// </summary>
        void ForceReleaseTargets() {
            AttackTarget at;
            for (int i = 0; i < _attackTargets.Count; i++) {
                at = _attackTargets[i];
                if (at != null && at.isInProgress && !at.target.end.wasInvoked) {
                    at.StopTakingDamage();
                }
            }
            _attackTargets.Clear();
        }

#endregion

    }
}
