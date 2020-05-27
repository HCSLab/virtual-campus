Shader "Posteffect/DepthOfField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZTest Always
        ZWrite Off
        Cull Off

        CGINCLUDE
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		float _MaxBlurSize;
        float2 _BlurRange;
        float _PowOfDist;
        sampler2D _CameraDepthNormalsTexture;
        static const float weight[3] = { 0.4026, 0.2442, 0.0545 };

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

        inline float calculateBlurSize(float2 uv)
        {
            float depth01 = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, uv).zw);
            if (1 - depth01 < 1e-9) return 0;
            float depth = clamp(depth01 * _ProjectionParams.z, _BlurRange.x, _BlurRange.y);
            return pow((depth - _BlurRange.x) / (_BlurRange.y - _BlurRange.x), _PowOfDist) * _MaxBlurSize;
        }

		v2f vertBlur(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		fixed4 fragBlurHorizontal(v2f i) : SV_Target
		{
            // return tex2D(_MainTex, i.uv);
			float blurSize = calculateBlurSize(i.uv);
            // if (blurSize == _MaxBlurSize) return fixed4(0, 0, 1, 1);
            // if (blurSize == 0) return fixed4(0, 1, 0, 1);
			if (blurSize < 0.2) return tex2D(_MainTex, i.uv);

			fixed3 sum = tex2D(_MainTex, i.uv).rgb * weight[0];
			for (int p = 1; p <= 2 ; p++)
			{
                float2 offset = float2(_MainTex_TexelSize.x * blurSize * p, 0);
				sum += tex2D(_MainTex, i.uv + offset).rgb * weight[p];
				sum += tex2D(_MainTex, i.uv - offset).rgb * weight[p];
			}
			return fixed4(sum, 1);
		}

        fixed4 fragBlurVertical(v2f i) : SV_Target
		{
            // return tex2D(_MainTex, i.uv);
            float blurSize = calculateBlurSize(i.uv);
            // if (blurSize == _MaxBlurSize) return fixed4(0, 0, 1, 1);
            // if (blurSize == 0) return fixed4(0, 1, 0, 1);
            if (blurSize < 0.2) return tex2D(_MainTex, i.uv);

			fixed3 sum = tex2D(_MainTex, i.uv).rgb * weight[0];
			for (int p = 1; p <= 2; p++)
			{
                float2 offset = float2(0, _MainTex_TexelSize.y * blurSize * p) ;
				sum += tex2D(_MainTex, i.uv + offset).rgb * weight[p];
				sum += tex2D(_MainTex, i.uv - offset).rgb * weight[p];
			}
			return fixed4(sum, 1);
		}
		ENDCG

        Pass
        {
			NAME "GUASSIAN_BLUR_VERTICAL"

            CGPROGRAM
			#pragma vertex vertBlur
			#pragma fragment fragBlurVertical
            ENDCG
        }

		Pass
		{
			NAME "GUASSIAN_BLUR_HORIZONTAL"

            CGPROGRAM
			#pragma vertex vertBlur
			#pragma fragment fragBlurHorizontal
            ENDCG
		}
    }

    Fallback Off
}
