using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRG {

    public static class KRGMenuEditorMusic {
        
        const string _editorMusicOff = "EDITOR_MUSIC_OFF";

        [MenuItem("KRG/Turn Editor Music Off", false, 500)]
        static void EditorMusicOff() {
            DefineSymbolHelper.AddDefineSymbol(_editorMusicOff);
        }

        [MenuItem("KRG/Turn Editor Music Off", true)]
        static bool ValidateEditorMusicOff() {
            return !DefineSymbolHelper.IsSymbolDefined(_editorMusicOff);
        }

        [MenuItem("KRG/Turn Editor Music On", false, 501)]
        static void EditorMusicOn() {
            DefineSymbolHelper.RemoveDefineSymbol(_editorMusicOff);
        }

        [MenuItem("KRG/Turn Editor Music On", true)]
        static bool ValidateEditorMusicOn() {
            return DefineSymbolHelper.IsSymbolDefined(_editorMusicOff);
        }
    }
}
