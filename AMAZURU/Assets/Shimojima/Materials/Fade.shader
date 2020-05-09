Shader "Unlit/Fade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Alpha ("AlphaCut", Range(0,1)) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite On
        ZTest[unity_GUIZTestMode]
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed _Alpha;
            sampler2D _MainTex;

            v2f vert (appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2( -1, 1);
#endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half alpha = tex2D(_MainTex, IN.texcoord).a;
                alpha = saturate(alpha + (_Alpha * 2 - 1));
                return fixed4(_Color.r, _Color.g, _Color.b, alpha);
            }
            ENDCG
        }
    }

    FallBack "UI?Default"
}
