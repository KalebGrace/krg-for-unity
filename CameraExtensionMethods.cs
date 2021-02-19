using UnityEngine;

namespace _0G.Legacy
{
    public static class CameraExtensionMethods
    {
        /// <summary>
        /// Get the orthographic bounds of the camera.
        /// </summary>
        /// <returns>The orthographic bounds.</returns>
        /// <param name="camera">Camera.</param>
        public static Bounds GetOrthographicBounds(this Camera camera)
        {
            if (!camera.orthographic)
            {
                G.U.Err("The {0} Camera does not use orthographic projection.", camera.name);
                return new Bounds();
            }
            float screenAspect = (float) Screen.width / (float) Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            return new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0)
            );
        }

        /// <summary>
        /// Get the perspective bounds of the camera.
        /// </summary>
        /// <returns>The perspective bounds.</returns>
        /// <param name="camera">Camera.</param>
        /// <param name="z">The distance to the specified plane from the camera along the z-axis.</param>
        public static Bounds GetPerspectiveBounds(this Camera camera, float z)
        {
            if (camera.orthographic)
            {
                G.U.Err("The {0} Camera does not use perspective projection.", camera.name);
                return new Bounds();
            }
            Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, z));
            Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, z));
            Bounds bounds = new Bounds();
            bounds.SetMinMax(bottomLeft, topRight);
            return bounds;
        }
    }
}