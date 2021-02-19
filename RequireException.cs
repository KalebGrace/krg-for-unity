using System;

namespace _0G.Legacy
{
    public class RequireException : Exception
    {
        readonly Type type;

        public RequireException(Type type)
        {
            this.type = type;
        }

        public override string Message { get { return string.Format("{0} ~ {1}", type, base.Message); } }
    }
}