using UnityEditor;

namespace KRG
{
    public static class KRGMenuDebugVisibility
    {
        private const string DEBUG_VISIBILITY = "DEBUG_VISIBILITY";

        [MenuItem("KRG/Turn Debug Visibility Off", false, 400)]
        public static void DebugVisibilityOff()
        {
            DefineSymbolHelper.RemoveDefineSymbol(DEBUG_VISIBILITY);
        }

        [MenuItem("KRG/Turn Debug Visibility Off", true)]
        public static bool ValidateDebugVisibilityOff()
        {
            return DefineSymbolHelper.IsSymbolDefined(DEBUG_VISIBILITY);
        }

        [MenuItem("KRG/Turn Debug Visibility On", false, 401)]
        public static void DebugVisibilityOn()
        {
            DefineSymbolHelper.AddDefineSymbol(DEBUG_VISIBILITY);
        }

        [MenuItem("KRG/Turn Debug Visibility On", true)]
        public static bool ValidateDebugVisibilityOn()
        {
            return !DefineSymbolHelper.IsSymbolDefined(DEBUG_VISIBILITY);
        }
    }
}
