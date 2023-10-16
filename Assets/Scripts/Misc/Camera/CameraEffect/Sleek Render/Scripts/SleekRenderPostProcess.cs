// Add Blur(SuperBlur.cs)
// Add Color Grading(Amplify Color - Advanced Color Grading for Unity. Copyright (c) Amplify Creations, Lda <info@amplify.pt>)

// Bloom, Blur, DOF 는 동시에 활성화 될 수 없도록 에디터 스크립트와 여기서,SO에서 예외 처리 필요
// SleekRenderSettings의 각 옵션들이 활성화 되지 않았을 때에 대해 최적화

using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleekRender
{
    // Custom component editor view definition
    [AddComponentMenu("Effects/Sleek Render Post Process")]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class SleekRenderPostProcess : MonoBehaviour
    {
        public static class Uniforms
        {
            public static readonly int _LuminanceConst = Shader.PropertyToID("_LuminanceConst");
            public static readonly int _BloomIntencity = Shader.PropertyToID("_BloomIntencity");
            public static readonly int _BloomTint = Shader.PropertyToID("_BloomTint");
            public static readonly int _MainTex = Shader.PropertyToID("_MainTex");
            public static readonly int _BloomTex = Shader.PropertyToID("_BloomTex");
            public static readonly int _PreComposeTex = Shader.PropertyToID("_PreComposeTex");
            public static readonly int _TexelSize = Shader.PropertyToID("_TexelSize");
            public static readonly int _Colorize = Shader.PropertyToID("_Colorize");
            public static readonly int _VignetteShape = Shader.PropertyToID("_VignetteShape");
            public static readonly int _VignetteColor = Shader.PropertyToID("_VignetteColor");
            public static readonly int _BrightnessContrast = Shader.PropertyToID("_BrightnessContrast");
            public static readonly int _ScreenBlurTex = Shader.PropertyToID("_ScreenBlurTex");
            public static readonly int _BlurRadius = Shader.PropertyToID("_Radius");
        }

        // Keywords for shader variants
        private static class Keywords
        {
            public const string COLORIZE_ON = "COLORIZE_ON";
            public const string BLOOM_ON = "BLOOM_ON";
            public const string VIGNETTE_ON = "VIGNETTE_ON";
            public const string BRIGHTNESS_CONTRAST_ON = "BRIGHTNESS_CONTRAST_ON";
            public const string GAMMA_CORRECTION = "GAMMA_CORRECTION";
        }

        private static readonly float RESOLUTION_RATIO_MIN = 0.5f;
        private static readonly float RESOLUTION_RATIO_MAX = 1.0f;

        // Currently linked settings in the inspector
        public SleekRenderSettings settings;

        // Various Material cached objects
        // Created dynamically from found and loaded shaders
        private Material _downsampleMaterial;
        private Material _horizontalBlurMaterial;
        private Material _verticalBlurMaterial;
        private Material _preComposeMaterial;
        private Material _composeMaterial;
        private Material _screenBlurMaterial;
        private Material _colorGradingBaseMaterial;
        private Material _colorGradingBlendMaterial;
        private Material _colorGradingBlendCacheMaterial;

        // Various RenderTextures used in post processing render passes
        private RenderTexture _downsampledBrightpassTexture;
        private RenderTexture _brightPassBlurTexture;
        private RenderTexture _horizontalBlurTexture;
        private RenderTexture _verticalBlurTexture;
        private RenderTexture _preComposeTexture;
        private RenderTexture _composeTexture;
        private RenderTexture _screenBlurTexture;
        private RenderTexture _blendCacheLut;

        // Currenly cached camera on which Post Processing stack is applied
        private Camera cam;
        // Quad mesh used in full screen custom Blitting
        private Mesh _fullscreenQuadMesh;

        // Cached camera width and height. Used in editor code for checking updated size for recreating resources
        private int _currentCameraPixelWidth;
        private int _currentCameraPixelHeight;

        // Various cached variables needed to avoid excessive shader enabling / disabling
        private bool _isColorizeAlreadyEnabled = false;
        private bool _isBloomAlreadyEnabled = false;
        private bool _isVignetteAlreadyEnabled = false;
        private bool _isAlreadyPreservingAspectRatio = false;
        private bool _isContrastAndBrightnessAlreadyEnabled = false;
        private bool _isScreenBlurEnabled = false;
        private bool _isColorGradingEnabled = false;

        // Color grading
        private const int LutSize = 32;
        private const int LutWidth = LutSize * LutSize;
        private const int LutHeight = LutSize;
        private Texture2D _defaultLut = null;
        public Texture2D DefaultLut { get { return (_defaultLut == null) ? CreateDefaultLut() : _defaultLut; } }



        //// TEST
        //public Texture2D testTex;
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        deVoid.Utils.Signals.Get<ChangeColorGradingLUT>().Dispatch(testTex, 2.0F);
        //    }
        //}


        //////////////////////////////////////////
        // Initialize
        //////////////////////////////////////////

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void OnEnable()
        {
#if UNITY_5_6_OR_NEWER
            bool nullDev = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
#else
		    bool nullDev = ( SystemInfo.graphicsDeviceName == "Null Device" );
#endif
            if (nullDev)
            {
                Debug.LogWarning("Null graphics device detected. Skipping effect silently.");
                return;
            }

            if (!CheckSupport())
                return;

            // If we are adding a component from scratch, we should supply fake settings with default values 
            // (until normal ones are linked)
            CreateDefaultSettingsIfNoneLinked();
            CreateResources();

            Texture2D lutTex2d = settings.LutTexture as Texture2D;
            Texture2D lutBlendTex2d = settings.LutBlendTexture as Texture2D;

            if ((lutTex2d != null && lutTex2d.mipmapCount > 1) || (lutBlendTex2d != null && lutBlendTex2d.mipmapCount > 1))
                Debug.LogError("Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. " +
                    "Change Texture Type to \"Advanced\" to access Mip settings.");
        }

        private void OnDisable()
        {
            ReleaseResources();
        }

        //private RenderTexture _rt;
        //private RenderTexture _rtTarget;
        //private void OnPreRender()
        //{
        //    _rt = RenderTexture.GetTemporary(Screen.width, Screen.height);
        //    _rtTarget = RenderTexture.GetTemporary(Screen.width, Screen.height);
        //    cam.targetTexture = _rt;
        //}

        //private void OnPostRender()
        //{
        //    cam.targetTexture = null;
        //    OnDoRenderImage(_rt, _rtTarget);
        //    RenderTexture.ReleaseTemporary(_rt);
        //}

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Editor only behaviour needed to recreate resources if viewport size changes (resizing editor window)
#if UNITY_EDITOR
            CreateDefaultSettingsIfNoneLinked();
            CheckScreenSizeAndRecreateTexturesIfNeeded(cam);
#endif
            // Applying post processing steps
            ApplyPostProcess(source);
            // Last step as separate pass
            Compose(source, destination);
            // Applying screen blur
            ScreenBlur(source, destination);
            // Applying Color grading
            ColorGrade(destination);
        }

        private void ApplyPostProcess(RenderTexture source)
        {
            var isBloomEnabled = settings.bloomEnabled;

            Downsample(source);
            Bloom(isBloomEnabled);
            Precompose(isBloomEnabled);
        }

        private void Downsample(RenderTexture source)
        {
            // Precomputing brightpass parameters
            // It's better to do it once per frame rather than once per pixel
            float oneOverOneMinusBloomThreshold = 1f / (1f - settings.bloomThreshold);
            var luma = settings.bloomLumaVector;
            Vector4 luminanceConst = new Vector4(
                luma.x * oneOverOneMinusBloomThreshold,
                luma.y * oneOverOneMinusBloomThreshold,
                luma.z * oneOverOneMinusBloomThreshold, -settings.bloomThreshold * oneOverOneMinusBloomThreshold);

            // Changing current Luminance Const value just to make sure that we have the latest settings in our Uniforms
            _downsampleMaterial.SetVector(Uniforms._LuminanceConst, luminanceConst);

            // Applying downsample + brightpass (stored in Alpha)
            Blit(source, _downsampledBrightpassTexture, _downsampleMaterial);

            if (settings.colorGradingEnabled && !_isColorGradingEnabled)
                _isColorGradingEnabled = true;
            else if (!settings.colorGradingEnabled && _isColorGradingEnabled)
                _isColorGradingEnabled = false;
        }

        private void Bloom(bool isBloomEnabled)
        {
            if (isBloomEnabled)
            {
                // Applying horizontal and vertical Separable Gaussian Blur passes
                Blit(_downsampledBrightpassTexture, _brightPassBlurTexture, _horizontalBlurMaterial);
                Blit(_brightPassBlurTexture, _verticalBlurTexture, _verticalBlurMaterial);
            }
        }

        private void Precompose(bool isBloomEnabled)
        {
            // Setting up vignette effect
            var isVignetteEnabledInSettings = settings.vignetteEnabled;
            if (isVignetteEnabledInSettings && !_isVignetteAlreadyEnabled)
            {
                _preComposeMaterial.EnableKeyword(Keywords.VIGNETTE_ON);
                _isVignetteAlreadyEnabled = true;
            }
            else if (!isVignetteEnabledInSettings && _isVignetteAlreadyEnabled)
            {
                _preComposeMaterial.DisableKeyword(Keywords.VIGNETTE_ON);
                _isVignetteAlreadyEnabled = false;
            }

            if (isVignetteEnabledInSettings)
            {
                // Calculating Vignette parameters once per frame rather than once per pixel
                float vignetteBeginRadius = settings.vignetteBeginRadius;
                float squareVignetteBeginRaduis = vignetteBeginRadius * vignetteBeginRadius;
                float vignetteRadii = vignetteBeginRadius + settings.vignetteExpandRadius;
                float oneOverVignetteRadiusDistance = 1f / (vignetteRadii - squareVignetteBeginRaduis);

                var vignetteColor = settings.vignetteColor;

                _preComposeMaterial.SetVector(Uniforms._VignetteShape, new Vector4(
                    4f * oneOverVignetteRadiusDistance * oneOverVignetteRadiusDistance,
                    -oneOverVignetteRadiusDistance * squareVignetteBeginRaduis));

                // Premultiplying Alpha of vignette color
                _preComposeMaterial.SetColor(Uniforms._VignetteColor, new Color(
                    vignetteColor.r * vignetteColor.a,
                    vignetteColor.g * vignetteColor.a,
                    vignetteColor.b * vignetteColor.a,
                    vignetteColor.a));
            }

            // Bloom is handled in two different passes (two blurring bloom passes and one precompose pass)
            // So we need to check for whether it's enabled in precompose step too (shader has variants without bloom)
            if (isBloomEnabled)
            {
                _preComposeMaterial.SetFloat(Uniforms._BloomIntencity, settings.bloomIntensity);
                _preComposeMaterial.SetColor(Uniforms._BloomTint, settings.bloomTint);

                if (!_isBloomAlreadyEnabled)
                {
                    _preComposeMaterial.EnableKeyword(Keywords.BLOOM_ON);
                    _isBloomAlreadyEnabled = true;
                }
            }
            else if (_isBloomAlreadyEnabled)
            {
                _preComposeMaterial.DisableKeyword(Keywords.BLOOM_ON);
                _isBloomAlreadyEnabled = false;
            }

            // Finally applying precompose step. It slaps bloom and vignette together
            Blit(_downsampledBrightpassTexture, _preComposeTexture, _preComposeMaterial);
        }

        private void Compose(RenderTexture source, RenderTexture destination)
        {
            // Composing pass includes using full size main render texture + precompose texture
            // Precompose texture contains valuable info in its Alpha channel (whether to apply it on the final image or not)
            // Compose step also includes uniform colorizing which is calculated and enabled / disabled separately
            Color colorize = settings.colorize;
            var a = colorize.a;
            var colorizeConstant = new Color(colorize.r * a, colorize.g * a, colorize.b * a, 1f - a);
            _composeMaterial.SetColor(Uniforms._Colorize, colorizeConstant);

            if (settings.colorizeEnabled && !_isColorizeAlreadyEnabled)
            {
                _composeMaterial.EnableKeyword(Keywords.COLORIZE_ON);
                _isColorizeAlreadyEnabled = true;
            }
            else if (!settings.colorizeEnabled && _isColorizeAlreadyEnabled)
            {
                _composeMaterial.DisableKeyword(Keywords.COLORIZE_ON);
                _isColorizeAlreadyEnabled = false;
            }

            float normalizedContrast = settings.contrast + 1f;
            float normalizedBrightness = (settings.brightness + 1f) / 2f;
            var brightnessContrastPrecomputed = (-0.5f) * (normalizedContrast + 1f) + (normalizedBrightness * 2f); // optimization
            _composeMaterial.SetVector(Uniforms._BrightnessContrast, new Vector4(normalizedContrast, normalizedBrightness, brightnessContrastPrecomputed));

            if (settings.brightnessContrastEnabled && !_isContrastAndBrightnessAlreadyEnabled)
            {
                _composeMaterial.EnableKeyword(Keywords.BRIGHTNESS_CONTRAST_ON);
                _isContrastAndBrightnessAlreadyEnabled = true;
            }
            else if (!settings.brightnessContrastEnabled && _isContrastAndBrightnessAlreadyEnabled)
            {
                _composeMaterial.DisableKeyword(Keywords.BRIGHTNESS_CONTRAST_ON);
                _isContrastAndBrightnessAlreadyEnabled = false;
            }

            if (settings.screenBlurEnabled && !_isScreenBlurEnabled)
                _isScreenBlurEnabled = true;
            else if (!settings.screenBlurEnabled && _isScreenBlurEnabled)
                _isScreenBlurEnabled = false;

            if (settings.colorGradingEnabled && !_isColorGradingEnabled)
                _isColorGradingEnabled = true;
            else if (!settings.colorGradingEnabled && _isColorGradingEnabled)
                _isColorGradingEnabled = false;

            if (_isScreenBlurEnabled || _isColorGradingEnabled)
                Blit(source, _composeTexture, _composeMaterial);
            else
                Blit(source, destination, _composeMaterial);
        }

        private void ScreenBlur(RenderTexture source, RenderTexture destination)
        {
            if (_isScreenBlurEnabled)
            {
                int downsample = settings.downsample;
                int tw = source.width >> downsample;
                int th = source.height >> downsample;

                var rt = RenderTexture.GetTemporary(tw, th, 0, source.format);

                Blit(_composeTexture, rt, _screenBlurMaterial, 0);

                if (settings.gammaCorrection)
                    _screenBlurMaterial.EnableKeyword(Keywords.GAMMA_CORRECTION);
                else
                    _screenBlurMaterial.DisableKeyword(Keywords.GAMMA_CORRECTION);

                int kernel = 0;
                switch (settings.kernelSize)
                {
                    case BlurKernelSize.Small:
                        kernel = 0;
                        break;
                    case BlurKernelSize.Medium:
                        kernel = 2;
                        break;
                    case BlurKernelSize.Big:
                        kernel = 4;
                        break;
                }

                int _iterations = settings.iterations;
                float _interpolation = settings.interpolation;
                for (int i = 0; i < _iterations; i++)
                {
                    // helps to achieve a larger blur
                    float blurRadius = (float)i * _interpolation + _interpolation;
                    _screenBlurMaterial.SetFloat(Uniforms._BlurRadius, blurRadius);

                    Blit(rt, _screenBlurTexture, _screenBlurMaterial, 1 + kernel);
                    rt.DiscardContents();

                    // is it a last iteration? If so, then blit to destination
                    if (i == _iterations - 1)
                    {
                        if (_isColorGradingEnabled)
                            Blit(_screenBlurTexture, _composeTexture, _screenBlurMaterial, 2 + kernel);
                        else
                            Blit(_screenBlurTexture, destination, _screenBlurMaterial, 2 + kernel);
                    }
                    else
                    {
                        Blit(_screenBlurTexture, rt, _screenBlurMaterial, 2 + kernel);
                        _screenBlurTexture.DiscardContents();
                    }
                }

                RenderTexture.ReleaseTemporary(rt);
            }
        }

        private void ColorGrade(RenderTexture destination)
        {
            if (_isColorGradingEnabled)
            {
                settings.ColorGradingBlendAmount = Mathf.Clamp01(settings.ColorGradingBlendAmount);
                settings.hdrExposure = Mathf.Max(settings.hdrExposure, 0);

                bool validLut = ValidateLutDimensions(settings.LutTexture);
                bool validLutBlend = ValidateLutDimensions(settings.LutBlendTexture);
                bool skip = (settings.LutTexture == null && settings.LutBlendTexture == null) || !validLut || !validLutBlend;

                Texture lut = (settings.LutTexture == null) ? _defaultLut : settings.LutTexture;
                Texture lutBlend = settings.LutBlendTexture;

                int pass = ComputeShaderPass();

                bool blend = (settings.ColorGradingBlendAmount > 0.0f) || settings.nowBlending;
                bool requiresBlend = blend || (blend && lutBlend != null);
                bool useBlendCache = requiresBlend;

                Material material;
                if (requiresBlend)
                    material = _colorGradingBlendMaterial;
                else
                    material = _colorGradingBaseMaterial;

                // HDR control params
                material.SetFloat("_Exposure", settings.hdrExposure);
                material.SetFloat("_ShoulderStrength", 0.22f);
                material.SetFloat("_LinearStrength", 0.30f);
                material.SetFloat("_LinearAngle", 0.10f);
                material.SetFloat("_ToeStrength", 0.20f);
                material.SetFloat("_ToeNumerator", 0.01f);
                material.SetFloat("_ToeDenominator", 0.30f);
                material.SetFloat("_LinearWhite", settings.hdrLinearWhitePoint);

                // Stereo
#if UNITY_5_6_OR_NEWER
                material.SetVector("_StereoScale", new Vector4(1, 1, 0, 0));
#else
		        material.SetVector( "_StereoScale", new Vector4( 1, 1, 0, 0 ) );
#endif

                if (!skip)
                {
                    if (useBlendCache)
                    {
                        _colorGradingBlendCacheMaterial.SetFloat("_LerpAmount", settings.ColorGradingBlendAmount);
                        _colorGradingBlendCacheMaterial.SetTexture("_RgbTex", lut);
                        _colorGradingBlendCacheMaterial.SetTexture("_LerpRgbTex", (lutBlend != null) ? lutBlend : _defaultLut);

                        Blit(lut, _blendCacheLut, _colorGradingBlendCacheMaterial);
                        material.SetTexture("_RgbBlendCacheTex", _blendCacheLut);
                    }
                    else
                    {
                        if (lut != null)
                            material.SetTexture("_RgbTex", lut);
                    }
                }

                Blit(_composeTexture, destination, material, pass);

                if (useBlendCache)
                    _blendCacheLut.DiscardContents();
            }
        }

        public void RefreshComposeTexture(float ratio)
        {
            int width = _currentCameraPixelWidth;
            int height = _currentCameraPixelHeight;

            ratio = Mathf.Clamp(ratio, RESOLUTION_RATIO_MIN, RESOLUTION_RATIO_MAX);

            int ratioWidth = Mathf.RoundToInt(width * ratio);
            int ratioHeight = Mathf.RoundToInt(height * ratio);

            DestroyImmediateIfNotNull(_composeTexture);
            _composeTexture = CreateTransientRenderTexture("Compose", ratioWidth, ratioHeight);
            Debug.Log($"RefreshComposeTexture : {ratio}");
        }

        private void CreateResources()
        {
            var downsampleShader = Shader.Find("Hidden/Sleek Render/Post Process/Downsample Brightpass");
            var horizontalBlurShader = Shader.Find("Hidden/Sleek Render/Post Process/Horizontal Blur");
            var verticalBlurShader = Shader.Find("Hidden/Sleek Render/Post Process/Vertical Blur");
            var preComposeShader = Shader.Find("Hidden/Sleek Render/Post Process/PreCompose");
            var composeShader = Shader.Find("Hidden/Sleek Render/Post Process/Compose");
            var screenBlurShader = Shader.Find("Hidden/Sleek Render/Post Process/ScreenBlur");
            var shaderBase = Shader.Find("Hidden/Sleek Render/Post Process/ColorGradingBase");
            var shaderBlend = Shader.Find("Hidden/Sleek Render/Post Process/ColorGradingBlend");
            var shaderBlendCache = Shader.Find("Sleek Render/Post Process/ColorGradingBlendCache");

            if (!CheckShaders())
            {
                Debug.LogError("Failed to initialize shaders!");
                return;
            }

            _downsampleMaterial = new Material(downsampleShader);
            _horizontalBlurMaterial = new Material(horizontalBlurShader);
            _verticalBlurMaterial = new Material(verticalBlurShader);
            _preComposeMaterial = new Material(preComposeShader);
            _screenBlurMaterial = new Material(screenBlurShader);
            _composeMaterial = new Material(composeShader);
            _colorGradingBaseMaterial = new Material(shaderBase);
            _colorGradingBlendMaterial = new Material(shaderBlend);
            _colorGradingBlendCacheMaterial = new Material(shaderBlendCache);

            _downsampleMaterial.hideFlags = HideFlags.HideAndDontSave;
            _horizontalBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            _verticalBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            _preComposeMaterial.hideFlags = HideFlags.HideAndDontSave;
            _screenBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            _composeMaterial.hideFlags = HideFlags.HideAndDontSave;
            _colorGradingBaseMaterial.hideFlags = HideFlags.HideAndDontSave;
            _colorGradingBlendMaterial.hideFlags = HideFlags.HideAndDontSave;
            _colorGradingBlendCacheMaterial.hideFlags = HideFlags.HideAndDontSave;

            _currentCameraPixelWidth = Mathf.RoundToInt(cam.pixelWidth);
            _currentCameraPixelHeight = Mathf.RoundToInt(cam.pixelHeight);

            // Point for future main render target size changing
            int width = _currentCameraPixelWidth;
            int height = _currentCameraPixelHeight;

            // Capping max base texture height in pixels
            // We usually don't need extra pixels for precompose and blur passes
            var maxHeight = Mathf.Min(height, 720);
            var ratio = (float)maxHeight / height;

            var _ratio = EncryptedPlayerPrefs.GetFloat("QualitySetting", 1.0f);
            _ratio = Mathf.Clamp(ratio, RESOLUTION_RATIO_MIN, RESOLUTION_RATIO_MAX);

            // Constant used to make the bloom look completely uniform on square or circle objects
            int blurHeight = settings.bloomTextureHeight;
            int blurWidth = settings.preserveAspectRatio ? Mathf.RoundToInt(blurHeight * GetCurrentAspect(cam)) : settings.bloomTextureWidth;

            // Downsampling texture size (downscale + brightpass and precompose)
            int downsampleWidth = Mathf.RoundToInt((width * ratio) / 5f);
            int downsampleHeight = Mathf.RoundToInt((height * ratio) / 5f);
            int blurDownsample = settings.downsample;

            int ratioWidth = Mathf.RoundToInt(width * _ratio);
            int ratioHeight = Mathf.RoundToInt(height * _ratio);

            _downsampledBrightpassTexture = CreateTransientRenderTexture("Bloom Downsample Pass", downsampleWidth, downsampleHeight);
            _brightPassBlurTexture = CreateTransientRenderTexture("Pre Bloom", blurWidth, blurHeight);
            _horizontalBlurTexture = CreateTransientRenderTexture("Horizontal Blur", blurWidth, blurHeight);
            _verticalBlurTexture = CreateTransientRenderTexture("Vertical Blur", blurWidth, blurHeight);
            _preComposeTexture = CreateTransientRenderTexture("Pre Compose", downsampleWidth, downsampleHeight);
            _composeTexture = CreateTransientRenderTexture("Compose", ratioWidth, ratioHeight);
            _screenBlurTexture = CreateTransientRenderTexture("Screen Blur", width >> blurDownsample, height >> blurDownsample);

            _blendCacheLut = new RenderTexture(LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear) { hideFlags = HideFlags.HideAndDontSave };
            _blendCacheLut.name = "BlendCacheLut";
            _blendCacheLut.wrapMode = TextureWrapMode.Clamp;
            _blendCacheLut.useMipMap = false;
            _blendCacheLut.anisoLevel = 0;
            _blendCacheLut.Create();

            CreateDefaultLut();

            _verticalBlurMaterial.SetTexture(Uniforms._MainTex, _downsampledBrightpassTexture);
            _verticalBlurMaterial.SetTexture(Uniforms._BloomTex, _horizontalBlurTexture);  // 주석 처리??

            var xSpread = 1 / (float)blurWidth;
            var ySpread = 1 / (float)blurHeight;
            var blurTexelSize = new Vector4(xSpread, ySpread);

            _verticalBlurMaterial.SetVector(Uniforms._TexelSize, blurTexelSize);
            _horizontalBlurMaterial.SetVector(Uniforms._TexelSize, blurTexelSize);

            _preComposeMaterial.SetTexture(Uniforms._BloomTex, _verticalBlurTexture);

            var downsampleTexelSize = new Vector4(1f / _downsampledBrightpassTexture.width, 1f / _downsampledBrightpassTexture.height);
            _downsampleMaterial.SetVector(Uniforms._TexelSize, downsampleTexelSize);

            _composeMaterial.SetTexture(Uniforms._PreComposeTex, _preComposeTexture);
            _composeMaterial.SetVector(Uniforms._LuminanceConst, new Vector4(0.2126f, 0.7152f, 0.0722f, 0f));

            _fullscreenQuadMesh = CreateScreenSpaceQuadMesh();

            _isColorizeAlreadyEnabled = false;
            _isBloomAlreadyEnabled = false;
            _isVignetteAlreadyEnabled = false;
            _isContrastAndBrightnessAlreadyEnabled = false;
            _isScreenBlurEnabled = false;


            bool CheckShaders()
            {
                return CheckShader(downsampleShader) && CheckShader(horizontalBlurShader) && CheckShader(verticalBlurShader)
                    && CheckShader(preComposeShader) && CheckShader(screenBlurShader) && CheckShader(composeShader)
                    && CheckShader(shaderBase) && CheckShader(shaderBlend) && CheckShader(shaderBlendCache);
            }

            bool CheckShader(Shader s)
            {
                if (s == null)
                {
                    ReportMissingShaders();
                    return false;
                }
                if (!s.isSupported)
                {
                    ReportNotSupported();
                    return false;
                }
                return true;
            }
        }

        private RenderTexture CreateTransientRenderTexture(string textureName, int width, int height)
        {
            var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            renderTexture.name = textureName;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            return renderTexture;
        }

        //        private RenderTexture CreateMainRenderTexture(int width, int height)
        //        {
        //            var isMetal = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal;
        //            var isTegra = SystemInfo.graphicsDeviceName.Contains("NVIDIA");
        //            var rgb565NotSupported = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565);

        //            var textureFormat = RenderTextureFormat.RGB565;
        //            if (isMetal || isTegra || rgb565NotSupported)
        //            {
        //                textureFormat = RenderTextureFormat.ARGB32;
        //            }

        //#if UNITY_EDITOR
        //            textureFormat = RenderTextureFormat.ARGB32;
        //#endif

        //            var renderTexture = new RenderTexture(width, height, 16, textureFormat);
        //            var antialiasingSamples = QualitySettings.antiAliasing;
        //            renderTexture.antiAliasing = antialiasingSamples == 0 ? 1 : antialiasingSamples;
        //            return renderTexture;
        //        }

        private void ReleaseResources()
        {
            DestroyImmediateIfNotNull(_downsampleMaterial);
            DestroyImmediateIfNotNull(_horizontalBlurMaterial);
            DestroyImmediateIfNotNull(_verticalBlurMaterial);
            DestroyImmediateIfNotNull(_preComposeMaterial);
            DestroyImmediateIfNotNull(_screenBlurMaterial);
            DestroyImmediateIfNotNull(_composeMaterial);

            DestroyImmediateIfNotNull(_downsampledBrightpassTexture);
            DestroyImmediateIfNotNull(_brightPassBlurTexture);
            DestroyImmediateIfNotNull(_horizontalBlurTexture);
            DestroyImmediateIfNotNull(_verticalBlurTexture);
            DestroyImmediateIfNotNull(_preComposeTexture);
            DestroyImmediateIfNotNull(_composeTexture);
            DestroyImmediateIfNotNull(_screenBlurTexture);

            DestroyImmediateIfNotNull(_blendCacheLut);
            DestroyImmediateIfNotNull(_defaultLut);

            DestroyImmediateIfNotNull(_fullscreenQuadMesh);
        }

        private void DestroyImmediateIfNotNull(Object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(RenderTexture))
                    (obj as RenderTexture).Release();

                DestroyImmediate(obj);
            }
        }

        public void Blit(Texture source, RenderTexture destination, Material material, int materialPass = 0)
        {
            SetActiveRenderTextureAndClear(destination);
            this.DrawFullscreenQuad(source, material, materialPass);
        }

        private static void SetActiveRenderTextureAndClear(RenderTexture destination)
        {
            RenderTexture.active = destination;
            GL.Clear(true, true, new Color(1f, 0.75f, 0.5f, 0.8f));
        }

        private void DrawFullscreenQuad(Texture source, Material material, int materialPass = 0)
        {
            material.SetTexture(Uniforms._MainTex, source);
            material.SetPass(materialPass);
            Graphics.DrawMeshNow(_fullscreenQuadMesh, Matrix4x4.identity);
        }

#if UNITY_EDITOR
        private void CheckScreenSizeAndRecreateTexturesIfNeeded(Camera mainCamera)
        {
            var cameraSizeHasChanged = mainCamera.pixelWidth != _currentCameraPixelWidth ||
                mainCamera.pixelHeight != _currentCameraPixelHeight;

            var bloomSizeHasChanged = _horizontalBlurTexture.height != settings.bloomTextureHeight;
            if (!settings.preserveAspectRatio)
            {
                bloomSizeHasChanged |= _horizontalBlurTexture.width != settings.bloomTextureWidth;
            }

            if (!bloomSizeHasChanged && settings.preserveAspectRatio)
            {
                if (_horizontalBlurTexture.width != Mathf.RoundToInt(_horizontalBlurTexture.height * GetCurrentAspect(mainCamera)))
                {
                    bloomSizeHasChanged = true;
                }
            }

            if (settings.preserveAspectRatio && !_isAlreadyPreservingAspectRatio
                || !settings.preserveAspectRatio && _isAlreadyPreservingAspectRatio)
            {
                _isAlreadyPreservingAspectRatio = settings.preserveAspectRatio;
                bloomSizeHasChanged = true;
            }

            if (cameraSizeHasChanged || bloomSizeHasChanged)
            {
                ReleaseResources();
                CreateResources();
            }
        }
#endif

        private float GetCurrentAspect(Camera mainCamera)
        {
            const float SQUARE_ASPECT_CORRECTION = 0.7f;
            return mainCamera.aspect * SQUARE_ASPECT_CORRECTION;
        }

        private bool CheckSupport()
        {
#if !UNITY_2019_1_OR_NEWER
			// Disable if we don't support image effect or render textures
#if UNITY_5_6_OR_NEWER
			if ( !SystemInfo.supportsImageEffects )
#else
			if ( !SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures )
#endif
			{
				ReportNotSupported();
				return false;
			}
#endif
            return true;
        }

        private void ReportMissingShaders()
        {
            Debug.LogError("Failed to initialize shader because it's missing!");
            enabled = false;
        }

        private void ReportNotSupported()
        {
            Debug.LogError("This image effect is not supported on this platform!");
            enabled = false;
        }

        private void CreateDefaultSettingsIfNoneLinked()
        {
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<SleekRenderSettings>();
                settings.name = "Default Settings";
            }
        }

        private Mesh CreateScreenSpaceQuadMesh()
        {
            var mesh = new Mesh();

            var vertices = new[]
            {
                new Vector3 (-1f, -1f, 0f), // BL
                new Vector3 (-1f, 1f, 0f), // TL
                new Vector3 (1f, 1f, 0f), // TR
                new Vector3 (1f, -1f, 0f) // BR
            };

            var uvs = new[]
            {
                new Vector2 (0f, 0f),
                new Vector2 (0f, 1f),
                new Vector2 (1f, 1f),
                new Vector2 (1f, 0f)
            };

            var colors = new[]
            {
                new Color (0f, 0f, 1f),
                new Color (0f, 1f, 1f),
                new Color (1f, 1f, 1f),
                new Color (1f, 0f, 1f),
            };

            var triangles = new[]
            {
                0,
                2,
                1,
                0,
                3,
                2
            };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.UploadMeshData(true);

            return mesh;
        }

        private Texture2D CreateDefaultLut()
        {
            const int maxSize = LutSize - 1;

            _defaultLut = new Texture2D(LutWidth, LutHeight, TextureFormat.RGB24, false, true) { hideFlags = HideFlags.HideAndDontSave };
            _defaultLut.name = "DefaultLut";
            _defaultLut.hideFlags = HideFlags.DontSave;
            _defaultLut.anisoLevel = 1;
            _defaultLut.filterMode = FilterMode.Bilinear;
            Color32[] colors = new Color32[LutWidth * LutHeight];

            for (int z = 0; z < LutSize; z++)
            {
                int zoffset = z * LutSize;

                for (int y = 0; y < LutSize; y++)
                {
                    int yoffset = zoffset + y * LutWidth;

                    for (int x = 0; x < LutSize; x++)
                    {
                        float fr = x / (float)maxSize;
                        float fg = y / (float)maxSize;
                        float fb = z / (float)maxSize;
                        byte br = (byte)(fr * 255);
                        byte bg = (byte)(fg * 255);
                        byte bb = (byte)(fb * 255);
                        colors[yoffset + x] = new Color32(br, bg, bb, 255);
                    }
                }
            }

            _defaultLut.SetPixels32(colors);
            _defaultLut.Apply();

            return _defaultLut;
        }

        public static bool ValidateLutDimensions(Texture lut)
        {
            bool valid = true;
            if (lut != null)
            {
                if ((lut.width / lut.height) != lut.height)
                {
                    Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
                    valid = false;
                }
                else
                {
                    if (lut.anisoLevel != 0)
                        lut.anisoLevel = 0;
                }
            }
            return valid;
        }

        private int ComputeShaderPass()
        {
            bool isMobile = (settings.colorGradingQualityLevel == ColorGradingQuality.Mobile);
            bool isLinear = (QualitySettings.activeColorSpace == ColorSpace.Linear);
#if UNITY_5_6_OR_NEWER
            bool isHDR = cam.allowHDR;
#else
		    bool isHDR = ownerCamera.hdr;
#endif

            int pass = isMobile ? 18 : 0;
            if (isHDR)
            {
                pass += 2;                      // skip LDR
                pass += isLinear ? 8 : 0;       // skip GAMMA, if applicable
                pass += settings.hdrApplyDithering ? 4 : 0; // skip DITHERING, if applicable
                pass += (int)settings.hdrTonemapper;
            }
            else
            {
                pass += isLinear ? 1 : 0;
            }
            return pass;
        }

        private bool CheckMaterialAndShader(Material material, string name)
        {
            if (material == null || material.shader == null)
            {
                Debug.LogWarning("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
                enabled = false;
            }
            else if (!material.shader.isSupported)
            {
                Debug.LogWarning("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
                enabled = false;
            }
            else
            {
                material.hideFlags = HideFlags.HideAndDontSave;
            }
            return enabled;
        }

        //        private void SetMaterialKeyword(string keyword, bool state)
        //        {
        //#if !UNITY_5_6_OR_NEWER
        //		if ( state )
        //			Shader.EnableKeyword( keyword );
        //		else
        //			Shader.DisableKeyword( keyword );
        //#else
        //            bool keywordEnabled = materialBase.IsKeywordEnabled(keyword);
        //            if (state && !keywordEnabled)
        //            {
        //                materialBase.EnableKeyword(keyword);
        //                materialBlend.EnableKeyword(keyword);
        //                materialBlendCache.EnableKeyword(keyword);
        //            }
        //            else if (!state && materialBase.IsKeywordEnabled(keyword))
        //            {
        //                materialBase.DisableKeyword(keyword);
        //                materialBlend.DisableKeyword(keyword);
        //                materialBlendCache.DisableKeyword(keyword);
        //            }
        //#endif
        //        }
    }
}

//    private void OnToggleScreenBlur(bool isOn, float duration)
//    {
//        if (!m_SleekRenderSettings)
//        {
//            Debug.Log("Cannot toggle blur because m_SleekRenderSettings is null!");
//            return;
//        }

//        if (DOTween.IsTweening(TWEEN_ID_SCREEN_BLUR))
//            DOTween.Kill(TWEEN_ID_SCREEN_BLUR);

//        float endInterpolation = isOn ? 1.0F : 0.0F;
//        int curAlpha = isOn ? 0 : FADE_END_VALUE;
//        int endAlpha = isOn ? FADE_END_VALUE : 0;
//        Color32 curCol = new Color32(0, 0, 0, 0);

//        Sequence seq = DOTween.Sequence()
//                        .Append(DOTween.To(() => m_SleekRenderSettings.interpolation, x => m_SleekRenderSettings.interpolation = x, endInterpolation, duration).SetEase(Ease.OutQuad))
//                        .Join(DOTween.To(() => curAlpha, x => curAlpha = x, endAlpha, duration).SetEase(Ease.OutQuad))
//                        .SetId(TWEEN_ID_SCREEN_BLUR)
//                        .OnStart(DoStart)
//                        .OnUpdate(DoUpdate)
//                        .OnComplete(DoComplete)
//                        .Play();


//        void DoStart()
//        {
//            m_SleekRenderSettings.interpolation = 1.0F - endInterpolation;
//            if (!m_SleekRenderSettings.screenBlurEnabled)
//                m_SleekRenderSettings.screenBlurEnabled = true;

//            curCol.a = (byte)curAlpha;
//            m_SleekRenderSettings.colorize = curCol;
//            if (!m_SleekRenderSettings.colorizeEnabled)
//                m_SleekRenderSettings.colorizeEnabled= true;
//        }

//        void DoUpdate()
//        {
//            curCol.a = (byte)curAlpha;
//            m_SleekRenderSettings.colorize = curCol;
//        }

//        void DoComplete()
//        {
//            m_SleekRenderSettings.interpolation = endInterpolation;
//            m_SleekRenderSettings.screenBlurEnabled = isOn;

//            curCol.a = (byte)endAlpha;
//            m_SleekRenderSettings.colorize = curCol;
//            m_SleekRenderSettings.colorizeEnabled = isOn;
//        }
//    }

//    private void OnChangeColorGradingLUT(Texture blendTo, float duration = 1.0F)
//    {
//        if (!m_SleekRenderSettings)
//        {
//            Debug.Log("Cannot change color grading because m_SleekRenderSettings is null!");
//            return;
//        }

//        if(!blendTo)
//        {
//            Debug.Log("Cannot change color grading because blendTo is null!");
//            return;
//        }

//        if (DOTween.IsTweening(TWEEN_ID_COLOR_GRADING))
//            DOTween.Kill(TWEEN_ID_COLOR_GRADING);

//        DOTween.To(() => m_SleekRenderSettings.ColorGradingBlendAmount, x => m_SleekRenderSettings.ColorGradingBlendAmount = x, 1.0F, duration).SetEase(Ease.OutQuad)
//            .SetId(TWEEN_ID_COLOR_GRADING)
//            .OnStart(DoStart)
//            .OnComplete(DoComplete)
//            .Play();


//        void DoStart()
//        {
//            m_SleekRenderSettings.LutBlendTexture = blendTo;
//            m_SleekRenderSettings.ColorGradingBlendAmount = 0.0f;
//            m_SleekRenderSettings.nowBlending = true;
//        }

//        void DoComplete()
//        {
//            m_SleekRenderSettings.LutTexture = m_SleekRenderSettings.LutBlendTexture;
//            m_SleekRenderSettings.ColorGradingBlendAmount = 0.0f;
//            m_SleekRenderSettings.nowBlending = false;
//            m_SleekRenderSettings.LutBlendTexture = null;
//        }
//    }

//    private void OnToggleDof(bool toggle, float distance)
//    {
//        if(!m_DofEffect)
//        {
//            Debug.LogError("Cannot toggle dof because m_DofEffect is null!");
//            return;
//        }

//        if (DOTween.IsTweening(TWEEN_ID_DOF))
//            DOTween.Kill(TWEEN_ID_DOF);

//        float blurEndVal = toggle ? m_DofBlurEndValue : 0.0F;

//        DOTween.To(() => m_DofEffect.BlurAmount, x => m_DofEffect.BlurAmount = x, blurEndVal, 0.5F).SetEase(Ease.OutQuad)
//            .SetId(TWEEN_ID_DOF)
//            .OnStart(DoStart)
//            .OnUpdate(DoUpdate)
//            .OnComplete(DoComplete)
//            .Play();


//        void DoStart()
//        {
//            if (toggle)
//            {
//                m_DofEffect.BlurAmount = 0.0F;
//                m_DofEffect.Focus = distance;
//                m_DofEffect.enabled = true;
//            }
//            else
//            {
//                if(m_ApertureCoroutine != null)
//                {
//                    GameManager.Instance.BehaviourProxy.StopCoroutine(m_ApertureCoroutine);
//                    m_ApertureCoroutine = null;
//                }
//            }
//        }

//        void DoUpdate()
//        {
//            float aperture_t = Mathf.InverseLerp(ZOOM_BOUNDS_PADDING[0], ZOOM_BOUNDS_PADDING[1], MainCam.fieldOfView);
//            m_DofEffect.Aperture = Mathf.Lerp(m_Aperture_Min, m_Aperture_Max, aperture_t);
//        }

//        void DoComplete()
//        {
//            if (toggle)
//            {
//                m_ApertureCoroutine = GameManager.Instance.BehaviourProxy.StartCoroutine(UpdateDofAperture());
//            }
//            else
//            {
//                m_DofEffect.enabled = false;
//            }
//        }

//        IEnumerator UpdateDofAperture()
//        {
//            while (true)
//            {
//                float aperture_t = Mathf.InverseLerp(ZOOM_BOUNDS_PADDING[0], ZOOM_BOUNDS_PADDING[1], MainCam.fieldOfView);
//                m_DofEffect.Aperture = Mathf.Lerp(m_Aperture_Min, m_Aperture_Max, aperture_t);
//                yield return null;
//            }
//        }
//    }