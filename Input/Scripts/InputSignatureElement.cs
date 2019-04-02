using System;
using System.Collections.Generic;
using KRG;
using UnityEngine;

namespace KRG
{
    [Serializable]
    public struct InputSignatureElement
    {
        [Enum(typeof(InputCommand))]
        public int inputCommand;
    }
}
