using UnityEngine;
using UnityEngine.UI;

namespace _0G.Legacy
{
    [ExecuteAlways]
    public class EditorOnlyVisibility : MonoBehaviour
    {
        public Color WireColor = Color.magenta;

        private BoxCollider m_BoxCollider;
        private Image m_Image;
        private Renderer m_Renderer;

        private void OnValidate()
        {
            m_Renderer = GetComponent<Renderer>();

            if (m_Renderer != null && (m_Renderer.sharedMaterial == null || m_Renderer.sharedMaterial.name == "Default-Material"))
            {
                m_Renderer.sharedMaterial = Resources.Load<Material>("Global/EditorOnlyVisibilityMaterial");
            }
        }

        private void Awake()
        {
            m_BoxCollider = GetComponent<BoxCollider>();
            m_Image = GetComponent<Image>();
            m_Renderer = GetComponent<Renderer>();

            if (m_Image != null)
            {
                m_Image.enabled = G.U.IsEditMode(this);
            }

            if (m_Renderer != null)
            {
                m_Renderer.enabled = G.U.IsEditMode(this);
            }
        }

        private void OnDrawGizmos()
        {
            if (m_BoxCollider != null)
            {
                Gizmos.color = WireColor;
                Vector3 center = transform.position + m_BoxCollider.center;
                Vector3 size = transform.lossyScale.Multiply(m_BoxCollider.size);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}