Shader "Puffy_Smoke/Basic/Raw Vertex Colors" {
		
	SubShader {
		
		Tags {"Queue"="Geometry" "RenderType"="Opaque" "IgnoreProjector"="True" "LightMode" = "Vertex" }
		LOD 200
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 2.0

		struct Input {
			float3 color : COLOR;
		};
			
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"

}