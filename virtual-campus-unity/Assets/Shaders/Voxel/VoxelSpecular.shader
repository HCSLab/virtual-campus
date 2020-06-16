Shader "Voxel/Specular" {

	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
        _Specular ("Specular Intensity", Float) = 0.1
        _Gloss ("Gloss", Float) = 256
        [HDR]_Emission ("Emission", Color) = (0, 0, 0, 0)
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _Lightmap ("Lightmap Intensity", Float) = 1
        _Ambient ("Ambient Intensity", Float) = 1
        _NoiseIntensity ("Noise Intensity", Float) = 0.1
        _NoiseScale ("Noise Scale", Float) = 100
	}


	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "CustomType"="Voxel" }

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
            fixed _Specular;
            fixed _Gloss;
            fixed3 _Emission;
            fixed _Lightmap;
            fixed _Ambient;
            float _NoiseIntensity;
            float _NoiseScale;
            sampler2D _NoiseTex;
            
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

            float noise(float seed) {
                return frac(sin(seed * 2001.0403) * 2333.2333);
            }
            
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
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo * _Ambient;
				
			 	fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));
			 	
			 	fixed3 halfDir = normalize(lightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(i.worldNormal, halfDir)), _Gloss);

				#ifndef LIGHTMAP_OFF
					fixed3 backed = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv_lm)) * albedo * _Lightmap;
				#endif
			
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                // fixed3 noiseCol = tex2D(_NoiseTex, i.worldPos.xy * _NoiseScale + albedo.rg).rgb + 
                //                   tex2D(_NoiseTex, i.worldPos.yz * _NoiseScale + albedo.rg).rgb + 
                //                   tex2D(_NoiseTex, i.worldPos.zx * _NoiseScale + albedo.rg).rgb;
                // noiseCol = (noiseCol - fixed3(1, 1, 1)) * (0.333 * _NoiseIntensity) * albedo;

				#ifndef LIGHTMAP_OFF
					// return fixed4(col_lm / 4, 1);
					return fixed4(ambient + _Emission + backed + (diffuse + specular /*+ noiseCol*/) * atten, 1.0);
				#else
					return fixed4(ambient + _Emission + (diffuse + specular /*+ noiseCol*/) * atten, 1.0);
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
            fixed _Specular;
            fixed _Gloss;
            
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
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
				
			 	fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));
			 	
			 	fixed3 halfDir = normalize(lightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(i.worldNormal, halfDir)), _Gloss);
			
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4((diffuse + specular) * atten, 1.0);
			}
			
			ENDCG
		}

	}

	Fallback "Specular"
}
