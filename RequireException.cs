using System;

namespace KRG
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