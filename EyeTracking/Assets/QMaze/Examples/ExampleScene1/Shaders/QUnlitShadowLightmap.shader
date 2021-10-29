Shader "Unlit/QUnlitShadowLightmap" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightMap("Lightmap", 2D) = "white" {}
		_ShadowIntensity("Shadow Intensity", Range(0,1)) = 0
		_SunIntensity("Sun Intensity", Range(0,5)) = 0
	}
	SubShader 
	{
		Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}

		Pass 
		{
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				struct v2f
				{
					float4	pos			: SV_POSITION;
					float2	uv			: TEXCOORD0;
					float2	uv2			: TEXCOORD1;
					LIGHTING_COORDS(2,3)
				};

				float4 _MainTex_ST;
				float4 _LightMap_ST;
				float _ShadowIntensity;
				float _SunIntensity;

				v2f vert (appdata_full v)
				{
					v2f o;
					
					o.pos = UnityObjectToClipPos( v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
					o.uv2 = TRANSFORM_TEX (v.texcoord1, _LightMap).xy;
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				sampler2D _MainTex;
				sampler2D _LightMap;

				fixed4 frag(v2f i) : COLOR
				{
					fixed atten = SHADOW_ATTENUATION(i); 
					fixed4 mainTex = tex2D(_MainTex, i.uv);
					mainTex = mainTex + pow(mainTex, 2) * atten * _SunIntensity;
					return lerp(mainTex, fixed4(0,0,0,0), _ShadowIntensity - atten * atten * _ShadowIntensity)  * tex2D(_LightMap, i.uv2);
				}
			ENDCG		
		}
	}
	FallBack "VertexLit"
}