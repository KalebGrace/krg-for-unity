using UnityEngine;

namespace KRG
{
    public class AttackTarget
    {
        // private constants

        private const string _infiniteLoopError = "To prevent an infinite loop, "
                                          + "damage has ceased for the remainder of this collision. "
                                          + "Consider doing one or more of the following: "
                                          + "1. Set AttackAbility Hp Damage Rate to true. "
                                          + "2. Set AttackAbility Max Hits Per Target to true. "
                                          + "3. Set AttackAbility Causes Invulnerability to true, "
                                          + "and DamageProfile Invulnerability Time to a positive value.";

        // private fields

        private readonly Attack _attack;
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

        public AttackTarget(Attack attack, DamageTaker target, System.Action damageDealtCallback)
        {
            _attack = attack;
            _attackAbility = attack.attackAbility;
            this.target = target;
            _damageDealtCallback = damageDealtCallback;
        }

        public void StartTakingDamage(Vector3 attackPositionCenter, Vector3 hitPositionCenter)
        {
            G.U.Assert(!isInProgress,
                "StartTakingDamage was called, but this AttackTarget has already started taking damage.");
            isInProgress = true;
            _attackPositionCenter = attackPositionCenter;
            _hitPositionCenter = hitPositionCenter;
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
                if (target != null && !target.IsKnockedOut && !isHitLimitReached)
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
            G.U.Assert(_attack != null);
            bool isHit = target.Damage(_attack, _attackPositionCenter, _hitPositionCenter);
            if (isHit)
            {
                _hitCount++;
                _damageDealtCallback();
            }
        }

        private void Damage(TimeTrigger tt)
        {
            Damage();
            if (target != null && !target.IsKnockedOut && !isHitLimitReached)
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
                G.U.Warn(_infiniteLoopError);
            }
        }
    }
}
