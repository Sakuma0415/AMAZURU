﻿Shader "Custom/Test1"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_ScrollX ("Scroll X", float) = 0
		_ScrollY ("Scroll Y", float) = 0
	}
	SubShader
	{
Tags { "RenderType"="Transparent" "Queue"="Transparent+2" }
        LOD 200

        ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex: POSITION;
				float2 texcoord: TEXCOORD0;
				fixed4 color: COLOR;
			};
			
			struct v2f
			{
				float4 vertex: SV_POSITION;
				float2 texcoord: TEXCOORD0;
				fixed4 color: COLOR;
				UNITY_FOG_COORDS(1)
			};
			
			fixed4 _TintColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScrollX;
			float _ScrollY;
			
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.x = o.texcoord.x +_ScrollX;
				o.texcoord.y = o.texcoord.y +_ScrollY;
				o.color = 2.0f * v.color * _TintColor;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i): SV_Target
			{
				fixed4 col = i.color * tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, (fixed4)0);
				return col;
			}
			ENDCG
			
		}
	}
	
	Fallback off
}