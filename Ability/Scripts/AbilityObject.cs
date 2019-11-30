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
