using UnityEngine;

namespace _0G.Legacy
{
    public static class AxisExtensionMethods
    {
        public static bool HasFlag(this Axis axs, Axis flag)
        {
            return (axs & flag) == flag;
        }

        public static Vector3 GetVector3(this Axis axs, float magnitude, Vector3 baseV3 = new Vector3())
        {
            if (magnitude < 0)
            {
                throw new System.Exception(string.Format("Magnitude is {0} but must be zero or positive.", magnitude));
            }
            //x
            if (axs.HasFlag(Axis.Xneg))
            {
                baseV3.x = -magnitude;
            }
            if (axs.HasFlag(Axis.Xpos))
            {
                baseV3.x = magnitude;
                if (axs.HasFlag(Axis.Xneg))
                {
                    throw new System.Exception("Both Xneg & Xpos are set, but only one (or neither) must be set.");
                }
            }
            //y
            if (axs.HasFlag(Axis.Yneg))
            {
                baseV3.y = -magnitude;
            }
            if (axs.HasFlag(Axis.Ypos))
            {
                baseV3.y = magnitude;
                if (axs.HasFlag(Axis.Yneg))
                {
                    throw new System.Exception("Both Yneg & Ypos are set, but only one (or neither) must be set.");
                }
            }
            //z
            if (axs.HasFlag(Axis.Zneg))
            {
                baseV3.z = -magnitude;
            }
            if (axs.HasFlag(Axis.Zpos))
            {
                baseV3.z = magnitude;
                if (axs.HasFlag(Axis.Zneg))
                {
                    throw new System.Exception("Both Zneg & Zpos are set, but only one (or neither) must be set.");
                }
            }
            //v3
            return baseV3;
        }
    }
}