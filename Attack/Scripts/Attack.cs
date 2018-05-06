using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Attack.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    [RequireComponent(typeof(BoxCollider), typeof(GraphicsController))]
    public abstract class Attack : MonoBehaviour, IEnd {

#region serialized fields

        [Header("Optional Standalone Attack Ability")]

        [SerializeField]
        [FormerlySerializedAs("m_attackAbility")]
        AttackAbility _attackAbility;

        [Header("Transform")]

        [SerializeField]
        List<Transform> _flippableTransforms;

#endregion

#region private fields

        List<AttackTarget> _attackTargets = new List<AttackTarget>();
        BoxCollider _boxCollider;
        GraphicsController _graphicsController;
        bool _isFlippedX;
        bool _isInitialized;
        bool _isPlayerCharacterAttacker;
        Transform _transform;

#endregion

#region IEnd implementation

        public End end { get; private set; }

#endregion

#region properties

        public virtual AttackAbility attackAbility { get { return _attackAbility; } }

        public virtual DamageDealtHandler damageDealtHandler { get; set; }

        public virtual bool isPlayerCharacterAttacker { get { return _isPlayerCharacterAttacker; } }

#endregion

#region MonoBehaviour methods

        protected virtual void Awake() {
            _transform = transform;

            G.U.Assert(gameObject.layer != Layer.Default, "This GameObject must exist on an attack Layer.");

            _boxCollider = G.U.Require<BoxCollider>(this);
            G.U.Assert(_boxCollider.isTrigger, "The BoxCollider Component must be a trigger.");

            _graphicsController = G.U.Require<GraphicsController>(this);

            end = new End(this);
            end.actions += ForceOnTriggerExit;
        }

        void Start() {
            //if the attack is spawned normally, through AttackAbility, Init should have already been called by now
            //however, if it hasn't, and there is an Optional Standalone Attack Ability assigned, use this ability
            if (!_isInitialized) {
                if (_attackAbility != null) {
                    InitInternal();
                } else {
                    G.U.Warning("If this Attack exists on its own, " +
                    "it should probably have a Standalone Attack Ability.");
                }
            }
        }

        void OnTriggerEnter(Collider other) {
            HitBox hitbox = other.GetComponent<HitBox>();
            if (hitbox == null) return;
            DamageTaker target = hitbox.damageTaker;
            if (target.end.isEnded) return;
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
            if (target.end.isEnded) return;
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

        protected virtual void OnDestroy() {
            end.InvokeActions();
            ForceReleaseTargets();
        }

#endregion

#region custom methods

        public void Init(AttackAbility attackAbility, bool isFlippedX, bool isPlayerCharacterAttacker) {
            if (_isInitialized) {
                G.U.Error("Init has already been called.");
                return;
            }
            _attackAbility = attackAbility;
            _isFlippedX = isFlippedX;
            _isPlayerCharacterAttacker = isPlayerCharacterAttacker;
            InitInternal();
        }

        protected virtual void InitInternal() {
            _isInitialized = true;

            if (_attackAbility.hasAttackLifetime) {
                //TODO: dispose callback causes optional delay for related attacks
                //E.G. SecS: stopping block makes the fire/shot m_attackRateSec restart at 1.5x:
                //alarm[e_pc_alarm.fire_ready] = global.framerate / fire_rate * 1.5;
                _attackAbility.timeThread.AddTrigger(_attackAbility.attackLifetime, Dispose);
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

            //TODO:
            //- set trajectory (travel direction)
            //- set speed/velocity (adjust for time thread & delta time)

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
                G.End(gameObject);
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
                if (at != null && at.isInProgress && !at.target.end.isEnded) {
                    at.StopTakingDamage();
                }
            }
            _attackTargets.Clear();
        }

#endregion

    }
}
