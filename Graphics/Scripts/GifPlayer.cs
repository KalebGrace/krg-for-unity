using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_UGIF
using uGIF;
#endif

namespace KRG {

#if NS_UGIF
    public abstract class GifPlayer : uGIF.GifPlayer {

        [Header("KRG-Specific Settings")]
        [SerializeField]
        protected GifCacheOperation _gifCacheOperation = default;
        [SerializeField]
        [FormerlySerializedAs("m_gifMaterialMode")]
        protected GifMaterialMode _gifMaterialMode = GifMaterialMode.RendererClone;

        [Header("Optional Standalone Raster Animation")]
        [SerializeField]
        [FormerlySerializedAs("m_rasterAnimation")]
        protected RasterAnimation _rasterAnimation;

        int _frameListIndex;
        bool _isLoaded;
        Gif _previewGif;
        RasterAnimationInfo _rasterAnimationInfo;
        RasterAnimationState _rasterAnimationState;

        public bool isLoaded { get { return _isLoaded; } }

        public RasterAnimationInfo rasterAnimationInfo {
            set {
                _rasterAnimationInfo = value;
                if (_rasterAnimationState != null) _rasterAnimationState.rasterAnimationInfo = value;
            }
        }

        public ITimeThread timeThread { get; set; }

#region MonoBehaviour methods, including uGIF overrides

        protected virtual void Reset() {
            mode = GifPlayerMode.Material;
            //TODO: re-assess this
            //behaviour = GifPlayerBehaviour.LoadProgressively;
        }

        protected virtual void OnValidate() {
            //can be overriden to force specific selections
        }

        protected override void Awake() {
            SetPlayMat();
            base.Awake();
            //if a raster animation was set in the inspector, we need to run it through the set method to initialize it
            //TODO: may need a mechanism to pass through frame sequence handlers
            if (_rasterAnimation != null) SetRasterAnimation(_rasterAnimation);
        }

        /// <summary>
        /// Update using RasterAnimation as applicable.
        /// This is based on uGIF 1.2.
        /// </summary>
        protected override void Update() {
            if (gif != null) {
                if (isLoading) {
                    if (!importer.LoadNextFrame()) {
                        isLoading = false;
                        importer.EndLoad();
                    }
                }
                if (gif.Frames > 0 && isPlaying) {
                    if (unscaledDeltaTime) {
                        time += Time.unscaledDeltaTime;
                    } else if (timeThread == null) {
                        time += Time.deltaTime;
                    } else {
                        time += timeThread.deltaTime;
                    }
                    if (time >= delay) {
                        time = time - delay;
                        //begin custom code
                        var ra = _rasterAnimation;
                        if (ra != null && ra.hasPlayableFrameSequences) {
                            //begin Frame Sequence code
                            //NOTE: GifPlayer uses zero-based frame index (i); RasterAnimation uses one-based frame num.
                            if (unloadAfterPlay) {
                                G.U.Err("Unload After Play is not supported when using Frame Sequences.");
                            }
                            int frameNumber;
                            if (_rasterAnimationState.frameSequenceHasFrameList) {
                                if (!_rasterAnimationState.AdvanceFrame(ref _frameListIndex, out frameNumber)) {
                                    this.Stop();
                                    return;
                                }
                            } else {
                                frameNumber = i + 1;
                                if (!_rasterAnimationState.AdvanceFrame(ref frameNumber)) {
                                    this.Stop();
                                    return;
                                }
                            }
                            //it's possible to advance to the same frame, so update only if it's different
                            if (i != frameNumber - 1) {
                                i = frameNumber - 1;
                                UpdateImage();
                            }
                            //end Frame Sequence code
                        } else {
                            //begin relocated original code
                            if (i + 1 == gif.Frames) {
                                if (!loop || unloadAfterPlay) {
                                    this.Stop();
                                    return;
                                }
                            }
                            if (unloadAfterPlay) {
                                Destroy(gif.frames[i]);
                            }
                            i = (i + 1) % gif.Frames;
                            UpdateImage();
                            //end relocated original code
                        }
                        //end custom code
                    }
                }
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            switch (mode) {
                case GifPlayerMode.Material:
                    switch (_gifMaterialMode) {
                        case GifMaterialMode.SharedMaterial:
                            //the uGIF.GifPlayer default mode; do nothing differently
                            break;
                        case GifMaterialMode.RendererClone:
                        case GifMaterialMode.GifPlayerClone:
                            //destroy the material clone
                            if (playMat != null) DestroyImmediate(playMat);
                            break;
                        default:
                            G.U.Unsupported(this, _gifMaterialMode);
                            break;
                    }
                    break;
            }
        }

#endregion

#region KRG Public Methods

        /// <summary>
        /// Sets the play material. Needs to be called manually if creating a GifPlayer through code.
        /// </summary>
        public void SetPlayMat() {
            switch (mode) {
                case GifPlayerMode.Material:
                    //GifMaterialMode.RendererClone or GifMaterialMode.GifPlayerClone is
                    //absolutely necessary when you have multiple instances of this
                    //material in the scene that each need to animate independently
                    switch (_gifMaterialMode) {
                        case GifMaterialMode.SharedMaterial:
                            //the uGIF.GifPlayer default mode; do nothing differently
                            break;
                        case GifMaterialMode.RendererClone:
                            LogRendererCloneWarning();
                            //create a unique, temporary material clone for this GameObject's renderer
                            playMat = GetComponent<Renderer>().material;
                            break;
                        case GifMaterialMode.GifPlayerClone:
                            //create a unique, temporary material clone for this GIF player
                            playMat = new Material(playMat);
                            break;
                        default:
                            G.U.Unsupported(this, _gifMaterialMode);
                            break;
                    }
                    break;
                case GifPlayerMode.UI:
                    //nada
                    break;
                default:
                    G.U.Unsupported(this, mode);
                    break;
            }
        }

        /// <summary>
        /// Sets the raster animation and initializes it.
        /// </summary>
        /// <param name="ra">The raster animation.</param>
        /// <param name="frameSequenceStartHandler">An optional frame sequence start handler.</param>
        /// <param name="frameSequenceStopHandler">An optional frame sequence stop handler.</param>
        public void SetRasterAnimation(
            RasterAnimation ra,
            RasterAnimationState.Handler frameSequenceStartHandler = null,
            RasterAnimationState.Handler frameSequenceStopHandler = null
        ) {
            _rasterAnimation = ra;
            if (ra != null) {
                file = ra.gifBytes;
                loop = false; //looping is handled differently for raster animations; it does not use this field
                _rasterAnimationState = new RasterAnimationState(
                    ra,
                    frameSequenceStartHandler,
                    frameSequenceStopHandler
                );
                _rasterAnimationState.rasterAnimationInfo = _rasterAnimationInfo;
                _frameListIndex = 0;
                i = _rasterAnimationState.frameSequenceFromFrame - 1; //1-based -> 0-based
            } else {
                file = null;
                loop = true;
                _rasterAnimationState = null;
                _frameListIndex = 0;
                i = 0;
            }
            time = 0;
        }

        /// <summary>
        /// Unloads the preview (first frame). Called by EditorSave.GifPlayerSave().
        /// </summary>
        public void UnloadPreview() {
            _previewGif = null;
        }

#endregion


#region KRG Private Methods

        void LogRendererCloneWarning() {
            if (playMat != null && !enablePreview) {
                G.U.Warning(
                    "Setting a Material is unnecessary for Renderer Clone mode (unless Enable Preview is checked).",
                    file.name
                );
            }
        }

#endregion

#region uGIF Methods - New

        /// <summary>
        /// Loads the preview (first frame).
        /// This is a new version of LoadPreview that can be called
        /// repeatedly without reloading the GIF every single time
        /// (potentially causing a memory leak).
        /// This is based on uGIF 1.2.
        /// </summary>
        public new void LoadPreview() {
            if (_previewGif != null
                || firstFrame != null
                || file == null
                || mode == GifPlayerMode.LegacyGUI
                || !enablePreview
                || (mode == GifPlayerMode.Material && playMat == null)
                || (mode == GifPlayerMode.UI && playImage == null)) {
                return;
            }

            RuntimeGifLoader i = new RuntimeGifLoader();
            _previewGif = i.BeginLoad(file.name, file.bytes);
            
            for (int counter = 0; counter < previewFrame; ++counter)
            {
                if (!i.SkipNextFrame())
                {
                    i.EndLoad();

                    Clear();

                    return;
                }
            }

            if (i.LoadNextFrame())
            {
                i.EndLoad();

                previewFrame = Mathf.Min(previewFrame, _previewGif.Frames - 1);

                SetPreviewTextureKRG(_previewGif.frames[previewFrame]);
            }
            else
            {
                i.EndLoad();

                Clear();
            }
        }

        private void Clear()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixels(new Color[] { Color.clear });
            tex.Apply();
            SetPreviewTextureKRG(tex);
        }

        void SetPreviewTextureKRG(Texture2D tex)
        {
            if (mode == GifPlayerMode.UI)
            {
                playImage.texture = tex;
            }
            else if (mode == GifPlayerMode.Material)
            {
                playMat.SetTexture(materialProperty, tex);
            }
        }

#endregion

#region uGIF Methods - Override

        /// <summary>
        /// Load the gif, and track that it has been loaded.
        /// </summary>
        public override void Load()
        {
            if (file)
            {
                var ft = _rasterAnimation?.FrameTextures;

                if (ft != null && ft.Count > 0)
                {
                    gif = G.gfx.GetGifFromFrameTextures(ft, file.name);
                }
                else
                {
                    gif = G.gfx.GetGifFromTextAsset(file, _gifCacheOperation);
                }

                if (gif != null)
                {
                    if (useCustomdelay)
                    {
                        delay = customDelay;
                    }
                    else
                    {
                        delay = gif.delay;
                    }

                    if (mode == GifPlayerMode.Material)
                    {
                        propertyID = Shader.PropertyToID(materialProperty);
                    }
                }
            }

            _isLoaded = true;
        }

        /// <summary>
        /// Start playback with a delay, and reset the raster animation frame sequence index.
        /// This is based on uGIF 1.2.
        /// </summary>
        /// <param name="del">Delay in seconds.</param>
        public override void Play(float del) {
            if (isPaused) {
                G.U.Warning("Using Play while isPaused resets the animation. To unpause, use Pause(false) instead.");
            }
            //RasterAnimation stuff
            var ra = _rasterAnimation;
            if (ra != null) _rasterAnimationState.Reset();
            _frameListIndex = 0;
            i = ra != null ? _rasterAnimationState.frameSequenceFromFrame - 1 : 0;
            //uGIF stuff
            isPlaying = true;
            isPaused = false;
            time = 0 - del;
            if (onPlayBegin != null) {
                onPlayBegin.Invoke();
            }
            UpdateImage();
        }

        /// <summary>
        /// Stop playback of the animation, and reset the raster animation frame sequence index.
        /// This is based on uGIF 1.2.
        /// </summary>
        public override void Stop() {
            isPlaying = false;
            isPaused = false; //TODO: see if adding this messes anything up
            //the following commented-out code messes up the RasterAnimationInfo for the
            //last frame of a non-looping animation, and probably should not be done anyway
            /*
            var ra = _rasterAnimation;
            if (ra != null) _rasterAnimationState.Reset();
            i = ra != null ? _rasterAnimationState.frameSequenceFromFrame - 1 : 0;
            time = 0;
            */
            if (onPlayEnd != null) {
                onPlayEnd.Invoke();
            }
        }

#endregion

    }
#endif
}
