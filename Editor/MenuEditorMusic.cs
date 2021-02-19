using UnityEditor;

namespace _0G.Legacy
{
    public static class MenuEditorMusic
    {
        [MenuItem("0G/Legacy/Turn Editor Music Off", false, 500)]
        public static void EditorMusicOff()
        {
            EditorPrefs.SetBool(G.U.EDITOR_MUSIC_KEY, false);
        }

        [MenuItem("0G/Legacy/Turn Editor Music Off", true)]
        public static bool ValidateEditorMusicOff()
        {
            return EditorPrefs.GetBool(G.U.EDITOR_MUSIC_KEY, false);
        }

        [MenuItem("0G/Legacy/Turn Editor Music On", false, 501)]
        public static void EditorMusicOn()
        {
            EditorPrefs.SetBool(G.U.EDITOR_MUSIC_KEY, true);
        }

        [MenuItem("0G/Legacy/Turn Editor Music On", true)]
        public static bool ValidateEditorMusicOn()
        {
            return !EditorPrefs.GetBool(G.U.EDITOR_MUSIC_KEY, false);
        }
    }
}