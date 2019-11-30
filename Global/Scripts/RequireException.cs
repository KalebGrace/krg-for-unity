using System;

namespace KRG
{
    public class RequireException : Exception
    {
        private readonly Type type;

        public RequireException(Type type)
        {
            this.type = type;
        }

        public override string Message => string.Format("{0} ~ {1}", type, base.Message);
    }
}
