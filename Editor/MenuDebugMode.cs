using UnityEditor;

namespace _0G.Legacy
{
    public static class MenuDebugMode
    {
        [MenuItem("0G/Legacy/Turn Debug Mode Off", false, 400)]
        public static void DebugModeOff()
        {
            EditorPrefs.SetBool(G.U.DEBUG_MODE_KEY, false);
        }

        [MenuItem("0G/Legacy/Turn Debug Mode Off", true)]
        public static bool ValidateDebugModeOff()
        {
            return EditorPrefs.GetBool(G.U.DEBUG_MODE_KEY, false);
        }

        [MenuItem("0G/Legacy/Turn Debug Mode On", false, 401)]
        public static void DebugModeOn()
        {
            EditorPrefs.SetBool(G.U.DEBUG_MODE_KEY, true);
        }

        [MenuItem("0G/Legacy/Turn Debug Mode On", true)]
        public static bool ValidateDebugModeOn()
        {
            return !EditorPrefs.GetBool(G.U.DEBUG_MODE_KEY, false);
        }
    }
}