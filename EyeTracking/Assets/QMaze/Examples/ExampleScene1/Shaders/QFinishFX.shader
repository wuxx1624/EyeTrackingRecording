Shader "FX/QFinishFX" 
{
	Properties 
	{
		_MainTex ("Base", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200		
		Lighting off
		ZWrite off
		Cull off
		Fog { Mode Off }
		Blend OneMinusDstColor One
		
		PASS 
		{		
			CGPROGRAM
				#pragma vertex vert
	        	#pragma fragment frag
	        	#pragma fragmentoption ARB_precision_hint_fastest
				
				sampler2D _MainTex;			
				float4 _MainTex_ST;
				
		        struct appdata 
		        {	
		            float4 vertex 	: POSITION;	
		            fixed2 uv1 		: TEXCOORD0;	            
		        };
		        
		        struct v2f 
				{				
		            float4 pos: SV_POSITION;	
		            fixed2 uv1: TEXCOORD0;	
		            fixed2 uv2: TEXCOORD1;	
		        };
			        	
		        v2f vert (appdata v)	
		        {	
		            v2f o;	
		            o.pos = UnityObjectToClipPos(v.vertex);	
		            o.uv1 = o.uv2 = v.uv1.xy;
		            o.uv1.x -= _Time.x; 	            
		            o.uv2.x += _Time.x;
		            o.uv2.x += 1 - (o.uv1.x - o.uv2.x) * 0.5;
		            return o;	
		        }	

		        fixed4 frag (v2f IN) : COLOR	
		        {	
		         	fixed4 mainTex1 = tex2D(_MainTex, IN.uv1);
		         	fixed4 mainTex2 = tex2D(_MainTex, IN.uv2);
		            return (mainTex1 + mainTex2) * mainTex2.a * mainTex1.a;	
		        }			
			ENDCG
		}
	}
}
