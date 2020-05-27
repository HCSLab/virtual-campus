Shader "PostEffect/BrightnessSaturationAndContrast"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
		_Brightness ("Brightness", Float) = 1
		_Saturation ("Saturation", Float) = 1
		_Contrast ("Contrast", Float) = 1
    }

    SubShader
    {
        Pass
        {
			ZTest Always
			ZWrite Off
			Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Brightness;
			float _Saturation;
			float _Contrast;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 src = tex2D(_MainTex, i.uv);

				fixed3 res = src.rgb * _Brightness;

				fixed luminance = 0.2125 * src.r + 0.7154 * src.g + 0.0721 * src.b;
				fixed3 luminanceCol = fixed3(luminance, luminance, luminance);
				res = lerp(luminanceCol, res, _Saturation);

				fixed3 avgCol = fixed3(0.5, 0.5, 0.5);
				res = lerp(avgCol, res, _Contrast);

				return fixed4(res, src.a);
            }
            ENDCG
        }
    }

	Fallback Off
}
