Shader "H5/Sprite"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
		_CutoffColor("Cutoff Color", Color) = (1, 1, 1, 1)
		_SetUV("Set UV", VECTOR) = (0, 0, 0, 0)
		_TestUV("Test UV", VECTOR) = (1, 1, 1, 1)
		_Mirror("Mirror", Int) = 0
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
	fixed4 _CutoffColor;
	VECTOR _SetUV;
	VECTOR _TestUV;
	bool _Mirror;

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
		if (_Mirror & 1)
			i.uv.x = 1 - i.uv.x;

		i.uv *= fixed2(_SetUV.x, _SetUV.y);
		i.uv += fixed2(_SetUV.z, _SetUV.w);

		if (i.uv.x < _TestUV.x || i.uv.x > _TestUV.y || i.uv.y < _TestUV.z || i.uv.y > _TestUV.w)
			discard;

		fixed4 col = tex2D(_MainTex, i.uv)/* * i.color*/;

		if (abs(col.r - _CutoffColor.r) < 0.01
			&& abs(col.g - _CutoffColor.g) < 0.01
			&& abs(col.b - _CutoffColor.b) < 0.01)
			discard;

		return col;
	}

		ENDCG
	}
	}
}
