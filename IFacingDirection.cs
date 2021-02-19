namespace _0G.Legacy
{
    public interface IFacingDirection : IBodyComponent
    {
        void OnFacingDirectionChange(Direction oldDirection, Direction newDirection);
    }
}