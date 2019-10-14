using System;
using UnityEngine;

namespace KRG
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
