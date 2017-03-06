Shader "Puffy_Smoke/Smoke No Details" {

	Properties {
		_ShadowColor ("Shadow Color", Color) = (0.0,0.5,0.5,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		
		_FadeIn ("Fade in", Range(0.0001,1.0)) = 0.1
		_Opacity ("Opacity", Range(0.0,1.0)) = 0.5
		_Scattering ("Scattering", Range(0.0,1.0)) = 1
		
		_EmissiveDensity ("Emissive Density", Range(0.0,1.0)) = 0.1

	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		Cull off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		// ---- Fragment program cards
		SubShader {
			Pass {
		
				CGPROGRAM
		 		#pragma target 2.0
		 		#pragma glsl

		 		// very important on mobile opengl es 2.0
				#pragma glsl_no_auto_normalization 
								
		        #pragma vertex vert  
		        #pragma fragment frag 
		        #pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				
				
				sampler2D _MainTex;
		     	
		     	uniform fixed4 _ShadowColor;
		     	uniform fixed4 _AmbientColor;
		     	uniform fixed4 _LightColor;
		     	uniform float4 _MainTex_ST;

				uniform fixed _FadeIn;

		        uniform fixed _Opacity;
		        uniform fixed _LightIntensity;
		       	uniform fixed _Scattering;
		       	uniform fixed _EmissiveDensity;
		       	
				struct vertexInput  {
					fixed4 vertex : POSITION;
					fixed4 color : COLOR;
					fixed3 normal : NORMAL;
					fixed2 texcoord : TEXCOORD0;
				};
		
				struct vertexOutput {
					fixed4 vertex : POSITION;
					fixed4 color : COLOR;
					fixed2 texcoord : TEXCOORD0;
					fixed3 CombinedData : TEXCOORD2;
					fixed4 ShadowColor : TEXCOORD3;
					fixed4 AmbientColor : TEXCOORD4;
					
				};
	
				vertexOutput vert (vertexInput input)
				{
					vertexOutput output; 
					output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
					
					fixed _old = input.normal.x;
					fixed _emissive = input.normal.y;
					
					fixed _young = 1 - _old;
					fixed4 temp = input.color;
					
					output.texcoord = TRANSFORM_TEX(input.texcoord,_MainTex);
					
					// mix smoke color and light color
					temp.x *= (max(0, _LightColor.r - _emissive) * _LightIntensity) + _emissive;
					temp.y *= (max(0, _LightColor.g - _emissive) * _LightIntensity) + _emissive;
					temp.z *= (max(0, _LightColor.b - _emissive) * _LightIntensity) + _emissive;	

					output.color = temp;
					
					// scattering effect on older particles
					output.ShadowColor = lerp(_ShadowColor, output.color , _old * _Scattering);
					
					// emissive particles are less transparent
					fixed AlphaTemp = input.color.a * (1 + _emissive*_emissive*2);
					
					fixed OldTemp = clamp(_old / _FadeIn, 0, 1);	
					fixed OpacityTemp = _Opacity * _young * OldTemp;

					output.CombinedData = fixed3(_emissive, OpacityTemp * AlphaTemp, _emissive * _EmissiveDensity);
					output.AmbientColor = (_AmbientColor * (1 - _emissive));
					
					
					return output;
				}
					
				fixed4 frag (vertexOutput i) : COLOR
				{
					// get main texture color and alpha
					fixed4 _sampled = tex2D(_MainTex, i.texcoord);
					
					// mix light color and shadow color
					fixed4 _finalcolor = lerp(i.ShadowColor , i.color , clamp(_sampled.r * _LightIntensity + i.CombinedData.x, 0, 1) ) + i.AmbientColor;

					// main alpha
					_finalcolor.a = i.CombinedData.y * _sampled.a;
					
					return _finalcolor;
				}
				
				ENDCG 
			}	
		}
		
		
		
	}
}