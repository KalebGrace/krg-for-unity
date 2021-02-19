using UnityEngine;

namespace _0G.Legacy
{
    public static class Vector2EM // Vector2 extension methods
    {
        public delegate float V2Func(float value);

        public static Vector2 Abs(this Vector2 v2)
        {
            v2.x = Mathf.Abs(v2.x);
            v2.y = Mathf.Abs(v2.y);
            return v2;
        }

        public static Vector2 Add(this Vector2 v2, float x = 0, float y = 0)
        {
            v2.x += x;
            v2.y += y;
            return v2;
        }

        public static Vector2 Func(this Vector2 v2, V2Func fx = null, V2Func fy = null)
        {
            if (fx != null) v2.x = fx(v2.x);
            if (fy != null) v2.y = fy(v2.y);
            return v2;
        }

        public static Vector2 Multiply(this Vector2 v2, float x = 1, float y = 1)
        {
            v2.x *= x;
            v2.y *= y;
            return v2;
        }

        public static Vector2 Multiply(this Vector2 v2, Vector2 m)
        {
            v2.x *= m.x;
            v2.y *= m.y;
            return v2;
        }

        public static Vector2 SetX(this Vector2 v2, float x)
        {
            v2.x = x;
            return v2;
        }

        public static Vector2 SetY(this Vector2 v2, float y)
        {
            v2.y = y;
            return v2;
        }
    }
}