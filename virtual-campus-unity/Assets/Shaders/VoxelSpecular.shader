Shader "Voxel/Specular" {

	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
        _PropTex ("Prop Tex", 2D) = "white" {}
	}


	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}


		Pass { 
			Tags { "LightMode"="ForwardBase" }
		
			CGPROGRAM
			
			#pragma multi_compile_fwdbase
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _PropTex;

            static const fixed propUV_y0 = 0.125;
            static const fixed propUV_y1 = 0.375;
            static const fixed propUV_y2 = 0.625;
            static const fixed propUV_y3 = 0.875;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 uv_lm : TEXCOORD1;
				#endif
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                SHADOW_COORDS(4)
				#ifdef LIGHTMAP_ON
					float2 uv_lm : TEXCOORD5;
				#endif
            };
            
            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifndef LIGHTMAP_OFF
					o.uv_lm = v.uv_lm * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);  
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  

                TRANSFER_SHADOW(o);
                
                return o;
            }

			fixed4 frag(v2f i) : SV_Target {
                fixed colorScaler = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).r;
                fixed3 specularCol = tex2D(_PropTex, fixed2(i.uv.x, propUV_y0)).rgb;
                fixed gloss = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).g;
				gloss = 256;

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb * colorScaler;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				
			 	fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));
			 	
			 	fixed3 halfDir = normalize(lightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * specularCol * pow(max(0, dot(i.worldNormal, halfDir)), gloss);

				#ifndef LIGHTMAP_OFF
					fixed3 backed = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv_lm)) * albedo;
				#endif
			
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				#ifndef LIGHTMAP_OFF
					// return fixed4(col_lm / 4, 1);
					return fixed4(ambient + backed + (diffuse + specular) * atten, 1.0);
				#else
					return fixed4(ambient + (diffuse + specular) * atten, 1.0);
				#endif
			}
			
			ENDCG
		}

		
		Pass { 
			Tags { "LightMode"="ForwardAdd" }
			
			Blend One One
		
			CGPROGRAM
			
			#pragma multi_compile_fwdadd
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _PropTex;

            static const fixed propUV_y0 = 0.125;
            static const fixed propUV_y1 = 0.375;
            static const fixed propUV_y2 = 0.625;
            static const fixed propUV_y3 = 0.875;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                fixed2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                SHADOW_COORDS(4)
            };
            
            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);  
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  

                TRANSFER_SHADOW(o);
                
                return o;
            }
			
			fixed4 frag(v2f i) : SV_Target {
                fixed colorScaler = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).r;
                fixed3 specularCol = tex2D(_PropTex, fixed2(i.uv.x, propUV_y0)).rgb;
                fixed gloss = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).g;

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb * colorScaler;
				
			 	fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));
			 	
			 	fixed3 halfDir = normalize(lightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * specularCol * pow(max(0, dot(i.worldNormal, halfDir)), gloss);
			
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4((diffuse + specular) * atten, 1.0);
			}
			
			ENDCG
		}
	}


	FallBack "Specular"
}
