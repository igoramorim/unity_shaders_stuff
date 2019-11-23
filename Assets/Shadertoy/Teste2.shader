Shader "Igor/Teste2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Audio("Audio", float) = 0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
			#define mod(x, y) (x-y*floor(x/y))

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}


			float3 hash3(float2 p)
			{
				float3 q = float3(dot(p, float2(127.1, 311.7)),
					dot(p, float2(269.5, 183.3)),
					dot(p, float2(419.2, 371.9)));
				return frac(sin(q)*43758.5453);
			}

			float iqnoise(in float2 x, float u, float v)
			{
				float2 p = floor(x);
				float2 f = frac(x);

				float k = 1.0 + 63.0*pow(1.0 - v, 6.0);

				float va = 0.0;
				float wt = 0.0;
				for (int j = -2; j <= 2; j++)
					for (int i = -2; i <= 2; i++)
					{
						float2 g = float2(float(i), float(j));
						float3 o = hash3(p + g)*float3(u, u, 1.0);
						float2 r = g - f + o.xy;
						float d = dot(r, r);
						float ww = pow(1.0 - smoothstep(0.0, 1.414, sqrt(d)), k);
						va += o.z*ww;
						wt += ww;
					}

				return va / wt;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = 0.5 - i.uv;


				float rInv = 1. / length(uv);
				uv = uv * rInv - float2(rInv + 2.*mod(_Time.y, 5000.), 1.);
				uv += sin(uv.y + sin(_Time.y) + sin(uv.y + _Time.y * _Audio) + iqnoise(5.*uv, 1., 1.));

				float3 color = float3(0.516, 0.674, 0.316*rInv);
				color = color * (iqnoise(5.*uv, 1., 1.) + 0.240*rInv);
				return float4(color, 1.0);
			}
			ENDCG
		}
	}
}
