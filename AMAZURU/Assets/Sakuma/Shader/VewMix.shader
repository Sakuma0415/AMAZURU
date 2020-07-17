Shader "Custom/VewMix"
{
     Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
		_MainTex2 ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_Point("Point",Vector)=(0,0,0,0)
		_Ass("Ass",Vector)=(0,0,0,0)
		_R("R",float)=0
		_Fog("Fog",Range(0,1))=0
		}

    SubShader{
        Tags { 
            "Queue"="Transparent"
        }
       
	ZWrite Off
        Blend One OneMinusSrcAlpha //乗算済みアルファ

        Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput {
                float4 pos	:	POSITION;    // 3D座標
                float4 color:	COLOR;
                float2 uv	:	TEXCOORD0;   // テクスチャ座標
            };

            struct VertexOutput {
                float4 v	:	SV_POSITION; // 2D座標
                float4 color:	COLOR;	
                float2 uv	:   TEXCOORD0;   // テクスチャ座標
            };

            //プロパティの内容を受け取る
            float4 _Color;
            sampler2D _MainTex;
            sampler2D _MainTex2;
			float4 _Point;
			float4 _Ass;
			float _R;
			float _Fog;

            VertexOutput vert (VertexInput input) {
            	VertexOutput output;
            	output.v = UnityObjectToClipPos(input.pos);
            	output.uv = input.uv;

            	//もとの色(SpriteRendererのColor)と設定した色(TintColor)を掛け合わせる
            	output.color = input.color * _Color; 

            	return output;
            }

            float4 frag (VertexOutput output) : SV_Target {
            	float4 c ;

				c.r=1;
				c.g=1;
				c.b=1;

				float sq=sqrt((output.uv.x-_Point.x)*(output.uv.x-_Point.x)*(_Ass.x/_Ass.y)*(_Ass.x/_Ass.y)+(output.uv.y-_Point.y)*(output.uv.y-_Point.y));
				if(sq<_R)
				{
					c.r=(sq/_R)*(sq/_R);
					c.g=(sq/_R)*(sq/_R);
					c.b=(sq/_R)*(sq/_R);
				}
				

				c.r=1-((1-c.r)*_Fog);
				float4 a = tex2D(_MainTex, output.uv) * output.color;
				float4 b = tex2D(_MainTex2, output.uv) * output.color;
				
				float4 d=(a*c.r)+(b*(1-c.r));

                return d;
            }
        ENDCG
        }
    }
}
