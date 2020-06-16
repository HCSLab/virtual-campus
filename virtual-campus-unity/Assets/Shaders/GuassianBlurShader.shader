Shader "PostEffect/GuassianBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurSize ("BlurSize", Float) = 1 
    }

    SubShader
    {
        CGINCLUDE
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		float _BlurSize;


		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		v2f vertBlurVertical(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			half2 uv = v.texcoord;
			float ps_y = _MainTex_TexelSize.y;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(0, ps_y) * _BlurSize;
			o.uv[2] = uv - float2(0, ps_y) * _BlurSize;
			o.uv[3] = uv + float2(0, ps_y * 2) * _BlurSize;
			o.uv[4] = uv - float2(0, ps_y * 2) * _BlurSize;

			return o;
		}

		v2f vertBlurHorizontal(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			half2 uv = v.texcoord;
			float ps_x = _MainTex_TexelSize.x;
			float ps_y = _MainTex_TexelSize.y;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(ps_x, 0) * _BlurSize;
			o.uv[2] = uv - float2(ps_x, 0) * _BlurSize;
			o.uv[3] = uv + float2(ps_x * 2, 0) * _BlurSize;
			o.uv[4] = uv - float2(ps_x * 2, 0) * _BlurSize;

			return o;
		}

		fixed4 fragBlur(v2f i) : SV_Target
		{
			float weight[3] = { 0.4026, 0.2442, 0.0545 };

			fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
			for (int p = 1; p < 3; p++)
			{
				sum += tex2D(_MainTex, i.uv[p * 2 - 1]).rgb * weight[p];
				sum += tex2D(_MainTex, i.uv[p * 2]).rgb * weight[p];
			}

			return fixed4(sum, 1.0);
		}

		ENDCG

		ZTest Always
		ZWrite Off
		Cull Off

        Pass
        {
			NAME "GUASSIAN_BLUR_VERTICAL"

            CGPROGRAM
			#pragma vertex vertBlurVertical
			#pragma fragment fragBlur
            ENDCG
        }

		Pass
		{
			NAME "GUASSIAN_BLUR_HORIZONTAL"

            CGPROGRAM
			#pragma vertex vertBlurHorizontal
			#pragma fragment fragBlur
            ENDCG
		}
    }

	Fallback Off
}
