namespace _0G.Legacy
{
    public class End
    {
        public event System.Action actions;

        public State state { get; private set; }

        public enum State
        {
            Endless = 0,
            Ending = 1,
            Ended = 2,
        }

        public bool wasInvoked
        {
            get
            {
                return state != State.Endless;
            }
        }

        public void Invoke()
        {
            if (wasInvoked) return;

            state = State.Ending;

            if (actions != null) actions();

            state = State.Ended;
        }
    }
}