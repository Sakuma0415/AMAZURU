Shader "SemiTransparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Fade ("Fade", float) = 0
		_NormalTex("_NormalTex", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200

		Pass{
  		  ZWrite ON
  		  ColorMask 0
		}

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		float _Fade;
		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = _Fade;
			o.Normal=UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
		}
		ENDCG
	}
	FallBack "Diffuse"
}