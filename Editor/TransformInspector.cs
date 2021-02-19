using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// Reverse engineered UnityEditor.TransformInspector.
    /// http://wiki.unity3d.com/index.php?title=TransformInspector (22:25, 26 September 2015‎)
    /// All "private"s have been made "protected", and all member methods have been made "virtual".
    /// LocalizationDatabase doesn't seem to work anymore, so this has been replaced.
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Transform))]
    public class TransformInspector : Editor
    {
        protected const float FIELD_WIDTH = 212.0f;
        protected const bool WIDE_MODE = true;

        protected const float POSITION_MAX = 100000.0f;

        protected static GUIContent positionGUIContent = new GUIContent(LocalString("Position"), LocalString("The local position of this Game Object relative to the parent."));
        protected static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation"), LocalString("The local rotation of this Game Object relative to the parent."));
        protected static GUIContent scaleGUIContent = new GUIContent(LocalString("Scale"), LocalString("The local scaling of this Game Object relative to the parent."));

        protected static string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

        protected SerializedProperty positionProperty;
        protected SerializedProperty rotationProperty;
        protected SerializedProperty scaleProperty;

        protected static string LocalString(string text)
        {
            //return LocalizationDatabase.GetLocalizedString(text);
            return text;
        }

        public virtual void OnEnable()
        {
            this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
            this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
            this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
        }

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.wideMode = TransformInspector.WIDE_MODE;
            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - TransformInspector.FIELD_WIDTH; // align field to right of inspector

            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
            this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
            EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);

            if (!ValidatePosition(((Transform) this.target).position))
            {
                EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual bool ValidatePosition(Vector3 position)
        {
            if (Mathf.Abs(position.x) > TransformInspector.POSITION_MAX) return false;
            if (Mathf.Abs(position.y) > TransformInspector.POSITION_MAX) return false;
            if (Mathf.Abs(position.z) > TransformInspector.POSITION_MAX) return false;
            return true;
        }

        protected virtual void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
        {
            Transform transform = (Transform) this.targets[0];
            Quaternion localRotation = transform.localRotation;
            foreach (UnityEngine.Object t in (UnityEngine.Object[]) this.targets)
            {
                if (!SameRotation(localRotation, ((Transform) t).localRotation))
                {
                    EditorGUI.showMixedValue = true;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();

            Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(this.targets, "Rotation Changed");
                foreach (UnityEngine.Object obj in this.targets)
                {
                    Transform t = (Transform) obj;
                    t.localEulerAngles = eulerAngles;
                }
                rotationProperty.serializedObject.SetIsDifferentCacheDirty();
            }

            EditorGUI.showMixedValue = false;
        }

        protected virtual bool SameRotation(Quaternion rot1, Quaternion rot2)
        {
            if (rot1.x != rot2.x) return false;
            if (rot1.y != rot2.y) return false;
            if (rot1.z != rot2.z) return false;
            if (rot1.w != rot2.w) return false;
            return true;
        }
    }
}