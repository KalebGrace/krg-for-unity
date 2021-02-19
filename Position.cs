using UnityEngine;

namespace _0G.Legacy
{
    //TODO: Could possibly be renamed Vector3P or something.
    public class Position
    {
        float _axisEW; //"x"
        float _axisAB; //"y"
        float _axisNS; //"z"

        //Absolute cardinal directions.
        public float n
        {
            get { return GetDirectionMagnitude(Direction.North); }
            set { SetDirectionMagnitude(Direction.North, value); }
        }

        public float e
        {
            get { return GetDirectionMagnitude(Direction.East); }
            set { SetDirectionMagnitude(Direction.East, value); }
        }

        public float s
        {
            get { return GetDirectionMagnitude(Direction.South); }
            set { SetDirectionMagnitude(Direction.South, value); }
        }

        public float w
        {
            get { return GetDirectionMagnitude(Direction.West); }
            set { SetDirectionMagnitude(Direction.West, value); }
        }

        //Altitude (above & below).
        public float a
        {
            get { return _axisAB; }
            set { _axisAB = value; }
        }

        public float b
        {
            get { return -_axisAB; }
            set { _axisAB = -value; }
        }

        //Relative direction (based on field camera rotation).
        public float u
        {
            get { return GetDirectionMagnitude(Direction.Up); }
            set { SetDirectionMagnitude(Direction.Up, value); }
        }

        public float r
        {
            get { return GetDirectionMagnitude(Direction.Right); }
            set { SetDirectionMagnitude(Direction.Right, value); }
        }

        public float d
        {
            get { return GetDirectionMagnitude(Direction.Down); }
            set { SetDirectionMagnitude(Direction.Down, value); }
        }

        public float l
        {
            get { return GetDirectionMagnitude(Direction.Left); }
            set { SetDirectionMagnitude(Direction.Left, value); }
        }

        public static implicit operator Vector3(Position p)
        {
            return p.v3;
        }

        public Vector3 v3
        {
            get
            {
                return new Vector3(_axisEW, _axisAB, _axisNS);
            }
            set
            {
                _axisEW = value.x;
                _axisAB = value.y;
                _axisNS = value.z;
            }
        }

        public Direction initialDirection { get; private set; }

        public SpatialOptions initialOptions { get; private set; }

        public Position() { }

        public Position(Vector3 vector3)
        {
            initialDirection = Direction.Unknown;
            initialOptions = SpatialOptions.None;
            v3 = vector3;
        }

        public Position(Vector3 vector3, SpatialOptions options)
        {
            initialDirection = Direction.Unknown;
            initialOptions = options;
            v3 = vector3;
        }

        public Position(float east, float above, float north)
        {
            initialDirection = Direction.Unknown;
            initialOptions = SpatialOptions.None;
            _axisEW = east;
            _axisAB = above;
            _axisNS = north;
        }

        public Position(int east, int above, int north) : this((float) east, (float) above, (float) north) { }

        public Position(Direction direction, float magnitude)
        {
            initialDirection = direction;
            initialOptions = SpatialOptions.None;
            SetDirectionMagnitude(direction, magnitude);
        }

        public Position(Direction direction, float magnitude, SpatialOptions options)
        {
            initialDirection = direction;
            initialOptions = options;
            SetDirectionMagnitude(direction, magnitude, options);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", _axisEW, _axisAB, _axisNS);
        }

        float GetDirectionMagnitude(Direction dir)
        {
            return GetDirectionMagnitude(dir, SpatialOptions.TrueOrdinal);
        }

        float GetDirectionMagnitude(Direction dir, SpatialOptions options)
        {
            switch (dir.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Altitude:
                    switch (dir)
                    {
                        case Direction.North:
                            return _axisNS;
                        case Direction.East:
                            return _axisEW;
                        case Direction.South:
                            return -_axisNS;
                        case Direction.West:
                            return -_axisEW;
                        case Direction.Above:
                            return _axisAB;
                        case Direction.Below:
                            return -_axisAB;
                        default:
                            G.U.Err("Invalid Cardinal/Altitude direction.");
                            return 0f;
                    }
                case DirectionType.Ordinal:
                    float mag1 = 0f;
                    float mag2 = 0f;
                    switch (dir)
                    {
                        case Direction.NorthEast:
                            mag1 = GetDirectionMagnitude(Direction.North);
                            mag2 = GetDirectionMagnitude(Direction.East);
                            break;
                        case Direction.SouthEast:
                            mag1 = GetDirectionMagnitude(Direction.South);
                            mag2 = GetDirectionMagnitude(Direction.East);
                            break;
                        case Direction.SouthWest:
                            mag1 = GetDirectionMagnitude(Direction.South);
                            mag2 = GetDirectionMagnitude(Direction.West);
                            break;
                        case Direction.NorthWest:
                            mag1 = GetDirectionMagnitude(Direction.North);
                            mag2 = GetDirectionMagnitude(Direction.West);
                            break;
                        default:
                            G.U.Err("Invalid Ordinal direction.");
                            return 0f;
                    }
                    if (!Mathf.Approximately(mag1, mag2))
                    {
                        G.U.Err("This position doesn't have an Ordinal direction.");
                        return 0f;
                    }
                    if (options == SpatialOptions.TrueOrdinal)
                    {
                        mag1 *= Mathf.Sqrt(2f);
                    }
                    //Else, if it's AdditiveCardinal, no change in magnitude is needed.
                    return mag1;
                case DirectionType.Relative:
                    return GetDirectionMagnitude(dir.GetAbsoluteDirection(), options);
                default:
                    G.U.Err("Currently unsupported.");
                    return 0f;
            }
        }

        void SetDirectionMagnitude(Direction dir, float mag)
        {
            SetDirectionMagnitude(dir, mag, SpatialOptions.TrueOrdinal);
        }

        void SetDirectionMagnitude(Direction dir, float mag, SpatialOptions options)
        {
            switch (dir.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Altitude:
                    switch (dir)
                    {
                        case Direction.North:
                            _axisNS = mag;
                            break;
                        case Direction.East:
                            _axisEW = mag;
                            break;
                        case Direction.South:
                            _axisNS = -mag;
                            break;
                        case Direction.West:
                            _axisEW = -mag;
                            break;
                        case Direction.Above:
                            _axisAB = mag;
                            break;
                        case Direction.Below:
                            _axisAB = -mag;
                            break;
                        default:
                            G.U.Err("Invalid Cardinal/Altitude direction.");
                            break;
                    }
                    break;
                case DirectionType.Ordinal:
                    if (options == SpatialOptions.TrueOrdinal)
                    {
                        mag *= Mathf.Sqrt(0.5f);
                    }
                    //Else, if it's AdditiveCardinal, no change in magnitude is needed.
                    switch (dir)
                    {
                        case Direction.NorthEast:
                            SetDirectionMagnitude(Direction.North, mag);
                            SetDirectionMagnitude(Direction.East, mag);
                            break;
                        case Direction.SouthEast:
                            SetDirectionMagnitude(Direction.South, mag);
                            SetDirectionMagnitude(Direction.East, mag);
                            break;
                        case Direction.SouthWest:
                            SetDirectionMagnitude(Direction.South, mag);
                            SetDirectionMagnitude(Direction.West, mag);
                            break;
                        case Direction.NorthWest:
                            SetDirectionMagnitude(Direction.North, mag);
                            SetDirectionMagnitude(Direction.West, mag);
                            break;
                        default:
                            G.U.Err("Invalid Ordinal direction.");
                            break;
                    }
                    break;
                case DirectionType.Relative:
                    SetDirectionMagnitude(dir.GetAbsoluteDirection(), mag, options);
                    break;
                default:
                    G.U.Err("Currently unsupported.");
                    break;
            }
        }
    }
}