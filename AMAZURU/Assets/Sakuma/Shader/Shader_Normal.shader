Shader "Custom/Shader_Normal" {
	Properties {
		_Color      ("Color"       , Color      ) = (1, 1, 1, 1)
		_MainTex    ("Albedo (RGB)", 2D         ) = "white" {}
		_Glossiness ("Smoothness"  , Range(0, 1)) = 0.5
		_Metallic   ("Metallic"    , Range(0, 1)) = 0.0

		_BumpMap    ("Normal Map"  , 2D         ) = "bump" {}
		_BumpScale  ("Normal Scale", Range(0, 1)) = 1.0
	}

	SubShader {
		Tags {
			"Queue"      = "Geometry"
			"RenderType" = "Opaque"
		}

		LOD 200
		
		CGPROGRAM
			#pragma target 3.0
			#pragma surface surf Standard fullforwardshadows

			fixed4 _Color;
			sampler2D _MainTex;
			half _Glossiness;
			half _Metallic;

			sampler2D _BumpMap;
			half _BumpScale;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

				o.Albedo     = c.rgb;
				o.Metallic   = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha      = c.a;
				
				fixed4 n = tex2D(_BumpMap, IN.uv_MainTex);
				
				o.Normal = UnpackScaleNormal(n, _BumpScale);
			}
		ENDCG
	}

	FallBack "Diffuse"
}