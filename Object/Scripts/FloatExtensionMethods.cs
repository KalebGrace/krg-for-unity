using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class FloatExtensionMethods {

        public static float ClampRotationDegrees(this float deg) {
            while (deg < 0f) deg += 360f;
            return deg % 360f;
        }

        public static Direction GetCardinalDirection(this float rotation) {
            rotation = rotation.ClampRotationDegrees();
            if (rotation >= 45f && rotation <= 135f) {
                return Direction.East;
            } else if (rotation > 135f && rotation < 225f) {
                return Direction.South;
            } else if (rotation >= 225f && rotation <= 315f) {
                return Direction.West;
            } else {
                return Direction.North;
            }
        }

        public static Direction GetOrdinalDirection(this float rotation) {
            rotation = rotation.ClampRotationDegrees();
            if (rotation >= 22.5f && rotation < 67.5f) {
                return Direction.Northeast;
            } else if (rotation >= 67.5f && rotation <= 112.5f) {
                return Direction.East;
            } else if (rotation > 112.5f && rotation <= 157.5f) {
                return Direction.Southeast;
            } else if (rotation > 157.5f && rotation < 202.5f) {
                return Direction.South;
            } else if (rotation >= 202.5f && rotation < 247.5f) {
                return Direction.Southwest;
            } else if (rotation >= 247.5f && rotation <= 292.5f) {
                return Direction.West;
            } else if (rotation > 292.5f && rotation <= 337.5f) {
                return Direction.Northwest;
            } else {
                return Direction.North;
            }
        }

        public static float GetSign(this float value) {
            return value < 0 ? -1 : 1;
        }

        /// <summary>
        /// Determines if this float value approximately equals the argument float value.
        /// </summary>
        /// <returns><c>true</c>, if the float values are approximately equal, <c>false</c> otherwise.</returns>
        /// <param name="value">This float value.</param>
        /// <param name="valueArg">The argument float value.</param>
        public static bool ApEquals(this float value, float valueArg) {
            return Mathf.Approximately(value, valueArg);
        }

        /// <summary>
        /// Determines if this float value is APPROXIMATELY zero.
        /// </summary>
        /// <returns><c>true</c> if this float value is APPROXIMATELY zero; otherwise, <c>false</c>.</returns>
        /// <param name="value">This float value.</param>
        public static bool IsZero(this float value) {
            return Mathf.Approximately(value, 0);
        }
    }
}
