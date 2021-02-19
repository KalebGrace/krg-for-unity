using System;

namespace _0G.Legacy
{
    [Serializable]
    public struct InputSignatureElement
    {
        [Enum(typeof(InputCommand))]
        public int inputCommand;
    }
}