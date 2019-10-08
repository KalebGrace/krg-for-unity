using UnityEngine;

namespace KRG
{
    public class StandaloneAnimation : GraphicController
    {
        [SerializeField]
        private RasterAnimation m_RasterAnimation;

        protected override void Start()
        {
            base.Start();

            if (m_RasterAnimation != null)
            {
                SetAnimation(m_RasterAnimation);
            }
        }
    }
}
