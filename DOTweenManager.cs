#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
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