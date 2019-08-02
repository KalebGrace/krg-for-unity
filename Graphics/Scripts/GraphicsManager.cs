using System.Collections.Generic;
using UnityEngine;

#if NS_UGIF
using uGIF;
#endif

namespace KRG
{
    public class GraphicsManager : Manager, IOnDestroy
    {
        public override float priority => 70;

        public override void Awake() { }

        public virtual void OnDestroy()
        {
#if NS_UGIF
            ClearGifCache();
#endif
        }

#if NS_UGIF
        protected Dictionary<string, Gif> m_GifCache = new Dictionary<string, Gif>();

        public virtual void ClearGifCache()
        {
            foreach (Gif gif in m_GifCache.Values)
            {
                if (gif != null)
                {
                    for (int i = 0; i < gif.Frames; ++i)
                    {
                        Object.Destroy(gif.frames[i]);
                    }
                }
            }

            m_GifCache.Clear();
        }

        public Gif GetGifFromTextAsset(TextAsset file, GifCacheOperation gifCacheOperation)
        {
            Gif gif;

            string key = file.name;

            if (gifCacheOperation != GifCacheOperation.None && m_GifCache.ContainsKey(key))
            {
                gif = m_GifCache[key];

                if (gif != null)
                {
                    return gif;
                }
            }

            gif = RuntimeGifLoader.LoadFromTextAsset(file);

            if (gif != null && gifCacheOperation == GifCacheOperation.ReadWrite)
            {
                m_GifCache.Add(key, gif);
            }

            return gif;
        }
#endif

    }
}
