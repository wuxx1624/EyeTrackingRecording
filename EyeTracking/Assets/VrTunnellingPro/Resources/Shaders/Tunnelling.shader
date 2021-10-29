Shader "Hidden/VrTunnellingPro/Tunnelling" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,1)
		_Effect("Effect Strength", Float) = 0
		_Feather("Feather", Float) = 0.1
		_BkgTex("Background Texture", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_Skybox("Skybox", Cube) = "" {}
	}
		SubShader{
			Cull Off
			ZWrite Off
			ZTest Always
			Colormask RGB

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ TUNNEL_BKG
				#pragma multi_compile __ TUNNEL_MASK
				#pragma multi_compile __ TUNNEL_CONSTANT
				#pragma multi_compile __ TUNNEL_INVERT_MASK
				#pragma multi_compile __ TUNNEL_SKYBOX
				#include "UnityCG.cginc"
				#include "TunnellingUtils.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					//float4 position_in_world_space : TEXCOORD0;
				};

				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					//o.position_in_world_space =
						//mul(unity_ObjectToWorld, v.vertex);
					// transformation of input.vertex from object 
					// coordinates to world coordinates;
					return o;
				}

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				sampler2D _BkgTex;
				sampler2D _MaskTex;
				//float _FxInner;
				//float _FxOuter;
				float _FxInnerX;
				float _FxOuterX;
				float _FxInnerY;
				float _FxOuterY;
				float _FxOriginX;
				float _FxOriginY;
				float _FxOffsetX;
				float _FxOffsetY;
				float _Transparency;



				fixed3 frag(v2f i) : SV_Target {
					float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
					fixed3 col = tex2D(_MainTex, uv).rgb;
					float4 coords = screenCoords(i.uv);
					fixed4 bkg;

					// Sample cage/skybox
					#if TUNNEL_BKG
						// Sample cage/blur RT
						// Don't do skybox - cage will include it already if needed
						bkg = tex2D(_BkgTex, uv);
						bkg.rgb *= _Color.rgb;
					#elif TUNNEL_SKYBOX
						// Sample skybox cubemap
						bkg.rgb = sampleSkybox(coords);
						bkg.rgb *= _Color.rgb;
					#else
						// Just use color
						bkg.rgb = _Color.rgb;
					#endif

						// Sample mask
						#if TUNNEL_MASK
							bkg.a = tex2D(_MaskTex, uv).r;
						#else
							bkg.a = 1;
						#endif
							// add transparency level
							bkg.a = bkg.a*_Transparency;
							// Apply color alpha at the end as final factor
							fixed a = bkg.a * _Color.a;

							// Invert mask for portal mode
							#if TUNNEL_INVERT_MASK
								a = 1 - a;
							#endif

								// Calculate radial blend factor r
								// circle restrictor
									/*
								float radius = length(coords.xy / (_ScreenParams.xy/2)) / 2;
								float fxMin = (1 - _FxInnerX);
								float fxMax = (1 - _FxOuterX);
								float r = saturate((radius - fxMin) / (fxMax - fxMin));
								*/

								float fxMin = (1 - _FxInnerX);
								float fxMax = (1 - _FxOuterX);
								float fyMin = (1 - _FxInnerY);
								float fyMax = (1 - _FxOuterY);

								//square restrictor
								/*
								float x = saturate((abs(coords.x / (_ScreenParams.x / 2)) - fxMin) / (fxMax - fxMin));
								float y = saturate((abs(coords.y / (_ScreenParams.y / 2)) - fyMin) / (fyMax - fyMin));
								float2 xy = float2(x, y);
								float r = length(xy);
								*/


								/*// symmetric ellipse restrictor
								float radius = length(coords.xy / (_ScreenParams.xy / 2));
								float x = coords.x / (_ScreenParams.x / 2);
								float y = coords.y / (_ScreenParams.y / 2);
								float rMin = fxMin * fyMin*sqrt((1 + pow(y / x, 2)) / (pow(fyMin, 2) + pow(fxMin*y / x, 2)));
								float rMax = fxMax * fyMax*sqrt((1 + pow(y / x, 2)) / (pow(fyMax, 2) + pow(fxMax*y / x, 2)));
								float r = saturate((radius - rMin) / (rMax - rMin));
								*/

								/*
								// asymmetric ellipse restrixtor
								if (coords.y < 0) {
									fyMin = fyMin + _FxOffsetY;
									fyMax = fyMax + _FxOffsetY;
								}
								float radius = length(coords.xy / (_ScreenParams.xy / 2));
								float x = coords.x / (_ScreenParams.x / 2);
								float y = coords.y / (_ScreenParams.y / 2);
								float rMin = fxMin * fyMin*sqrt((1 + pow(y / x, 2)) / (pow(fyMin, 2) + pow(fxMin*y / x, 2)));
								float rMax = fxMax * fyMax*sqrt((1 + pow(y / x, 2)) / (pow(fyMax, 2) + pow(fxMax*y / x, 2)));
								float r = saturate((radius - rMin) / (rMax - rMin));
								*/

								// Unbias ellipse restrictor
								/*
								//float4 origin_coord = screenCoordsUnbias(float2(_FxOriginX, _FxOriginY));
								float4 origin_coord = float4(_FxOriginX, _FxOriginY, 1, 1);
								if (coords.y < origin_coord.y) {
									fyMin = fyMin + _FxOffsetY;
									fyMax = fyMax + _FxOffsetY;
								}
								float radius = length((coords.xy - origin_coord.xy) / (_ScreenParams.xy / 2));
								float x = (coords.x - origin_coord.x)/ (_ScreenParams.x / 2);
								float y = (coords.y - origin_coord.y)/ (_ScreenParams.y / 2);
								float rMin = fxMin * fyMin*sqrt((1 + pow(y / x, 2)) / (pow(fyMin, 2) + pow(fxMin*y / x, 2)));
								float rMax = fxMax * fyMax*sqrt((1 + pow(y / x, 2)) / (pow(fyMax, 2) + pow(fxMax*y / x, 2)));
								float r = saturate((radius - rMin) / (rMax - rMin));
								*/

								float4 origin_coord = float4(_FxOriginX, _FxOriginY, CLIP_FAR, 1);
								if ((_FxOffsetX < 0) && (coords.x / (_ScreenParams.x / 2)) < origin_coord.x) {
									fxMin = fxMin - _FxOffsetX;
									fxMax = fxMax - _FxOffsetX;
								}
								if ((_FxOffsetX > 0) && (coords.x / (_ScreenParams.x / 2)) > origin_coord.x) {
									fxMin = fxMin + _FxOffsetX;
									fxMax = fxMax + _FxOffsetX;
								}

								if ((_FxOffsetY > 0) && (coords.y / (_ScreenParams.y / 2)) > origin_coord.y) {
									fyMin = fyMin + _FxOffsetY;
									fyMax = fyMax + _FxOffsetY;
								}
								if ((_FxOffsetY < 0) && (coords.y / (_ScreenParams.y / 2)) < origin_coord.y) {
									fyMin = fyMin - _FxOffsetY;
									fyMax = fyMax - _FxOffsetY;
								}

								float radius = length((coords.xy / (_ScreenParams.xy / 2)) - origin_coord.xy);
								float x = (coords.x / (_ScreenParams.x / 2)) - origin_coord.x;
								float y = (coords.y / (_ScreenParams.y / 2)) - origin_coord.y;
								float rMin = fxMin * fyMin*sqrt((1 + pow(y / x, 2)) / (pow(fyMin, 2) + pow(fxMin*y / x, 2)));
								float rMax = fxMax * fyMax*sqrt((1 + pow(y / x, 2)) / (pow(fyMax, 2) + pow(fxMax*y / x, 2)));
								float r = saturate((radius - rMin) / (rMax - rMin));

								#if TUNNEL_CONSTANT
								// Add constant windows/portals to vignette
								return lerp(col, bkg.rgb, saturate(r + a));
							#else
								// Blend result based on alpha and vignette
								return lerp(col, bkg.rgb, min(r,a));
							#endif
						}
						ENDCG
					}
	}
}