Shader "Voxel Importer/Explosion/VoxelExplosion-Transparent" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[PerRendererData] _ExplosionRate ("ExplosionRate", Range(0,1)) = 0.0
		[PerRendererData] _ExplosionCenter ("ExplosionCenter", Vector) = (0,0,0,1)
		[PerRendererData] _ExplosionRotate("ExplosionRotate", Float) = 0.5
	}
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "VoxelExplosion.cginc"

		void vert(inout appdata_full v)
		{
			transform(v);
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		struct Input 
		{
			float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color * IN.color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		//========================================================================================================
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "VoxelExplosion.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_full v)
			{
				transform(v);

				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}
			
			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
