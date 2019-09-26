namespace KRG
{
    public interface IBodyComponent
    {
        GameObjectBody Body { get; }

        // EXAMPLE IMPLEMENTATION:
        // [SerializeField] private GameObjectBody m_Body;
        // public GameObjectBody Body => m_Body;

        void InitBody(GameObjectBody body);

        // EXAMPLE IMPLEMENTATION:
        // public void InitBody(GameObjectBody body) { m_Body = body; }
    }
}
