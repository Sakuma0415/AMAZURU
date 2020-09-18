Shader "Custom/Add" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader{
        Tags { 
            "Queue"="Transparent"
        }


		Cull Off
        Lighting Off
        ZWrite Off

		Blend Zero SrcColor 
        Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha:fade

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

            VertexOutput vert (VertexInput input) {
            	VertexOutput output;
            	output.v = UnityObjectToClipPos(input.pos);
            	output.uv = input.uv;

            	//もとの色(SpriteRendererのColor)と設定した色(TintColor)を掛け合わせる
            	output.color = input.color * _Color; 

            	return output;
            }

            float4 frag (VertexOutput output) : SV_Target {
            	float4 c = tex2D(_MainTex, output.uv) * output.color;
                c.rgb *= c.a;
				if(c.a<0.1f){
					c.r=1;
					c.g=1;
					c.b=1;
				}
                return c;
            }
        ENDCG
        }
    }
}