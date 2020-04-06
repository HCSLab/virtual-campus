Shader "PostEffect/Bloom"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Bloom ("Bloom", 2D) = "balck" {}
		_LuminanceThreshold ("LuminanceThreshold", Float) = 0.5
		_BloomAmount ("BloomAmount", Float) = 0.5
    }

    SubShader
    {
        CGINCLUDE
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		sampler2D _Bloom;
		float4 _Bloom_TexelSize;
		float _LuminanceThreshold;
		float _BloomAmount;

		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		struct v2fBloom
		{
			float4 pos : SV_POSITION;
			half4 uv : TEXCOORD;
		};

		v2f vertExtractBright(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		fixed lumiance(fixed4 color)
		{
			return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
		}

		fixed4 fragExtractBright(v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.uv);
			fixed val = clamp(lumiance(col) - _LuminanceThreshold, 0, 1);
			return col * val;
		}

		v2fBloom vertBloom(appdata_img v)
		{
			v2fBloom o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = v.texcoord;
			o.uv.zw = v.texcoord;
			
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv.w = 1 - o.uv.w;
			#endif

			return o;
		}

		fixed4 fragBloom(v2fBloom i) : SV_Target
		{
			return tex2D(_MainTex, i.uv.xy) + tex2D(_Bloom, i.uv.zw) * _BloomAmount;
		}

		ENDCG

		ZTest Always
		ZWrite Off
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vertExtractBright
			#pragma fragment fragExtractBright
			ENDCG
		}

		UsePass "PostEffect/GuassianBlur/GUASSIAN_BLUR_VERTICAL"
        UsePass "PostEffect/GuassianBlur/GUASSIAN_BLUR_HORIZONTAL"

		Pass
		{
			CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			ENDCG
		}
    }

	Fallback Off
}
