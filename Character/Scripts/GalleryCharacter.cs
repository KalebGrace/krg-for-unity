using UnityEngine;

namespace KRG
{
    [ExecuteAlways]
    public class GalleryCharacter : MonoBehaviour
    {
        public Texture2D CharacterTexture;

        private void Start()
        {
            UpdateTexture();
        }

        private void OnValidate()
        {
            UpdateTexture();
        }

        private void UpdateTexture()
        {
            var r = GetComponent<MeshRenderer>();
            if (G.U.IsEditMode(this))
            {
                var m = r.sharedMaterial;
                m.mainTexture = CharacterTexture;
                r.sharedMaterial = m;
            }
            else
            {
                var m = r.material;
                m.mainTexture = CharacterTexture;
                r.material = m;
            }
        }
    }
}
