Shader "Earth" {
	Properties {
		_AtmosphereColor ("Atmosphere Color", Vector) = (0.1,0.35,1,1)
		_AtmospherePow ("Atmosphere Power", Range(1.5, 8)) = 2
		_AtmosphereMultiply ("Atmosphere Multiply", Range(1, 3)) = 1.5
		_DiffuseTex ("Diffuse", 2D) = "white" {}
		_CloudAndNightTex ("Cloud And Night", 2D) = "black" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}