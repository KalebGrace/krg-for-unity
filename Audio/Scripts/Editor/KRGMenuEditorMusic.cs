using UnityEditor;

namespace KRG
{
    public static class KRGMenuEditorMusic
    {
        private const string EDITOR_MUSIC_OFF = "EDITOR_MUSIC_OFF";

        [MenuItem("KRG/Turn Editor Music Off", false, 500)]
        static void EditorMusicOff()
        {
            DefineSymbolHelper.AddDefineSymbol(EDITOR_MUSIC_OFF);
        }

        [MenuItem("KRG/Turn Editor Music Off", true)]
        static bool ValidateEditorMusicOff()
        {
            return !DefineSymbolHelper.IsSymbolDefined(EDITOR_MUSIC_OFF);
        }

        [MenuItem("KRG/Turn Editor Music On", false, 501)]
        static void EditorMusicOn()
        {
            DefineSymbolHelper.RemoveDefineSymbol(EDITOR_MUSIC_OFF);
        }

        [MenuItem("KRG/Turn Editor Music On", true)]
        static bool ValidateEditorMusicOn()
        {
            return DefineSymbolHelper.IsSymbolDefined(EDITOR_MUSIC_OFF);
        }
    }
}
