using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    public static class EditorExtensionMethods
    {
        public static void DrawProperty(
            this Editor editor,
            string name,
            string label,
            string tooltip = "",
            bool doMultiEdit = true
        )
        {
            SerializedProperty prop = editor.serializedObject.FindProperty(name);
            if (prop == null)
            {
                G.U.Err("Cannot find {0} property on {1} editor.", name, editor.name);
                return;
            }
            if (doMultiEdit) EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), true);
            if (doMultiEdit && EditorGUI.EndChangeCheck())
            {
                editor.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}