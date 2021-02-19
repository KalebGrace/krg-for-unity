using System;
using UnityEngine;

namespace _0G.Legacy
{
    public class EnumAttribute : PropertyAttribute
    {
        public Type EnumType { get; private set; }

        public EnumAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}