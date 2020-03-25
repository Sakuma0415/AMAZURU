Shader "Custom/InObj"
{

	Properties
	{
		[Header(Main)]
		_MainColor ("Main Color", Color) = (0, 0, 0, 1)
		_MainTexture ("Main Texture", 2D) = "white" {}
		_DiffuseShade("Diffuse", Range(0,1)) = 0

		[Header(Specular)]
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SpecularPower ("Specular Power", Range(0.1, 20.0)) = 20.0

		[Header(Script Check)]
		_High("_High", float) = 0
		_LightMap ("LightMap", 2D) = "white" {}
	}

    SubShader
    {
	
        Tags { "RenderType"="Opaque" }


        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight


            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal :NORMAL; 
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexWS : TEXCOORD2;
				float3 normalWS : TEXCOORD3;
				fixed4 diff : COLOR0;
				SHADOW_COORDS(1)
			};


			fixed4 _MainColor;
			sampler2D _MainTexture;
			float4 _MainTexture_ST;
			float _DiffuseShade;
			fixed4 _SpecularColor;
			float _SpecularPower;


			//

			float _High;
			sampler2D _LightMap;



			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;


				o.vertexWS = mul(unity_ObjectToWorld, v.vertex);
				o.normalWS = UnityObjectToWorldNormal(v.normal);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half NdotL = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = NdotL * _LightColor0;


				TRANSFER_SHADOW(o)
				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				float3 L = normalize(-_WorldSpaceLightPos0.xyz);
				float3 N = normalize(i.normalWS);
				fixed4 finalColor = fixed4(0, 0, 0, 1);

				fixed4 shadow = SHADOW_ATTENUATION(i);
                fixed4 shadowBf= i.diff * shadow;

				float2 mainUv = i.uv * _MainTexture_ST.xy + _MainTexture_ST.zw;
				finalColor = tex2D(_MainTexture, mainUv) * _MainColor;

				fixed3 diffColor = max(0, dot(N, -L) * _DiffuseShade + (1 - _DiffuseShade)) * finalColor.rgb;
				finalColor.rgb = diffColor;

				float3 V = normalize(i.vertexWS - _WorldSpaceCameraPos);
				float3 H = normalize(-L + (-V));

				finalColor=(finalColor+shadowBf)/2;

				if(_High>i.vertexWS.y){

					float hi=_High-i.vertexWS.y;
					float size=3;
					float2 wuv=float2((((hi*L.x)/L.y)+i.vertexWS.x-_Time.x)/size,(((hi*L.z)/L.y)+i.vertexWS.z-_Time.x)/size);
					wuv.x-=(int)wuv.x;
					wuv.y-=(int)wuv.y;
					fixed4 finalColor2 = tex2D(_LightMap, wuv);
					finalColor.rbg *= (1+(finalColor2.a/6));
				}
				
				return finalColor;

			}
            ENDCG
        }
		Pass
		{
			Tags{ "LightMode"="ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert (appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
    }
}

	
