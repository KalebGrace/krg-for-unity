using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class SpriteLogic : MonoBehaviour
    {
		public const float DEFAULT_SPRITE_FPS = 20;

		public GameObject SpriteGameObject;

        public string RasterAnimationName;

        private AssetBundle m_AssetBundle;

		private int m_FrameIndex;

		private int m_ImageIndex;

        private Material m_Material;

        private Mesh m_Mesh;

        private RasterAnimation m_RasterAnimation;

        private List<Texture2D> m_TextureList;

        private float m_TimeElapsed;

        private float FrameRate => DEFAULT_SPRITE_FPS;

        private int ImageCount => m_TextureList != null ? m_TextureList.Count : 0;

        private int ImageGridCols => 2;

        private int ImageGridRows => 2;

        private void Awake()
        {
            //TODO: Move this to GraphicsManager
            Resources.UnloadUnusedAssets();

            //only for texture cycle type...
            Renderer r = SpriteGameObject.GetComponent<Renderer>();
            m_Material = r.material; //instance material

            //only for uv grid type...
            MeshFilter f = SpriteGameObject.GetComponent<MeshFilter>();
            m_Mesh = f.mesh; //instance mesh

            if (!string.IsNullOrEmpty(RasterAnimationName))
            {
                string bundleName = RasterAnimationName.Replace("_RasterAnimation", "").ToLower();
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);
                m_AssetBundle = AssetBundle.LoadFromFile(path);
                if (m_AssetBundle == null)
                {
                    G.U.Err("Failed to load AssetBundle!");
                    return;
                }
                m_RasterAnimation = m_AssetBundle.LoadAsset<RasterAnimation>(RasterAnimationName);
                m_TextureList = m_RasterAnimation.FrameTextures;
            }

            Show(m_ImageIndex);
        }

        private void Update()
		{
			float ts = m_TimeElapsed;
			float td = Time.deltaTime; //TODO: get "proper" deltaTime
			float te = ts + td;

			int fs = m_FrameIndex;
            int fe = Mathf.FloorToInt(FrameRate * te);

            while (fs < fe)
			{
                m_FrameIndex = ++fs;
                FrameUpdate();
            }

            m_TimeElapsed = te;
        }

        private void FrameUpdate()
        {
            if (ImageCount == 0) return;

            //loop through images
            m_ImageIndex = (m_ImageIndex + 1) % ImageCount;

            Show(m_ImageIndex);
		}

        private void Show(int imageIndex)
        {
            if (ImageCount == 0) return;

            //if texture cycle type...
            m_Material.mainTexture = m_TextureList[imageIndex];

            //if uv grid type...
            /*
            int x = imageIndex % ImageGridCols;
            int y = imageIndex / ImageGridCols;

            SetUVGrid(x, y);
            */
        }

        private void SetUVGrid(int x, int y)
        {
            Vector2[] uv = m_Mesh.uv;
            uv[0] = new Vector2(x, y);
            uv[1] = new Vector2(x + 1, y);
            uv[2] = new Vector2(x, y + 1);
            uv[3] = new Vector2(x + 1, y + 1);
            m_Mesh.uv = uv;
        }

        private void OnDestroy()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
            }
        }
    }
}
