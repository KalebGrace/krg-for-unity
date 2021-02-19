using UnityEditor;

namespace _0G.Legacy
{
    [CustomEditor(typeof(Flipper))]
    public class FlipperEditor : Editor
    {
        private SerializedObject m_Flipper;
        private SerializedProperty m_FacingDirection;
        private SerializedProperty m_FlipLocalPosition;
        private SerializedProperty m_FlipLocalRotation;
        private SerializedProperty m_FlipLocalScale;
        private SerializedProperty m_FlipCollider;
        private SerializedProperty m_Body;

        private void OnEnable()
        {
            m_Flipper = new SerializedObject(target);
            m_FacingDirection = m_Flipper.FindProperty("m_FacingDirection");
            m_FlipLocalPosition = m_Flipper.FindProperty("m_FlipLocalPosition");
            m_FlipLocalRotation = m_Flipper.FindProperty("m_FlipLocalRotation");
            m_FlipLocalScale = m_Flipper.FindProperty("m_FlipLocalScale");
            m_FlipCollider = m_Flipper.FindProperty("m_FlipCollider");
            m_Body = m_Flipper.FindProperty("m_Body");
        }

        public override void OnInspectorGUI()
        {
            m_Flipper.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(m_FacingDirection);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_FlipLocalPosition);
            EditorGUILayout.PropertyField(m_FlipLocalRotation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_FlipLocalScale);
            EditorGUILayout.PropertyField(m_FlipCollider);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(m_Body);

            m_Flipper.ApplyModifiedProperties();
        }
    }
}