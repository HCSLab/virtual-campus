Shader "Unlit/EdgeDetectionShader"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
		_EdgeOnly ("EdgeOnly", Float) = 1 
		_EdgeColor ("EdgeColor", Color) = (0, 0, 0, 1)
		_BackgroundColor ("BackgroundColor", Color) = (1, 1, 1, 1)
		_Pow ("PowOfEdge", Float) = 1
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
                half2 uv[9] : TEXCOORD0;  // ???
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
			float _EdgeOnly;
			fixed4 _EdgeColor;
			fixed4 _BackgroundColor;
			fixed _Pow;

			fixed lumiance(fixed4 color)
			{
				return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
			}

			half Sobel(v2f i)
			{
				const half Gx[9] = { -1, -2, -1,
									  0,  0,  0,
									  1,  2,  1  };
				const half Gy[9] = { -1,  0,  1,
									 -2,  0,  2,
									 -1,  0,  1  };

				half texCol;
				half edgeX = 0;
				half edgeY = 0;
				for (int p = 0; p < 9; p++)
				{
					texCol = lumiance(tex2D(_MainTex, i.uv[p]));
					edgeX += texCol * Gx[p];
					edgeY += texCol * Gy[p];
				}

				return abs(edgeX) + abs(edgeY);
			}

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
				half2 uv = v.texcoord;

				o.uv[0] = uv + _MainTex_TexelSize * half2(-1, -1);
				o.uv[1] = uv + _MainTex_TexelSize * half2( 0, -1);
				o.uv[2] = uv + _MainTex_TexelSize * half2( 1, -1);
				o.uv[3] = uv + _MainTex_TexelSize * half2(-1,  0);
				o.uv[4] = uv + _MainTex_TexelSize * half2( 0,  0);
				o.uv[5] = uv + _MainTex_TexelSize * half2( 1,  0);
				o.uv[6] = uv + _MainTex_TexelSize * half2(-1,  1);
				o.uv[7] = uv + _MainTex_TexelSize * half2( 0,  1);
				o.uv[8] = uv + _MainTex_TexelSize * half2( 1,  1);

				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				// the bigger the value is, the more likely it is an edge
                half edge = Sobel(i);
				if (_Pow > 1.01 || _Pow < 0.99) edge = pow(edge, _Pow);

				fixed4 withEdgeCol = lerp(tex2D(_MainTex, i.uv[4]), _EdgeColor, edge);
				fixed4 onlyEdgeCol = lerp(_BackgroundColor, _EdgeColor, edge);
				return lerp(withEdgeCol, onlyEdgeCol, _EdgeOnly);
            }
            ENDCG
        }
    }

	Fallback Off
}
