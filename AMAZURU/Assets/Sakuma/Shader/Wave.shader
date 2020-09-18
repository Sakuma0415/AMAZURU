Shader "Custom/Wave" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_Gage("Gage",Range(0,1))=0
		_Top("Top",float)=1
		_Under("Under",float)=0
    }

    SubShader{
        Tags { 
            "Queue"="Transparent"
        }

        ZWrite Off

		Blend One OneMinusSrcAlpha

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
			float _Gage;
			float _Top;
			float _Under;
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
				float gageSpan=1-(_Under+(1-_Top));
				float time=_Time.w;
				if((_Gage*gageSpan)+_Under<output.uv.y  -((1-sin(time+((output.uv.x*720))*3.1415f/180))*0.01f)  ){
					c.a=0;
					c.r=0;
					c.g=0;
					c.b=0;
				}
                return c;
            }
        ENDCG
        }
    }
}