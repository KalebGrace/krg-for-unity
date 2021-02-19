namespace _0G.Legacy
{
#if KRG_X_ODIN
    public class OrderAttribute : Sirenix.OdinInspector.PropertyOrderAttribute
    {
        public OrderAttribute(int order) : base(order) { }
    }
#else
    public class OrderAttribute : UnityEngine.PropertyAttribute
    {
        public OrderAttribute(int order) { }
    }
#endif
}