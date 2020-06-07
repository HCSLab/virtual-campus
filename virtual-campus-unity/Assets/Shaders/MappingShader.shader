// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/MappingShader"
{
	Properties
	{
	_MainTex("Texture", 2D) = "white" {}
	_UA("旋转中点x",Float) = 0.5
	_UB("旋转中点y",Float) = 0.5
	_CenterX("平移x",float) = 0
	_CenterY("平移y",float) = 0
	_Scale("缩放",Float) = 10
	_RotateNum("旋转角度",Range(-360,360)) = 0
	}
		SubShader
	{
	Tags { "RenderType" = "Opaque" }
	LOD 100
	Pass
	{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	sampler2D _MainTex; float4 _MainTex_ST;
	float _UA;
	float _UB;
	float _RA;
	float _Scale;
	float _RotateNum;
	float _CenterX;
	float _CenterY;

	struct v2f
	{
	float2 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
	};

	v2f vert(appdata_full v)
	{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
	return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//计算旋转的坐标
	 float Rote = (_RotateNum * 3.1415926) / 180;
	 float sinNum = sin(Rote);
	 float cosNum = cos(Rote);
			 float2 di = float2(_UA,_UB);
			 //计算平移之后的坐标，需要乘平移矩阵   
			 float2 uv = mul(float3(i.uv - di,1),float3x3(1,0,0,0,1,0,_CenterX,_CenterY,1)).xy;
			 //计算缩放之后的坐标，需要乘缩放矩阵
			 uv = mul(uv,float2x2(_Scale,0,0,_Scale));
			 //计算旋转之后的坐标，需要乘旋转矩阵
			 uv = mul(uv,float2x2(cosNum,-sinNum,sinNum,cosNum)) + di;
			 fixed4 col;
			 //用最终的坐标来采样当前的纹理，就ok了
			 col = tex2D(_MainTex,TRANSFORM_TEX(uv,_MainTex));;
			 return col;
			 }
			 ENDCG
			 }
	}
}

