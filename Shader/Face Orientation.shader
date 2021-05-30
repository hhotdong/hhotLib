// 출처: https://docs.unity3d.com/Manual/SL-ShaderSemantics.html
Shader "Unlit/Face Orientation"
{
    Properties
    {
        _ColorFront ("Front Color", Color) = (1,0.7,0.7,1)
        _ColorBack ("Back Color", Color) = (0.7,1,0.7,1)
    }
    SubShader
    {
        Pass
        {
            Cull Off // turn off backface culling

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0  // This feature only exists from shader model 3.0 onwards

            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 _ColorFront;
            fixed4 _ColorBack;

            fixed4 frag (fixed facing : VFACE) : SV_Target
            {
                // VFACE input positive for frontbaces,
                // negative for backfaces. Output one
                // of the two colors depending on that.
                return facing > 0 ? _ColorFront : _ColorBack;
            }
            ENDCG
        }
    }
}