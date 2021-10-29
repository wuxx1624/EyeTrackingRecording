
Shader "Custom/ZTest" {
	Properties{
	//_MainTex("Texture", 2D) = "white" {}
	}
	SubShader{
		ZWrite On
		ZTest Always
		Pass {
			//Material {
				//Diffuse(0,0,0,0)
			}
			
		}
	}
}