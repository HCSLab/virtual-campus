Shader "PostEffect/Outline"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _OutlineTex ("Edge Tex", 2D) = "black" {}
        _CullTex ("Cull Tex", 2D) = "black" {}
        _Color ("Outline Color", Color) = (1, 0, 0, 0)
        _BlurSize ("Blur Size", Float) = 1
        _Threshold ("Outline Threshold", Float) = 0
        _HardEdge ("Is Hard Edge", Float) = 0
        _Intensity ("Intensity Scaler", Float) = 1
    }

    SubShader
    {
        // draw mesh outline
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 0, 0, 1);
            }
            ENDCG
        }

        UsePass "PostEffect/GuassianBlur/GUASSIAN_BLUR_VERTICAL"
        UsePass "PostEffect/GuassianBlur/GUASSIAN_BLUR_HORIZONTAL" 

        // blend outline with original image
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed3 _Color;
            fixed _Threshold;
            fixed _HardEdge;
            fixed _Intensity;
            sampler2D _MainTex;
            sampler2D _OutlineTex;
            sampler2D _CullTex;

            v2f vert (a2v v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed cull = tex2D(_CullTex, i.uv).r;
                fixed outline = tex2D(_OutlineTex, i.uv).r;
                fixed3 color = tex2D(_MainTex, i.uv).rgb;

                if (cull) {
                    return fixed4(color, 1);
                }

                fixed a = outline > _Threshold ? (_HardEdge > 0 ? 1 : outline * _Intensity) : 0;
                return fixed4(color * (1 - a) + _Color * a, 1);
            }
            ENDCG
        }
    }

    Fallback Off
}
