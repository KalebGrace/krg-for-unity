using UnityEngine;

namespace _0G.Legacy
{
    public static class KRGGizmos
    {
        public static void DrawArrowSide(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);
            Vector3 mid = (from + to) / 2f;
            float y = Vector3.Distance(to, mid);
            Vector3 vy = new Vector3(0, y);
            Gizmos.DrawLine(to, mid + vy);
            Gizmos.DrawLine(to, mid - vy);
        }

        public static void DrawCrosshairXY(Vector3 center, float diameter)
        {
            float r = diameter / 2f;
            Gizmos.DrawLine(center.Add(x: -r), center.Add(x: r));
            Gizmos.DrawLine(center.Add(y: -r), center.Add(y: r));
        }
    }
}