namespace KRG
{
    public sealed class Hurtbox : ColliderController
    {
        public DamageTaker DamageTaker => Body.Refs.DamageTaker;
    }
}
