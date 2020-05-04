Shader "Custom/LoadUI"
{
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_Pos ("Pos", Vector) = (0, 0, 0, 0)
		_Hi ("Hi", float) = 0
		_Scale("Scale", Vector)=(0, 0, 0, 0)
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
			float4 _Pos;
			float _Hi;
			float4 _Scale;

            VertexOutput vert (VertexInput input) {
            	VertexOutput output;
            	output.v = UnityObjectToClipPos(input.pos);
            	output.uv = input.uv;
            	output.color = input.color; 

            	return output;
            }

            float4 frag (VertexOutput output) : SV_Target {
            	float4 c = tex2D(_MainTex, output.uv);
				float wav=sin(((_Time.y*75)+(output.uv.x*_Scale.x)-(_Scale.x/2))*1.25*3.1415/180)*10;
				if(_Pos.y+(output.uv.y*_Scale.y)-(_Scale.y/2)+wav<_Hi)
				{
						c*=_Color;
				}

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
