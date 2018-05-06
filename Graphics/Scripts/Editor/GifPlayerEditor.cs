using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if NS_UGIF
using uGIF;
#endif

namespace KRG {

#if NS_UGIF
    [CustomEditor(typeof(GifPlayer))]
    public class GifPlayerEditor : uGIF.GifPlayerEditor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            DrawProperty("_gifMaterialMode", "Gif Material Mode:");
            DrawProperty("_rasterAnimation", "Raster Animation:");
        }

        /// <summary>
        /// This is based on uGIF 1.2.
        /// </summary>
        protected void DrawProperty(string name, string label, string tooltip = "") {
            SerializedProperty prop = serializedObject.FindProperty(name);
            if (prop == null) {
                G.U.Error("Player Editor: Property \"" + name + "\" could not be found.");
                return;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), true);
            if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
