Shader "SupGames/DOFBlurBloom/Specular"{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Glossiness("Glossiness", Range(0,1)) = 0.1
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "UnityCG.cginc" 
			#include "AutoLight.cginc"

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			fixed _Glossiness;
			fixed4 _MainTex_ST;
			fixed _Focus;
			fixed _Aperture;
			fixed4 _Color;
			fixed4 _LightColor0;
			fixed4 _SpecColor;

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed2 uv : TEXCOORD0;
#ifdef LIGHTMAP_ON
				fixed2 luv : TEXCOORD1;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct v2f {
				fixed4 pos : SV_POSITION;
#ifdef LIGHTMAP_ON
				fixed3 uv : TEXCOORD0;
				fixed3 fogCoord : TEXCOORD1;
#else
				fixed3 uv : TEXCOORD0;
				fixed4 diff : TEXCOORD1;
				fixed4 fogCoord : TEXTCOORD2;
				SHADOW_COORDS(3)
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv,_MainTex);
				o.uv.z = saturate(abs((1.0h - clamp(-UnityObjectToViewPos(v.vertex).z / _Focus, 0.0h, 2.0h)) * _Aperture));
#ifdef LIGHTMAP_ON
				o.fogCoord.yz = v.luv * unity_LightmapST.xy + unity_LightmapST.zw;
#else
				fixed3 normalDirection = UnityObjectToWorldNormal(v.normal);
				o.diff = fixed4(_LightColor0.rgb * max(0.0h, dot(normalDirection, _WorldSpaceLightPos0.xyz)), saturate(dot(normalDirection, normalize(_WorldSpaceLightPos0.xyz + normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz)))));
				o.fogCoord.yzw = ShadeSH9(fixed4(normalDirection, 1.0h));
				TRANSFER_SHADOW(o);
#endif
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 color = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
#ifdef LIGHTMAP_ON
				color.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.fogCoord.yz)) * _Color.rgb;
#else
				fixed3 spec = _LightColor0.rgb * _SpecColor.rgb * pow(i.diff.w, exp2(10.0h * _Glossiness * color.a + 1.0h));
				color.rgb = spec + (i.diff.xyz + i.fogCoord.yzw) * color.rgb * _Color.rgb * SHADOW_ATTENUATION(i);
#endif
				UNITY_APPLY_FOG(i.fogCoord, color);
				return fixed4(color.rgb, i.uv.z);
			}
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}