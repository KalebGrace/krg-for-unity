using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [Serializable]
    public class EnumGeneric
    {
        [SerializeField]
        [FormerlySerializedAs("m_intValue")]
        int _intValue;

#pragma warning disable 0414
        //used only for EnumGenericDrawer; may be modified to not match internal type
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_stringType")]
        string _stringType;
#pragma warning restore 0414

        Type _type;

        public bool hasType { get { return _type != null; } }

        public int length { get; private set; }

        public string[] names { get; private set; }

        public int intValue { get { return _intValue; } }

        public Type type
        {
            get
            {
                G.U.Assert(hasType, "The type is null. (If you wish to do a null compare, use hasType instead.)");
                return _type;
            }
            set
            {
                if (AssertIsEnum(value))
                {
                    _stringType = value.ToString();
                    _type = value;
                    names = Enum.GetNames(value);
                    length = names.Length;
                    values = Enum.GetValues(value);
                }
            }
        }

        public Array values { get; private set; }

        public EnumGeneric(string enumType)
        {
            Type t = GetType(enumType);
            if (t != null)
            {
                type = t;
            }
        }

        public EnumGeneric(Type enumType)
        {
            type = enumType;
        }

        public EnumGeneric(Type enumType, int intValue)
        {
            type = enumType;
            _intValue = intValue;
        }

        public bool HasSequentialValues(int startValue, int increment)
        {
            return HasSequentialValues(values, startValue, increment);
        }

        public static bool HasSequentialValues(Type enumType, int startValue, int increment)
        {
            if (AssertIsEnum(enumType))
            {
                Array vals = Enum.GetValues(enumType);
                return HasSequentialValues(vals, startValue, increment);
            }
            else
            {
                return false;
            }
        }

        static bool HasSequentialValues(Array vals, int startValue, int increment)
        {
            int len = vals.Length;
            if (len == 0)
            {
                return false;
            }
            bool match;
            for (int i = 0; i < len; i++)
            {
                match = false;
                foreach (var value in vals)
                {
                    if ((int) value == startValue + increment * i)
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    return false;
                }
            }
            return true;
        }

        static Type GetType(string enumType)
        {
            if (string.IsNullOrEmpty(enumType))
            {
                G.U.Err("The type is null or empty. The type needs to be an enum (enumeration).");
                return null;
            }
            Type t = System.Reflection.Assembly.GetExecutingAssembly().GetType(enumType);
            if (t == null)
            {
                var parts = enumType.Split('.');
                string enumType0G = "_0G.Legacy." + parts[parts.Length - 1];
                t = System.Reflection.Assembly.GetExecutingAssembly().GetType(enumType0G);
                G.U.Assert(t != null, string.Format("The type \"{0}\" cannot be found.", enumType));
            }
            return t;
        }

        public Enum ToEnum(int value)
        {
            return ToEnum(_type, value);
        }

        public static Enum ToEnum(string enumType, int value)
        {
            Type t = GetType(enumType);
            return t != null ? ToEnum(t, value) : null;
        }

        public static Enum ToEnum(Type enumType, int value)
        {
            if (AssertIsEnum(enumType))
            {
                object o = Enum.ToObject(enumType, value);
                return (Enum) o;
            }
            else
            {
                return null;
            }
        }

        static bool AssertIsEnum(Type enumType)
        {
            if (enumType == null)
            {
                G.U.Err("The type is null. The type needs to be an enum (enumeration).");
                return false;
            }
            else if (!enumType.IsEnum)
            {
                G.U.Err("The type \"{0}\" is invalid. The type needs to be an enum (enumeration).", enumType);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}