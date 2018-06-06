﻿Shader "H5/Tile"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
	_Color("Color", Color) = (0, 0, 0, 0)
		_Neighbor("Neighbor", Int) = 0
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMaterial AmbientAndDiffuse
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Color;
	int _Neighbor;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = /*tex2D(_MainTex, i.uv) * */_Color;

		if (_Neighbor & 32)
		{
			col.a = 0.2;

			if (_Neighbor & 2 && i.uv.y >= 0.9
				|| _Neighbor & 4 && i.uv.y <= 0.1
				|| _Neighbor & 8 && i.uv.x <= 0.1
				|| _Neighbor & 16 && i.uv.x >= 0.9)
				col.a = 0.5;
		}
		else
		{
			col.a = 0;
		}

		return col;
	}
		ENDCG
	}
	}
}
