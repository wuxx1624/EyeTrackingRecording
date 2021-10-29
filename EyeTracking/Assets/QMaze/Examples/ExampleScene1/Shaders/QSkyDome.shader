Shader "Skybox/QSkyDome" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "Queue"="Background" "IgnoreProjector"="True" "RenderType"="Opaque" }
		LOD 200		
		Lighting off
		ZWrite off
		Fog { Mode Off }
		
		PASS 
		{
			CGPROGRAM
				#pragma vertex vert
	        	#pragma fragment frag
	        	#pragma fragmentoption ARB_precision_hint_fastest
				
				sampler2D _MainTex;			
				float4 _MainTex_ST;
				
				struct v2f 
				{				
		            float4 pos: SV_POSITION;	
		            fixed2 uv1: TEXCOORD0;	
		        };
			 
		        struct appdata 
		        {	
		            float4 vertex 	: POSITION;	
		            fixed2 texcoord : TEXCOORD0;		
		        };
			        	
		        v2f vert (appdata v)	
		        {	
		            v2f o;	
		            o.pos = UnityObjectToClipPos(v.vertex);	
		            o.uv1 = v.texcoord.xy; 
		            return o;	
		        }	

		        fixed4 frag (v2f IN) : COLOR	
		        {	
		            return tex2D(_MainTex, IN.uv1);	
		        }			
			ENDCG
		}
	}
}
