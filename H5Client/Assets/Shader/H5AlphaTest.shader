Shader "H5/AlphaTest"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.9
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
		fixed4 col = tex2D(_MainTex, i.uv) * i.color;
	if (col.a < _Cutoff)
		discard;
	return col;
	}

		ENDCG
	}
	}
}
