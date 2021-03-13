Shader "Igor/Music/NeonWaves"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Audio("Audio", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define amp 6.
            #define freq 4.
            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Audio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2x2 rot(float a) {
                return float2x2(cos(a), -sin(a), sin(a), cos(a));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float3 c = float3(0.0, 0.0, 0.0);
                float dy = 0.0;

                for (float i = 1.; i <= 3.; i++) {
                    uv = abs(uv);
                    uv = mul(uv, rot(PI*.25)); // uv = rot(PI*.25)*uv;
                    float sx = amp * (amp * _Audio) * sin(freq * uv.x - 3. * _Time.y);
                    sx += sin(freq * uv.x * 2.1 + _Time.y) * 4.;
                    sx += sin(freq * uv.x * 1.7 + _Time.y * 1.26) * 4.5;
                    sx += sin(freq * uv.x * 4.1 + _Time.y * .67) * 5.;
                    sx += sin(freq * uv.x * 3.6 + _Time.y * 4.2) * 2.4;
                    uv = abs(uv);
                    uv.y += i * .1;
                    dy = 100. / (50. * abs(30 * uv.y - sx));

                    c += float3((uv.x + .27) * dy, .27 * dy, dy * abs(sin(_Time.y*i))) * clamp(5. * _Audio, .2, 5.);
                    // c = float3(c);
                }

                return float4(c, 1.0);
            }
            ENDCG
        }
    }
}
