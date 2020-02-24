namespace KRG
{
    public sealed class Hurtbox : ColliderController
    {
        public DamageTaker DamageTaker => DamageTakerOverride != null ? DamageTakerOverride : Body.Refs.DamageTaker;

        public DamageTaker DamageTakerOverride { get; set; }
    }
}
