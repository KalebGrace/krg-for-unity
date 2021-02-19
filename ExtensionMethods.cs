using UnityEngine;

namespace _0G.Legacy
{
    public static class ExtensionMethods
    {
        // COLLIDER

        public static GameObjectBody GetBody(this Collider other) => other.GetComponent<GameObjectBody>();

        public static bool IsFromAttack(this Collider other)
        {
            GameObjectBody body = other.GetBody();
            return body != null && body.IsAttack;
        }

        public static bool IsFromCharacter(this Collider other)
        {
            GameObjectBody body = other.GetBody();
            return body != null && body.IsCharacter;
        }

        // COLLISION

        public static GameObjectBody GetBody(this Collision collision) => collision.collider.GetComponent<GameObjectBody>();

        public static bool IsFromAttack(this Collision collision)
        {
            GameObjectBody body = collision.GetBody();
            return body != null && body.IsAttack;
        }

        public static bool IsFromCharacter(this Collision collision)
        {
            GameObjectBody body = collision.GetBody();
            return body != null && body.IsCharacter;
        }

        // VECTOR3

        public delegate float V3Func(float value);

        public static Vector2 Abs(this Vector3 v3)
        {
            v3.x = Mathf.Abs(v3.x);
            v3.y = Mathf.Abs(v3.y);
            v3.z = Mathf.Abs(v3.z);
            return v3;
        }

        public static Vector3 Add(this Vector3 v3, float x = 0, float y = 0, float z = 0)
        {
            v3.x += x;
            v3.y += y;
            v3.z += z;
            return v3;
        }

        public static void AddRef(this ref Vector3 v3, float x = 0, float y = 0, float z = 0)
        {
            v3.x += x;
            v3.y += y;
            v3.z += z;
        }

        /// <summary>
        /// Is approximately equal to...
        /// </summary>
        /// <param name="me">The first value to be compared.</param>
        /// <param name="v3">The second value to be compared.</param>
        /// <param name="tolerance">If greater than 0, use this value. Else use Mathf.Approximately.</param>
        public static bool Ap(this Vector3 me, Vector3 v3, float tolerance = 0)
        {
            bool x, y, z;
            if (tolerance > 0)
            {
                x = Mathf.Abs(me.x - v3.x) <= tolerance;
                y = Mathf.Abs(me.y - v3.y) <= tolerance;
                z = Mathf.Abs(me.z - v3.z) <= tolerance;
            }
            else
            {
                x = Mathf.Approximately(me.x, v3.x);
                y = Mathf.Approximately(me.y, v3.y);
                z = Mathf.Approximately(me.z, v3.z);
            }
            return x && y && z;
        }

        public static Vector3 Func(this Vector3 v3, V3Func fx = null, V3Func fy = null, V3Func fz = null)
        {
            if (fx != null) v3.x = fx(v3.x);
            if (fy != null) v3.y = fy(v3.y);
            if (fz != null) v3.z = fz(v3.z);
            return v3;
        }

        public static Vector3 Multiply(this Vector3 v3, float x = 1, float y = 1, float z = 1)
        {
            v3.x *= x;
            v3.y *= y;
            v3.z *= z;
            return v3;
        }

        public static Vector3 Multiply(this Vector3 v3, Vector3 m)
        {
            v3.x *= m.x;
            v3.y *= m.y;
            v3.z *= m.z;
            return v3;
        }

        public static Vector3 Set2(this Vector3 v3, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue) v3.x = x.Value;
            if (y.HasValue) v3.y = y.Value;
            if (z.HasValue) v3.z = z.Value;
            return v3;
        }

        public static Vector3 SetSign(this Vector3 v3, bool? x = null, bool? y = null, bool? z = null)
        {
            if (x.HasValue) v3.x = Mathf.Abs(v3.x) * (x.Value ? 1f : -1f);
            if (y.HasValue) v3.y = Mathf.Abs(v3.y) * (y.Value ? 1f : -1f);
            if (z.HasValue) v3.z = Mathf.Abs(v3.z) * (z.Value ? 1f : -1f);
            return v3;
        }

        public static Vector3 SetX(this Vector3 v3, float x)
        {
            v3.x = x;
            return v3;
        }

        public static Vector3 SetY(this Vector3 v3, float y)
        {
            v3.y = y;
            return v3;
        }

        public static Vector3 SetZ(this Vector3 v3, float z)
        {
            v3.z = z;
            return v3;
        }

        public static Vector2 ToVector2(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }
    }
}