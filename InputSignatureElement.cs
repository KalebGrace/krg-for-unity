using System;

namespace KRG
{
    [Serializable]
    public struct InputSignatureElement
    {
        [Enum(typeof(InputCommand))]
        public int inputCommand;
    }
}