Shader "Unlit/Rainy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Size ("Size", float) = 1
		_T ("Time", float) = 1
		_Distortion ("Distortion", range(-5, 5)) = 1
		_Blur("Blur", range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

		GrabPass { "_GrabTexture"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

			#define S(a ,b, t) smoothstep(a, b, t)

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 grabUv : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _GrabTexture;
            float4 _MainTex_ST;
			float _Size, _T, _Distortion, _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float N21(float2 p) {
				p = frac(p*float2(123.34, 345.45));
				p += dot(p, p + 34.345);
				return frac(p.x*p.y);
			}

			float3 Layer(float2 UV, float t) {
				// Make the boxes taller than wider
				float2 aspect = float2(2, 1);
				float2 uv = UV * _Size * aspect;
				// Moves the grid down so the drops do not seem move up
				uv.y += t * .25;
				// Make the origin in the middle
				float2 gv = frac(uv) - .5;
				// Get a different value for every box. Box0, Box1, Box2 ...
				float2 id = floor(uv);

				// Pseudo random number between 0 - 1
				float n = N21(id);
				t += n * 6.2831; // TWO PI

				float w = UV.y * 10;
				// x position between -.4 and .4
				float x = (n - .5)*.8;
				// (.4-abs(x)) make the drop closer to the edge move straight (do not sin())
				x += (.4 - abs(x)) * sin(3 * w)*pow(sin(w), 6)*.45;
				float y = -sin(t + sin(t + sin(t)*.5))*.45;
				// Make the drop a little saggy
				y -= (gv.x - x)*(gv.x - x);

				float2 dropPos = (gv - float2(x, y)) / aspect;
				float drop = S(.05, .03, length(dropPos));

				float2 trailPos = (gv - float2(x, t * .25)) / aspect;
				trailPos.y = (frac(trailPos.y * 8) - .5) / 8;
				float trail = S(.03, .01, length(trailPos));
				// Show the trail only above the main drop
				float fogTrail = S(-.05, .05, dropPos.y);
				// Make the trail gradient. Top of the box to the main drop
				fogTrail *= S(.5, y, gv.y);
				trail *= fogTrail;
				// Make the fogTrail appears only in the middle
				fogTrail *= S(.05, .04, abs(dropPos.x));

				// col += fogTrail * .5;
				// col += trail;
				// col += drop;

				float2 offs = drop * dropPos + trail * trailPos;

				// if (gv.x > .48 || gv.y > .49) col = float4(1, 0, 0, 1);

				return float3(offs, fogTrail);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float t = fmod(_Time.y + _T, 7200);

				float4 col = 0;

				float3 drops = Layer(i.uv, t);
				drops += Layer(i.uv*1.23 + 7.54, t);
				drops += Layer(i.uv*2.15 + 1.92, t);
				drops += Layer(i.uv*1.71 + 3.46, t);

				// Get the difference between neighbor pixel
				// Saturate is the same of clamp()
				float fade = 1-saturate(fwidth(i.uv)*50);

				float blur = _Blur * 7 * (1 - drops.z*fade);

				// Show the texture with the drops
				// col = tex2Dlod(_MainTex, float4(i.uv+drops.xy*_Distortion, 0, blur));
				
				float2 projUv = i.grabUv.xy / i.grabUv.w;
				projUv += drops.xy * _Distortion * fade;
				blur *= .01;

				const float numSamples = 32;
				float a = N21(i.uv)*6.2831;
				for (float i = 0; i < numSamples; i++) {
					float2 offs = float2(sin(a), cos(a)) * blur;
					float d = frac(sin((i + 1)*546.)*542.);
					d = sqrt(d);
					offs *= d;
					col += tex2D(_GrabTexture, projUv+offs);
					a++;
				}
				col /= numSamples;

                return col*.9;
            }
            ENDCG
        }
    }
}
