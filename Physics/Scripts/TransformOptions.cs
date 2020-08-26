using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{

    [System.Flags]
    public enum TransformOptions
    {

        None = 0,
        ResetInitialPosition = 1 << 0,
        ResetInitialRotation = 1 << 1,
        ResetInitialLocalScale = 1 << 2,
        ParentToSource = 1 << 3,
        OffsetPositionToSource = 1 << 4,
        OffsetRotationToSource = 1 << 5,
        MultiplyLocalScaleBySource = 1 << 6,
    }
}