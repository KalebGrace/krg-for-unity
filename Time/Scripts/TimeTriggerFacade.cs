using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class TimeTriggerFacade : TimeTrigger {

        public override bool doesMultiFire {
            get {
                return base.doesMultiFire;
            }
            // Analysis disable ValueParameterNotUsed
            set {
                G.U.Error("doesMultiFire is unsupported in a time trigger facade.");
            }
            // Analysis restore ValueParameterNotUsed
        }

        public TimeTriggerFacade(TimeThread th) : base(th) {
        }

        public override void AddHandler(TimeTriggerHandler handler) {
            G.U.Error("AddHandler is unsupported in a time trigger facade.");
        }

        public override bool RemoveHandler(TimeTriggerHandler handler) {
            G.U.Error("RemoveHandler is unsupported in a time trigger facade.");
            return false;
        }

        public override bool HasHandler(TimeTriggerHandler handler) {
            G.U.Error("HasHandler is unsupported in a time trigger facade.");
            return false;
        }

        public override void Trigger() {
            G.U.Error("Trigger is unsupported in a time trigger facade.");
        }

        public override void Update(float delta) {
            G.U.Error("Update is unsupported in a time trigger facade.");
        }

        public override void Proceed() {
            G.U.Error("Proceed is unsupported in a time trigger facade.");
        }

        public override void Restart() {
            G.U.Error("Restart is unsupported in a time trigger facade.");
        }
    }
}
