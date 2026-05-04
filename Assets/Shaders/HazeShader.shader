Shader "Custom/DustHaze"
{
    Properties
    {
        _DustColor ("Dust Color", Color) = (0.88, 0.86, 0.5, 0.2)
        _HazeStrength ("Haze Strength", Range(0, 1)) = 0.5
        
        _Layer1Parallax ("Far Parallax", Float) = 0.1
        _Layer1Scale ("Far Scale", Float) = 0.5
        _Layer1Intensity ("Far Intensity", Range(0, 1)) = 0.15
        
        _Layer2Parallax ("Mid Parallax", Float) = 0.4
        _Layer2Scale ("Mid Scale", Float) = 1.0
        _Layer2Intensity ("Mid Intensity", Range(0, 1)) = 0.20
        
        _Layer3Parallax ("Near Parallax", Float) = 0.8
        _Layer3Scale ("Near Scale", Float) = 2.0
        _Layer3Intensity ("Near Intensity", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            float2 _CameraWorldPos;
            float4 _DustColor;
            float _HazeStrength;
            float _Layer1Parallax, _Layer1Scale, _Layer1Intensity;
            float _Layer2Parallax, _Layer2Scale, _Layer2Intensity;
            float _Layer3Parallax, _Layer3Scale, _Layer3Intensity;

            // Hash + value noise (simple, cheap, looks decent)
            float hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f); // smoothstep

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float dustLayer(float2 uv, float parallax, float scale, float intensity)
            {
                float2 sampleCoord = (uv - _CameraWorldPos * parallax) * scale;
                return valueNoise(sampleCoord) * intensity;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert UVs to a larger coord space so noise scaling feels meaningful
                float2 baseUV = i.uv * 10.0;

                float layer1 = dustLayer(baseUV, _Layer1Parallax, _Layer1Scale, _Layer1Intensity);
                float layer2 = dustLayer(baseUV, _Layer2Parallax, _Layer2Scale, _Layer2Intensity);
                float layer3 = dustLayer(baseUV, _Layer3Parallax, _Layer3Scale, _Layer3Intensity);

                float dust = saturate((layer1 + layer2 + layer3) * _HazeStrength);

                // Output: dust color with alpha = dust amount → blends over scene
                return float4(_DustColor.rgb, dust);
            }
            ENDCG
        }
    }
}