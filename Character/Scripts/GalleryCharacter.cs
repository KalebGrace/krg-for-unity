using UnityEngine;

namespace KRG
{
    [ExecuteAlways]
    public class GalleryCharacter : MonoBehaviour
    {
        private Material m_Material;
        private MeshRenderer m_Renderer;

        public Material Material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = m_Renderer.material;
                }
                return m_Material;
            }
        }

        private void Awake()
        {
            m_Renderer = GetComponent<MeshRenderer>();
        }

        public void ScaleTexture(float v)
        {
            transform.localScale = new Vector3(v, v, 1);
        }

        public void SetKnown(bool isKnown)
        {
            Material.color = Material.color.SetAlpha(isKnown ? 1 : 0.2f);
        }

        public void UpdateTexture(Texture2D characterTexture)
        {
            Material.mainTexture = characterTexture;
            ScaleTexture(1);
        }
    }
}