Shader "Skybox/Panoramic Blended" {
	/*Properties{
		_Tint("Tint Color", Color) = (.5, .5, .5, .5)
		_Blend("Blend", Range(0.0,1.0)) = 0.5
		_Exposure("Exposure", Range(0.0,8.0)) = 1.0
		_Rotation("Rotation", Range(0,360)) = 0
		_Spherical("Spherical (HDR)", 2D) = "white" {}
		_Spherical2("2 Spherical (HDR)", 2D) = "white" {}
	}*/
	Properties{
		_Tint("Tint Color", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
		_Rotation("Rotation", Range(0, 360)) = 0
		[NoScaleOffset] _MainTex("Spherical  (HDR)", 2D) = "grey" {}
		[NoScaleOffset] _MainTex2("Spherical 2 (HDR)", 2D) = "grey" {}
		_Blend("Blend", Range(0.0,1.0)) = 0.5
		[KeywordEnum(6 Frames Layout, Latitude Longitude Layout)] _Mapping("Mapping", Float) = 1
		[Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType("Image Type", Float) = 0
		[Toggle] _MirrorOnBack("Mirror on Back", Float) = 0
		[Enum(None, 0, Side by Side, 1, Over Under, 2)] _Layout("3D Layout", Float) = 0
	}

	SubShader{
		Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off
		//Fog { Mode Off }
		//Lighting Off
		//Color[_Tint]
		Pass {
			SetTexture[_MainTex] { combine texture }
			SetTexture[_MainTex2] { constantColor(0,0,0,[_Blend]) combine texture lerp(constant) previous }
		}
	}
	Fallback "Skybox/Panoramic", 1
}