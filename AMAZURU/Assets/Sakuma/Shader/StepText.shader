Shader "Custom/StepText"
{
  	Properties
	{
		_Color ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_ScrollX ("Scroll X", float) = 0
		_ScrollY ("Scroll Y", float) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		
		ZWrite Off
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
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScrollX;
			float _ScrollY;
			
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.x = o.texcoord.x +_ScrollX * _Time.y;
				o.texcoord.y = o.texcoord.y +_ScrollY * _Time.y;
				o.color = 2.0f * v.color;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i): SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, (fixed4)0);
				col.a=_Color.a;
				return col;
			}
			ENDCG
			
		}
	}
	
	Fallback off
}
