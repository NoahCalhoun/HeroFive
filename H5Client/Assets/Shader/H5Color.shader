Shader "H5/Color"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
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
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
	};

	fixed4 _Color;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return i.color;
	}

		ENDCG
	}
	}
}
