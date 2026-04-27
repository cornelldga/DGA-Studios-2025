Shader "Unlit/LandingIndicator"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BirthTime ("Birth Time", Float) = 0
        _Duration ("Duration", Float) = .2
        _ShineColor ("Shine Color", Color) = (0.9, .3, 1.0, 0.9)
        _ShineWidth ("Shine Ring Width", Float) = 0.15
        _ShineIntensity ("Shine Intensity", Float) = 2.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _BirthTime;
                float _Duration;
                float4 _ShineColor;
                float _ShineWidth;
                float _ShineIntensity;
            CBUFFER_END

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float age = _Time.y - _BirthTime;
                float life = saturate(age / _Duration);

                half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                if (sprite.a < 0.01) discard;

                float2 centered = IN.uv - 0.5;
                float dist = length(centered) * 2.0;

                float pulse = sin(life * 3.14159);

                float core = (1.0 - smoothstep(0.0, life + 0.1, dist)) * (1.0 - life);

                float ringFront = life * 1.2;
                float shine = smoothstep(ringFront - 0.25, ringFront, dist)
                            * (1.0 - smoothstep(ringFront - 0.05, ringFront + 0.08, dist));
                shine = pow(shine, 0.5);

                float inner = (1.0 - smoothstep(0.0, 0.4, dist)) * pulse * 0.8;

                float totalShine = (shine * 1.8 + core * 1.2 + inner) * _ShineIntensity;

                float fadeIn = smoothstep(0.0, 0.25, life);

                half4 col = IN.color * sprite;
                col.rgb += _ShineColor.rgb * totalShine;
                col.rgb = min(col.rgb, 3.0);
                col.a = sprite.a * fadeIn;

                return col;
            }
            ENDHLSL
        }
    }
}