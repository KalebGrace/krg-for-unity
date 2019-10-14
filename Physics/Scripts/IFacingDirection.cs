namespace KRG
{
    public interface IFacingDirection : IBodyComponent
    {
        void OnFacingDirectionChange(Direction oldDirection, Direction newDirection);
    }
}
