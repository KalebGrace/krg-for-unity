using UnityEngine;

namespace _0G.Legacy
{
    public class AttackTarget
    {
        // private fields

        private readonly Attack _attack;
        private readonly AttackAbility _attackAbility;
        private AttackAbility _attackAbilityDPSClone;
        private Vector3 _attackPositionCenter;
        private System.Action _damageDealtCallback;
        private int _hitCount;
        private Vector3 _hitPositionCenter;
        private bool _isDelayedDueToInvulnerability;
        private bool _isDelayedDueToTimeThreadPause;
        private TimeTrigger _timeTriggerDPSClone;

        // properties

        private bool isHitLimitReached
        {
            get
            {
                // has the "hit" count reached the maximum number of "hits" (i.e. damage method calls)?
                return _hitCount >= 1;
            }
        }

        public DamageTaker target { get; private set; }

        // methods 1 - Public Methods

        public AttackTarget(Attack attack, DamageTaker target, System.Action damageDealtCallback)
        {
            _attack = attack;
            _attackAbility = attack.attackAbility;
            this.target = target;
            _damageDealtCallback = damageDealtCallback;
        }

        public void StartTakingDamage(Vector3 attackPositionCenter, Vector3 hitPositionCenter)
        {
            _attackPositionCenter = attackPositionCenter;
            _hitPositionCenter = hitPositionCenter;
            if (!isHitLimitReached) CheckForDelay();

            if (_attackAbility.requiresDPSClone)
            {
                _attackAbilityDPSClone = _attackAbility.GetDPSClone(AttackAbility.DPS_INTERVAL);
                _timeTriggerDPSClone = _attackAbilityDPSClone.TimeThread.AddTrigger(AttackAbility.DPS_INTERVAL, Damage);
                _timeTriggerDPSClone.doesMultiFire = true;
            }
        }

        public void StopTakingDamage()
        {
            if (!isHitLimitReached) CheckForDelayCallbackRemoval(null);

            _timeTriggerDPSClone?.Dispose();
            _attackAbilityDPSClone = null;
        }

        // methods 2 - Check For, And Handle, Delays

        private void CheckForDelay()
        {
            if (!target.CanBeDamagedBy(_attack.attacker, _attackAbility))
            {
                _isDelayedDueToInvulnerability = true;
                // actually no delay, just ignore
            }
            else if (_attackAbility.TimeThread.isPaused)
            {
                _isDelayedDueToTimeThreadPause = true;
                _attackAbility.TimeThread.AddUnpauseHandler(DelayCallback);
            }
            else
            {
                Damage();
            }
        }

        private void CheckForDelayCallbackRemoval(System.Action onNoRemoval)
        {
            if (_isDelayedDueToInvulnerability)
            {
                _isDelayedDueToInvulnerability = false;
                // actually no delay, just ignore
            }
            else if (_isDelayedDueToTimeThreadPause)
            {
                _isDelayedDueToTimeThreadPause = false;
                _attackAbility.TimeThread.RemoveUnpauseHandler(DelayCallback);
            }
            else
            {
                onNoRemoval?.Invoke();
            }
        }

        private void DelayCallback()
        {
            CheckForDelayCallbackRemoval(DelayCallbackRemovalError);
            CheckForDelay(); //check for any possible additional delays
        }

        private void DelayCallbackRemovalError()
        {
            G.U.Err("DelayCallback was made with no delay flags set to true.");
        }

        // methods 3 - The Actual Damage

        private void Damage()
        {
            bool isHit = target.Damage(_attack, _attackPositionCenter, _hitPositionCenter);
            if (isHit)
            {
                _hitCount++;
                _damageDealtCallback();
            }
        }

        // DPS clone damage method
        private void Damage(TimeTrigger tt)
        {
            if (target == null)
            {
                G.U.Warn("Taking DPS clone damage when target has been destroyed.");
                return;
            }
            Vector3 p = target.transform.position;
            target.Damage(_attack.attacker, _attackAbilityDPSClone, p, p);
            tt.Proceed();
        }
    }
}