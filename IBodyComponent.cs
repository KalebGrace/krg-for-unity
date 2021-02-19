namespace _0G.Legacy
{
    public interface IBodyComponent
    {
        GameObjectBody Body { get; }

        void InitBody(GameObjectBody body);

        // COPYPASTA THE FOLLOWING:
        /*
        
        [SerializeField]
        private GameObjectBody m_Body = default;

        public GameObjectBody Body => m_Body;

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        */
    }
}