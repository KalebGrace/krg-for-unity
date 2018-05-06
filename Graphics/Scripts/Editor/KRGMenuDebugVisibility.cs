using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRG {

    public static class KRGMenuDebugVisibility {

        const string _debugVisibility = "DEBUG_VISIBILITY";

        [MenuItem("KRG/Turn Debug Visibility Off", false, 400)]
        static void DebugVisibilityOff() {
            DefineSymbolHelper.RemoveDefineSymbol(_debugVisibility);
        }

        [MenuItem("KRG/Turn Debug Visibility Off", true)]
        static bool ValidateDebugVisibilityOff() {
            return DefineSymbolHelper.IsSymbolDefined(_debugVisibility);
        }

        [MenuItem("KRG/Turn Debug Visibility On", false, 401)]
        static void DebugVisibilityOn() {
            DefineSymbolHelper.AddDefineSymbol(_debugVisibility);
        }

        [MenuItem("KRG/Turn Debug Visibility On", true)]
        static bool ValidateDebugVisibilityOn() {
            return !DefineSymbolHelper.IsSymbolDefined(_debugVisibility);
        }
    }
}
