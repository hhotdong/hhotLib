using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace SleekRender
{
    public class SleekRenderSettings : ScriptableObject
    {
        [Header("Bloom")]
        public bool bloomExpanded = false;
        public bool bloomEnabled = true;
        public float bloomThreshold = 0.6f;
        public float bloomIntensity = 2.5f;
        public Color bloomTint = Color.white;
        public bool preserveAspectRatio = false;
        public int bloomTextureWidth = 128;
        public int bloomTextureHeight = 128;
        public LumaVectorType bloomLumaCalculationType = LumaVectorType.Uniform;
        public Vector3 bloomLumaVector = new Vector3(1f / 3f, 1f / 3f, 1f / 3f);

        [Header("Color overlay (alpha sets intensity)")]
        public bool colorizeExpanded = true;
        public bool colorizeEnabled = true;
        public Color32 colorize = Color.clear;

        [Header("Vignette")]
        public bool vignetteExpanded = true;
        public bool vignetteEnabled = true;
        public float vignetteBeginRadius = 0.166f;
        public float vignetteExpandRadius = 1.34f;
        public Color vignetteColor = Color.black;

        [Header("Contrast/Brightness")]
        public bool brightnessContrastExpanded = false;
        public bool brightnessContrastEnabled = true;
        public float contrast = 0f;
        public float brightness = 0f;

        [Header("Screen blur")]
        public bool screenBlurExpanded = false;
        public bool screenBlurEnabled = true;
        public BlurKernelSize kernelSize = BlurKernelSize.Big;
        [Range(0f, 1f)] public float interpolation = 1f;
        [Range(0, 4)] public int downsample = 1;
        [Range(1, 8)] public int iterations = 1;
        public bool gammaCorrection = false;

        [Header("HDR Control")]
        public bool hdrControlExpanded = false;
        public bool hdrControlEnabled = false;
        public TonemappingSetting hdrTonemapper = TonemappingSetting.Disabled;
        public float hdrExposure = 1.0f;
        public float hdrLinearWhitePoint = 11.2f;
        [FormerlySerializedAs("UseDithering")] public bool hdrApplyDithering = false;

        [Header("Color Grading")]
        public bool colorGradingExpanded = false;
        public bool colorGradingEnabled = false;
        public bool nowBlending;
        [Range(0.0F, 1.0F)] public float ColorGradingBlendAmount = 0.0F;
        public ColorGradingQuality colorGradingQualityLevel = ColorGradingQuality.Standard;
        public Texture LutTexture = null;
        public Texture LutBlendTexture = null;

        private static readonly string settingPath = "Resources";
        private static readonly string settingAssetsName = "SleekRenderSettings";
        private static readonly string settingExtension = ".asset";
        private static readonly string TWEEN_ID_SCREEN_BLUR = "TWEEN_ID_SCREEN_BLUR";
        private static readonly string TWEEN_ID_COLOR_GRADING = "TWEEN_ID_COLOR_GRADING";

        private static SleekRenderSettings instance;
        public static SleekRenderSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(settingAssetsName) as SleekRenderSettings;
                    if (instance == null)
                    {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<SleekRenderSettings>();
#if UNITY_EDITOR
                        string properPath = Path.Combine(Application.dataPath, settingPath);
                        if (!Directory.Exists(properPath))
                        {
                            AssetDatabase.CreateFolder("Assets", settingPath);
                        }

                        string fullPath = Path.Combine(Path.Combine("Assets", settingPath), settingAssetsName + settingExtension);
                        AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }
                }
                return instance;
            }
        }

        [MenuItem("Assets/Create/SleekRenderSettings", priority = 0)]
        public static void OpenSleekRenderSettings()
        {
            var inst = Instance;
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(inst));
            AssetDatabase.Refresh();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnLoad()
        {
            Instance.Initialize();
        }

        private void Initialize()
        {
            bloomEnabled
                = brightnessContrastEnabled
                = colorizeEnabled
                = hdrControlEnabled
                = screenBlurEnabled
                = vignetteEnabled
                = false;

            colorGradingEnabled = true;
        }

        public void ScreenBlur(bool toggle, float duration)
        {
            if (DOTween.IsTweening(TWEEN_ID_SCREEN_BLUR))
                DOTween.Kill(TWEEN_ID_SCREEN_BLUR);

            const int FADE_END_VALUE = 245;
            float endInterpolation = toggle ? 1.0F : 0.0F;
            int curAlpha = toggle ? 0 : FADE_END_VALUE;
            int endAlpha = FADE_END_VALUE - curAlpha;
            Color32 curCol = new Color32(0, 0, 0, 0);

            Sequence seq = DOTween.Sequence()
                            .Append(DOTween.To(() => interpolation, x => interpolation = x, endInterpolation, duration).SetEase(Ease.OutQuad))
                            .Join(DOTween.To(() => curAlpha, x => curAlpha = x, endAlpha, duration).SetEase(Ease.OutQuad))
                            .SetId(TWEEN_ID_SCREEN_BLUR)
                            .OnStart(DoStart)
                            .OnUpdate(DoUpdate)
                            .OnComplete(DoComplete)
                            .Play();

            void DoStart()
            {
                interpolation = 1.0F - endInterpolation;
                if (!screenBlurEnabled)
                    screenBlurEnabled = true;

                curCol.a = (byte)curAlpha;
                colorize = curCol;
                if (!colorizeEnabled)
                    colorizeEnabled = true;
            }

            void DoUpdate()
            {
                curCol.a = (byte)curAlpha;
                colorize = curCol;
            }

            void DoComplete()
            {
                interpolation = endInterpolation;
                screenBlurEnabled = toggle;

                curCol.a = (byte)endAlpha;
                colorize = curCol;
                colorizeEnabled = toggle;
            }
        }

        public void SetColorGradingLUT(Texture blendTo, float duration = 1.0F)
        {
            if (!blendTo)
            {
                Debug.Log("Cannot change color grading because blendTo is null!");
                return;
            }

            if (DOTween.IsTweening(TWEEN_ID_COLOR_GRADING))
                DOTween.Kill(TWEEN_ID_COLOR_GRADING);

            DOTween.To(() => ColorGradingBlendAmount, x => ColorGradingBlendAmount = x, 1.0F, duration).SetEase(Ease.OutQuad)
                .SetId(TWEEN_ID_COLOR_GRADING)
                .OnStart(DoStart)
                .OnComplete(DoComplete)
                .Play();

            void DoStart()
            {
                LutBlendTexture = blendTo;
                ColorGradingBlendAmount = 0.0f;
                nowBlending = true;
            }

            void DoComplete()
            {
                LutTexture = LutBlendTexture;
                ColorGradingBlendAmount = 0.0f;
                nowBlending = false;
                LutBlendTexture = null;
            }
        }
    }

    public enum LumaVectorType
    {
        Uniform,
        sRGB,
        Custom
    }

    public enum BlurKernelSize
    {
        Small,
        Medium,
        Big
    }

    public enum ColorGradingQuality
    {
        Mobile,
        Standard
    }

    public enum TonemappingSetting
    {
        Disabled = 0,
        Photographic = 1,
        FilmicHable = 2,
        FilmicACES = 3
    }
}