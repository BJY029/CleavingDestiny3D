// Made with Amplify Shader Editor v1.9.7.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ALP/Water Gestner Waves"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector][Enum(Front,2,Back,1,Both,0)]_Cull("Render Face", Int) = 2
		[DE_DrawerCategory(COLOR WATER LAYERS,true,_WaterShorelineTint,0,0)]_CATEGORY_COLORWATERLAYERS("CATEGORY_COLOR WATER LAYERS", Float) = 1
		[HDR]_WaterShorelineTint("Shoreline Tint", Color) = (0.2784314,0.4235294,0.4431373,1)
		_WaterShorelineDepth("Shoreline Depth", Range( 0 , 100)) = 40
		_WaterShorelineOffset("Shoreline Offset", Range( -1 , 1)) = 0.1
		[HDR]_WaterMidwaterTint("Midwater Tint", Color) = (0.1490196,0.4235294,0.4705882,1)
		[HDR]_WaterDepthTint("Depth Tint", Color) = (0.1960784,0.4313726,0.509804,1)
		_WaterDepthOffset("Depth Offset", Range( 0 , 10)) = 2
		[DE_DrawerSpace(10)]_SPACE_COLORWATERLAYERS("SPACE_COLOR WATER LAYERS", Float) = 0
		[DE_DrawerCategory(OPACITY,true,_WaterOpacity,0,0)]_CATEGORY_OPACITY("CATEGORY_OPACITY", Float) = 1
		_WaterOpacity("Opacity", Range( 0.001 , 1)) = 0.05
		_WaterOpacityShoreline("Opacity Shoreline", Range( 0 , 5)) = 1
		[DE_DrawerSpace(10)]_SPACE_OPACITY("SPACE_OPACITY", Float) = 0
		[DE_DrawerCategory(FOAM SHORELINE,true,_FoamShorelineEnable,0,0)]_CATEGORY_FOAMSHORELINE("CATEGORY_FOAMSHORELINE", Float) = 0
		[DE_DrawerToggleLeft]_FoamShorelineEnable("ENABLE FOAM", Float) = 1
		[HDR][Header(Foam)]_FoamShorelineColor("Foam Color", Color) = (1,1,1,0)
		[DE_DrawerTextureSingleLine]_FoamShorelineMap("Foam Map", 2D) = "black" {}
		[DE_DrawerTilingOffset]_FoamShorelineUVs("Foam UVs", Vector) = (50,50,0,0)
		_FoamShorelineStrength("Foam Strength", Range( 0 , 5)) = 1
		[HDR][Header(Foam Detail)]_FoamShorelineDetailColor("Foam Color", Color) = (1,1,1,0)
		[DE_DrawerTextureSingleLine]_FoamShorelineMapDetail("Foam Detail Map", 2D) = "black" {}
		[DE_DrawerTilingOffset]_FoamShorelineUVsDetail("Foam UVs", Vector) = (0.25,0.25,2,2)
		_FoamShorelineSpeed("Foam Speed", Float) = 0.001
		_FoamShorelineDistance("Foam Distance", Float) = 5
		_FoamShorelineDetailStrength("Foam Strength", Range( 0 , 5)) = 1
		[DE_DrawerSpace(10)]_SPACE_FOAMOFFSHORE("SPACE_FOAMOFFSHORE", Float) = 0
		[DE_DrawerCategory(FOAM OFFSHORE,true,_FoamOffshoreEnable,0,0)]_CATEGORY_FOAMOFFSHORE("CATEGORY_FOAMOFFSHORE", Float) = 0
		[DE_DrawerToggleLeft]_FoamOffshoreEnable("ENABLE FOAM", Float) = 1
		[HDR][Header(Foam)]_FoamOffshoreColor("Foam Color", Color) = (1,1,1,0)
		[DE_DrawerTextureSingleLine]_FoamOffshoreMap("Foam Map", 2D) = "black" {}
		[DE_DrawerTilingOffset]_FoamOffshoreUVs("Foam UVs", Vector) = (50,50,0,0)
		_FoamOffshoreStrength("Foam Strength", Range( 0 , 5)) = 1
		[HDR][Header(Foam Detail)]_FoamOffshoreDetailColor("Foam Color", Color) = (1,1,1,0)
		[DE_DrawerTextureSingleLine]_FoamOffshoreMapDetail("Foam Detail Map", 2D) = "black" {}
		[DE_DrawerTilingOffset]_FoamOffshoreUVsDetail("Foam UVs", Vector) = (0.25,0.25,0,0)
		_FoamOffshoreSpeed("Foam Speed", Float) = 0.001
		_FoamOffshoreDistance("Foam Distance", Float) = 55
		_FoamOffshoreEdge("Foam Edge", Float) = 10
		_FoamOffshoreDetailStrength("Foam Strength", Range( 0 , 5)) = 1
		[DE_DrawerSpace(10)]_SPACE_FOAMSHORELINE("SPACE_FOAMSHORELINE", Float) = 0
		[DE_DrawerCategory(NORMAL RIPPLE,true,_WaterNormalEnable,0,0)]_CATEGORY_NORMALRIPPLE("CATEGORY_NORMAL RIPPLE", Float) = 0
		[DE_DrawerToggleLeft]_WaterNormalEnable("ENABLE NORMAL MAP", Float) = 1
		[Normal][DE_DrawerTextureSingleLine]_WaterNormalMap("Normal Map", 2D) = "bump" {}
		_WaterNormalStrength("Normal Strength", Float) = 0.1
		[Header(Scale)]_WaterNormalScaleX("Normal Scale X", Float) = 0.45
		_WaterNormalScaleY("Normal Scale Y", Float) = 0.35
		_WaterNormalScaleZ("Normal Scale Z", Float) = 0.17
		_WaterNormalScaleW("Normal Scale W", Float) = 0.4
		[Header(Speed)]_WaterNormalSpeedX("Normal Speed X", Float) = -0.12
		_WaterNormalSpeedY("Normal Speed Y", Float) = 0.02
		_WaterNormalSpeedZ("Normal Speed Z", Float) = -0.1
		_WaterNormalSpeedW("Normal Speed W", Float) = -0.1
		[DE_DrawerSpace(10)]_SPACE_NORMALRIPPLE("SPACE_NORMAL RIPPLE", Float) = 0
		[DE_DrawerCategory(SMOOTHNESS,true,_WaterSmoothnessStrength,0,0)]_CATEGORY_SMOOTHNESS("CATEGORY_SMOOTHNESS", Float) = 1
		_WaterSmoothnessStrength("Smoothness Strength", Range( 0 , 2.5)) = 1
		[DE_DrawerSpace(10)]_SPACE_SMOOTHNESS("SPACE_SMOOTHNESS", Float) = 0
		[DE_DrawerCategory(SPECULAR,true,_WaterSpecularEnable,0,0)]_CATEGORY_SPECULAR("CATEGORY_SPECULAR", Float) = 0
		[DE_DrawerToggleLeft]_WaterSpecularEnable("ENABLE SPECULAR", Float) = 0
		[HDR]_WaterSpecularColor("Specular Color", Color) = (0.08865561,0.1301365,0.1946179,1)
		_WaterSpecularPower("Specular Power", Range( 0 , 1)) = 0
		_WaterSpecularStrengthDielectricIOR("Specular Strength Dielectric IOR", Range( 1 , 2.5)) = 1.1
		_WaterSpecularWrapScale("Specular Wrap Scale", Range( 0 , 5)) = 0.85
		_WaterSpecularWrapOffset("Specular Wrap Offset", Range( 0 , 3)) = 0
		[DE_DrawerSpace(10)]_SPACE_SPECULAR("SPACE_SPECULAR", Float) = 0
		[DE_DrawerCategory(REFRACTION,true,_WaterRefractionScale,0,0)]_CATEGORY_REFRACTION("CATEGORY_REFRACTION", Float) = 0
		_WaterRefractionScale("Refraction Scale", Range( 0 , 1)) = 0.2
		[DE_DrawerSpace(10)]_SPACE_REFRACTION("SPACE_REFRACTION", Float) = 0
		[DE_DrawerCategory(REFLECTION,true,_WaterReflectionEnable,0,0)]_CATEGORY_REFLECTION("CATEGORY_REFLECTION", Float) = 0
		[DE_DrawerToggleLeft]_WaterReflectionEnable("ENABLE REFLECTION", Float) = 0
		[DE_DrawerFloatEnum(CubeMap _Probe)]_WaterReflectionType("Reflection Type", Float) = 0
		[HDR][DE_DrawerTextureSingleLine]_WaterReflectionCubemap("Reflection Cubemap", CUBE) = "white" {}
		_WaterReflectionCloud("Reflection Cloud", Range( 0 , 1)) = 1
		_WaterReflectionWobble("Reflection Wobble", Range( 0 , 0.1)) = 0
		_WaterReflectionSmoothness("Reflection Smoothness", Range( 0 , 2)) = 2
		_WaterReflectionProbeDetailURP("Reflection Probe Detail", Float) = 0
		_WaterReflectionBumpStrength("Reflection Bump Strength", Range( 0 , 1)) = 0.05
		_WaterReflectionBumpScale("Reflection Bump Scale", Range( 0 , 1)) = 0.05
		_WaterReflectionBumpClamp("Reflection Bump Clamp", Range( 0 , 0.15)) = 0.15
		[DE_DrawerToggleNoKeyword]_WaterReflectionEnableFresnel("Enable Fresnel", Float) = 0
		_WaterReflectionFresnelStrength("Fresnel Strength", Range( 0.001 , 1)) = 0.15
		_WaterReflectionFresnelBias("Fresnel Bias", Range( 0 , 1)) = 1
		_WaterReflectionFresnelScale("Fresnel Scale", Range( 0 , 1)) = 0.5
		[DE_DrawerSpace(10)]_SPACE_REFLECTION("SPACE_REFLECTION", Float) = 0
		[DE_DrawerCategory(WAVES GERSTNER,true,_WaveGerstnerEnable,0,0)]_CATEGORY_WAVESGERSTNER("CATEGORY_WAVESGERSTNER", Float) = 0
		[DE_DrawerToggleLeft]_WaveGerstnerEnable("ENABLE WAVES", Float) = 0
		[Header(Wave 01)][DE_DrawerToggleLeft]_WaveGerstner01Enable("Enable Wave 01", Float) = 1
		_WaveGerstner01Direction("Wave 01 Direction", Vector) = (-0.2,0,-0.7,0)
		_WaveGerstner01Speed("Wave 01 Speed", Float) = 0.5
		_WaveGerstner01Length("Wave 01 Length", Float) = 2.5
		_WaveGerstner01Height("Wave 01 Height", Float) = 0.075
		_WaveGerstner01PeakSharpness("Wave 01 Peak Sharpness", Float) = 0.3
		[Header(Wave 02)][DE_DrawerToggleLeft]_WaveGerstner02Enable("Enable Wave 02", Float) = 1
		_WaveGerstner02Direction("Wave 02 Direction", Vector) = (-1,0,0,0)
		_WaveGerstner02Speed("Wave 02 Speed", Float) = 0.5
		_WaveGerstner02Length("Wave 02 Length", Float) = 3
		_WaveGerstner02Height("Wave 02 Height", Float) = 0.05
		_WaveGerstner02PeakSharpness("Wave 02 Peak Sharpness", Float) = 0.3
		[Header(Wave 03)][DE_DrawerToggleLeft]_WaveGerstner03Enable("Enable Wave 03", Float) = 1
		_WaveGerstner03Direction("Wave 03 Direction", Vector) = (-0.5,0,0.5,0)
		_WaveGerstner03Speed("Wave 03 Speed", Float) = 0.5
		_WaveGerstner03Length("Wave 03 Length", Float) = 1.8
		_WaveGerstner03Height("Wave 03 Height", Float) = 0.04
		_WaveGerstner03PeakSharpness("Wave 03 Peak Sharpness", Float) = 0.4
		[Header(Wave 04)][DE_DrawerToggleLeft]_WaveGerstner04Enable("Enable Wave 04", Float) = 1
		_WaveGerstner04Direction("Wave 04 Direction", Vector) = (-0.4,0,0.4,0)
		_WaveGerstner04Speed("Wave 04 Speed", Float) = 0.5
		_WaveGerstner04Length("Wave 04 Length", Float) = 1.8
		_WaveGerstner04Height("Wave 04 Height", Float) = 0.04
		_WaveGerstner04PeakSharpness("Wave 04 Peak Sharpness", Float) = 0.4
		[DE_DrawerToggleLeft][Space(10)]_WaveGerstnerEdgeFadeEnable("ENABLE EDGE FADE", Float) = 1
		_WaveGerstnerEdgeFadeRange("Edge Fade Range", Range( 0 , 250)) = 50
		[DE_DrawerSpace(10)]_SPACE_WAVESGERSTNER("SPACE_WAVESGERSTNER", Float) = 0
		[DE_DrawerCategory(TESSELLATION,true,_TessellationStrength,0,0)]_CATEGORY_TESSELLATION("CATEGORY_TESSELLATION", Float) = 1
		_TessellationStrength("Tessellation Strength", Range( 0.001 , 100)) = 6
		_TessellationPhong("Tessellation Phong", Range( 0 , 1)) = 0.5
		_TessellationDistanceMin("Tessellation Distance Min", Float) = 0
		_TessellationDistanceMax("Tessellation Distance Max ", Float) = 5
		[DE_DrawerSpace(10)]_SPACE_TESSELLATION("SPACE_TESSELLATION", Float) = 0


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Lit" }

		Cull [_Cull]
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.6
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _FORWARD_PLUS

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_FORWARD

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_NORMAL


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD7;
				#endif	
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD8;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				float4 ase_texcoord11 : TEXCOORD11;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);
			TEXTURE2D(_FoamShorelineMap);
			SAMPLER(sampler_FoamShorelineMap);
			TEXTURE2D(_FoamShorelineMapDetail);
			SAMPLER(sampler_FoamShorelineMapDetail);
			TEXTURE2D(_FoamOffshoreMap);
			SAMPLER(sampler_FoamOffshoreMap);
			TEXTURE2D(_FoamOffshoreMapDetail);
			SAMPLER(sampler_FoamOffshoreMapDetail);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			
			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				float4 appendResult3363_g61884 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord9.x = eyeDepth;
				
				float3 temp_output_143_0_g61897 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g61897 = temp_output_143_0_g61897;
				output.ase_texcoord11.xyz = vertexToFrag145_g61897;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord9.yz = input.texcoord.xy;
				output.ase_texcoord10 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord9.w = 0;
				output.ase_texcoord11.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = lerpResult1996_g61884;
				input.tangentOS = appendResult3363_g61884;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x );
				output.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y );
				output.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV( input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy );
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				OUTPUT_SH4( vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir( vertexInput.positionWS ), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						, bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( input.tSpace0.xyz );
					float3 WorldTangent = input.tSpace1.xyz;
					float3 WorldBiTangent = input.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(input.tSpace0.w,input.tSpace1.w,input.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = input.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord9.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord9.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , WorldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( WorldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float3 temp_output_2248_0_g61884 = (temp_output_2481_0_g61884).rgb;
				float2 temp_output_2904_0_g61884 = ( ( input.ase_texcoord9.yz * (_FoamShorelineUVs).xy ) + (_FoamShorelineUVs).zw );
				float4 appendResult2087_g61884 = (float4(WorldPosition.x , WorldPosition.z , WorldPosition.x , WorldPosition.z));
				float4 appendResult2869_g61884 = (float4(_WaterNormalScaleX , _WaterNormalScaleY , _WaterNormalScaleZ , _WaterNormalScaleW));
				float4 temp_output_2088_0_g61884 = ( appendResult2087_g61884 * appendResult2869_g61884 );
				float4 UVRipples2543_g61884 = temp_output_2088_0_g61884;
				float4 break2151_g61884 = ( _FoamShorelineUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2155_g61884 = (float2(break2151_g61884.x , ( break2151_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2172_0_g61884 = ( _TimeParameters.x * _FoamShorelineSpeed );
				float eyeDepth2496_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float3 unityObjectToViewPos2499_g61884 = TransformWorldToView( TransformObjectToWorld( input.ase_texcoord10.xyz) );
				float FoamDistanceShoreline2836_g61884 = saturate( ( ( ( 1.0 - ( eyeDepth2496_g61884 - -unityObjectToViewPos2499_g61884.z ) ) - 1.0 ) / ( ( ( 1.0 - (-99.0 + (_FoamShorelineDistance - 0.0) * (1.0 - -99.0) / (1.0 - 0.0)) ) * 0.01 ) - 1.0 ) ) );
				float3 lerpResult2381_g61884 = lerp( ( ( ( (_FoamShorelineColor).rgb * (SAMPLE_TEXTURE2D( _FoamShorelineMap, sampler_FoamShorelineMap, temp_output_2904_0_g61884 )).rgb * _FoamShorelineStrength ) + ( (_FoamShorelineDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2172_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2172_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2172_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( appendResult2155_g61884 + ( temp_output_2172_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamShorelineDetailStrength ) ) / 3.0 ) , float3( 0,0,0 ) , FoamDistanceShoreline2836_g61884);
				float3 FoamShoreline2343_g61884 = lerpResult2381_g61884;
				float3 lerpResult2269_g61884 = lerp( temp_output_2248_0_g61884 , ( FoamShoreline2343_g61884 + temp_output_2248_0_g61884 ) , ( _FoamShorelineEnable + ( ( _CATEGORY_FOAMSHORELINE + _SPACE_FOAMSHORELINE ) * 0.0 ) ));
				float2 temp_output_2935_0_g61884 = ( ( input.ase_texcoord9.yz * (_FoamOffshoreUVs).xy ) + (_FoamOffshoreUVs).zw );
				float4 break2965_g61884 = ( _FoamOffshoreUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2969_g61884 = (float2(break2965_g61884.x , ( break2965_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2963_0_g61884 = ( _TimeParameters.x * _FoamOffshoreSpeed );
				float FoamDistanceOffshore2834_g61884 = saturate( ( ( ( ( eyeDepth2496_g61884 + unityObjectToViewPos2499_g61884.z ) - _FoamOffshoreEdge ) * ScreenPos.w ) / ( ( _FoamOffshoreDistance - _FoamOffshoreEdge ) * ScreenPos.w ) ) );
				float3 lerpResult2931_g61884 = lerp( float3( 0,0,0 ) , ( ( ( (_FoamOffshoreColor).rgb * (SAMPLE_TEXTURE2D( _FoamOffshoreMap, sampler_FoamOffshoreMap, temp_output_2935_0_g61884 )).rgb * _FoamOffshoreStrength ) + ( (_FoamOffshoreDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2963_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2963_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2963_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( appendResult2969_g61884 + ( temp_output_2963_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamOffshoreDetailStrength ) ) / 3.0 ) , FoamDistanceOffshore2834_g61884);
				float3 FoamOffshore2978_g61884 = lerpResult2931_g61884;
				float3 lerpResult2984_g61884 = lerp( lerpResult2269_g61884 , ( FoamOffshore2978_g61884 + lerpResult2269_g61884 ) , ( _FoamOffshoreEnable + ( ( _CATEGORY_FOAMOFFSHORE + _SPACE_FOAMOFFSHORE ) * 0.0 ) ));
				
				float4 appendResult2874_g61884 = (float4(_WaterNormalSpeedX , _WaterNormalSpeedY , _WaterNormalSpeedZ , _WaterNormalSpeedW));
				float4 temp_output_2093_0_g61884 = ( temp_output_2088_0_g61884 + ( _TimeParameters.x * appendResult2874_g61884 ) );
				float3 unpack2097_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).xy ), _WaterNormalStrength );
				unpack2097_g61884.z = lerp( 1, unpack2097_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2098_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).zw ), _WaterNormalStrength );
				unpack2098_g61884.z = lerp( 1, unpack2098_g61884.z, saturate(_WaterNormalStrength) );
				float4 temp_output_2107_0_g61884 = ( temp_output_2093_0_g61884 * float4( 0.17,0.19,-0.13,0.23 ) );
				float3 unpack2104_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).xy ), _WaterNormalStrength );
				unpack2104_g61884.z = lerp( 1, unpack2104_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2105_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).zw ), _WaterNormalStrength );
				unpack2105_g61884.z = lerp( 1, unpack2105_g61884.z, saturate(_WaterNormalStrength) );
				float3 temp_output_3496_0_g61884 = BlendNormal( BlendNormal( unpack2097_g61884 , unpack2098_g61884 ) , BlendNormal( unpack2104_g61884 , unpack2105_g61884 ) );
				float3 break2345_g61884 = temp_output_3496_0_g61884;
				float3 appendResult2346_g61884 = (float3(break2345_g61884.x , break2345_g61884.y , ( break2345_g61884.z + 0.001 )));
				float3 lerpResult2863_g61884 = lerp( float3(0,0,1) , appendResult2346_g61884 , ( _WaterNormalEnable + ( ( _CATEGORY_NORMALRIPPLE + _SPACE_NORMALRIPPLE ) * 0.0 ) ));
				
				float ase_lightIntensity = max( max( _MainLightColor.r, _MainLightColor.g ), _MainLightColor.b );
				float4 ase_lightColor = float4( _MainLightColor.rgb / ase_lightIntensity, ase_lightIntensity );
				float3 vertexToFrag145_g61897 = input.ase_texcoord11.xyz;
				float3 temp_output_124_0_g61897 = cross( ddy( vertexToFrag145_g61897 ) , ddx( vertexToFrag145_g61897 ) );
				float3 normalizeResult126_g61897 = ASESafeNormalize( temp_output_124_0_g61897 );
				float3 normalizeResult3457_g61884 = normalize( ( ase_viewDirWS + _MainLightPosition.xyz ) );
				float dotResult3230_g61884 = dot( normalizeResult126_g61897 , normalizeResult3457_g61884 );
				float temp_output_3475_0_g61884 = (max( saturate( dotResult3230_g61884 ) , 0.0 )*_WaterSpecularWrapScale + _WaterSpecularWrapOffset);
				float saferPower3427_g61884 = abs( temp_output_3475_0_g61884 );
				float3 lerpResult3436_g61884 = lerp( float3(0,0,0) , saturate( ( ( (_WaterSpecularColor).rgb * ( ase_lightColor.rgb * max( ase_lightColor.a , 0.0 ) ) ) * ( ( pow( saferPower3427_g61884 , _WaterSpecularPower ) * 15.0 ) * ( pow( ( _WaterSpecularStrengthDielectricIOR - 1.0 ) , 2.0 ) / pow( ( _WaterSpecularStrengthDielectricIOR + 1.0 ) , 2.0 ) ) ) ) ) , ( _WaterSpecularEnable + ( ( _CATEGORY_SPECULAR + _SPACE_SPECULAR ) * 0.0 ) ));
				
				float3 _Vector3 = float3(0,0,1);
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , WorldPosition ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3398_g61884 = lerp( input.ase_normal , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1996_g61884 = lerp( input.ase_normal , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				float3 VertexNormal3185_g61884 = lerpResult1996_g61884;
				float3 tanNormal3180_g61884 = VertexNormal3185_g61884;
				float3 worldNormal3180_g61884 = float3(dot(tanToWorld0,tanNormal3180_g61884), dot(tanToWorld1,tanNormal3180_g61884), dot(tanToWorld2,tanNormal3180_g61884));
				float3 temp_output_3171_0_g61884 = ddx( worldNormal3180_g61884 );
				float dotResult3173_g61884 = dot( temp_output_3171_0_g61884 , temp_output_3171_0_g61884 );
				float3 temp_output_3170_0_g61884 = ddy( worldNormal3180_g61884 );
				float dotResult3172_g61884 = dot( temp_output_3170_0_g61884 , temp_output_3170_0_g61884 );
				
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float3 BaseColor = lerpResult2984_g61884;
				float3 Normal = lerpResult2863_g61884;
				float3 Emission = 0;
				float3 Specular = lerpResult3436_g61884;
				float Metallic = 0;
				float Smoothness = saturate( ( ( _WaterSmoothnessStrength + ( ( _CATEGORY_SMOOTHNESS + _SPACE_SMOOTHNESS ) * 0.0 ) ) - min( ( ( dotResult3173_g61884 + dotResult3172_g61884 ) * 30.0 ) , 0.5 ) ) );
				float Occlusion = 1;
				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = input.positionCS;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent, WorldBiTangent, WorldNormal));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = ShadowCoords;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
				#endif

				#ifdef _ASE_LIGHTING_SIMPLE
					half4 color = UniversalFragmentBlinnPhong( inputData, surfaceData);
				#else
					half4 color = UniversalFragmentPBR( inputData, surfaceData);
				#endif

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					#define SUM_LIGHT_TRANSMISSION(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 transmission = max( 0, -dot( inputData.normalWS, Light.direction ) ) * atten * Transmission;\
						color.rgb += BaseColor * transmission;

					SUM_LIGHT_TRANSMISSION( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSMISSION( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSMISSION( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					#define SUM_LIGHT_TRANSLUCENCY(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 lightDir = Light.direction + inputData.normalWS * normal;\
						half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );\
						half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;\
						color.rgb += BaseColor * translucency * strength;

					SUM_LIGHT_TRANSLUCENCY( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSLUCENCY( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSLUCENCY( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						color.rgb = MixFog(color.rgb, inputData.fogCoord);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#pragma multi_compile_instancing
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord3.x = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord4.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord5.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord6.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord3.yz = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						, bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float3 WorldPosition = input.positionWS;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord3.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord3.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord4.xyz;
				float3 ase_worldNormal = input.ase_texcoord5.xyz;
				float3 ase_worldBitangent = input.ase_texcoord6.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma shader_feature EDITOR_VISUALIZATION

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef EDITOR_VISUALIZATION
					float4 VizUV : TEXCOORD2;
					float4 LightCoord : TEXCOORD3;
				#endif
				float4 ase_texcoord4 : TEXCOORD4;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);
			TEXTURE2D(_FoamShorelineMap);
			SAMPLER(sampler_FoamShorelineMap);
			TEXTURE2D(_FoamShorelineMapDetail);
			SAMPLER(sampler_FoamShorelineMapDetail);
			TEXTURE2D(_FoamOffshoreMap);
			SAMPLER(sampler_FoamOffshoreMap);
			TEXTURE2D(_FoamOffshoreMapDetail);
			SAMPLER(sampler_FoamOffshoreMapDetail);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				output.ase_texcoord4 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord5.x = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord6.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord7.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord8.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord5.yz = input.texcoord0.xy;
				output.ase_texcoord9 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;
				output.ase_texcoord7.w = 0;
				output.ase_texcoord8.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = positionWS;
				#endif

				output.positionCS = MetaVertexPosition( input.positionOS, input.texcoord1.xy, input.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

				#ifdef EDITOR_VISUALIZATION
					float2 VizUV = 0;
					float4 LightCoord = 0;
					UnityEditorVizData(input.positionOS.xyz, input.texcoord0.xy, input.texcoord1.xy, input.texcoord2.xy, VizUV, LightCoord);
					output.VizUV = float4(VizUV, 0, 0);
					output.LightCoord = LightCoord;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = output.positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.texcoord0 = input.texcoord0;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.texcoord0 = patch[0].texcoord0 * bary.x + patch[1].texcoord0 * bary.y + patch[2].texcoord0 * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 screenPos = input.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord5.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord5.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord6.xyz;
				float3 ase_worldNormal = input.ase_texcoord7.xyz;
				float3 ase_worldBitangent = input.ase_texcoord8.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float3 temp_output_2248_0_g61884 = (temp_output_2481_0_g61884).rgb;
				float2 temp_output_2904_0_g61884 = ( ( input.ase_texcoord5.yz * (_FoamShorelineUVs).xy ) + (_FoamShorelineUVs).zw );
				float4 appendResult2087_g61884 = (float4(WorldPosition.x , WorldPosition.z , WorldPosition.x , WorldPosition.z));
				float4 appendResult2869_g61884 = (float4(_WaterNormalScaleX , _WaterNormalScaleY , _WaterNormalScaleZ , _WaterNormalScaleW));
				float4 temp_output_2088_0_g61884 = ( appendResult2087_g61884 * appendResult2869_g61884 );
				float4 UVRipples2543_g61884 = temp_output_2088_0_g61884;
				float4 break2151_g61884 = ( _FoamShorelineUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2155_g61884 = (float2(break2151_g61884.x , ( break2151_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2172_0_g61884 = ( _TimeParameters.x * _FoamShorelineSpeed );
				float eyeDepth2496_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float3 unityObjectToViewPos2499_g61884 = TransformWorldToView( TransformObjectToWorld( input.ase_texcoord9.xyz) );
				float FoamDistanceShoreline2836_g61884 = saturate( ( ( ( 1.0 - ( eyeDepth2496_g61884 - -unityObjectToViewPos2499_g61884.z ) ) - 1.0 ) / ( ( ( 1.0 - (-99.0 + (_FoamShorelineDistance - 0.0) * (1.0 - -99.0) / (1.0 - 0.0)) ) * 0.01 ) - 1.0 ) ) );
				float3 lerpResult2381_g61884 = lerp( ( ( ( (_FoamShorelineColor).rgb * (SAMPLE_TEXTURE2D( _FoamShorelineMap, sampler_FoamShorelineMap, temp_output_2904_0_g61884 )).rgb * _FoamShorelineStrength ) + ( (_FoamShorelineDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2172_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2172_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2172_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( appendResult2155_g61884 + ( temp_output_2172_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamShorelineDetailStrength ) ) / 3.0 ) , float3( 0,0,0 ) , FoamDistanceShoreline2836_g61884);
				float3 FoamShoreline2343_g61884 = lerpResult2381_g61884;
				float3 lerpResult2269_g61884 = lerp( temp_output_2248_0_g61884 , ( FoamShoreline2343_g61884 + temp_output_2248_0_g61884 ) , ( _FoamShorelineEnable + ( ( _CATEGORY_FOAMSHORELINE + _SPACE_FOAMSHORELINE ) * 0.0 ) ));
				float2 temp_output_2935_0_g61884 = ( ( input.ase_texcoord5.yz * (_FoamOffshoreUVs).xy ) + (_FoamOffshoreUVs).zw );
				float4 break2965_g61884 = ( _FoamOffshoreUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2969_g61884 = (float2(break2965_g61884.x , ( break2965_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2963_0_g61884 = ( _TimeParameters.x * _FoamOffshoreSpeed );
				float FoamDistanceOffshore2834_g61884 = saturate( ( ( ( ( eyeDepth2496_g61884 + unityObjectToViewPos2499_g61884.z ) - _FoamOffshoreEdge ) * screenPos.w ) / ( ( _FoamOffshoreDistance - _FoamOffshoreEdge ) * screenPos.w ) ) );
				float3 lerpResult2931_g61884 = lerp( float3( 0,0,0 ) , ( ( ( (_FoamOffshoreColor).rgb * (SAMPLE_TEXTURE2D( _FoamOffshoreMap, sampler_FoamOffshoreMap, temp_output_2935_0_g61884 )).rgb * _FoamOffshoreStrength ) + ( (_FoamOffshoreDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2963_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2963_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2963_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( appendResult2969_g61884 + ( temp_output_2963_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamOffshoreDetailStrength ) ) / 3.0 ) , FoamDistanceOffshore2834_g61884);
				float3 FoamOffshore2978_g61884 = lerpResult2931_g61884;
				float3 lerpResult2984_g61884 = lerp( lerpResult2269_g61884 , ( FoamOffshore2978_g61884 + lerpResult2269_g61884 ) , ( _FoamOffshoreEnable + ( ( _CATEGORY_FOAMOFFSHORE + _SPACE_FOAMOFFSHORE ) * 0.0 ) ));
				
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float3 BaseColor = lerpResult2984_g61884;
				float3 Emission = 0;
				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;
				#ifdef EDITOR_VISUALIZATION
					metaInput.VizUV = input.VizUV.xy;
					metaInput.LightCoord = input.LightCoord;
				#endif

				return UnityMetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);
			TEXTURE2D(_FoamShorelineMap);
			SAMPLER(sampler_FoamShorelineMap);
			TEXTURE2D(_FoamShorelineMapDetail);
			SAMPLER(sampler_FoamShorelineMapDetail);
			TEXTURE2D(_FoamOffshoreMap);
			SAMPLER(sampler_FoamOffshoreMap);
			TEXTURE2D(_FoamOffshoreMapDetail);
			SAMPLER(sampler_FoamOffshoreMapDetail);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				output.ase_texcoord2 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord3.x = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord4.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord5.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord6.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord3.yz = input.ase_texcoord.xy;
				output.ase_texcoord7 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 screenPos = input.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord3.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord3.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord4.xyz;
				float3 ase_worldNormal = input.ase_texcoord5.xyz;
				float3 ase_worldBitangent = input.ase_texcoord6.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float3 temp_output_2248_0_g61884 = (temp_output_2481_0_g61884).rgb;
				float2 temp_output_2904_0_g61884 = ( ( input.ase_texcoord3.yz * (_FoamShorelineUVs).xy ) + (_FoamShorelineUVs).zw );
				float4 appendResult2087_g61884 = (float4(WorldPosition.x , WorldPosition.z , WorldPosition.x , WorldPosition.z));
				float4 appendResult2869_g61884 = (float4(_WaterNormalScaleX , _WaterNormalScaleY , _WaterNormalScaleZ , _WaterNormalScaleW));
				float4 temp_output_2088_0_g61884 = ( appendResult2087_g61884 * appendResult2869_g61884 );
				float4 UVRipples2543_g61884 = temp_output_2088_0_g61884;
				float4 break2151_g61884 = ( _FoamShorelineUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2155_g61884 = (float2(break2151_g61884.x , ( break2151_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2172_0_g61884 = ( _TimeParameters.x * _FoamShorelineSpeed );
				float eyeDepth2496_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float3 unityObjectToViewPos2499_g61884 = TransformWorldToView( TransformObjectToWorld( input.ase_texcoord7.xyz) );
				float FoamDistanceShoreline2836_g61884 = saturate( ( ( ( 1.0 - ( eyeDepth2496_g61884 - -unityObjectToViewPos2499_g61884.z ) ) - 1.0 ) / ( ( ( 1.0 - (-99.0 + (_FoamShorelineDistance - 0.0) * (1.0 - -99.0) / (1.0 - 0.0)) ) * 0.01 ) - 1.0 ) ) );
				float3 lerpResult2381_g61884 = lerp( ( ( ( (_FoamShorelineColor).rgb * (SAMPLE_TEXTURE2D( _FoamShorelineMap, sampler_FoamShorelineMap, temp_output_2904_0_g61884 )).rgb * _FoamShorelineStrength ) + ( (_FoamShorelineDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2172_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2172_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2172_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( appendResult2155_g61884 + ( temp_output_2172_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamShorelineDetailStrength ) ) / 3.0 ) , float3( 0,0,0 ) , FoamDistanceShoreline2836_g61884);
				float3 FoamShoreline2343_g61884 = lerpResult2381_g61884;
				float3 lerpResult2269_g61884 = lerp( temp_output_2248_0_g61884 , ( FoamShoreline2343_g61884 + temp_output_2248_0_g61884 ) , ( _FoamShorelineEnable + ( ( _CATEGORY_FOAMSHORELINE + _SPACE_FOAMSHORELINE ) * 0.0 ) ));
				float2 temp_output_2935_0_g61884 = ( ( input.ase_texcoord3.yz * (_FoamOffshoreUVs).xy ) + (_FoamOffshoreUVs).zw );
				float4 break2965_g61884 = ( _FoamOffshoreUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2969_g61884 = (float2(break2965_g61884.x , ( break2965_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2963_0_g61884 = ( _TimeParameters.x * _FoamOffshoreSpeed );
				float FoamDistanceOffshore2834_g61884 = saturate( ( ( ( ( eyeDepth2496_g61884 + unityObjectToViewPos2499_g61884.z ) - _FoamOffshoreEdge ) * screenPos.w ) / ( ( _FoamOffshoreDistance - _FoamOffshoreEdge ) * screenPos.w ) ) );
				float3 lerpResult2931_g61884 = lerp( float3( 0,0,0 ) , ( ( ( (_FoamOffshoreColor).rgb * (SAMPLE_TEXTURE2D( _FoamOffshoreMap, sampler_FoamOffshoreMap, temp_output_2935_0_g61884 )).rgb * _FoamOffshoreStrength ) + ( (_FoamOffshoreDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2963_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2963_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2963_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( appendResult2969_g61884 + ( temp_output_2963_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamOffshoreDetailStrength ) ) / 3.0 ) , FoamDistanceOffshore2834_g61884);
				float3 FoamOffshore2978_g61884 = lerpResult2931_g61884;
				float3 lerpResult2984_g61884 = lerp( lerpResult2269_g61884 , ( FoamOffshore2978_g61884 + lerpResult2269_g61884 ) , ( _FoamOffshoreEnable + ( ( _CATEGORY_FOAMOFFSHORE + _SPACE_FOAMOFFSHORE ) * 0.0 ) ));
				
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float3 BaseColor = lerpResult2984_g61884;
				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#pragma multi_compile_instancing
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
			//#define SHADERPASS SHADERPASS_DEPTHNORMALS

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_VERT_TANGENT


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 tangentWS : TEXCOORD3;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD4;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			TEXTURECUBE(_WaterReflectionCubemap);
			SAMPLER(sampler_WaterReflectionCubemap);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				float4 appendResult3363_g61884 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord5.x = eyeDepth;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
				float ase_vertexTangentSign = input.tangentOS.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord6.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord5.yz = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;
				input.tangentOS = appendResult3363_g61884;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				float3 normalWS = TransformObjectToWorldNormal( input.normalOS );
				float4 tangentWS = float4( TransformObjectToWorldDir( input.tangentOS.xyz ), input.tangentOS.w );

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalWS;
				output.tangentWS = tangentWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(	PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						, bool ase_vface : SV_IsFrontFace )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float3 WorldNormal = input.normalWS;
				float4 WorldTangent = input.tangentWS;
				float3 WorldPosition = input.positionWS;
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 appendResult2087_g61884 = (float4(WorldPosition.x , WorldPosition.z , WorldPosition.x , WorldPosition.z));
				float4 appendResult2869_g61884 = (float4(_WaterNormalScaleX , _WaterNormalScaleY , _WaterNormalScaleZ , _WaterNormalScaleW));
				float4 temp_output_2088_0_g61884 = ( appendResult2087_g61884 * appendResult2869_g61884 );
				float4 appendResult2874_g61884 = (float4(_WaterNormalSpeedX , _WaterNormalSpeedY , _WaterNormalSpeedZ , _WaterNormalSpeedW));
				float4 temp_output_2093_0_g61884 = ( temp_output_2088_0_g61884 + ( _TimeParameters.x * appendResult2874_g61884 ) );
				float3 unpack2097_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).xy ), _WaterNormalStrength );
				unpack2097_g61884.z = lerp( 1, unpack2097_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2098_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).zw ), _WaterNormalStrength );
				unpack2098_g61884.z = lerp( 1, unpack2098_g61884.z, saturate(_WaterNormalStrength) );
				float4 temp_output_2107_0_g61884 = ( temp_output_2093_0_g61884 * float4( 0.17,0.19,-0.13,0.23 ) );
				float3 unpack2104_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).xy ), _WaterNormalStrength );
				unpack2104_g61884.z = lerp( 1, unpack2104_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2105_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).zw ), _WaterNormalStrength );
				unpack2105_g61884.z = lerp( 1, unpack2105_g61884.z, saturate(_WaterNormalStrength) );
				float3 temp_output_3496_0_g61884 = BlendNormal( BlendNormal( unpack2097_g61884 , unpack2098_g61884 ) , BlendNormal( unpack2104_g61884 , unpack2105_g61884 ) );
				float3 break2345_g61884 = temp_output_3496_0_g61884;
				float3 appendResult2346_g61884 = (float3(break2345_g61884.x , break2345_g61884.y , ( break2345_g61884.z + 0.001 )));
				float3 lerpResult2863_g61884 = lerp( float3(0,0,1) , appendResult2346_g61884 , ( _WaterNormalEnable + ( ( _CATEGORY_NORMALRIPPLE + _SPACE_NORMALRIPPLE ) * 0.0 ) ));
				
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord5.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord5.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldBitangent = input.ase_texcoord6.xyz;
				float3 tanToWorld0 = float3( WorldTangent.xyz.x, ase_worldBitangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.xyz.y, ase_worldBitangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.xyz.z, ase_worldBitangent.z, WorldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , WorldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( WorldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float3 Normal = lerpResult2863_g61884;
				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(WorldNormal);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float crossSign = (WorldTangent.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
							float3 bitangent = crossSign * cross(WorldNormal.xyz, WorldTangent.xyz);
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent.xyz, bitangent, WorldNormal.xyz));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = WorldNormal;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_GBUFFER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif
			
			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_NORMAL


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD8;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				float4 ase_texcoord11 : TEXCOORD11;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);
			TEXTURE2D(_FoamShorelineMap);
			SAMPLER(sampler_FoamShorelineMap);
			TEXTURE2D(_FoamShorelineMapDetail);
			SAMPLER(sampler_FoamShorelineMapDetail);
			TEXTURE2D(_FoamOffshoreMap);
			SAMPLER(sampler_FoamOffshoreMap);
			TEXTURE2D(_FoamOffshoreMapDetail);
			SAMPLER(sampler_FoamOffshoreMapDetail);


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			
			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				float4 appendResult3363_g61884 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord9.x = eyeDepth;
				
				float3 temp_output_143_0_g61897 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g61897 = temp_output_143_0_g61897;
				output.ase_texcoord11.xyz = vertexToFrag145_g61897;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord9.yz = input.texcoord.xy;
				output.ase_texcoord10 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord9.w = 0;
				output.ase_texcoord11.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;
				input.tangentOS = appendResult3363_g61884;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x);
				output.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y);
				output.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				OUTPUT_SH4( vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir( vertexInput.positionWS ), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						// @diogo: no fog applied in GBuffer
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			FragmentOutput frag ( PackedVaryings input
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								, bool ase_vface : SV_IsFrontFace )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( input.tSpace0.xyz );
					float3 WorldTangent = input.tSpace1.xyz;
					float3 WorldBiTangent = input.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(input.tSpace0.w,input.tSpace1.w,input.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = input.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , WorldPosition ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord9.x;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord9.yz ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , WorldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( WorldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float3 temp_output_2248_0_g61884 = (temp_output_2481_0_g61884).rgb;
				float2 temp_output_2904_0_g61884 = ( ( input.ase_texcoord9.yz * (_FoamShorelineUVs).xy ) + (_FoamShorelineUVs).zw );
				float4 appendResult2087_g61884 = (float4(WorldPosition.x , WorldPosition.z , WorldPosition.x , WorldPosition.z));
				float4 appendResult2869_g61884 = (float4(_WaterNormalScaleX , _WaterNormalScaleY , _WaterNormalScaleZ , _WaterNormalScaleW));
				float4 temp_output_2088_0_g61884 = ( appendResult2087_g61884 * appendResult2869_g61884 );
				float4 UVRipples2543_g61884 = temp_output_2088_0_g61884;
				float4 break2151_g61884 = ( _FoamShorelineUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2155_g61884 = (float2(break2151_g61884.x , ( break2151_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2172_0_g61884 = ( _TimeParameters.x * _FoamShorelineSpeed );
				float eyeDepth2496_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float3 unityObjectToViewPos2499_g61884 = TransformWorldToView( TransformObjectToWorld( input.ase_texcoord10.xyz) );
				float FoamDistanceShoreline2836_g61884 = saturate( ( ( ( 1.0 - ( eyeDepth2496_g61884 - -unityObjectToViewPos2499_g61884.z ) ) - 1.0 ) / ( ( ( 1.0 - (-99.0 + (_FoamShorelineDistance - 0.0) * (1.0 - -99.0) / (1.0 - 0.0)) ) * 0.01 ) - 1.0 ) ) );
				float3 lerpResult2381_g61884 = lerp( ( ( ( (_FoamShorelineColor).rgb * (SAMPLE_TEXTURE2D( _FoamShorelineMap, sampler_FoamShorelineMap, temp_output_2904_0_g61884 )).rgb * _FoamShorelineStrength ) + ( (_FoamShorelineDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2172_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2172_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( ( appendResult2155_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2172_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamShorelineMapDetail, sampler_FoamShorelineMapDetail, ( appendResult2155_g61884 + ( temp_output_2172_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamShorelineDetailStrength ) ) / 3.0 ) , float3( 0,0,0 ) , FoamDistanceShoreline2836_g61884);
				float3 FoamShoreline2343_g61884 = lerpResult2381_g61884;
				float3 lerpResult2269_g61884 = lerp( temp_output_2248_0_g61884 , ( FoamShoreline2343_g61884 + temp_output_2248_0_g61884 ) , ( _FoamShorelineEnable + ( ( _CATEGORY_FOAMSHORELINE + _SPACE_FOAMSHORELINE ) * 0.0 ) ));
				float2 temp_output_2935_0_g61884 = ( ( input.ase_texcoord9.yz * (_FoamOffshoreUVs).xy ) + (_FoamOffshoreUVs).zw );
				float4 break2965_g61884 = ( _FoamOffshoreUVsDetail * UVRipples2543_g61884 );
				float2 appendResult2969_g61884 = (float2(break2965_g61884.x , ( break2965_g61884.y + ( _TimeParameters.x * 0.02 ) )));
				float temp_output_2963_0_g61884 = ( _TimeParameters.x * _FoamOffshoreSpeed );
				float FoamDistanceOffshore2834_g61884 = saturate( ( ( ( ( eyeDepth2496_g61884 + unityObjectToViewPos2499_g61884.z ) - _FoamOffshoreEdge ) * ScreenPos.w ) / ( ( _FoamOffshoreDistance - _FoamOffshoreEdge ) * ScreenPos.w ) ) );
				float3 lerpResult2931_g61884 = lerp( float3( 0,0,0 ) , ( ( ( (_FoamOffshoreColor).rgb * (SAMPLE_TEXTURE2D( _FoamOffshoreMap, sampler_FoamOffshoreMap, temp_output_2935_0_g61884 )).rgb * _FoamOffshoreStrength ) + ( (_FoamOffshoreDetailColor).rgb * (( ( ( ( SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.4181,0.3548 ) ) + ( temp_output_2963_0_g61884 * float2( 1,1 ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.864861,0.148384 ) ) + ( temp_output_2963_0_g61884 * float2( -1,-1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( ( appendResult2969_g61884 + float2( 0.65134,0.751638 ) ) + ( temp_output_2963_0_g61884 * float2( -1,1 ) ) ) ) ) + SAMPLE_TEXTURE2D( _FoamOffshoreMapDetail, sampler_FoamOffshoreMapDetail, ( appendResult2969_g61884 + ( temp_output_2963_0_g61884 * float2( 1,-1 ) ) ) ) ) * 0.25 )).rgb * _FoamOffshoreDetailStrength ) ) / 3.0 ) , FoamDistanceOffshore2834_g61884);
				float3 FoamOffshore2978_g61884 = lerpResult2931_g61884;
				float3 lerpResult2984_g61884 = lerp( lerpResult2269_g61884 , ( FoamOffshore2978_g61884 + lerpResult2269_g61884 ) , ( _FoamOffshoreEnable + ( ( _CATEGORY_FOAMOFFSHORE + _SPACE_FOAMOFFSHORE ) * 0.0 ) ));
				
				float4 appendResult2874_g61884 = (float4(_WaterNormalSpeedX , _WaterNormalSpeedY , _WaterNormalSpeedZ , _WaterNormalSpeedW));
				float4 temp_output_2093_0_g61884 = ( temp_output_2088_0_g61884 + ( _TimeParameters.x * appendResult2874_g61884 ) );
				float3 unpack2097_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).xy ), _WaterNormalStrength );
				unpack2097_g61884.z = lerp( 1, unpack2097_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2098_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2093_0_g61884).zw ), _WaterNormalStrength );
				unpack2098_g61884.z = lerp( 1, unpack2098_g61884.z, saturate(_WaterNormalStrength) );
				float4 temp_output_2107_0_g61884 = ( temp_output_2093_0_g61884 * float4( 0.17,0.19,-0.13,0.23 ) );
				float3 unpack2104_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).xy ), _WaterNormalStrength );
				unpack2104_g61884.z = lerp( 1, unpack2104_g61884.z, saturate(_WaterNormalStrength) );
				float3 unpack2105_g61884 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, (temp_output_2107_0_g61884).zw ), _WaterNormalStrength );
				unpack2105_g61884.z = lerp( 1, unpack2105_g61884.z, saturate(_WaterNormalStrength) );
				float3 temp_output_3496_0_g61884 = BlendNormal( BlendNormal( unpack2097_g61884 , unpack2098_g61884 ) , BlendNormal( unpack2104_g61884 , unpack2105_g61884 ) );
				float3 break2345_g61884 = temp_output_3496_0_g61884;
				float3 appendResult2346_g61884 = (float3(break2345_g61884.x , break2345_g61884.y , ( break2345_g61884.z + 0.001 )));
				float3 lerpResult2863_g61884 = lerp( float3(0,0,1) , appendResult2346_g61884 , ( _WaterNormalEnable + ( ( _CATEGORY_NORMALRIPPLE + _SPACE_NORMALRIPPLE ) * 0.0 ) ));
				
				float ase_lightIntensity = max( max( _MainLightColor.r, _MainLightColor.g ), _MainLightColor.b );
				float4 ase_lightColor = float4( _MainLightColor.rgb / ase_lightIntensity, ase_lightIntensity );
				float3 vertexToFrag145_g61897 = input.ase_texcoord11.xyz;
				float3 temp_output_124_0_g61897 = cross( ddy( vertexToFrag145_g61897 ) , ddx( vertexToFrag145_g61897 ) );
				float3 normalizeResult126_g61897 = ASESafeNormalize( temp_output_124_0_g61897 );
				float3 normalizeResult3457_g61884 = normalize( ( ase_viewDirWS + _MainLightPosition.xyz ) );
				float dotResult3230_g61884 = dot( normalizeResult126_g61897 , normalizeResult3457_g61884 );
				float temp_output_3475_0_g61884 = (max( saturate( dotResult3230_g61884 ) , 0.0 )*_WaterSpecularWrapScale + _WaterSpecularWrapOffset);
				float saferPower3427_g61884 = abs( temp_output_3475_0_g61884 );
				float3 lerpResult3436_g61884 = lerp( float3(0,0,0) , saturate( ( ( (_WaterSpecularColor).rgb * ( ase_lightColor.rgb * max( ase_lightColor.a , 0.0 ) ) ) * ( ( pow( saferPower3427_g61884 , _WaterSpecularPower ) * 15.0 ) * ( pow( ( _WaterSpecularStrengthDielectricIOR - 1.0 ) , 2.0 ) / pow( ( _WaterSpecularStrengthDielectricIOR + 1.0 ) , 2.0 ) ) ) ) ) , ( _WaterSpecularEnable + ( ( _CATEGORY_SPECULAR + _SPACE_SPECULAR ) * 0.0 ) ));
				
				float3 _Vector3 = float3(0,0,1);
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , WorldPosition ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3398_g61884 = lerp( input.ase_normal , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1996_g61884 = lerp( input.ase_normal , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				float3 VertexNormal3185_g61884 = lerpResult1996_g61884;
				float3 tanNormal3180_g61884 = VertexNormal3185_g61884;
				float3 worldNormal3180_g61884 = float3(dot(tanToWorld0,tanNormal3180_g61884), dot(tanToWorld1,tanNormal3180_g61884), dot(tanToWorld2,tanNormal3180_g61884));
				float3 temp_output_3171_0_g61884 = ddx( worldNormal3180_g61884 );
				float dotResult3173_g61884 = dot( temp_output_3171_0_g61884 , temp_output_3171_0_g61884 );
				float3 temp_output_3170_0_g61884 = ddy( worldNormal3180_g61884 );
				float dotResult3172_g61884 = dot( temp_output_3170_0_g61884 , temp_output_3170_0_g61884 );
				
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float3 BaseColor = lerpResult2984_g61884;
				float3 Normal = lerpResult2863_g61884;
				float3 Emission = 0;
				float3 Specular = lerpResult3436_g61884;
				float Metallic = 0;
				float Smoothness = saturate( ( ( _WaterSmoothnessStrength + ( ( _CATEGORY_SMOOTHNESS + _SPACE_SMOOTHNESS ) * 0.0 ) ) - min( ( ( dotResult3173_g61884 + dotResult3172_g61884 ) * 30.0 ) , 0.5 ) ) );
				float Occlusion = 1;
				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = input.positionCS;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = WorldNormal;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( WorldViewDirection );

				#ifdef ASE_FOG
					// @diogo: no fog applied in GBuffer
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(input.positionCS,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData
				(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				output.ase_texcoord = screenPos;
				output.ase_texcoord1.xyz = ase_worldPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord1.w = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord3.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord5.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 screenPos = input.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float3 ase_worldPos = input.ase_texcoord1.xyz;
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , ase_worldPos ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord1.w;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord2.xy ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord3.xyz;
				float3 ase_worldNormal = input.ase_texcoord4.xyz;
				float3 ase_worldBitangent = input.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				surfaceDescription.Alpha = clampResult3169_g61884;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				float3 _Vector3 = float3(0,0,1);
				float3 break452_g61886 = ( ( temp_output_426_0_g61886 * temp_output_431_0_g61886 ) * WaveDirection429_g61886 );
				float3 appendResult454_g61886 = (float3(break452_g61886.x , ( 1.0 - ( ( temp_output_422_0_g61886 * temp_output_431_0_g61886 ) * temp_output_435_0_g61886 ) ) , break452_g61886.z));
				float3 lerpResult2420_g61884 = lerp( _Vector3 , appendResult454_g61886 , _WaveGerstner01Enable);
				float3 break452_g61891 = ( ( temp_output_426_0_g61891 * temp_output_431_0_g61891 ) * WaveDirection429_g61891 );
				float3 appendResult454_g61891 = (float3(break452_g61891.x , ( 1.0 - ( ( temp_output_422_0_g61891 * temp_output_431_0_g61891 ) * temp_output_435_0_g61891 ) ) , break452_g61891.z));
				float3 lerpResult2422_g61884 = lerp( _Vector3 , appendResult454_g61891 , _WaveGerstner02Enable);
				float3 break452_g61890 = ( ( temp_output_426_0_g61890 * temp_output_431_0_g61890 ) * WaveDirection429_g61890 );
				float3 appendResult454_g61890 = (float3(break452_g61890.x , ( 1.0 - ( ( temp_output_422_0_g61890 * temp_output_431_0_g61890 ) * temp_output_435_0_g61890 ) ) , break452_g61890.z));
				float3 lerpResult2423_g61884 = lerp( _Vector3 , appendResult454_g61890 , _WaveGerstner03Enable);
				float3 break452_g61894 = ( ( temp_output_426_0_g61894 * temp_output_431_0_g61894 ) * WaveDirection429_g61894 );
				float3 appendResult454_g61894 = (float3(break452_g61894.x , ( 1.0 - ( ( temp_output_422_0_g61894 * temp_output_431_0_g61894 ) * temp_output_435_0_g61894 ) ) , break452_g61894.z));
				float3 lerpResult3195_g61884 = lerp( _Vector3 , appendResult454_g61894 , _WaveGerstner04Enable);
				float4 weightedBlendVar3205_g61884 = float4(0.25,0.25,0.25,0.25);
				float3 weightedBlend3205_g61884 = ( weightedBlendVar3205_g61884.x*lerpResult2420_g61884 + weightedBlendVar3205_g61884.y*lerpResult2422_g61884 + weightedBlendVar3205_g61884.z*lerpResult2423_g61884 + weightedBlendVar3205_g61884.w*lerpResult3195_g61884 );
				float3 lerpResult3398_g61884 = lerp( input.normalOS , weightedBlend3205_g61884 , Fade3386_g61884);
				float3 lerpResult3397_g61884 = lerp( weightedBlend3205_g61884 , lerpResult3398_g61884 , _WaveGerstnerEdgeFadeEnable);
				float3 lerpResult1996_g61884 = lerp( input.normalOS , lerpResult3397_g61884 , temp_output_1991_0_g61884);
				
				output.ase_texcoord = screenPos;
				output.ase_texcoord1.xyz = ase_worldPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord1.w = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord3.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord5.xyz = ase_worldBitangent;
				
				output.ase_normal = input.normalOS;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = lerpResult1996_g61884;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 screenPos = input.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float3 ase_worldPos = input.ase_texcoord1.xyz;
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , ase_worldPos ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord1.w;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord2.xy ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord3.xyz;
				float3 ase_worldNormal = input.ase_texcoord4.xyz;
				float3 ase_worldBitangent = input.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				surfaceDescription.Alpha = clampResult3169_g61884;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
						clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			ColorMask RG

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SPECULAR_SETUP 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#pragma multi_compile_instancing
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif
	
            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

			#define ASE_NEEDS_VERT_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _FoamOffshoreUVsDetail;
			half4 _FoamOffshoreColor;
			half4 _WaterDepthTint;
			half4 _WaterShorelineTint;
			half4 _WaterMidwaterTint;
			half4 _FoamOffshoreDetailColor;
			float4 _FoamShorelineUVsDetail;
			half4 _FoamShorelineDetailColor;
			half4 _WaterSpecularColor;
			float4 _FoamOffshoreUVs;
			float4 _FoamShorelineUVs;
			half4 _FoamShorelineColor;
			float3 _WaveGerstner02Direction;
			float3 _WaveGerstner04Direction;
			float3 _WaveGerstner01Direction;
			float3 _WaveGerstner03Direction;
			half _WaterReflectionFresnelScale;
			float _SPACE_FOAMSHORELINE;
			float _CATEGORY_FOAMSHORELINE;
			half _FoamShorelineEnable;
			half _FoamShorelineDistance;
			half _WaterReflectionEnableFresnel;
			float _CATEGORY_REFLECTION;
			half _FoamShorelineDetailStrength;
			float _FoamShorelineSpeed;
			half _WaterNormalScaleW;
			half _WaterNormalScaleZ;
			float _SPACE_REFLECTION;
			half _WaterNormalScaleY;
			half _WaterNormalScaleX;
			half _WaterReflectionEnable;
			half _FoamShorelineStrength;
			float _CATEGORY_COLORWATERLAYERS;
			half _WaterReflectionFresnelBias;
			half _WaterSmoothnessStrength;
			float _SPACE_SPECULAR;
			float _CATEGORY_SPECULAR;
			half _WaterSpecularEnable;
			half _WaterSpecularStrengthDielectricIOR;
			half _WaterSpecularPower;
			half _WaterSpecularWrapOffset;
			half _WaterSpecularWrapScale;
			float _SPACE_NORMALRIPPLE;
			float _CATEGORY_NORMALRIPPLE;
			half _WaterNormalEnable;
			float _WaterNormalStrength;
			half _WaterNormalSpeedW;
			half _WaterNormalSpeedZ;
			half _WaterNormalSpeedY;
			half _WaterNormalSpeedX;
			float _SPACE_FOAMOFFSHORE;
			float _CATEGORY_FOAMOFFSHORE;
			half _FoamOffshoreEnable;
			half _FoamOffshoreDistance;
			half _FoamOffshoreEdge;
			half _FoamOffshoreDetailStrength;
			float _FoamOffshoreSpeed;
			half _FoamOffshoreStrength;
			half _WaterReflectionFresnelStrength;
			half _WaterReflectionSmoothness;
			half _WaterReflectionProbeDetailURP;
			half _WaveGerstner03PeakSharpness;
			half _WaveGerstner03Height;
			half _WaveGerstner03Speed;
			half _WaveGerstner03Length;
			half _WaveGerstner02Enable;
			half _WaveGerstner02PeakSharpness;
			half _WaveGerstner02Height;
			half _WaveGerstner02Speed;
			half _WaveGerstner02Length;
			half _WaveGerstner01Enable;
			half _WaveGerstner03Enable;
			half _WaveGerstner01PeakSharpness;
			half _WaveGerstner01Speed;
			half _WaveGerstner01Length;
			float _CATEGORY_TESSELLATION;
			float _SPACE_TESSELLATION;
			half _TessellationPhong;
			half _TessellationDistanceMax;
			half _TessellationDistanceMin;
			half _TessellationStrength;
			int _Cull;
			float _SPACE_COLORWATERLAYERS;
			half _WaveGerstner01Height;
			half _WaveGerstner04Length;
			half _WaveGerstner04Speed;
			half _WaveGerstner04Height;
			half _WaterReflectionCloud;
			float _CATEGORY_SMOOTHNESS;
			half _WaterReflectionWobble;
			half _WaterReflectionBumpClamp;
			half _WaterReflectionBumpScale;
			half _WaterReflectionBumpStrength;
			float _SPACE_REFRACTION;
			float _CATEGORY_REFRACTION;
			half _WaterRefractionScale;
			float _SPACE_OPACITY;
			float _CATEGORY_OPACITY;
			half _WaterOpacity;
			half _WaterOpacityShoreline;
			half _WaterDepthOffset;
			half _WaterShorelineOffset;
			half _WaterShorelineDepth;
			float _SPACE_WAVESGERSTNER;
			float _CATEGORY_WAVESGERSTNER;
			half _WaveGerstnerEnable;
			float _WaveGerstnerEdgeFadeEnable;
			float _WaveGerstnerEdgeFadeRange;
			half _WaveGerstner04Enable;
			half _WaveGerstner04PeakSharpness;
			half _WaterReflectionType;
			float _SPACE_SMOOTHNESS;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURECUBE(_WaterReflectionCubemap);
			TEXTURE2D(_WaterNormalMap);
			SAMPLER(sampler_WaterNormalMap);
			SAMPLER(sampler_WaterReflectionCubemap);


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 worldToObj2005_g61884 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 worldToObj419_g61886 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61886 = normalize( _WaveGerstner01Direction );
				float temp_output_409_0_g61886 = ( TWO_PI / _WaveGerstner01Length );
				float dotResult417_g61886 = dot( worldToObj419_g61886 , ( normalizeResult407_g61886 * temp_output_409_0_g61886 ) );
				float temp_output_421_0_g61886 = ( dotResult417_g61886 - ( sqrt( ( temp_output_409_0_g61886 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner01Speed ) ) );
				float temp_output_422_0_g61886 = cos( temp_output_421_0_g61886 );
				float temp_output_432_0_g61886 = _WaveGerstner01Height;
				float WaveHeight433_g61886 = temp_output_432_0_g61886;
				float3 WaveDirection429_g61886 = normalizeResult407_g61886;
				float temp_output_426_0_g61886 = sin( temp_output_421_0_g61886 );
				float temp_output_431_0_g61886 = ( temp_output_409_0_g61886 * temp_output_432_0_g61886 );
				float temp_output_435_0_g61886 = ( _WaveGerstner01PeakSharpness / temp_output_431_0_g61886 );
				float3 lerpResult2419_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61886 * float3(0,1,0) ) * WaveHeight433_g61886 ) - ( WaveDirection429_g61886 * ( temp_output_426_0_g61886 * ( temp_output_435_0_g61886 * temp_output_432_0_g61886 ) ) ) ) , _WaveGerstner01Enable);
				float3 worldToObj419_g61891 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61891 = normalize( _WaveGerstner02Direction );
				float temp_output_409_0_g61891 = ( TWO_PI / _WaveGerstner02Length );
				float dotResult417_g61891 = dot( worldToObj419_g61891 , ( normalizeResult407_g61891 * temp_output_409_0_g61891 ) );
				float temp_output_421_0_g61891 = ( dotResult417_g61891 - ( sqrt( ( temp_output_409_0_g61891 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner02Speed ) ) );
				float temp_output_422_0_g61891 = cos( temp_output_421_0_g61891 );
				float temp_output_432_0_g61891 = _WaveGerstner02Height;
				float WaveHeight433_g61891 = temp_output_432_0_g61891;
				float3 WaveDirection429_g61891 = normalizeResult407_g61891;
				float temp_output_426_0_g61891 = sin( temp_output_421_0_g61891 );
				float temp_output_431_0_g61891 = ( temp_output_409_0_g61891 * temp_output_432_0_g61891 );
				float temp_output_435_0_g61891 = ( _WaveGerstner02PeakSharpness / temp_output_431_0_g61891 );
				float3 lerpResult2421_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61891 * float3(0,1,0) ) * WaveHeight433_g61891 ) - ( WaveDirection429_g61891 * ( temp_output_426_0_g61891 * ( temp_output_435_0_g61891 * temp_output_432_0_g61891 ) ) ) ) , _WaveGerstner02Enable);
				float3 worldToObj419_g61890 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61890 = normalize( _WaveGerstner03Direction );
				float temp_output_409_0_g61890 = ( TWO_PI / _WaveGerstner03Length );
				float dotResult417_g61890 = dot( worldToObj419_g61890 , ( normalizeResult407_g61890 * temp_output_409_0_g61890 ) );
				float temp_output_421_0_g61890 = ( dotResult417_g61890 - ( sqrt( ( temp_output_409_0_g61890 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner03Speed ) ) );
				float temp_output_422_0_g61890 = cos( temp_output_421_0_g61890 );
				float temp_output_432_0_g61890 = _WaveGerstner03Height;
				float WaveHeight433_g61890 = temp_output_432_0_g61890;
				float3 WaveDirection429_g61890 = normalizeResult407_g61890;
				float temp_output_426_0_g61890 = sin( temp_output_421_0_g61890 );
				float temp_output_431_0_g61890 = ( temp_output_409_0_g61890 * temp_output_432_0_g61890 );
				float temp_output_435_0_g61890 = ( _WaveGerstner03PeakSharpness / temp_output_431_0_g61890 );
				float3 lerpResult2414_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61890 * float3(0,1,0) ) * WaveHeight433_g61890 ) - ( WaveDirection429_g61890 * ( temp_output_426_0_g61890 * ( temp_output_435_0_g61890 * temp_output_432_0_g61890 ) ) ) ) , _WaveGerstner03Enable);
				float3 worldToObj419_g61894 = mul( GetWorldToObjectMatrix(), float4( ase_worldPos, 1 ) ).xyz;
				float3 normalizeResult407_g61894 = normalize( _WaveGerstner04Direction );
				float temp_output_409_0_g61894 = ( TWO_PI / _WaveGerstner04Length );
				float dotResult417_g61894 = dot( worldToObj419_g61894 , ( normalizeResult407_g61894 * temp_output_409_0_g61894 ) );
				float temp_output_421_0_g61894 = ( dotResult417_g61894 - ( sqrt( ( temp_output_409_0_g61894 * 9.8 ) ) * ( _TimeParameters.x * _WaveGerstner04Speed ) ) );
				float temp_output_422_0_g61894 = cos( temp_output_421_0_g61894 );
				float temp_output_432_0_g61894 = _WaveGerstner04Height;
				float WaveHeight433_g61894 = temp_output_432_0_g61894;
				float3 WaveDirection429_g61894 = normalizeResult407_g61894;
				float temp_output_426_0_g61894 = sin( temp_output_421_0_g61894 );
				float temp_output_431_0_g61894 = ( temp_output_409_0_g61894 * temp_output_432_0_g61894 );
				float temp_output_435_0_g61894 = ( _WaveGerstner04PeakSharpness / temp_output_431_0_g61894 );
				float3 lerpResult3196_g61884 = lerp( float3( 0,0,0 ) , ( ( ( temp_output_422_0_g61894 * float3(0,1,0) ) * WaveHeight433_g61894 ) - ( WaveDirection429_g61894 * ( temp_output_426_0_g61894 * ( temp_output_435_0_g61894 * temp_output_432_0_g61894 ) ) ) ) , _WaveGerstner04Enable);
				float3 temp_output_2006_0_g61884 = ( worldToObj2005_g61884 + ( ( ( lerpResult2419_g61884 + lerpResult2421_g61884 ) + lerpResult2414_g61884 ) + lerpResult3196_g61884 ) );
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth3376_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth3376_g61884 = saturate( abs( ( screenDepth3376_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaveGerstnerEdgeFadeRange ) ) );
				float saferPower3373_g61884 = abs( saturate( distanceDepth3376_g61884 ) );
				float Fade3386_g61884 = ( saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WaveGerstnerEdgeFadeRange ) ) * pow( saferPower3373_g61884 , 1.0 ) );
				float3 lerpResult3389_g61884 = lerp( input.positionOS.xyz , temp_output_2006_0_g61884 , Fade3386_g61884);
				float3 lerpResult3388_g61884 = lerp( temp_output_2006_0_g61884 , lerpResult3389_g61884 , _WaveGerstnerEdgeFadeEnable);
				float temp_output_1991_0_g61884 = ( _WaveGerstnerEnable + ( ( _CATEGORY_WAVESGERSTNER + _SPACE_WAVESGERSTNER ) * 0.0 ) );
				float3 lerpResult1995_g61884 = lerp( input.positionOS.xyz , lerpResult3388_g61884 , temp_output_1991_0_g61884);
				
				output.ase_texcoord2 = screenPos;
				output.ase_texcoord3.xyz = ase_worldPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
				float eyeDepth = -objectToViewPos.z;
				output.ase_texcoord3.w = eyeDepth;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord5.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.ase_normal);
				output.ase_texcoord6.xyz = ase_worldNormal;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord7.xyz = ase_worldBitangent;
				
				output.ase_normal = input.ase_normal;
				output.ase_texcoord4.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.zw = 0;
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;
				output.ase_texcoord7.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = lerpResult1995_g61884;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(APLICATION_SPACE_WARP_MOTION)
					// We do not need jittered position in ASW
					output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.positionOS));;
					output.positionCS = output.positionCSNoJitter;
				#else
					// Jittered. Match the frame.
					output.positionCS = vertexInput.positionCS;
					output.positionCSNoJitter = mul( _NonJitteredViewProjMatrix, mul( UNITY_MATRIX_M, input.positionOS));
				#endif

				float4 prevPos = ( unity_MotionVectorsParams.x == 1 ) ? float4( input.positionOld, 1 ) : input.positionOS;

				#if _ADD_PRECOMPUTED_VELOCITY
					prevPos = prevPos - float4(input.alembicMotionVector, 0);
				#endif

				output.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, mul( UNITY_PREV_MATRIX_M, prevPos ) );
				// removed in ObjectMotionVectors.hlsl found in unity 6000.0.23 and higher
				//ApplyMotionVectorZBias( output.positionCS );
				return output;
			}

			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}

			half4 frag(	PackedVaryings input , bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float4 screenPos = input.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth2129_g61884 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth2129_g61884 = abs( ( screenDepth2129_g61884 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _WaterShorelineDepth ) );
				float4 lerpResult25_g61884 = lerp( _WaterShorelineTint , _WaterMidwaterTint , saturate( (distanceDepth2129_g61884*1.0 + _WaterShorelineOffset) ));
				float4 lerpResult105_g61884 = lerp( _WaterDepthTint , lerpResult25_g61884 , saturate( (distanceDepth2129_g61884*-1.0 + _WaterDepthOffset) ));
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch37_g61884 = 0.0;
				#else
				float staticSwitch37_g61884 = ( 1.0 - ( ( 1.0 - saturate( (distanceDepth2129_g61884*-5.0 + _WaterOpacityShoreline) ) ) * ( 1.0 - ( _WaterOpacity + ( ( _CATEGORY_OPACITY + _SPACE_OPACITY ) * 0.0 ) ) ) ) );
				#endif
				float3 ase_worldPos = input.ase_texcoord3.xyz;
				float temp_output_8_0_g61907 = saturate( ( ( distance( _WorldSpaceCameraPos , ase_worldPos ) - 8.0 ) / 80.0 ) );
				float eyeDepth = input.ase_texcoord3.w;
				float eyeDepth7_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float2 temp_output_21_0_g61905 = ( (( input.ase_normal * ( 1.0 - temp_output_8_0_g61907 ) )).xy * ( ( _WaterRefractionScale + ( ( _CATEGORY_REFRACTION + _SPACE_REFRACTION ) * 0.0 ) ) / max( eyeDepth , 0.1 ) ) * saturate( ( eyeDepth7_g61905 - eyeDepth ) ) );
				float eyeDepth27_g61905 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( float4( temp_output_21_0_g61905, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ),_ZBufferParams);
				float2 temp_output_15_0_g61905 = (( float4( ( temp_output_21_0_g61905 * saturate( ( eyeDepth27_g61905 - eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
				float4 fetchOpaqueVal89_g61905 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( temp_output_15_0_g61905 ), 1.0 );
				float RefractionScale119_g61905 = _WaterRefractionScale;
				float4 temp_output_2480_0_g61884 = ( ( fetchOpaqueVal89_g61905 * RefractionScale119_g61905 ) * staticSwitch37_g61884 );
				float4 temp_output_59_0_g61898 = temp_output_2480_0_g61884;
				float3 unpack244_g61898 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaterNormalMap, sampler_WaterNormalMap, input.ase_texcoord4.xy ), _WaterReflectionBumpStrength );
				unpack244_g61898.z = lerp( 1, unpack244_g61898.z, saturate(_WaterReflectionBumpStrength) );
				float3 temp_output_70_0_g61899 = unpack244_g61898;
				float temp_output_96_0_g61899 = _WaterReflectionBumpClamp;
				float2 temp_cast_5 = (-temp_output_96_0_g61899).xx;
				float2 temp_cast_6 = (temp_output_96_0_g61899).xx;
				float2 clampResult87_g61899 = clamp( ( (( temp_output_70_0_g61899 * 50.0 )).xy * _WaterReflectionBumpScale ) , temp_cast_5 , temp_cast_6 );
				float4 appendResult85_g61899 = (float4(1.0 , 0.0 , 1.0 , temp_output_70_0_g61899.x));
				float3 unpack82_g61899 = UnpackNormalScale( appendResult85_g61899, 0.15 );
				unpack82_g61899.z = lerp( 1, unpack82_g61899.z, saturate(0.15) );
				float3 ase_worldTangent = input.ase_texcoord5.xyz;
				float3 ase_worldNormal = input.ase_texcoord6.xyz;
				float3 ase_worldBitangent = input.ase_texcoord7.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 worldRefl88_g61899 = reflect( -ase_viewDirWS, float3( dot( tanToWorld0, unpack82_g61899 ), dot( tanToWorld1, unpack82_g61899 ), dot( tanToWorld2, unpack82_g61899 ) ) );
				float4 texCUBENode67_g61899 = SAMPLE_TEXTURECUBE_LOD( _WaterReflectionCubemap, sampler_WaterReflectionCubemap, ( float3( clampResult87_g61899 ,  0.0 ) + worldRefl88_g61899 + _WaterReflectionWobble ), ( 1.0 - _WaterReflectionSmoothness ) );
				float4 temp_cast_8 = (texCUBENode67_g61899.r).xxxx;
				float4 lerpResult91_g61899 = lerp( texCUBENode67_g61899 , temp_cast_8 , _WaterReflectionCloud);
				float4 temp_output_154_21_g61898 = lerpResult91_g61899;
				float3 temp_output_70_0_g61900 = unpack244_g61898;
				float temp_output_95_0_g61900 = _WaterReflectionBumpScale;
				float temp_output_96_0_g61900 = _WaterReflectionBumpClamp;
				float2 temp_cast_11 = (-temp_output_96_0_g61900).xx;
				float2 temp_cast_12 = (temp_output_96_0_g61900).xx;
				float2 clampResult87_g61900 = clamp( ( (( temp_output_70_0_g61900 * 50.0 )).xy * temp_output_95_0_g61900 ) , temp_cast_11 , temp_cast_12 );
				float temp_output_17_0_g61900 = _WaterReflectionWobble;
				float3 appendResult134_g61900 = (float3(temp_output_17_0_g61900 , temp_output_17_0_g61900 , temp_output_17_0_g61900));
				float3 temp_output_107_0_g61900 = SHADERGRAPH_REFLECTION_PROBE(reflect( ase_viewDirWS , ase_worldNormal ),( float3( clampResult87_g61900 ,  0.0 ) + appendResult134_g61900 ),_WaterReflectionProbeDetailURP);
				float3 temp_cast_14 = (temp_output_107_0_g61900.x).xxx;
				float3 lerpResult91_g61900 = lerp( temp_output_107_0_g61900 , temp_cast_14 , _WaterReflectionCloud);
				float4 appendResult111_g61900 = (float4(lerpResult91_g61900 , 0.0));
				float4 lerpResult139_g61898 = lerp( temp_output_154_21_g61898 , appendResult111_g61900 , _WaterReflectionType);
				float fresnelNdotV23_g61898 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode23_g61898 = ( _WaterReflectionFresnelBias + _WaterReflectionFresnelScale * pow( max( 1.0 - fresnelNdotV23_g61898 , 0.0001 ), 5.0 ) );
				float4 lerpResult73_g61898 = lerp( float4( 0,0,0,0 ) , lerpResult139_g61898 , ( _WaterReflectionFresnelStrength * fresnelNode23_g61898 ));
				float4 lerpResult133_g61898 = lerp( lerpResult139_g61898 , lerpResult73_g61898 , _WaterReflectionEnableFresnel);
				float4 switchResult85_g61898 = (((ase_vface>0)?(lerpResult133_g61898):(float4( 0,0,0,0 ))));
				float4 temp_cast_16 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch95_g61898 = temp_cast_16;
				#else
				float4 staticSwitch95_g61898 = ( ( ( 1.0 - 0.5 ) * switchResult85_g61898 ) + ( temp_output_59_0_g61898 * 0.5 ) );
				#endif
				float4 lerpResult138_g61898 = lerp( temp_output_59_0_g61898 , staticSwitch95_g61898 , ( _WaterReflectionEnable + ( ( _CATEGORY_REFLECTION + _SPACE_REFLECTION ) * 0.0 ) ));
				float4 temp_output_2481_0_g61884 = ( ( lerpResult105_g61884 * ( 1.0 - staticSwitch37_g61884 ) ) + lerpResult138_g61898 );
				float clampResult3169_g61884 = clamp( temp_output_2481_0_g61884.a , 0.5 , 1.0 );
				

				float Alpha = clampResult3169_g61884;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(APLICATION_SPACE_WARP_MOTION)
					return float4( CalcAswNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 1 );
				#else
					return float4( CalcNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 0, 0 );
				#endif
			}		
			ENDHLSL
		}
		
	}
	
	CustomEditor "ALP8310_ShaderGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19702
Node;AmplifyShaderEditor.CommentaryNode;1115;-70.92462,-3082.146;Inherit;False;308.1923;376.7019;TESSELLATION;12;1120;1119;1118;1117;1066;1065;1064;1063;1062;1061;1060;1059;;0,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1114;-64,-2688;Inherit;False;DESF Core Water;1;;61884;1313c42205ab0e94aaeb8af860761fd8;1,2505,1;0;9;FLOAT3;0;FLOAT3;123;FLOAT3;1651;FLOAT;122;FLOAT;2483;FLOAT3;419;FLOAT3;417;FLOAT4;3364;FLOAT4;2399
Node;AmplifyShaderEditor.IntNode;1030;256,-2768;Inherit;False;Property;_Cull;Render Face;0;2;[HideInInspector];[Enum];Create;False;0;0;1;Front,2,Back,1,Both,0;True;0;False;2;0;False;0;1;INT;0
Node;AmplifyShaderEditor.StickyNoteNode;1116;-337.9246,-3082.146;Inherit;False;248;127;;;0,0,0,1;_TessellationStrength$_TessellationPhong$_TessellationDistanceMin$_TessellationDistanceMax$;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1117;-49.92462,-3034.146;Half;False;Property;_TessellationStrength;Tessellation Strength;145;0;Create;False;1;;0;0;True;0;False;6;6;0.001;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1118;-49.92462,-2954.146;Half;False;Property;_TessellationDistanceMin;Tessellation Distance Min;147;0;Create;False;1;;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1119;-49.92462,-2874.146;Half;False;Property;_TessellationDistanceMax;Tessellation Distance Max ;148;0;Create;False;1;;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1120;-49.92462,-2794.146;Half;False;Property;_TessellationPhong;Tessellation Phong;146;0;Create;False;1;;0;0;True;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1122;256,-2848;Inherit;False;Property;_SPACE_TESSELLATION;SPACE_TESSELLATION;149;0;Create;True;0;0;0;True;1;DE_DrawerSpace(10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1121;256,-2928;Inherit;False;Property;_CATEGORY_TESSELLATION;CATEGORY_TESSELLATION;144;0;Create;True;0;0;0;True;1;DE_DrawerCategory(TESSELLATION,true,_TessellationStrength,0,0);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1059;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1060;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1061;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1062;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;2;5;False;;10;False;;0;1;False;;7;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1063;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1064;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;2;5;False;;10;False;;0;1;False;;7;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1065;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1066;198.1309,-2867.459;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1057;441.8826,-2688;Float;False;False;-1;2;DE_ShaderGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1058;256,-2688;Float;False;True;-1;2;ALP8310_ShaderGUI;0;12;ALP/Water Gestner Waves;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;True;6;True;12;all;0;True;True;2;5;False;;10;False;;0;1;False;;7;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;45;Lighting Model;0;0;Workflow;0;638177056814425728;Surface;1;638242629156398165;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Alpha Clipping;0;638677682956210609;  Use Shadow Threshold;0;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;0;638242650202671029;Transmission;0;638242641488163752;  Transmission Shadow;0.5,True,_ASETransmissionShadow;638242641291946386;Translucency;0;638242640839524002;  Translucency Strength;1,True,_ASETranslucencyStrength;0;  Normal Distortion;0.5,True,_ASETranslucencyNormalDistortion;0;  Scattering;2,True,_ASETranslucencyScattering;0;  Direct;0.9,True,_ASETranslucencyScattering;0;  Ambient;0.1,True,_ASETranslucencyAmbient;0;  Shadow;0.5,True,_ASETranslucencyShadow;0;Cast Shadows;0;638242679477277815;Receive Shadows;1;0;Receive SSAO;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;GPU Instancing;1;638242687425151649;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;638242679733392300;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;638242650038145835;Tessellation;0;638297010234979777;  Phong;1;638297010364633154;  Strength;0.5,True,_TessellationPhong;638297010398817049;  Type;1;638177056948599080;  Tess;16,True,_TessellationStrength;638297010301304224;  Min;10,True,_TessellationDistanceMin;638297010325356173;  Max;25,True,_TessellationDistanceMax;638177057106186920;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;638242679321728066;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;0;638223051539584530;Debug Display;0;0;Clear Coat;0;638242649206523682;0;11;False;True;False;True;True;True;True;True;True;True;True;False;;True;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1123;256,-2588;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;1058;0;1114;0
WireConnection;1058;1;1114;123
WireConnection;1058;9;1114;1651
WireConnection;1058;4;1114;122
WireConnection;1058;6;1114;2483
WireConnection;1058;8;1114;419
WireConnection;1058;10;1114;417
WireConnection;1058;30;1114;3364
ASEEND*/
//CHKSM=F469750478B7E8787AF6FE96A43B780B460841D8