using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class DirectionExtensionMethods {

        /// <summary>
        /// This event can be used to return a new _compassTopCustom value.
        /// As the name implies, this value is used as a custom compass top.
        /// Change it to handle e.g. field camera rotation or backwards controller effect.
        /// </summary>
        public static event System.Func<Direction> compassTopCustomUpdate;

        static Direction _compassTopCustom = Direction.North;

        public static Direction GetAbsoluteDirection(this Direction direction) {
            if (compassTopCustomUpdate != null) {
                _compassTopCustom = compassTopCustomUpdate();
            }
            return direction.GetAbsoluteDirection(_compassTopCustom);
        }

        public static Direction GetAbsoluteDirection(this Direction direction, Direction compassTop) {
            switch (compassTop.GetDirectionType()) {
                case DirectionType.Cardinal:
                    break;
                default:
                    string s = "The compassTop parameter must be cardinal. " +
                               "Its current value is \"{0}\" and its direction type is \"{1}\".";
                    G.U.Error(s, compassTop.ToString(), compassTop.GetDirectionType());
                    return direction;
            }
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return direction;
                case DirectionType.Relative:
                    int convertedForNorth = Mathf.Abs((int)direction + 1) * 45;
                    return (Direction)((convertedForNorth + (int)compassTop) % 360);
                default:
                    string s = "The direction \"{0}\" is of an unsupported direction type \"{1}\".";
                    G.U.Error(s, direction.ToString(), direction.GetDirectionType());
                    return direction;
            }
        }

        public static Direction GetCardinalDirection(this Direction direction) {
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                    return direction;
                case DirectionType.Ordinal:
                    if (direction == Direction.Northwest || direction == Direction.Southwest) {
                        return Direction.West;
                    } else {
                        return Direction.East;
                    }
                case DirectionType.Relative:
                    if (direction == Direction.UpLeft || direction == Direction.DownLeft) {
                        return Direction.Left;
                    } else if (direction == Direction.UpRight || direction == Direction.DownRight) {
                        return Direction.Right;
                    } else {
                        return direction;
                    }
                default:
                    G.U.Error("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        //by 45 degrees
        public static Direction GetClockwiseDirection(this Direction direction) {
            int newDir;
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    newDir = (int)direction + 45;
                    return (Direction)(newDir == 360 ? 0 : newDir);
                case DirectionType.Relative:
                    newDir = (int)direction - 1;
                    return (Direction)(newDir == -9 ? -1 : newDir);
                default:
                    G.U.Error("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        //by 45 degrees
        public static Direction GetCounterClockwiseDirection(this Direction direction) {
            int newDir;
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    newDir = (int)direction - 45;
                    return (Direction)(newDir == -45 ? 315 : newDir);
                case DirectionType.Relative:
                    newDir = (int)direction + 1;
                    return (Direction)(newDir == 0 ? -8 : newDir);
                default:
                    G.U.Error("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        public static DirectionType GetDirectionType(this Direction direction) {
            int dir = (int)direction;
            if (dir >= 0) {
                switch (dir % 90) {
                    case 0:
                        return DirectionType.Cardinal;
                    case 45:
                        return DirectionType.Ordinal;
                    default:
                        return DirectionType.Unknown;
                }
            } else if (dir >= -8) {
                return DirectionType.Relative;
            } else if (dir >= -10) {
                return DirectionType.Altitude;
            } else {
                return DirectionType.Unknown;
            }
        }

        public static Direction GetOppositeDirection(this Direction direction) {
            int newDir;
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    newDir = (int)direction + 180;
                    return (Direction)(newDir >= 360 ? newDir - 360 : newDir);
                case DirectionType.Relative:
                    newDir = (int)direction - 4;
                    return (Direction)(newDir <= -9 ? newDir + 8 : newDir);
                case DirectionType.Altitude:
                    return direction == Direction.Above ? Direction.Below : Direction.Above;
                default:
                    G.U.Error("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        public static Direction GetRotationalCorrection(this Direction target, Direction culprit) {
            int newDir;
            switch (target.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    switch (culprit.GetDirectionType()) {
                        case DirectionType.Cardinal:
                        case DirectionType.Ordinal:
                            newDir = (int)target - (int)culprit;
                            return (Direction)(newDir.ClampRotationDegrees());
                        default:
                            G.U.Error("Currently unsupported.");
                            return Direction.Unknown;
                    }
                default:
                    G.U.Error("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        public static float GetRotationalFloat(this Direction direction) {
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return (int)direction;
                default:
                    G.U.Error("Currently unsupported.");
                    return 0f;
            }
        }

        public static Vector3 GetRotationalVector3(this Direction direction) {
            switch (direction.GetDirectionType()) {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return new Vector3(0f, (int)direction, 0f);
                default:
                    G.U.Error("Currently unsupported.");
                    return Vector3.zero;
            }
        }
    }
}
