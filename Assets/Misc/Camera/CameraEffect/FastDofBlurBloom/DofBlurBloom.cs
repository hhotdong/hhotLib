using UnityEngine;

[ExecuteInEditMode]
public class DofBlurBloom : MonoBehaviour
{
    public enum DepthMethod
    {
        CustomMaterials,
        Depth
    }
    public DepthMethod DepthCalculationMetod = DepthMethod.Depth;
    [Range(0, 1)] public float BlurAmount = 1f;
    public float Focus = 0f;
    public float Aperture = 1f;

    //public Color BloomColor = Color.white;
    //[Range(0, 5)] public float BloomAmount = 1f;
    //[Range(0, 1)] public float BloomThreshold = 0f;
    //[Range(0, 1)] public float BloomSoftness = 0f;
    static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
    //static readonly int bloomColorString = Shader.PropertyToID("_BloomColor");
    //static readonly int blDataString = Shader.PropertyToID("_BloomData");
    static readonly int blurTexString = Shader.PropertyToID("_BlurTex");
    static readonly int focusAmountString = Shader.PropertyToID("_Focus");
    static readonly int apertureAmountString = Shader.PropertyToID("_Aperture");
    static readonly string isDepthKeyword = "ISDEPTH";
    //static readonly string isAlphaKeyword = "ISALPHA";
    public Material material = null;
    private int numberOfPasses = 3;
    private Camera cam;
    //private float knee;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (DepthCalculationMetod == DepthMethod.Depth)
        {
            cam.depthTextureMode = DepthTextureMode.Depth;
            material.EnableKeyword(isDepthKeyword);
        }
    }

    //private void OnDisable()
    //{
    //    cam.depthTextureMode = DepthTextureMode.None;
    //}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (DepthCalculationMetod == DepthMethod.Depth && cam.depthTextureMode != DepthTextureMode.Depth)
        //{
        //    cam.depthTextureMode = DepthTextureMode.Depth;
        //    material.EnableKeyword(isDepthKeyword);
        //}
        //else if (DepthCalculationMetod == DepthMethod.CustomMaterials && cam.depthTextureMode == DepthTextureMode.Depth)
        //{
        //    cam.depthTextureMode = DepthTextureMode.None;
        //    material.DisableKeyword(isDepthKeyword);
        //}
        //if (BloomAmount <= BloomThreshold)
        //{
        //    material.EnableKeyword(isAlphaKeyword);
        //}
        //else
        //{
        //    material.DisableKeyword(isAlphaKeyword);
        //}

        Shader.SetGlobalFloat(focusAmountString, Focus);
        Shader.SetGlobalFloat(apertureAmountString, Aperture);

        //material.SetColor(bloomColorString, BloomAmount * BloomColor);
        //knee = BloomThreshold * BloomSoftness;
        //material.SetVector(blDataString, new Vector4(BloomThreshold, BloomThreshold - knee, 2f * knee, 1f / (4f * knee + 0.00001f)));

        if (BlurAmount > 0)
        {
            numberOfPasses = Mathf.Max(Mathf.CeilToInt(BlurAmount * 4), 1);
            material.SetFloat(blurAmountString, numberOfPasses > 1 ? (BlurAmount * 4 - Mathf.FloorToInt(BlurAmount * 4 - 0.001f)) * 0.5f + 0.5f : BlurAmount * 4);
            RenderTexture blurTex = null;
            if (numberOfPasses == 1)
            {
                blurTex = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 0, source.format);
                Graphics.Blit(source, blurTex, material, 0);
            }
            else if (numberOfPasses == 2)
            {
                blurTex = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 0, source.format);
                var temp1 = RenderTexture.GetTemporary(Screen.width / 4, Screen.height / 4, 0, source.format);
                Graphics.Blit(source, temp1, material, 0);
                Graphics.Blit(temp1, blurTex, material, 0);
                RenderTexture.ReleaseTemporary(temp1);
            }
            else if (numberOfPasses == 3)
            {
                blurTex = RenderTexture.GetTemporary(Screen.width / 4, Screen.height / 4, 0, source.format);
                var temp1 = RenderTexture.GetTemporary(Screen.width / 8, Screen.height / 8, 0, source.format);
                Graphics.Blit(source, blurTex, material, 0);
                Graphics.Blit(blurTex, temp1, material, 0);
                Graphics.Blit(temp1, blurTex, material, 0);
                RenderTexture.ReleaseTemporary(temp1);
            }
            else if (numberOfPasses == 4)
            {
                blurTex = RenderTexture.GetTemporary(Screen.width / 4, Screen.height / 4, 0, source.format);
                var temp1 = RenderTexture.GetTemporary(Screen.width / 8, Screen.height / 8, 0, source.format);
                var temp2 = RenderTexture.GetTemporary(Screen.width / 16, Screen.height / 16, 0, source.format);
                Graphics.Blit(source, blurTex, material, 0);
                Graphics.Blit(blurTex, temp1, material, 0);
                Graphics.Blit(temp1, temp2, material, 0);
                Graphics.Blit(temp2, temp1, material, 0);
                Graphics.Blit(temp1, blurTex, material, 0);
                RenderTexture.ReleaseTemporary(temp1);
                RenderTexture.ReleaseTemporary(temp2);
            }
            material.SetTexture(blurTexString, blurTex);
            RenderTexture.ReleaseTemporary(blurTex);
        }
        else
        {
            material.SetTexture(blurTexString, source);
        }

        Graphics.Blit(source, destination, material, 1);
    }
}
