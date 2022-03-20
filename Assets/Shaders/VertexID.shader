// 출처: https://docs.unity3d.com/Manual/SL-ShaderSemantics.html
// A vertex shader can receive a variable that has the “vertex number” as an unsigned integer.
// This is mostly useful when you want to fetch additional per-vertex data from textures or ComputeBuffers.
// This feature only exists from DX10 (shader model 4.0) and GLCore / OpenGL ES 3, so the shader needs to have the #pragma target 3.5 compilation directive.

Shader "Unlit/VertexID"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            struct v2f {
                fixed4 color : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (
                float4 vertex : POSITION, // vertex position input
                uint vid : SV_VertexID // vertex ID, needs to be uint
                )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                // output funky colors based on vertex ID
                float f = (float)vid;
                o.color = half4(sin(f/10),sin(f/100),sin(f/1000),0) * 0.5 + 0.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}