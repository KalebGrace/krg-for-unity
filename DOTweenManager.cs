#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public class DOTweenManager : Manager
    {
        public override float priority { get { return 30; } }

        public override void Awake()
        {
#if NS_DG_TWEENING
            if (config.doTweenUseInitSettings)
            {
                DOTween.Init(config.doTweenRecycleAllByDefault, config.doTweenUseSafeMode, config.doTweenLogBehaviour);
            }
            else
            {
                DOTween.Init();
            }
#endif
        }
    }
}