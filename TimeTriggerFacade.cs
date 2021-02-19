namespace _0G.Legacy
{
    public class TimeTriggerFacade : TimeTrigger
    {
        public override bool doesMultiFire
        {
            get
            {
                return base.doesMultiFire;
            }
            // Analysis disable ValueParameterNotUsed
            set
            {
                G.U.Err("doesMultiFire is unsupported in a time trigger facade.");
            }
            // Analysis restore ValueParameterNotUsed
        }

        public TimeTriggerFacade(TimeThread th) : base(th) { }

        public override void AddHandler(TimeTriggerHandler handler)
        {
            G.U.Err("AddHandler is unsupported in a time trigger facade.");
        }

        public override bool RemoveHandler(TimeTriggerHandler handler)
        {
            G.U.Err("RemoveHandler is unsupported in a time trigger facade.");
            return false;
        }

        public override bool HasHandler(TimeTriggerHandler handler)
        {
            G.U.Err("HasHandler is unsupported in a time trigger facade.");
            return false;
        }

        public override void Trigger()
        {
            G.U.Err("Trigger is unsupported in a time trigger facade.");
        }

        public override void Update(float delta)
        {
            G.U.Err("Update is unsupported in a time trigger facade.");
        }

        public override void Proceed()
        {
            G.U.Err("Proceed is unsupported in a time trigger facade.");
        }

        public override void Restart()
        {
            G.U.Err("Restart is unsupported in a time trigger facade.");
        }
    }
}