// 출처: https://styly.cc/ja/tips/polygondestruction-go-shader/
Shader "Custom/Color Gradient"
{
    Properties
    {
        _Distance ("Distance", float) = 3.0
        _FarColor ("Far Color", Color) = (0, 0, 0, 1) 
        _NearColor ("Near Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            float _Distance;
            fixed4 _FarColor;
            fixed4 _NearColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // 로컬 좌표계를 월드 좌표계로 변환
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 카메라와 오브젝트의 거리 계산
                float dist = length(_WorldSpaceCameraPos - i.worldPos);
                // 선형보간으로 색상 변화
                fixed4 col = fixed4(lerp(_NearColor.rgb, _FarColor.rgb, dist/_Distance), 1); 
                return col;
            }
            ENDCG
        }
    }
}