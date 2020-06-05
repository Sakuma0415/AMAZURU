Shader "Unlit/Refraction"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_BumpTex ("BumpTexture", 2D) = "white" {}
		_Distortion ("Distortion", Range(-1, 1)) = 0
		_ReflectionTex ("ReflectionTexture", 2D) = "white" {}
		_X("_X", float) = 1
		_Y("_Y", float) = 1
		_Intensity ("Intensity", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent-1"}

		GrabPass{ "_GrabTex" }

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
                float4 tangent : TANGENT;
				float2 texcoord : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 projCoord : TEXCOORD3;
				float4 grabPos : TEXCOORD1;
				float4 scrPos : TEXCOORD2;
				//法線奴
				half3 lightDir : TEXCOORD4;
                half3 viewDir : TEXCOORD5;
				half2 uv2 : TEXCOORD6;
			};
			sampler2D _BumpTex;
			float4 _BumpTex_ST;

			sampler2D _GrabTex;
			float4 _GrabTex_ST;
			float4 _GrabTex_TexelSize;
			sampler2D _ReflectionTex;

			sampler2D _CameraDepthTexture;
			float2 _CameraDepthTexture_ST;
			float4 _CameraDepthTexture_TexelSize;

			float4 _Color;
			float _Distortion;

			float4 _LightColor0;
			half _Shininess;

			float _X;
			float _Y;

			float _Intensity;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BumpTex);
				o.projCoord = ComputeScreenPos(o.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);

				o.uv2  = v.texcoord.xy;
				TANGENT_SPACE_ROTATION;
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));


				return o;
			}

			float2 AlignWithGrabTexel (float2 uv)
			{
				return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 bumpUv=float2(i.uv.x+((_Time.x)-(int)(_Time.x)),i.uv.y);
				float3 bump = UnpackNormal(tex2D(_BumpTex, i.uv+ half2(0, _Time.x * 1.5f)/*float2( bumpUv.x>1?1-bumpUv.x:bumpUv.x,i.uv.y)*/));
				float4 depthUV = i.grabPos;
				depthUV.xy = i.grabPos.xy + (bump.xy * _Distortion);
				
				float surfDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.scrPos.z);
				float refFix = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(depthUV))));
				float depthDiff = saturate(refFix - surfDepth);

				float2 uvoffset = bump.xy * _Distortion;
				float2 grabUV;
				grabUV = AlignWithGrabTexel((i.grabPos.xy + uvoffset * depthDiff) / i.grabPos.w);


				fixed4 col2 = tex2Dproj(_ReflectionTex,UNITY_PROJ_COORD(i.projCoord));
				//fixed4 col2 = tex2D(_ReflectionTex, grabUV);
				float4 col = tex2D(_GrabTex, grabUV) * _Color;

				//col = tex2D(_GrabTex, grabUV) * depthDiff * _Color;

				fixed4 col3=(col2.r<0.95f)?(col2+col*3)/4:col;






				//法線奴

				i.lightDir = normalize(i.lightDir);
                i.viewDir = normalize(i.viewDir);
                half3 halfDir = normalize(i.lightDir + i.viewDir);

                half4 tex = col3;

                // ノーマルマップから法線情報を取得する
				float2 bumpUv2=i.uv2;
				bumpUv2.x=((bumpUv2.x+_Time.x/1.5)*_X)-(int)((bumpUv2.x+_Time.x/1.5)*_X);
				bumpUv2.y=((bumpUv2.y)*_Y)-(int)((bumpUv2.y)*_Y);
                half3 normal3 = UnpackNormal(tex2D(_BumpTex, bumpUv2));
				bumpUv2=i.uv2;
				bumpUv2.x=((bumpUv2.x)*_X)-(int)((bumpUv2.x)*_X);
				bumpUv2.y=((bumpUv2.y+_Time.x/1.5)*_Y)-(int)((bumpUv2.y+_Time.x/1.5)*_Y);
                half3 normal2 = UnpackNormal(tex2D(_BumpTex, bumpUv2));
				half3 normal=(normal2*5+normal3*2)/7;
				float data=0.7f;
				_LightColor0= float4(data,data,data,data);
                // ノーマルマップから得た法線情報をつかってライティング計算をする
                half4 diff = saturate(dot(normal, i.lightDir)) * _LightColor0;
                half3 spec = pow(max(0, dot(normal, halfDir)), _Shininess * 64.0) * _LightColor0.rgb * tex.rgb;

                fixed4 color;
                color.rgb  = tex.rgb * diff + spec;

				//color.r=bumpUv2.x;

				half NdotL = max(0, dot (normal, i.lightDir));
				float3 R = normalize( - i.lightDir + 2.0 * normal * NdotL );
				float3 spec2 = pow(max(0, dot(R, i.viewDir)), 40.0);
				half4 c;
				c.rgb = color * _LightColor0.rgb * NdotL/1.2 + (_Intensity*spec2) +  fixed4(0.1f, 0.1f, 0.1f, 1);
				c.a=_Color.a;
				//c.rgb=(_Color*9+c.rgb)/10;
				return c;//color;
			}



			ENDCG
		}
	}
}