using UnityEngine.UI;

namespace KRG
{
    public static class ImageExtensionMethods
    {
        public static void SetAlpha(this Image i, float a)
        {
            i.color = i.color.SetAlpha(a);
        }
    }
}