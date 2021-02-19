namespace _0G.Legacy
{
    public interface IDestroyedEvent<T>
    {
        event System.Action<T> Destroyed;

        // COPYPASTA THE FOLLOWING:
        /*
        
        public event System.Action<T> Destroyed;
        
        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        */
    }
}