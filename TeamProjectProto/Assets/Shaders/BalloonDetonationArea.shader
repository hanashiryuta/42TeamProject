Shader "Custom/BalloonDetonationArea"
{
	Properties
	{
		//_CenterPosition("Center Position", Vector) = (0, 0, 0, 0)
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{ 
				"Queue" = "Transparent"
				"RenderType" = "Transparent" 
			}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard
		#pragma surface surf Standard alpha:fade 
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed3 _CenterPosition;
		fixed4 _Color;
		float _Radius = 0;

		struct Input
		{
			float3 worldPos;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float dist = distance(_CenterPosition, IN.worldPos);
			float radius = _Radius;
			float width = 0.2f;

			if (radius < dist && dist < radius + width)
			{
				o.Albedo = _Color;
				o.Alpha = 0.8f;	
			}
			else
			{
				discard;//不表示
			}

			//if (radius < dist &&			//radius以内の範囲
			//	(radius - width) > dist)	//
			//{
			//	o.Albedo = _Color;
			//	o.Alpha = 0.8f;
			//}
			//else
			//{

			//	discard;//不表示
			//}
		}
		ENDCG
	}
	FallBack "Diffuse"
}