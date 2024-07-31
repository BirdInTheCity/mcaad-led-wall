Shader "Custom/URPUnlitSpriteWithShadow" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowOffset ("Shadow Offset", Vector) = (0,0,0,0)
        _ShadowBlur ("Shadow Blur", Range(0,1)) = 0.1
        _ShadowOpacity ("Shadow Opacity", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ShadowColor;
            float2 _ShadowOffset;
            float _ShadowBlur;
            float _ShadowOpacity;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate shadow position
                float2 shadowUV = i.uv + _ShadowOffset;

                // Apply shadow blur
                float shadowBlur = _ShadowBlur * 0.01;
                float shadowBlurX = shadowBlur / _ScreenParams.x;
                float shadowBlurY = shadowBlur / _ScreenParams.y;
                float shadowBlurred = (
                    tex2D(_MainTex, shadowUV + float2(-shadowBlurX, -shadowBlurY)).r +
                    tex2D(_MainTex, shadowUV + float2(-shadowBlurX, shadowBlurY)).r +
                    tex2D(_MainTex, shadowUV + float2(shadowBlurX, -shadowBlurY)).r +
                    tex2D(_MainTex, shadowUV + float2(shadowBlurX, shadowBlurY)).r
                ) / 4.0;

                // Apply shadow color and opacity
                fixed4 shadowColor = _ShadowColor;
                shadowColor.a = shadowBlurred * _ShadowOpacity;

                // Combine main color and shadow color
                fixed4 finalColor = col + shadowColor;

                return finalColor;
            }
            ENDCG
        }
    }
}