using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRG {

    public static class DefineSymbolHelper {

        const char _defineSymbolDelimiter = ';';

        public static void AddDefineSymbol(string symbolToAdd) {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            if (!string.IsNullOrEmpty(ds)) ds += _defineSymbolDelimiter;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(tg, ds + symbolToAdd);
        }

        public static bool IsSymbolDefined(string symbolToCheck) {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            string[] dsArray = ds.Split(_defineSymbolDelimiter);
            for (int i = 0; i < dsArray.Length; i++) {
                if (dsArray[i] == symbolToCheck) {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveDefineSymbol(string symbolToRemove) {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            string[] dsArray = ds.Split(_defineSymbolDelimiter);
            string curSymbol;
            ds = ""; //clear before proceeding
            for (int i = 0; i < dsArray.Length; i++) {
                curSymbol = dsArray[i];
                if (curSymbol != symbolToRemove) {
                    if (!string.IsNullOrEmpty(ds)) ds += _defineSymbolDelimiter;
                    ds += curSymbol;
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(tg, ds);
        }
    }
}
