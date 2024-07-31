Shader "Custom/UIDropShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowDistance ("Shadow Distance", Vector) = (1, -1, 0, 0)
        _ShadowStrength ("Shadow Strength", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ShadowColor;
            float2 _ShadowDistance;
            float _ShadowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 shadowUV = i.uv - (_ShadowDistance.xy * _ShadowStrength);
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 shadowTex = tex2D(_MainTex, shadowUV) * _ShadowColor;
                return lerp(shadowTex, tex, tex.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}