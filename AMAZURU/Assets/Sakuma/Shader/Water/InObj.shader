// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Custom/InObj"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_LightMap ("LightMap", 2D) = "white" {}
		
		_InColor ("InColor", Color) = (1, 1, 1, 1)
		[Header(Script Check)]
		_High("_High", float) = 0
		_Xside("_Xside", float) = 0
		_Zside("_Zside", float) = 0
		_NormalTex("_NormalTex", 2D) = "white" {}

		_LightVec("_LightVec", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		#include "UnityCG.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		float _High;
		float _Xside;
		float _Zside;
		sampler2D _LightMap;
		sampler2D _NormalTex;
		fixed4 _InColor;
		float4 _LightVec;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float3 L = normalize(_LightVec.xyz);
			if(_High>IN.worldPos.y&&(_Xside>IN.worldPos.x&&IN.worldPos.x>0.01f)&&(_Zside>IN.worldPos.z&&IN.worldPos.z>0.01f)){

				float hi=_High-IN.worldPos.y;
				float size=3;
				float2 wuv=float2((((hi*L.x)/L.y)+IN.worldPos.x-_Time.x)/size,(((hi*L.z)/L.y)+IN.worldPos.z-_Time.x)/size);
				wuv.x-=(int)wuv.x;
				wuv.y-=(int)wuv.y;
				fixed4 finalColor2 = tex2D(_LightMap, wuv);
				c.rbg *= (1+(pow( finalColor2.a,1)/4));

				c.rgb-=_InColor.rgb*(hi/50);
				if(hi<0.2){
				c.rbg+=(1-(hi*5))*float3(1,1,1)/2;
				}
			}
			
			o.Normal=UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}