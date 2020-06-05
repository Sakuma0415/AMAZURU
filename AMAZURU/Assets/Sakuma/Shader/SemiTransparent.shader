Shader "SemiTransparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Fade ("Fade", float) = 0
		_NormalTex("_NormalTex", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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
		half _Glossiness;
        half _Metallic;
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
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a*_Fade;
			o.Normal=UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
		}
		ENDCG
	}
	FallBack "Diffuse"
}