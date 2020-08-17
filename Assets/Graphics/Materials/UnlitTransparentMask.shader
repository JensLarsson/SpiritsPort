// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitTransparentMask"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MaskTexture("Mask", 2D) = "white" {}
		_Texture("Texture", 2D) = "white"{}
	}

		SubShader{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "False" "RenderType" = "Transparent" }

			/////////////////////////////////////////////////////////
			/// First Pass
			/////////////////////////////////////////////////////////

			Pass {
				// Only render alpha channel
				ColorMask A
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				fixed4 _Color;

				float4 vert(float4 vertex : POSITION) : SV_POSITION {
					return UnityObjectToClipPos(vertex);
				}

				fixed4 frag() : SV_Target {
					return _Color;
				}

				ENDCG
			}

		/////////////////////////////////////////////////////////
		/// Second Pass
		/////////////////////////////////////////////////////////

		Pass {
					// Now render color channel
					ColorMask RGB
					Blend SrcAlpha OneMinusSrcAlpha

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag




			
					float r(float n)
				{
					return frac(cos(n * 89.42) * 343.42);
				}

				float2 r(float2 n)
		
				{
					return float2(r(n.x * 23.62 - 300.0 + n.y * 34.35),r(n.x * 45.13 + 256.0 + n.y * 38.89));
				}
				float worley(float2 pos,float scale)
				{
					float dis = 2.0;
					for (int x = -1; x <= 1; x++)
					{
						for (int y = -1; y <= 1; y++)
						{
							float2 p = floor(pos / scale) + float2(x,y);
							float d = length(r(p) + float2(x,y) - frac(pos / scale));
							dis =
							if (dis > d)
							{
								dis = d;
							}
						}
					}
					return 1.0 - dis;
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
					};

					v2f vert(appdata v) {
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target{
						float4 mask = tex2D(_MaskTexture, i.uv);
						//float4 col = _Color * tex2D(_Texture, i.uv) * mask;
						float4 col = 1-worley(i.uv*300, 100);
						return col;
					}
					ENDCG
				}
	}

		Fallback "Diffuse"
}