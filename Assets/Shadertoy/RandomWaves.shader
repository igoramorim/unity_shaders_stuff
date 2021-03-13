Shader "Igor/RandomWaves"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			

			float2 rot(float2 p, float2 pivot, float a) {
				float s = sin(a);
				float c = cos(a);

				p -= pivot;
				p = float2(p.x*c - p.y*s, p.x*s + p.y*c);
				p += pivot;

				return p;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = 5. * i.uv - 2.5;

				float3 col = 0.0;
				// mirror coord
				//    uv = abs(uv);
				// rotate 45 degrees
				//    uv = rot(uv, float2(0.0, 0.0), .25*PI);
				// mirror again
				//    uv.x = abs(uv.x);

				//    uv = lerp(uv-.5, uv, sin(_Time.y)*3.)*.5+.5;

				//    uv = rot(uv, float2(0.0, 0.0), .25*PI+sin(_Time.y)*2.);

				for (float i = 1; i<10; i++) {
					//        uv = abs(uv);
					uv = rot(uv, float2(0.0, 0.0), .25*PI*i);
					//        uv = abs(uv);
					uv.y += sin(i*20. + sin(_Time.y)*3.*5. + _Time.y * 1. + uv.x*1.5) * sin(_Time.y)*3.; // spec.x*3.;
					col += abs((0.05*sin(_Time.y)*3.) / uv.y);
				}

				return float4(col, 1.0);
			}
            ENDCG
        }
    }
}
