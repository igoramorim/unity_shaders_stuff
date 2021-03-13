Shader "Igor/Music/Spiral"
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

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y;
                float2 uv = i.uv - .5;
                //uv = mul(uv, rot(PI * .25 + t));
                float3 c = 0.0;

                float r = length(uv);
                float a = atan2(uv.x, uv.y);

                //float ss = smoothstep(0., 1., sin(r + _Audio * 5. + sin(t * .5)));
                float ss = (5. * _Audio) * sin(r * _Audio);
                ss += sin(5. + r * 2.1 + _Audio * .67) * 2.2;
                ss += sin(5. + r * 1.3 + _Audio * 2.1) * 4.;

                c.r = smoothstep(0., 1., sin((a + .1) * 5. + r * sin(5. * ss * r + t * 3.) * 5. - (t * 4.)) + sin(a * 2.));
                c.g = smoothstep(0., 1., sin((a + .2) * 5. + r * sin(5. * ss * r + t * 3.) * 6. - (t * 4.)) + sin(a * 2.));
                c.b = smoothstep(0., 1., sin((a + .3) * 5. + r * sin(5. * ss * r + t * 3.) * 7. - (t * 4.)) + sin(a * 2.));

                return float4(c, 1.);
            }
            ENDCG
        }
    }
}
