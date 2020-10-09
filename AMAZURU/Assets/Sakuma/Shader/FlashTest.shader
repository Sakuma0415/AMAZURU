Shader "Custom/FlashTest"
{
    Properties {
        _Color ("Tint", Color) = (1,1,1,1)
		_Table("Table",Range(0,1))=0
		_Check("Check",Range(0,1))=0
		_Aspect("Aspect",Vector)=(0,0,0,0)
    }

    SubShader{
        Tags { 
            "Queue"="Transparent"
        }
       
	ZWrite Off
        Blend One OneMinusSrcAlpha //乗算済みアルファ

        Pass {
        CGPROGRAM
            #pragma vertex vert alpha:fade 
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
			float _Table;
			float _Check;
			float4 _Aspect;



            VertexOutput vert (VertexInput input) {
            	VertexOutput output;
            	output.v = UnityObjectToClipPos(input.pos);
            	output.uv = input.uv;

            	output.color = input.color * _Color; 

            	return output;
            }

            float4 frag (VertexOutput output) : SV_Target {

				float4 c=_Color;
				float2 center=float2(0.5,0.5);

                c.rgb *= c.a;

				if(_Table<_Check)
				{

					float late =_Table/_Check;
					float Ass=_Aspect.x/_Aspect.y;
					float dis=sqrt( ((output.uv.x-center.x)*(output.uv.x-center.x))+((output.uv.y-center.y)*(output.uv.y-center.y)/(Ass*Ass)) )/0.57;

					if(dis<late)
					{
						c.a=1;
					}else{
						float fl= dis-late;
						c.a=0;
						if(fl<0.2f*late)
						{
							c.a=1-(fl/(0.2f*late));
						}

					}


				}else{
				
					float late =(_Table-_Check)/(1-_Check);
					c.a=1-late;

				}





				c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
