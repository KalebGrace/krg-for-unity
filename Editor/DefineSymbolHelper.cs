using UnityEditor;

namespace _0G.Legacy
{
    public static class DefineSymbolHelper
    {
        private const char DEFINE_SYMBOL_DELIMITER = ';';

        public static void AddDefineSymbol(string symbolToAdd)
        {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            if (!string.IsNullOrEmpty(ds)) ds += DEFINE_SYMBOL_DELIMITER;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(tg, ds + symbolToAdd);
        }

        public static bool IsSymbolDefined(string symbolToCheck)
        {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            string[] dsArray = ds.Split(DEFINE_SYMBOL_DELIMITER);
            for (int i = 0; i < dsArray.Length; i++)
            {
                if (dsArray[i] == symbolToCheck)
                {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveDefineSymbol(string symbolToRemove)
        {
            BuildTargetGroup tg = EditorUserBuildSettings.selectedBuildTargetGroup;
            string ds = PlayerSettings.GetScriptingDefineSymbolsForGroup(tg);
            string[] dsArray = ds.Split(DEFINE_SYMBOL_DELIMITER);
            string curSymbol;
            ds = ""; //clear before proceeding
            for (int i = 0; i < dsArray.Length; i++)
            {
                curSymbol = dsArray[i];
                if (curSymbol != symbolToRemove)
                {
                    if (!string.IsNullOrEmpty(ds)) ds += DEFINE_SYMBOL_DELIMITER;
                    ds += curSymbol;
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(tg, ds);
        }
    }
}