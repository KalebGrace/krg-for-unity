namespace KRG
{
    public struct RasterAnimationOptions
    {
        public RasterAnimationHandler FrameSequenceStartHandler;
        public RasterAnimationHandler FrameSequenceStopHandler;
        public RasterAnimationHandler FrameSequencePlayLoopStartHandler;
        public RasterAnimationHandler FrameSequencePlayLoopStopHandler;
        public bool IgnoreInfiniteLoops;
    }
}