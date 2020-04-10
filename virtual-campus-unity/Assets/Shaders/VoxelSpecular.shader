Shader "Voxel/Specular" {

	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
        _PropTex ("Prop Tex", 2D) = "white" {}
	}


	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}


        CGINCLUDE

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
                float4 texcoord : TEXCOORD0;
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

        ENDCG

		
		Pass { 
			Tags { "LightMode"="ForwardBase" }
		
			CGPROGRAM
			
			#pragma multi_compile_fwdbase	
			
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target {
                fixed colorScaler = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).r;
                fixed3 specularCol = tex2D(_PropTex, fixed2(i.uv.x, propUV_y0)).rgb;
                fixed gloss = tex2D(_PropTex, fixed2(i.uv.x, propUV_y1)).g;

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb * colorScaler;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				
			 	fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(i.worldNormal, lightDir));
			 	
			 	fixed3 halfDir = normalize(lightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * specularCol * pow(max(0, dot(i.worldNormal, halfDir)), gloss);
			
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				return fixed4(ambient + (diffuse + specular) * atten, 1.0);
			}
			
			ENDCG
		}

		
		Pass { 
			Tags { "LightMode"="ForwardAdd" }
			
			Blend One One
		
			CGPROGRAM
			
			#pragma multi_compile_fwdadd_fullshadows
			
			#pragma vertex vert
			#pragma fragment frag
			
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
