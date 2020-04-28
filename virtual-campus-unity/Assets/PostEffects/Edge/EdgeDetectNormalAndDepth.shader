Shader "PostEffect/EdgeDetectNormalAndDepth"
{
    Properties
    {
        _MainTex ("Bace (RGB)", 2D) = "white" {}
		_EdgeOnly ("EdgeOnly", Float) = 1
		_EdgeColor ("EdgeColor", Color) = (0, 0, 0, 1)
		_BackgroundColor ("BackgroundColor", Color) = (1, 1, 1, 1)
		_SampleDistance ("SampleDistance", Float) = 1
		_SensitivityDepth ("SensitivityDepth", Float) = 1
		_SensitivityNormal ("SensitivityNormal", Float) = 1
    }

    SubShader
    {
        CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		fixed _EdgeOnly;
		fixed4 _EdgeColor;
		fixed4 _BackgroundColor;
		float _SampleDistance;
		half _SensitivityDepth;
		half _SensitivityNormal;
		sampler2D _CameraDepthNormalsTexture;

		struct v2f 
		{
			float4 pos : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		v2f vert(appdata_img v) 
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			half2 uv = v.texcoord;
			o.uv[0] = uv;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
			{
				uv.y = 1 - uv.y;
			}
			#endif

			o.uv[1] = uv + _MainTex_TexelSize.xy * half2(-1, 1) * _SampleDistance;
			o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, 1) * _SampleDistance;
			o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, -1) * _SampleDistance;
			o.uv[4] = uv + _MainTex_TexelSize.xy * half2(1, -1) * _SampleDistance;

			return o;
		}

		half CheckSame(half4 center, half4 sample)
		{
			half2 centerNormal = center.xy;
			float centerDepth = DecodeFloatRG(center.zw);
			half2 sampleNormal = sample.xy;
			float sampleDepth = DecodeFloatRG(sample.zw);

			half2 diffNormal = abs(centerNormal - sampleNormal) * _SensitivityNormal;
			int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1;

			float diffDepth = abs(centerDepth - sampleDepth) * _SensitivityDepth;
			int isSameDepth = diffDepth < 0.1 * centerDepth;

			return isSameNormal * isSameDepth;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
			half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
			half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
			half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4]);

			half edge = 1;
			edge *= CheckSame(sample1, sample4);
			edge *= CheckSame(sample2, sample3);

			fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
			fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);

			return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
			// float depth;
			// float3 normal;
			// DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv[0]), depth, normal);
			// depth *= _ProjectionParams.z;
			// return fixed4(depth, depth, depth, 1);
		}

		ENDCG


        Pass
        {
			ZTest Always
			Cull Off
			ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
	
	Fallback Off
}
