// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HideOnlyShader" {
Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Col ("Color", Color) = (0.5,0,0,1)
}
SubShader {
    Tags { "RenderType"="Transparent" "Queue"="Transparent" "HideDisp"="Yes" }
    Pass {
//      Tags { "LightMode"="ForwardBase" "HideDisp"="Yes" }
        ZTest Greater
        ZWrite Off
        Lighting Off
        SeparateSpecular Off
        Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _Col;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
    float4  col : COLOR;
};

float4 _MainTex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    o.col = _Col;
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 c = tex2D (_MainTex, i.uv);
    return c*i.col;
}

ENDCG

    }
}
Fallback "VertexLit"
}