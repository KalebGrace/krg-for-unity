using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public enum Direction {
        
        North     =   0,
        East      =  90,
        South     = 180,
        West      = 270,
        NorthEast =  45,
        SouthEast = 135,
        SouthWest = 225,
        NorthWest = 315,
        Up        =  -1,
        UpRight   =  -2,
        Right     =  -3,
        DownRight =  -4,
        Down      =  -5,
        DownLeft  =  -6,
        Left      =  -7,
        UpLeft    =  -8,
        Above     =  -9,
        Below     = -10,
        Unknown   = -99,
    }
}
