Shader "Custom/Mirza Beig/Particles/Distortion/Alpha Blended" {
	Properties {
		_Opacity ("Opacity", Range(0, 2)) = 0.5
		_Intensity ("Intensity", Range(0, 10)) = 1
		_Distortion ("Distortion", Range(0, 1)) = 0.05
		_MainTex ("Particle Texture", 2D) = "white" {}
		_DistTex ("Distortion Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 8)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}