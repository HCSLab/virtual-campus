// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Highlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "black" {}
		_OccludeMap ("Occlusion Map", 2D) = "black" {}
	}
	
	SubShader {

		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		
		
		// Glow
		
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OccludeMap;
				float4 _MainTex_TexelSize;

				fixed4 _Color;
			
				fixed4 frag(v2f_img IN) : COLOR 
				{
					float2 uv = IN.uv;
#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
#endif
					fixed3 overCol = tex2D(_OccludeMap, uv).r * _Color.rgb * _Color.a;
					return tex2D (_MainTex, IN.uv) + fixed4(overCol, 1.0);
				}
			ENDCG
		}
		
		// Occclusion
		
		Pass {
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OccludeMap;
			
				fixed4 frag(v2f_img IN): COLOR
				{
					return tex2D (_MainTex, IN.uv).r - tex2D(_OccludeMap, IN.uv).r;
				}
			ENDCG
		}
		
		// Overlay mode

		Pass {
           	Tags {"RenderType"="Opaque"}
        	ZWrite On
        	ZTest LEqual
        	Fog { Mode Off }
        
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 vert(float4 v:POSITION) : POSITION {
                return UnityObjectToClipPos (v);
            }

            fixed4 frag() : COLOR {
                return 1.0;
            }

            ENDCG
        }	
    	
		// Depth buffer mode

        Pass {        	
           	Tags {"Queue"="Transparent"}
            Cull Back
            Lighting Off
            ZWrite Off
            ZTest LEqual
            ColorMask RGBA
            Blend OneMinusDstColor One

        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthTexture;
            
            struct v2f {
                float4 vertex : POSITION;
                float4 projPos : TEXCOORD1;
            };
     
            v2f vert( float4 v : POSITION ) {        
                v2f o;
                o.vertex = UnityObjectToClipPos( v );
                o.projPos = ComputeScreenPos(o.vertex);             
                return o;
            }

			fixed4 _Color;

            fixed4 frag( v2f i ) : COLOR {          
                float depthVal = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
                float zPos = i.projPos.z;
                
                return step( zPos, depthVal ) * _Color.a;
            }
            ENDCG
        }
	} 
}
