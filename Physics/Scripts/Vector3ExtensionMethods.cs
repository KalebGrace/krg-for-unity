using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class Vector3ExtensionMethods {

        public delegate float V3Func(float value);

        public static Vector3 Add(this Vector3 v3, float x = 0, float y = 0, float z = 0) {
            v3.x += x;
            v3.y += y;
            v3.z += z;
            return v3;
        }

        public static Vector3 Func(this Vector3 v3, V3Func fx = null, V3Func fy = null, V3Func fz = null) {
            if (fx != null) v3.x = fx(v3.x);
            if (fy != null) v3.y = fy(v3.y);
            if (fz != null) v3.z = fz(v3.z);
            return v3;
        }

        public static Vector3 Multiply(this Vector3 v3, float x = 1, float y = 1, float z = 1) {
            v3.x *= x;
            v3.y *= y;
            v3.z *= z;
            return v3;
        }

        public static Vector3 Multiply(this Vector3 v3, Vector3 m) {
            v3.x *= m.x;
            v3.y *= m.y;
            v3.z *= m.z;
            return v3;
        }

        public static Vector3 Set2(this Vector3 v3, float? x = null, float? y = null, float? z = null) {
            if (x.HasValue) v3.x = x.Value;
            if (y.HasValue) v3.y = y.Value;
            if (z.HasValue) v3.z = z.Value;
            return v3;
        }

        public static Vector3 SetX(this Vector3 v3, float x) {
            v3.x = x;
            return v3;
        }

        public static Vector3 SetY(this Vector3 v3, float y) {
            v3.y = y;
            return v3;
        }

        public static Vector3 SetZ(this Vector3 v3, float z) {
            v3.z = z;
            return v3;
        }
    }
}
