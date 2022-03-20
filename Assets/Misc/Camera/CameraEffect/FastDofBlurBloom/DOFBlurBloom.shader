Shader "SupGames/DepthBlurBloom"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_BlurTex);
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	
	fixed4 _MainTex_TexelSize;
	uniform fixed4 _BloomColor;
	uniform fixed _BlurAmount;
	uniform fixed4 _BloomData;
	fixed _Focus;
	fixed _Aperture;


	struct appdata
	{
		fixed4 pos : POSITION;
		fixed2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		fixed4 pos : SV_POSITION;
		fixed2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2fb
	{
		fixed4 pos : SV_POSITION;
		fixed4  uv : TEXCOORD0;
		fixed2  uv1 : TEXCOORD1;
		fixed4  uv2 : TEXCOORD2;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2fb vertBlur(appdata i)
	{
		v2fb o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2fb, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		fixed2 offset = _MainTex_TexelSize.xy * _BlurAmount;
		o.uv = fixed4(i.uv - offset, i.uv + offset);
		o.uv1 = i.uv;
		o.uv2 = fixed4(-offset, offset);
		return o;
	}

	v2f vert(appdata i)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		return o;
	}

	fixed4 fragBlur(v2fb i) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
#ifdef ISDEPTH
		fixed4 result = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xw);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zy);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw);
#else
#ifdef ISALPHA
		fixed a1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy).a;
		fixed a2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xw).a;
		fixed a3 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zy).a;
		fixed a4 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw).a;
		fixed4 result = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1 + i.uv2.xy * a1);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1 + i.uv2.xw * a2);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1 + i.uv2.zy * a3);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1 + i.uv2.zw * a4);
#else
		fixed4 result = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xw);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zy);
		result += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw);
#endif
#endif
		return result * 0.25h;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		fixed4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		fixed4 b = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BlurTex, i.uv);
		fixed br = max(b.r, max(b.g, b.b));
		fixed soft = clamp(br - _BloomData.y, 0.0h, _BloomData.z);
	    fixed a = max(soft * soft * _BloomData.w, br - _BloomData.x) / max(br, 0.00001h) * _BloomColor;
#ifdef ISDEPTH
		fixed depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
		return lerp(c, b, saturate(abs((1.0h - clamp(depth / _Focus, 0.0h, 2.0h)) * _Aperture))) + b * 0.00001h * _BloomColor * a;
#endif
		return lerp(c, b, min(c.a, b.a)) + b * _BloomColor * a;
	}
	ENDCG
		
	SubShader
	{		
		Pass //0
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma vertex vertBlur
			#pragma fragment fragBlur
			#pragma shader_feature_local ISDEPTH
			#pragma shader_feature_local ISALPHA
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}

		Pass //1
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local ISDEPTH
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
}
