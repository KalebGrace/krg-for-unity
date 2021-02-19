namespace _0G.Legacy
{
    public struct RasterAnimationOptions
    {
        public RasterAnimationHandler FrameSequenceStartHandler;
        public RasterAnimationHandler FrameSequenceStopHandler;
        public RasterAnimationHandler FrameSequencePlayLoopStartHandler;
        public RasterAnimationHandler FrameSequencePlayLoopStopHandler;
        public int InfiniteLoopReplacement; // 0 will retain the infinite loop(s)
    }
}