// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Stealth" {
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1"}
        Pass {

            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : COLOR {
                return half4(0, 0, 0, 0);
            }

            ENDCG
        }
    } 
}