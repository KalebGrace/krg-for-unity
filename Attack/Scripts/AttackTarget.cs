using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Attack target.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    public class AttackTarget {

#region private constants

        const string _infiniteLoopError = "To prevent an infinite loop, "
                                          + "damage has ceased for the remainder of this collision. "
                                          + "Consider doing one or more of the following: "
                                          + "1. Set AttackAbility Hp Damage Rate to true. "
                                          + "2. Set AttackAbility Max Hits Per Target to true. "
                                          + "3. Set AttackAbility Causes Invulnerability to true, "
                                          + "and DamageProfile Invulnerability Time to a positive value.";

#endregion

#region private fields

        readonly AttackAbility _attackAbility;
        Vector3 _attackPositionCenter;
        System.Action _damageDealtCallback;
        TimeTrigger _damageTimeTrigger;
        int _hitCount;
        Vector3 _hitPositionCenter;
        bool _isDelayedDueToInvulnerability;
        bool _isDelayedDueToTimeThreadPause;

#endregion

#region properties

        bool isHitLimitReached {
            get {
                //has the "hit" count reached the maximum number of "hits" (i.e. damage method calls)?
                return _attackAbility.hasMaxHitsPerTarget && _hitCount >= _attackAbility.maxHitsPerTarget;
            }
        }

        public bool isInProgress { get; private set; }

        public DamageTaker target { get; private set; }

#endregion

#region methods 1 - Public Methods

        public AttackTarget(AttackAbility attackAbility, DamageTaker target, System.Action damageDealtCallback) {
            _attackAbility = attackAbility;
            this.target = target;
            _damageDealtCallback = damageDealtCallback;
        }

        public void StartTakingDamage(Vector3 attackPositionCenter, Vector3 hitPositionCenter) {
            G.U.Assert(!isInProgress,
                "StartTakingDamage was called, but this AttackTarget has already started taking damage.");
            isInProgress = true;
            _attackPositionCenter = attackPositionCenter;
            _hitPositionCenter = hitPositionCenter;
            if (!isHitLimitReached) CheckForDelay(StartTakingDamageForReal);
        }

        public void StopTakingDamage() {
            G.U.Assert(isInProgress,
                "StopTakingDamage was called, but this AttackTarget has already stopped taking damage.");
            isInProgress = false;
            //
            if (!isHitLimitReached) CheckForDelayCallbackRemoval(StopTakingDamageForReal);
        }

#endregion

#region methods 2 - Check For, And Handle, Delays

        void CheckForDelay(System.Action onNoDelay) {
            if (target.IsInvulnerableTo(_attackAbility)) {
                _isDelayedDueToInvulnerability = true;
                target.AddEndInvulnerabilityHandler(DelayCallback);
            } else if (_attackAbility.timeThread.isPaused) {
                _isDelayedDueToTimeThreadPause = true;
                _attackAbility.timeThread.AddUnpauseHandler(DelayCallback);
            } else {
                onNoDelay();
            }
        }

        void CheckForDelayCallbackRemoval(System.Action onNoRemoval) {
            if (_isDelayedDueToInvulnerability) {
                _isDelayedDueToInvulnerability = false;
                target.RemoveEndInvulnerabilityHandler(DelayCallback);
            } else if (_isDelayedDueToTimeThreadPause) {
                _isDelayedDueToTimeThreadPause = false;
                _attackAbility.timeThread.RemoveUnpauseHandler(DelayCallback);
            } else {
                onNoRemoval();
            }
        }

        void DelayCallback() {
            CheckForDelayCallbackRemoval(DelayCallbackRemovalError);
            CheckForDelay(StartTakingDamageForReal); //check for any possible additional delays
        }

        void DelayCallbackRemovalError() {
            G.U.Err("DelayCallback was made with no delay flags set to true.");
        }

#endregion

#region methods 3 - Start & Stop Damage For Real

        void StartTakingDamageForReal() {
            if (_damageTimeTrigger != null) {
                _attackAbility.timeThread.LinkTrigger(_damageTimeTrigger);
            } else {
                Damage();
                if (!target.end.wasInvoked && !target.isKnockedOut && !isHitLimitReached) {
                    if (_attackAbility.hasHPDamageRate) {
                        _damageTimeTrigger = _attackAbility.timeThread.AddTrigger(
                            _attackAbility.hpDamageRateSec, Damage);
                        _damageTimeTrigger.doesMultiFire = true;
                    } else {
                        CheckForDelay(DamageInfiniteLoopCheck);
                    }
                }
            }
        }

        void StopTakingDamageForReal() {
            if (_damageTimeTrigger != null) {
                G.U.Assert(_attackAbility.timeThread.UnlinkTrigger(_damageTimeTrigger));
            }
        }

#endregion

#region methods 4 - The Actual Damage

        void Damage() {
            bool isHit = target.Damage(_attackAbility, _attackPositionCenter, _hitPositionCenter);
            if (isHit) {
                _hitCount++;
                _damageDealtCallback();
            }
        }

        void Damage(TimeTrigger tt) {
            Damage();
            if (!target.end.wasInvoked && !target.isKnockedOut && !isHitLimitReached) {
                tt.Proceed();
            }
        }

        void DamageInfiniteLoopCheck() {
            if (_attackAbility.hasMaxHitsPerTarget) {
                StartTakingDamageForReal();
            } else {
                G.U.Err(_infiniteLoopError);
            }
        }

#endregion

    }
}
