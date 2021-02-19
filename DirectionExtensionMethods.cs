using UnityEngine;

namespace _0G.Legacy
{
    public static class DirectionExtensionMethods
    {
        /// <summary>
        /// This event can be used to return a new _compassTopCustom value.
        /// As the name implies, this value is used as a custom compass top.
        /// Change it to handle e.g. field camera rotation or backwards controller effect.
        /// </summary>
        public static event System.Func<Direction> compassTopCustomUpdate;

        static Direction _compassTopCustom = Direction.North;

        public static Direction GetAbsoluteDirection(this Direction direction)
        {
            if (compassTopCustomUpdate != null)
            {
                _compassTopCustom = compassTopCustomUpdate();
            }
            return direction.GetAbsoluteDirection(_compassTopCustom);
        }

        public static Direction GetAbsoluteDirection(this Direction direction, Direction compassTop)
        {
            switch (compassTop.GetDirectionType())
            {
                case DirectionType.Cardinal:
                    break;
                default:
                    string s = "The compassTop parameter must be cardinal. " +
                        "Its current value is \"{0}\" and its direction type is \"{1}\".";
                    G.U.Err(s, compassTop.ToString(), compassTop.GetDirectionType());
                    return direction;
            }
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return direction;
                case DirectionType.Relative:
                    int convertedForNorth = Mathf.Abs((int) direction + 1) * 45;
                    return (Direction) ((convertedForNorth + (int) compassTop) % 360);
                default:
                    string s = "The direction \"{0}\" is of an unsupported direction type \"{1}\".";
                    G.U.Err(s, direction.ToString(), direction.GetDirectionType());
                    return direction;
            }
        }

        public static Direction GetCardinalDirection(this Direction direction)
        {
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                    return direction;
                case DirectionType.Ordinal:
                    if (direction == Direction.NorthWest || direction == Direction.SouthWest)
                    {
                        return Direction.West;
                    }
                    else
                    {
                        return Direction.East;
                    }
                case DirectionType.Relative:
                    if (direction == Direction.UpLeft || direction == Direction.DownLeft)
                    {
                        return Direction.Left;
                    }
                    else if (direction == Direction.UpRight || direction == Direction.DownRight)
                    {
                        return Direction.Right;
                    }
                    else
                    {
                        return direction;
                    }
                default:
                    G.U.Err("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        //by 45 degrees
        public static Direction GetClockwiseDirection(this Direction direction)
        {
            int newDir;
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    newDir = (int) direction + 45;
                    return (Direction) (newDir == 360 ? 0 : newDir);
                case DirectionType.Relative:
                    newDir = (int) direction - 1;
                    return (Direction) (newDir == -9 ? -1 : newDir);
                default:
                    G.U.Err("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        //by 45 degrees
        public static Direction GetCounterClockwiseDirection(this Direction direction)
        {
            int newDir;
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    newDir = (int) direction - 45;
                    return (Direction) (newDir == -45 ? 315 : newDir);
                case DirectionType.Relative:
                    newDir = (int) direction + 1;
                    return (Direction) (newDir == 0 ? -8 : newDir);
                default:
                    G.U.Err("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        public static DirectionType GetDirectionType(this Direction direction)
        {
            int dir = (int) direction;
            if (dir >= 0)
            {
                switch (dir % 90)
                {
                    case 0:
                        return DirectionType.Cardinal;
                    case 45:
                        return DirectionType.Ordinal;
                    default:
                        return DirectionType.Unknown;
                }
            }
            else if (dir >= -8)
            {
                return DirectionType.Relative;
            }
            else if (dir >= -10)
            {
                return DirectionType.Altitude;
            }
            else
            {
                return DirectionType.Unknown;
            }
        }

        public static Direction GetOpposite(this Direction direction)
        {
            int d = (int) direction;
            if (d >= 0) // Cardinal & Ordinal
            {
                return (Direction) ((d + 180) % 360);
            }
            if (d >= -8) // Relative
            {
                d -= 4;
                return (Direction) (d >= -8 ? d : d + 8);
            }
            switch (direction)
            {
                case Direction.Above:
                    return Direction.Below;
                case Direction.Below:
                    return Direction.Above;
            }
            G.U.Err("Unsupported opposite for direction {0}.", direction);
            return Direction.Unknown;
        }

        public static Direction GetRotationalCorrection(this Direction target, Direction culprit)
        {
            int newDir;
            switch (target.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    switch (culprit.GetDirectionType())
                    {
                        case DirectionType.Cardinal:
                        case DirectionType.Ordinal:
                            newDir = (int) target - (int) culprit;
                            return (Direction) (newDir.ClampRotationDegrees());
                        default:
                            G.U.Err("Currently unsupported.");
                            return Direction.Unknown;
                    }
                default:
                    G.U.Err("Currently unsupported.");
                    return Direction.Unknown;
            }
        }

        public static float GetRotationalFloat(this Direction direction)
        {
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return (int) direction;
                default:
                    G.U.Err("Currently unsupported.");
                    return 0f;
            }
        }

        public static Vector3 GetRotationalVector3(this Direction direction)
        {
            switch (direction.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    return new Vector3(0f, (int) direction, 0f);
                default:
                    G.U.Err("Currently unsupported.");
                    return Vector3.zero;
            }
        }

        public static bool IsOpposite(this Direction source, Direction target)
        {
            if (source == Direction.Unknown || target == Direction.Unknown)
            {
                return false;
            }
            return source.GetOpposite() == target;
        }
    }
}