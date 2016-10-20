Shader "Custom/BlockShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_AtlasSize ("AtlasSize", Int) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float4 color : COLOR;
			float3 worldPos;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		uint _AtlasSize;		
		
		static const float blockUvStep = 1.0 / (int)(_AtlasSize);			

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{			
			uint ti = (IN.color.r * 255.0) - 1;

			/*if (ti == 2)
				o.Albedo = float3(0, 1, 0);
			else if(ti == 1)
				o.Albedo = float3(1, 0, 0);
			else
				o.Albedo = float3(1, 1, 1);*/

			uint i = ti / _AtlasSize;
			uint j = ti - i * _AtlasSize;

			float2 tileUvFull = float2(dot(IN.worldNormal.zxy, IN.worldPos),
				dot(IN.worldNormal.yzx, IN.worldPos));
			float2 tileUv = frac(tileUvFull);			
			float2 uv = (float2(i, j) + tileUv) * blockUvStep;

			//We need manually computed dx and dy because texture sampler selects mip level according to d(uv)/dx etc. and
			//using frac(tileUv) leads to incorrect mip level selection at the seams
			o.Albedo = tex2D(_MainTex, uv, ddx(tileUvFull * blockUvStep), ddy(tileUvFull * blockUvStep)).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
