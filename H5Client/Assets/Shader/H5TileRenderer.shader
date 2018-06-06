Shader "H5/TileRenderer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.9
		_UVStartPos("UV Value", VECTOR) = (0, 0, 0, 0)
		_Neighbor("Neighbor", Int) = 0
		_CutoffColor("Cutoff Color", Color) = (1, 1, 1, 1)
	}

		SubShader
	{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "true" }
		//Tags{ "RenderType" = "Opaque" }
		ZWrite On
		//Blend SrcAlpha OneMinusSrcAlpha
		//AlphaTest Greater[_Cutoff]
		//ColorMaterial AmbientAndDiffuse
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
	float _Cutoff;
	VECTOR _UVPos;
	int _Neighbor;
	fixed4 _CutoffColor;

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
		float2 originuv = i.uv;
		i.uv += fixed2(_UVPos.z, _UVPos.w);
		i.uv *= fixed2(_UVPos.x, _UVPos.y);

		fixed4 col = tex2D(_MainTex, i.uv)/* * i.color*/;

		//if (col.a < _Cutoff
		//	 || (abs(col.r - _CutoffColor.r) < 0.01
		//		 && abs(col.g - _CutoffColor.g) < 0.01
		//		 && abs(col.b - _CutoffColor.b) < 0.01))
		if (abs(col.r - _CutoffColor.r) < 0.01
			&& abs(col.g - _CutoffColor.g) < 0.01
			&& abs(col.b - _CutoffColor.b) < 0.01)
			discard;

		//if (_Neighbor & 32)
		//{
		//	float alphaValue = 0.2;

		//	if (_Neighbor & 2 && originuv.y >= 0.9
		//		|| _Neighbor & 4 && originuv.y <= 0.1
		//		|| _Neighbor & 8 && originuv.x <= 0.1
		//		|| _Neighbor & 16 && originuv.x >= 0.9)
		//		alphaValue = 0.5;

		//	col = col * (1 - alphaValue) + fixed4(1, 0, 0, 1) * alphaValue;
		//}

		return col;
	}

		ENDCG
	}
	}
}
