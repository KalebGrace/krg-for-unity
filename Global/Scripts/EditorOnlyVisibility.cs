using UnityEngine;

namespace KRG
{
    [ExecuteInEditMode]
    public class EditorOnlyVisibility : MonoBehaviour
    {
        private BoxCollider m_BoxCollider;
        private Renderer m_Renderer;

        private void OnValidate()
        {
            m_Renderer = GetComponent<Renderer>();

            if (m_Renderer != null && m_Renderer.sharedMaterial.name == "Default-Material")
            {
                m_Renderer.sharedMaterial = Resources.Load<Material>("Global/EditorOnlyVisibilityMaterial");
            }
        }

        private void Awake()
        {
            m_BoxCollider = GetComponent<BoxCollider>();
            m_Renderer = GetComponent<Renderer>();

            if (m_Renderer != null)
            {
                m_Renderer.enabled = G.isInEditMode;
            }
        }

        private void OnDrawGizmos()
        {
            if (m_BoxCollider != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.position + m_BoxCollider.center, m_BoxCollider.size);
            }
        }
    }
}
