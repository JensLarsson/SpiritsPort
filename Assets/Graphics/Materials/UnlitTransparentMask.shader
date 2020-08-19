// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitTransparentMask"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MaskTexture("Mask", 2D) = "white" {}
		_Texture("Texture", 2D) = "white"{}
	}
 SubShader {
	 Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
	 LOD 100

	 ZWrite Off
	 Blend SrcAlpha OneMinusSrcAlpha

		Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag





					float random(float n)
				{
					return frac(cos(n * 89.42) * 343.42);
				}

				float2 random(float2 n)

				{
					return float2(random(n.x * 23.62 - 300.0 + n.y * 34.35),random(n.x * 45.13 + 256.0 + n.y * 38.89));
				}
				float worley(float2 pos,float scale)
				{
					float dis = 2.0;
					for (int x = -1; x <= 1; x++)
					{
						for (int y = -1; y <= 1; y++)
						{
							float2 p = floor(pos / scale) + float2(x,y);
							float d = length(random(p) + float2(x,y) - frac(pos / scale));
							dis = min(dis, d);
						}
					}
					return 1.0 - dis;
				}

				//float random(float2 st) {
				//	return frac(sin(dot(st.xy,
				//		float2(12.9898, 78.233)))
				//		* 43758.5453123);
				//}

				float noise(float2 xy) {
					float2 i = floor(xy);
					float2 f = frac(xy);

					float a = random(i);
					float b = random(i + float2(1.0, 0.0));
					float c = random(i + float2(0.0, 1.0));
					float d = random(i + float2(1.0, 1.0));

					float2 u = f * f * (3.0 - 2.0 * f);

					return lerp(a, b, u.x) +
						(c - a) * u.y * (1.0 - u.x) +
						(d - b) * u.x * u.y;
				}

				float noise(float x, float y) {
					float2 i = floor(float2(x, y));
					float2 f = frac(float2(x, y));

					float a = random(i);
					float b = random(i + float2(1.0, 0.0));
					float c = random(i + float2(0.0, 1.0));
					float d = random(i + float2(1.0, 1.0));

					float2 u = f * f * (3.0 - 2.0 * f);

					return lerp(a, b, u.x) +
						(c - a) * u.y * (1.0 - u.x) +
						(d - b) * u.x * u.y;
				}

	#define NUM_OCTAVES 5
				float fBm(in float2 _st) {
					float v = 0.0;
					float a = 0.5;
					float2 shift = float2(100.0,100.0);
					// Rotate to reduce axial bias
					float2x2 rot = float2x2(cos(0.5), sin(0.5),-sin(0.5), cos(0.50));
					for (int i = 0; i < NUM_OCTAVES; ++i) {
						v += a * noise(_st);
						_st = mul(rot, _st) * 2.0 + shift;
						a *= 0.5;
					}
					return v;
				}


						sampler2D _MaskTexture;
						sampler2D _Texture;
						fixed4 _Color;

						struct appdata {
							float4 vertex : POSITION;
							float2 uv : TEXCOORD0;
						};

						struct v2f {
							float2 uv : TEXCOORD0;
							float4 vertex : SV_POSITION;
							float4 worldSpacePos : TEXCOORD1;
						};

						v2f vert(appdata v) {
							v2f o;
							o.vertex = UnityObjectToClipPos(v.vertex);
							o.uv = v.uv;
							o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
							return o;
						}

						fixed4 frag(v2f i) : SV_Target{
							float4 mask = tex2D(_MaskTexture, i.uv);
							//float4 col = _Color * tex2D(_Texture, i.uv) * mask;
							float wor = max(0.4, 1 - worley((i.worldSpacePos) * 300, 100));
							float fbm = max(0.4, fBm(i.worldSpacePos * 10 + _SinTime.y));
							float4 col = (wor + fbm) * 0.8 * _Color;
							return col*mask;
						}
						ENDCG
					}
	}

		Fallback "Diffuse"
}