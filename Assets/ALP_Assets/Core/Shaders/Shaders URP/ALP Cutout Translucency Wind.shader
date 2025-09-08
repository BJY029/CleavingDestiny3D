// Made with Amplify Shader Editor v1.9.7.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ALP/Cutout Translucency Wind"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector][Enum(Front,2,Back,1,Both,0)]_Cull("Render Face", Int) = 0
		[DE_DrawerCategory(ALPHA CLIPPING,true,_GlancingClipMode,0,0)]_CATEGORY_ALPHACLIPPING("CATEGORY_ALPHACLIPPING", Float) = 0
		[DE_DrawerToggleLeft]_GlancingClipMode("Enable Clip Glancing Angle", Float) = 0
		[DE_DrawerSliderSimple(_AlphaRemapMin, _AlphaRemapMax,0, 1)]_AlphaRemap("Alpha Remapping", Vector) = (0,1,0,0)
		[HideInInspector]_AlphaRemapMin("AlphaRemapMin", Float) = 0
		[HideInInspector]_AlphaRemapMax("AlphaRemapMax", Float) = 1
		_AlphaCutoffBias("Alpha Cutoff Bias", Range( 0 , 1)) = 0.5
		_AlphaCutoffBiasShadow("Alpha Cutoff Bias Shadow", Range( 0.01 , 1)) = 0.5
		[DE_DrawerSpace(10)]_SPACE_ALPHACLIP("SPACE_ALPHACLIP", Float) = 0
		[DE_DrawerCategory(COLOR,true,_BaseColor,0,0)]_CATEGORY_COLOR("CATEGORY_COLOR", Float) = 1
		_BaseColor("Base Color", Color) = (1,1,1,0)
		_Brightness("Brightness", Range( 0 , 2)) = 1
		[DE_DrawerSpace(10)]_SPACE_COLOR("SPACE_COLOR", Float) = 0
		[DE_DrawerCategory(SURFACE INPUTS,true,_MainTex,0,0)]_CATEGORY_SURFACEINPUTS("CATEGORY_SURFACE INPUTS", Float) = 1
		[DE_DrawerTextureSingleLine]_MainTex("Base Map", 2D) = "white" {}
		[DE_DrawerFloatEnum(UV0 _UV1 _UV2 _UV3)]_UVMode("UV Mode", Float) = 0
		[DE_DrawerTilingOffset]_MainUVs("Main UVs", Vector) = (1,1,0,0)
		[Enum(MSO,0,MRO,1)]_MainMaskType("Main Mask Type", Float) = 0
		[DE_DrawerTextureSingleLine]_MainMaskMap("Main Mask Map", 2D) = "white" {}
		_MetallicStrength("Metallic Strength", Range( 0 , 1)) = 0
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 1)) = 0
		[DE_DrawerToggleLeft]_SmoothnessFresnelEnable("ENABLE FRESNEL", Float) = 0
		_SmoothnessFresnelScale("Fresnel Scale", Range( 0 , 3)) = 1.1
		_SmoothnessFresnelPower("Fresnel Power", Range( 0 , 20)) = 10
		_OcclusionStrengthAO("Occlusion Strength", Range( 0 , 1)) = 0
		[Normal][DE_DrawerTextureSingleLine][Space(10)]_BumpMap("Normal Map", 2D) = "bump" {}
		[DE_DrawerFloatEnum(Flip _Mirror _None)]_DoubleSidedNormalMode("Normal Mode", Float) = 2
		_NormalStrength("Normal Strength", Float) = 1
		[DE_DrawerSpace(10)]_SPACE_SURFACEINPUTS("SPACE_SURFACE INPUTS", Float) = 0
		[DE_DrawerCategory(COLOR SHIFT,true,_ColorShiftEnable,0,0)]_CATEGORY_COLORSHIFT("CATEGORY_COLOR SHIFT", Float) = 0
		[DE_DrawerToggleLeft]_ColorShiftEnable("ENABLE COLOR SHIFT", Float) = 0
		[DE_DrawerFloatEnum(Object Space _World Space _Vertex Color _Vertex Normal)]_ColorShiftSource("Shift Source", Float) = 0
		_ColorShiftVariation("Shift Variation", Range( 0 , 1)) = 0
		_ColorShiftVariationRGB("Shift Variation RGB", Range( -0.5 , 0.5)) = 0.2
		_ColorShiftInfluence("Shift Influence ", Range( 0 , 1)) = 0.75
		_ColorShiftSaturation("Shift Saturation", Range( 0 , 1)) = 0.85
		_ColorShiftNoiseScale("Shift Noise Scale", Range( 0 , 2)) = 1
		[Header(COLOR SHIFT WORLD SPACE)]_ColorShiftWorldSpaceDistance("World Space Distance", Range( 0.01 , 5)) = 5
		_ColorShiftWorldSpaceOffset("World Space Offset", Range( -1 , 1)) = 1
		_ColorShiftWorldSpaceNoiseShift("World Space Noise Shift", Range( 1 , 5)) = 5
		[DE_DrawerToggleLeft][Space(10)]_ColorShiftEnableMask("ENABLE COLOR SHIFT MASK", Float) = 1
		[DE_DrawerToggleNoKeyword]_ColorShiftMaskInverted("Color Shift Mask Inverted", Float) = 0
		[DE_DrawerTextureSingleLine]_ColorShiftMaskMap("Color Shift Mask Map", 2D) = "black" {}
		_ColorShiftMaskFuzziness("Color Shift Mask Fuzziness", Range( 0 , 1)) = 0.25
		[DE_DrawerSpace(10)]_SPACE_COLORSHIFT("SPACE_COLORSHIFT", Float) = 0
		[DE_DrawerCategory(TRANSMISSION,true,_TransmissionEnable,0,0)]_CATEGORY_TRANSMISSION("CATEGORY_TRANSMISSION", Float) = 0
		[DE_DrawerToggleLeft]_TransmissionEnable("ENABLE TRANSMISSION", Float) = 0
		[DE_DrawerFloatEnum(Color Picker Only_Transmission Mask Map)]_TransmissionSource("Transmission Source", Float) = 1
		[DE_DrawerTextureSingleLine]_TransmissionMaskMap("Transmission Mask Map", 2D) = "white" {}
		[DE_DrawerToggleNoKeyword]_TransmissionMaskInverted("Transmission Mask Inverted", Float) = 0
		_TransmissionMaskStrength("Transmission Mask Strength", Range( 0 , 1.5)) = 1.466198
		_TransmissionMaskFeather("Transmission Mask Feather", Range( 0 , 1)) = 1
		[HDR]_TransmissionColor("Transmission Color", Color) = (0.5,0.5,0.5,1)
		_TransmissionStrength("Transmission Strength", Float) = 1
		[DE_DrawerSpace(10)]_SPACE_TRANSMISSION("SPACE_TRANSMISSION", Float) = 0
		[DE_DrawerCategory(TRANSLUCENCY,true,_TranslucencyEnable,0,0)]_CATEGORY_TRANSLUCENCY("CATEGORY_TRANSLUCENCY", Float) = 0
		[DE_DrawerToggleLeft]_TranslucencyEnable("ENABLE TRANSLUCENCY", Float) = 0
		[DE_DrawerFloatEnum(Color Picker Only _Thickness Mask Map)]_TranslucencySource("Translucency Source", Float) = 1
		[DE_DrawerTextureSingleLine]_ThicknessMap("Thickness Mask Map", 2D) = "white" {}
		[DE_DrawerToggleNoKeyword]_ThicknessMapInverted("Thickness Mask Inverted", Float) = 0
		_ThicknessStrength("Thickness Mask Strength", Range( 0 , 1.5)) = 1.466198
		_ThicknessFeather("Thickness Mask Feather", Range( 0 , 1)) = 1
		[HDR]_TranslucencyColor("Translucency Color", Color) = (0.5,0.5,0.5,1)
		_TranslucencyStrength("Translucency Strength", Float) = 1
		[DE_DrawerSpace(10)]_SPACE_TRANSLUCENCY("SPACE_TRANSLUCENCY", Float) = 0
		[DE_DrawerCategory(TRANSLUCENCY ASE,true,_ASETranslucencyStrength,0,0)]_CATEGORY_TRANSLUCENCYASE("CATEGORY_TRANSLUCENCY ASE", Float) = 0
		_ASETransmissionShadow("ASE Transmission Shadow", Range( 0 , 1)) = 0.5
		_ASETranslucencyStrength("ASE Translucency Strength", Range( 1 , 50)) = 1
		_ASETranslucencyNormalDistortion("ASE Translucency Normal Distortion ", Range( 0 , 1)) = 0.2735869
		_ASETranslucencyScattering("ASE Translucency Scattering ", Range( 1 , 50)) = 2
		_ASETranslucencyDirect("ASE Translucency Direct ", Range( 0 , 1)) = 0.9
		_ASETranslucencyAmbient("ASE Translucency Ambient", Range( 0 , 1)) = 0.8339342
		[DE_DrawerCategory(WIND,true,_WindEnable,0,0)]_CATEGORY_WIND("CATEGORY_WIND", Float) = 0
		[DE_DrawerToggleLeft]_WindEnable("ENABLE WIND", Float) = 0
		[DE_DrawerFloatEnum(Global _Local)]_WindEnableMode("Enable Wind Mode", Float) = 0
		[DE_DrawerFloatEnum(Leaf _Palm _Grass _Simple _Ivy)]_WindEnableType("Enable Wind Type", Float) = 0
		[Header(WIND GLOBAL)]_WindGlobalIntensity("Wind Intensity", Float) = 1
		_WindGlobalTurbulence("Wind Turbulence", Float) = 0.35
		[Header(WIND LOCAL)]_WindLocalIntensity("Local Wind Intensity", Float) = 1
		_WindLocalTurbulence("Local Wind Turbulence", Float) = 0.35
		_WindLocalPulseFrequency("Local Wind Pulse Frequency", Float) = 0.1
		_WindLocalRandomOffset("Local Random Offset", Float) = 0.2
		_WindLocalDirection("Local Wind Direction", Range( 0 , 360)) = 0
		[DE_DrawerSpace(10)]_SPACE_WIND("SPACE_WIND", Float) = 0
		_ASETranslucencyShadow("ASE Translucency Shadow ", Range( 0 , 1)) = 0.5
		[DE_DrawerSpace(10)]_SPACE_TRANSLUCENCYASE("SPACE_TRANSLUCENCYASE", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Shadow", Range( 0, 1 ) ) = 0.5
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

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }

		Cull [_Cull]
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
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
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_fragment _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_SHADOWCOORDS


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
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord11 : TEXCOORD11;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_ColorShiftMaskMap);
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);
			TEXTURE2D(_MainMaskMap);
			SAMPLER(sampler_MainMaskMap);
			TEXTURE2D(_TransmissionMaskMap);
			TEXTURE2D(_ThicknessMap);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float ColorShift_Modefloatswitch168_g58704( float m_switch, float m_ObjectSpace, float m_WorldSpace, float m_VertexColor, float m_VertexNormal )
			{
				if(m_switch ==0)
					return m_ObjectSpace;
				else if(m_switch ==1)
					return m_WorldSpace;
				else if(m_switch ==2)
					return m_VertexColor;
				else if(m_switch ==3)
					return m_VertexNormal;
				else
				return float(0);
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 _NormalModefloat3switch( float m_switch, float3 m_Flip, float3 m_Mirror, float3 m_None )
			{
				if(m_switch ==0)
					return m_Flip;
				else if(m_switch ==1)
					return m_Mirror;
				else if(m_switch ==2)
					return m_None;
				else
				return float3(0,0,0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.tangentOS.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.texcoord.xy;
				float2 m_UV180_g58689 = input.texcoord1.xy;
				float2 m_UV280_g58689 = input.texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord9.xy = vertexToFrag70_g58689;
				
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord11.xyz = vertexToFrag145_g58715;
				
				output.ase_texcoord10 = input.positionOS;
				output.ase_color = input.ase_color;
				output.ase_normal = input.normalOS;
				output.ase_texcoord9.zw = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord11.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.texcoord = input.texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

				float3 temp_output_1923_0_g58671 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58689 = input.ase_texcoord9.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float3 temp_output_3_0_g58671 = ( temp_output_1923_0_g58671 * (tex2DNode2048_g58671).rgb * _Brightness );
				float3 temp_output_134_0_g58704 = temp_output_3_0_g58671;
				float m_switch168_g58704 = _ColorShiftSource;
				float m_ObjectSpace168_g58704 = ( _ColorShiftNoiseScale / 3 );
				float3 p1_g58705 = ( WorldPosition * _ColorShiftWorldSpaceNoiseShift );
				float localSimpleNoise3D1_g58705 = SimpleNoise3D( p1_g58705 );
				float4 transform374_g58704 = mul(GetObjectToWorldMatrix(),input.ase_texcoord10);
				float m_WorldSpace168_g58704 = ( ( localSimpleNoise3D1_g58705 / _ColorShiftNoiseScale ) - ( ( (transform374_g58704).w - _ColorShiftWorldSpaceOffset ) / _ColorShiftWorldSpaceDistance ) );
				float m_VertexColor168_g58704 = ( input.ase_color.g - 0.5 );
				float m_VertexNormal168_g58704 = ( input.ase_normal.y - 0.5 );
				float localColorShift_Modefloatswitch168_g58704 = ColorShift_Modefloatswitch168_g58704( m_switch168_g58704 , m_ObjectSpace168_g58704 , m_WorldSpace168_g58704 , m_VertexColor168_g58704 , m_VertexNormal168_g58704 );
				float temp_output_112_0_g58704 = sin( ( _ColorShiftNoiseScale * PI ) );
				float3 BaseColor136_g58704 = temp_output_134_0_g58704;
				float2 appendResult120_g58704 = (float2(( (0.3 + (( 1.0 - temp_output_112_0_g58704 ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) * BaseColor136_g58704.x ) , 0.0));
				float2 RGB146_g58704 = appendResult120_g58704;
				float3 hsvTorgb122_g58704 = RGBToHSV( float3( RGB146_g58704 ,  0.0 ) );
				float VALUE219_g58704 = temp_output_112_0_g58704;
				float3 hsvTorgb126_g58704 = HSVToRGB( float3(( ( saturate( localColorShift_Modefloatswitch168_g58704 ) * _ColorShiftVariation ) + _ColorShiftVariationRGB + hsvTorgb122_g58704 ).x,( _ColorShiftSaturation * hsvTorgb122_g58704.y ),( hsvTorgb122_g58704.z + ( VALUE219_g58704 / 40 ) )) );
				float2 uv_ColorShiftMaskMap = input.ase_texcoord9.zw * _ColorShiftMaskMap_ST.xy + _ColorShiftMaskMap_ST.zw;
				float4 transform376_g58704 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord10.xyz , 0.0 ));
				float3 temp_output_337_0_g58704 = saturate( ( 1.0 - ( (( SAMPLE_TEXTURE2D( _ColorShiftMaskMap, sampler_MainTex, uv_ColorShiftMaskMap ) * transform376_g58704 )).rgb / max( _ColorShiftMaskFuzziness , 1E-05 ) ) ) );
				float3 lerpResult314_g58704 = lerp( hsvTorgb126_g58704 , BaseColor136_g58704 , temp_output_337_0_g58704);
				float3 lerpResult311_g58704 = lerp( BaseColor136_g58704 , hsvTorgb126_g58704 , temp_output_337_0_g58704);
				float3 lerpResult389_g58704 = lerp( lerpResult314_g58704 , lerpResult311_g58704 , _ColorShiftMaskInverted);
				float3 lerpResult387_g58704 = lerp( hsvTorgb126_g58704 , lerpResult389_g58704 , _ColorShiftEnableMask);
				float3 lerpResult392_g58704 = lerp( temp_output_134_0_g58704 , lerpResult387_g58704 , _ColorShiftInfluence);
				float temp_output_402_0_g58704 = ( _ColorShiftEnable + ( ( _CATEGORY_COLORSHIFT + _SPACE_COLORSHIFT ) * 0.0 ) );
				float3 lerpResult393_g58704 = lerp( temp_output_134_0_g58704 , lerpResult392_g58704 , temp_output_402_0_g58704);
				
				float3 unpack1891_g58671 = UnpackNormalScale( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, UV213_g58671 ), _NormalStrength );
				unpack1891_g58671.z = lerp( 1, unpack1891_g58671.z, saturate(_NormalStrength) );
				float3 temp_output_24_0_g58703 = unpack1891_g58671;
				float temp_output_50_0_g58703 = _DoubleSidedNormalMode;
				float m_switch64_g58703 = temp_output_50_0_g58703;
				float3 m_Flip64_g58703 = float3(-1,-1,-1);
				float3 m_Mirror64_g58703 = float3(1,1,-1);
				float3 m_None64_g58703 = float3(1,1,1);
				float3 local_NormalModefloat3switch64_g58703 = _NormalModefloat3switch( m_switch64_g58703 , m_Flip64_g58703 , m_Mirror64_g58703 , m_None64_g58703 );
				float3 switchResult58_g58703 = (((ase_vface>0)?(temp_output_24_0_g58703):(( temp_output_24_0_g58703 * local_NormalModefloat3switch64_g58703 ))));
				
				float4 tex2DNode2050_g58671 = SAMPLE_TEXTURE2D( _MainMaskMap, sampler_MainMaskMap, UV213_g58671 );
				
				float lerpResult2650_g58671 = lerp( tex2DNode2050_g58671.g , ( 1.0 - tex2DNode2050_g58671.g ) , _MainMaskType);
				float temp_output_2693_0_g58671 = ( lerpResult2650_g58671 * _SmoothnessStrength );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float2 appendResult2645_g58671 = (float2(ase_viewDirSafeWS.xy));
				float3 appendResult2644_g58671 = (float3(appendResult2645_g58671 , ( ase_viewDirSafeWS.z / 1.06 )));
				float3 break2680_g58671 = unpack1891_g58671;
				float3 normalizeResult2641_g58671 = normalize( ( ( WorldTangent * break2680_g58671.x ) + ( WorldBiTangent * break2680_g58671.y ) + ( WorldNormal * break2680_g58671.z ) ) );
				float3 Normal_Per_Pixel2690_g58671 = normalizeResult2641_g58671;
				float fresnelNdotV2685_g58671 = dot( normalize( Normal_Per_Pixel2690_g58671 ), appendResult2644_g58671 );
				float fresnelNode2685_g58671 = ( 0.0 + ( 1.0 - _SmoothnessFresnelScale ) * pow( max( 1.0 - fresnelNdotV2685_g58671 , 0.0001 ), _SmoothnessFresnelPower ) );
				float lerpResult2636_g58671 = lerp( temp_output_2693_0_g58671 , ( temp_output_2693_0_g58671 - fresnelNode2685_g58671 ) , _SmoothnessFresnelEnable);
				
				float lerpResult3414_g58671 = lerp( 1.0 , min( tex2DNode2050_g58671.b , input.ase_color.a ) , _OcclusionStrengthAO);
				
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord11.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				
				float3 temp_output_249_0_g58676 = (_TransmissionColor).rgb;
				float4 color71_g58676 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float3 temp_output_247_0_g58676 = (color71_g58676).rgb;
				float2 temp_output_101_0_g58676 = UV213_g58671;
				float3 temp_output_162_0_g58676 = saturate( ( (SAMPLE_TEXTURE2D( _TransmissionMaskMap, sampler_MainTex, temp_output_101_0_g58676 )).rgb / max( _TransmissionMaskFeather , 1E-05 ) ) );
				float3 lerpResult172_g58676 = lerp( temp_output_247_0_g58676 , temp_output_162_0_g58676 , _TransmissionMaskStrength);
				float temp_output_165_0_g58676 = ( 0.0 - 1.0 );
				float temp_output_168_0_g58676 = ( temp_output_162_0_g58676.x - 1.0 );
				float lerpResult173_g58676 = lerp( ( temp_output_165_0_g58676 / temp_output_168_0_g58676 ) , ( temp_output_168_0_g58676 / temp_output_165_0_g58676 ) , ( 0.7 + _TransmissionMaskStrength ));
				float3 lerpResult148_g58676 = lerp( ( temp_output_249_0_g58676 * _TransmissionStrength * lerpResult172_g58676 ) , ( temp_output_249_0_g58676 * _TransmissionStrength * saturate( lerpResult173_g58676 ) ) , _TransmissionMaskInverted);
				float3 lerpResult147_g58676 = lerp( ( temp_output_249_0_g58676 * _TransmissionStrength ) , lerpResult148_g58676 , _TransmissionSource);
				float3 lerpResult122_g58676 = lerp( float3( 0,0,0 ) , lerpResult147_g58676 , ( _TransmissionEnable + ( ( _CATEGORY_TRANSLUCENCY + _SPACE_TRANSMISSION ) * 0.0 ) ));
				
				float3 temp_output_248_0_g58676 = (_TranslucencyColor).rgb;
				float3 temp_output_113_0_g58676 = saturate( ( (SAMPLE_TEXTURE2D( _ThicknessMap, sampler_MainTex, temp_output_101_0_g58676 )).rgb / max( _ThicknessFeather , 1E-05 ) ) );
				float3 lerpResult34_g58676 = lerp( temp_output_247_0_g58676 , temp_output_113_0_g58676 , _ThicknessStrength);
				float temp_output_69_0_g58676 = ( 0.0 - 1.0 );
				float temp_output_22_0_g58676 = ( temp_output_113_0_g58676.x - 1.0 );
				float lerpResult66_g58676 = lerp( ( temp_output_69_0_g58676 / temp_output_22_0_g58676 ) , ( temp_output_22_0_g58676 / temp_output_69_0_g58676 ) , ( 0.7 + _ThicknessStrength ));
				float3 lerpResult153_g58676 = lerp( ( temp_output_248_0_g58676 * lerpResult34_g58676 * _TranslucencyStrength ) , ( temp_output_248_0_g58676 * saturate( lerpResult66_g58676 ) * _TranslucencyStrength ) , _ThicknessMapInverted);
				float3 lerpResult150_g58676 = lerp( ( temp_output_248_0_g58676 * _TranslucencyStrength ) , lerpResult153_g58676 , _TranslucencySource);
				float ase_lightAtten = 0;
				Light ase_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_mainLight.distanceAttenuation * ase_mainLight.shadowAttenuation;
				float ase_lightIntensity = max( max( _MainLightColor.r, _MainLightColor.g ), _MainLightColor.b );
				float4 ase_lightColor = float4( _MainLightColor.rgb / ase_lightIntensity, ase_lightIntensity );
				float3 temp_output_88_0_g58676 = ( ( ase_lightAtten * ase_lightColor.rgb ) * max( ase_lightColor.a , 0.0 ) );
				float3 lerpResult123_g58676 = lerp( float3( 0,0,0 ) , ( lerpResult150_g58676 * saturate( temp_output_88_0_g58676 ) ) , ( _TranslucencyEnable + ( ( _SPACE_TRANSLUCENCY + _CATEGORY_TRANSMISSION ) * 0.0 ) ));
				

				float3 BaseColor = lerpResult393_g58704;
				float3 Normal = switchResult58_g58703;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = ( _MetallicStrength * tex2DNode2050_g58671.r );
				float Smoothness = saturate( lerpResult2636_g58671 );
				float Occlusion = saturate( lerpResult3414_g58671 );
				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;
				float AlphaClipThresholdShadow = _AlphaCutoffBiasShadow;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = lerpResult122_g58676;
				float3 Translucency = lerpResult123_g58676;

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
					float shadow = _ASETransmissionShadow;

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
					float shadow = _ASETranslucencyShadow;
					float normal = _ASETranslucencyNormalDistortion;
					float scattering = _ASETranslucencyScattering;
					float direct = _ASETranslucencyScattering;
					float ambient = _ASETranslucencyAmbient;
					float strength = _ASETranslucencyStrength;

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
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			

			float3 _LightDirection;
			float3 _LightPosition;

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord3.xy = vertexToFrag70_g58689;
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord4.xyz = vertexToFrag145_g58715;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				output.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				//code for UNITY_REVERSED_Z is moved into Shadows.hlsl from 6000.0.22 and or higher
				positionCS = ApplyShadowClamping(positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = positionCS;
				output.clipPosV = positionCS;
				output.positionWS = positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
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

				float2 vertexToFrag70_g58689 = input.ase_texcoord3.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord4.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;
				float AlphaClipThresholdShadow = _AlphaCutoffBiasShadow;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
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
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord3.xy = vertexToFrag70_g58689;
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord4.xyz = vertexToFrag145_g58715;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				output.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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
						 ) : SV_Target
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

				float2 vertexToFrag70_g58689 = input.ase_texcoord3.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord4.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;

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
			#pragma multi_compile_fragment _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
			#define ASE_NEEDS_FRAG_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_ColorShiftMaskMap);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float ColorShift_Modefloatswitch168_g58704( float m_switch, float m_ObjectSpace, float m_WorldSpace, float m_VertexColor, float m_VertexNormal )
			{
				if(m_switch ==0)
					return m_ObjectSpace;
				else if(m_switch ==1)
					return m_WorldSpace;
				else if(m_switch ==2)
					return m_VertexColor;
				else if(m_switch ==3)
					return m_VertexNormal;
				else
				return float(0);
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.texcoord0.xy;
				float2 m_UV180_g58689 = input.texcoord1.xy;
				float2 m_UV280_g58689 = input.texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord4.xy = vertexToFrag70_g58689;
				
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord6.xyz = vertexToFrag145_g58715;
				
				output.ase_texcoord5 = input.positionOS;
				output.ase_color = input.ase_color;
				output.ase_normal = input.normalOS;
				output.ase_texcoord4.zw = input.texcoord0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

			half4 frag(PackedVaryings input  ) : SV_Target
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

				float3 temp_output_1923_0_g58671 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58689 = input.ase_texcoord4.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float3 temp_output_3_0_g58671 = ( temp_output_1923_0_g58671 * (tex2DNode2048_g58671).rgb * _Brightness );
				float3 temp_output_134_0_g58704 = temp_output_3_0_g58671;
				float m_switch168_g58704 = _ColorShiftSource;
				float m_ObjectSpace168_g58704 = ( _ColorShiftNoiseScale / 3 );
				float3 p1_g58705 = ( WorldPosition * _ColorShiftWorldSpaceNoiseShift );
				float localSimpleNoise3D1_g58705 = SimpleNoise3D( p1_g58705 );
				float4 transform374_g58704 = mul(GetObjectToWorldMatrix(),input.ase_texcoord5);
				float m_WorldSpace168_g58704 = ( ( localSimpleNoise3D1_g58705 / _ColorShiftNoiseScale ) - ( ( (transform374_g58704).w - _ColorShiftWorldSpaceOffset ) / _ColorShiftWorldSpaceDistance ) );
				float m_VertexColor168_g58704 = ( input.ase_color.g - 0.5 );
				float m_VertexNormal168_g58704 = ( input.ase_normal.y - 0.5 );
				float localColorShift_Modefloatswitch168_g58704 = ColorShift_Modefloatswitch168_g58704( m_switch168_g58704 , m_ObjectSpace168_g58704 , m_WorldSpace168_g58704 , m_VertexColor168_g58704 , m_VertexNormal168_g58704 );
				float temp_output_112_0_g58704 = sin( ( _ColorShiftNoiseScale * PI ) );
				float3 BaseColor136_g58704 = temp_output_134_0_g58704;
				float2 appendResult120_g58704 = (float2(( (0.3 + (( 1.0 - temp_output_112_0_g58704 ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) * BaseColor136_g58704.x ) , 0.0));
				float2 RGB146_g58704 = appendResult120_g58704;
				float3 hsvTorgb122_g58704 = RGBToHSV( float3( RGB146_g58704 ,  0.0 ) );
				float VALUE219_g58704 = temp_output_112_0_g58704;
				float3 hsvTorgb126_g58704 = HSVToRGB( float3(( ( saturate( localColorShift_Modefloatswitch168_g58704 ) * _ColorShiftVariation ) + _ColorShiftVariationRGB + hsvTorgb122_g58704 ).x,( _ColorShiftSaturation * hsvTorgb122_g58704.y ),( hsvTorgb122_g58704.z + ( VALUE219_g58704 / 40 ) )) );
				float2 uv_ColorShiftMaskMap = input.ase_texcoord4.zw * _ColorShiftMaskMap_ST.xy + _ColorShiftMaskMap_ST.zw;
				float4 transform376_g58704 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord5.xyz , 0.0 ));
				float3 temp_output_337_0_g58704 = saturate( ( 1.0 - ( (( SAMPLE_TEXTURE2D( _ColorShiftMaskMap, sampler_MainTex, uv_ColorShiftMaskMap ) * transform376_g58704 )).rgb / max( _ColorShiftMaskFuzziness , 1E-05 ) ) ) );
				float3 lerpResult314_g58704 = lerp( hsvTorgb126_g58704 , BaseColor136_g58704 , temp_output_337_0_g58704);
				float3 lerpResult311_g58704 = lerp( BaseColor136_g58704 , hsvTorgb126_g58704 , temp_output_337_0_g58704);
				float3 lerpResult389_g58704 = lerp( lerpResult314_g58704 , lerpResult311_g58704 , _ColorShiftMaskInverted);
				float3 lerpResult387_g58704 = lerp( hsvTorgb126_g58704 , lerpResult389_g58704 , _ColorShiftEnableMask);
				float3 lerpResult392_g58704 = lerp( temp_output_134_0_g58704 , lerpResult387_g58704 , _ColorShiftInfluence);
				float temp_output_402_0_g58704 = ( _ColorShiftEnable + ( ( _CATEGORY_COLORSHIFT + _SPACE_COLORSHIFT ) * 0.0 ) );
				float3 lerpResult393_g58704 = lerp( temp_output_134_0_g58704 , lerpResult392_g58704 , temp_output_402_0_g58704);
				
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord6.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float3 BaseColor = lerpResult393_g58704;
				float3 Emission = 0;
				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;

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

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#pragma multi_compile_fragment _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
			#define ASE_NEEDS_FRAG_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_ColorShiftMaskMap);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float ColorShift_Modefloatswitch168_g58704( float m_switch, float m_ObjectSpace, float m_WorldSpace, float m_VertexColor, float m_VertexNormal )
			{
				if(m_switch ==0)
					return m_ObjectSpace;
				else if(m_switch ==1)
					return m_WorldSpace;
				else if(m_switch ==2)
					return m_VertexColor;
				else if(m_switch ==3)
					return m_VertexNormal;
				else
				return float(0);
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord2.xy = vertexToFrag70_g58689;
				
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord4.xyz = vertexToFrag145_g58715;
				
				output.ase_texcoord3 = input.positionOS;
				output.ase_color = input.ase_color;
				output.ase_normal = input.normalOS;
				output.ase_texcoord2.zw = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

			half4 frag(PackedVaryings input  ) : SV_Target
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

				float3 temp_output_1923_0_g58671 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58689 = input.ase_texcoord2.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float3 temp_output_3_0_g58671 = ( temp_output_1923_0_g58671 * (tex2DNode2048_g58671).rgb * _Brightness );
				float3 temp_output_134_0_g58704 = temp_output_3_0_g58671;
				float m_switch168_g58704 = _ColorShiftSource;
				float m_ObjectSpace168_g58704 = ( _ColorShiftNoiseScale / 3 );
				float3 p1_g58705 = ( WorldPosition * _ColorShiftWorldSpaceNoiseShift );
				float localSimpleNoise3D1_g58705 = SimpleNoise3D( p1_g58705 );
				float4 transform374_g58704 = mul(GetObjectToWorldMatrix(),input.ase_texcoord3);
				float m_WorldSpace168_g58704 = ( ( localSimpleNoise3D1_g58705 / _ColorShiftNoiseScale ) - ( ( (transform374_g58704).w - _ColorShiftWorldSpaceOffset ) / _ColorShiftWorldSpaceDistance ) );
				float m_VertexColor168_g58704 = ( input.ase_color.g - 0.5 );
				float m_VertexNormal168_g58704 = ( input.ase_normal.y - 0.5 );
				float localColorShift_Modefloatswitch168_g58704 = ColorShift_Modefloatswitch168_g58704( m_switch168_g58704 , m_ObjectSpace168_g58704 , m_WorldSpace168_g58704 , m_VertexColor168_g58704 , m_VertexNormal168_g58704 );
				float temp_output_112_0_g58704 = sin( ( _ColorShiftNoiseScale * PI ) );
				float3 BaseColor136_g58704 = temp_output_134_0_g58704;
				float2 appendResult120_g58704 = (float2(( (0.3 + (( 1.0 - temp_output_112_0_g58704 ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) * BaseColor136_g58704.x ) , 0.0));
				float2 RGB146_g58704 = appendResult120_g58704;
				float3 hsvTorgb122_g58704 = RGBToHSV( float3( RGB146_g58704 ,  0.0 ) );
				float VALUE219_g58704 = temp_output_112_0_g58704;
				float3 hsvTorgb126_g58704 = HSVToRGB( float3(( ( saturate( localColorShift_Modefloatswitch168_g58704 ) * _ColorShiftVariation ) + _ColorShiftVariationRGB + hsvTorgb122_g58704 ).x,( _ColorShiftSaturation * hsvTorgb122_g58704.y ),( hsvTorgb122_g58704.z + ( VALUE219_g58704 / 40 ) )) );
				float2 uv_ColorShiftMaskMap = input.ase_texcoord2.zw * _ColorShiftMaskMap_ST.xy + _ColorShiftMaskMap_ST.zw;
				float4 transform376_g58704 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord3.xyz , 0.0 ));
				float3 temp_output_337_0_g58704 = saturate( ( 1.0 - ( (( SAMPLE_TEXTURE2D( _ColorShiftMaskMap, sampler_MainTex, uv_ColorShiftMaskMap ) * transform376_g58704 )).rgb / max( _ColorShiftMaskFuzziness , 1E-05 ) ) ) );
				float3 lerpResult314_g58704 = lerp( hsvTorgb126_g58704 , BaseColor136_g58704 , temp_output_337_0_g58704);
				float3 lerpResult311_g58704 = lerp( BaseColor136_g58704 , hsvTorgb126_g58704 , temp_output_337_0_g58704);
				float3 lerpResult389_g58704 = lerp( lerpResult314_g58704 , lerpResult311_g58704 , _ColorShiftMaskInverted);
				float3 lerpResult387_g58704 = lerp( hsvTorgb126_g58704 , lerpResult389_g58704 , _ColorShiftEnableMask);
				float3 lerpResult392_g58704 = lerp( temp_output_134_0_g58704 , lerpResult387_g58704 , _ColorShiftInfluence);
				float temp_output_402_0_g58704 = ( _ColorShiftEnable + ( ( _CATEGORY_COLORSHIFT + _SPACE_COLORSHIFT ) * 0.0 ) );
				float3 lerpResult393_g58704 = lerp( temp_output_134_0_g58704 , lerpResult392_g58704 , temp_output_402_0_g58704);
				
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord4.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float3 BaseColor = lerpResult393_g58704;
				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;

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
			Tags { "LightMode"="DepthNormalsOnly" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#pragma multi_compile _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
			#define ASE_NEEDS_VERT_TANGENT
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
				float4 tangentOS : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
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
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			
			float3 _NormalModefloat3switch( float m_switch, float3 m_Flip, float3 m_Mirror, float3 m_None )
			{
				if(m_switch ==0)
					return m_Flip;
				else if(m_switch ==1)
					return m_Mirror;
				else if(m_switch ==2)
					return m_None;
				else
				return float3(0,0,0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.tangentOS.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord5.xy = vertexToFrag70_g58689;
				
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord6.xyz = vertexToFrag145_g58715;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord5.zw = 0;
				output.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

				float2 vertexToFrag70_g58689 = input.ase_texcoord5.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float3 unpack1891_g58671 = UnpackNormalScale( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, UV213_g58671 ), _NormalStrength );
				unpack1891_g58671.z = lerp( 1, unpack1891_g58671.z, saturate(_NormalStrength) );
				float3 temp_output_24_0_g58703 = unpack1891_g58671;
				float temp_output_50_0_g58703 = _DoubleSidedNormalMode;
				float m_switch64_g58703 = temp_output_50_0_g58703;
				float3 m_Flip64_g58703 = float3(-1,-1,-1);
				float3 m_Mirror64_g58703 = float3(1,1,-1);
				float3 m_None64_g58703 = float3(1,1,1);
				float3 local_NormalModefloat3switch64_g58703 = _NormalModefloat3switch( m_switch64_g58703 , m_Flip64_g58703 , m_Mirror64_g58703 , m_None64_g58703 );
				float3 switchResult58_g58703 = (((ase_vface>0)?(temp_output_24_0_g58703):(( temp_output_24_0_g58703 * local_NormalModefloat3switch64_g58703 ))));
				
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord6.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float3 Normal = switchResult58_g58703;
				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;

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
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
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

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord.xy = vertexToFrag70_g58689;
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord1.xyz = vertexToFrag145_g58715;
				output.ase_texcoord2.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 vertexToFrag70_g58689 = input.ase_texcoord.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord1.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_worldPos = input.ase_texcoord2.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				surfaceDescription.Alpha = temp_output_98_0_g58711;
				surfaceDescription.AlphaClipThreshold = _AlphaCutoffBias;

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
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
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

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.normalOS * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.normalOS , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord.xy = vertexToFrag70_g58689;
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord1.xyz = vertexToFrag145_g58715;
				output.ase_texcoord2.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.ase_color = input.ase_color;
				output.ase_tangent = input.ase_tangent;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 vertexToFrag70_g58689 = input.ase_texcoord.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord1.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_worldPos = input.ase_texcoord2.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				surfaceDescription.Alpha = temp_output_98_0_g58711;
				surfaceDescription.AlphaClipThreshold = _AlphaCutoffBias;

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

			#pragma multi_compile _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_TRANSMISSION 1
			#define ASE_TRANSLUCENCY 1
			#define _ALPHATEST_SHADOW_ON 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
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
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorShiftMaskMap_ST;
			half4 _TransmissionColor;
			half4 _BaseColor;
			half4 _TranslucencyColor;
			float4 _MainUVs;
			float4 _AlphaRemap;
			half _ColorShiftEnable;
			float _SPACE_COLORSHIFT;
			half _NormalStrength;
			half _DoubleSidedNormalMode;
			float _MetallicStrength;
			float _AlphaRemapMin;
			float _MainMaskType;
			half _ColorShiftInfluence;
			half _SmoothnessStrength;
			half _ColorShiftEnableMask;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _SmoothnessFresnelEnable;
			half _OcclusionStrengthAO;
			float _CATEGORY_COLORSHIFT;
			float _AlphaRemapMax;
			half _GlancingClipMode;
			half _ThicknessMapInverted;
			half _ThicknessStrength;
			half _ThicknessFeather;
			half _TranslucencyStrength;
			float _SPACE_TRANSMISSION;
			float _CATEGORY_TRANSLUCENCY;
			half _TransmissionEnable;
			half _ColorShiftMaskInverted;
			half _TransmissionSource;
			half _TransmissionMaskStrength;
			half _TransmissionMaskFeather;
			half _TransmissionStrength;
			half _AlphaCutoffBiasShadow;
			half _AlphaCutoffBias;
			float _SPACE_ALPHACLIP;
			float _CATEGORY_ALPHACLIPPING;
			half _TransmissionMaskInverted;
			half _ColorShiftMaskFuzziness;
			float _SPACE_TRANSLUCENCY;
			half _ColorShiftSaturation;
			half _WindEnableType;
			int _Cull;
			float _SPACE_TRANSLUCENCYASE;
			float _CATEGORY_TRANSLUCENCYASE;
			float _ASETransmissionShadow;
			float _ASETranslucencyStrength;
			float _ASETranslucencyNormalDistortion;
			half _WindGlobalIntensity;
			float _ASETranslucencyScattering;
			float _ASETranslucencyAmbient;
			float _ASETranslucencyShadow;
			float _CATEGORY_COLOR;
			float _SPACE_COLOR;
			float _SPACE_SURFACEINPUTS;
			float _CATEGORY_SURFACEINPUTS;
			float _CATEGORY_TRANSMISSION;
			float _ASETranslucencyDirect;
			half _TranslucencySource;
			half _WindLocalIntensity;
			half _WindLocalRandomOffset;
			half _ColorShiftVariationRGB;
			half _ColorShiftVariation;
			half _ColorShiftWorldSpaceDistance;
			half _ColorShiftWorldSpaceOffset;
			half _ColorShiftWorldSpaceNoiseShift;
			half _ColorShiftNoiseScale;
			half _ColorShiftSource;
			half _WindEnableMode;
			half _Brightness;
			float _SPACE_WIND;
			float _CATEGORY_WIND;
			half _WindEnable;
			half _WindLocalTurbulence;
			half _WindGlobalTurbulence;
			half _WindLocalDirection;
			half _WindLocalPulseFrequency;
			float _UVMode;
			half _TranslucencyEnable;
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

			float _GlobalWindIntensity;
			float _GlobalWindRandomOffset;
			float _GlobalWindPulse;
			float _GlobalWindDirection;
			float _GlobalWindTurbulence;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);


			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				 float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				 float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				 float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				 float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 DirectionalEquation( float _WindDirection )
			{
				float d = _WindDirection * 0.0174532924;
				float xL = cos(d) + 1 / 2;
				float zL = sin(d) + 1 / 2;
				return float2(zL,xL);
			}
			
			float3 Wind_Typefloat3switch2439_g58668( float m_switch, float3 m_Leaf, float3 m_Palm, float3 m_Grass, float3 m_Simple, float3 m_Ivy )
			{
				if(m_switch ==0)
					return m_Leaf;
				else if(m_switch ==1)
					return m_Palm;
				else if(m_switch ==2)
					return m_Grass;
				else if(m_switch ==3)
					return m_Simple;
				else if(m_switch ==4)
					return m_Ivy;
				else
				return float3(0,0,0);
			}
			
			float2 float2switchUVMode80_g58689( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
			{
				if(m_switch ==0)
					return m_UV0;
				else if(m_switch ==1)
					return m_UV1;
				else if(m_switch ==2)
					return m_UV2;
				else if(m_switch ==3)
					return m_UV3;
				else
				return float2(0,0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float m_switch2439_g58668 = _WindEnableType;
				float3 VERTEX_POSITION_MATRIX2352_g58668 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float GlobalWindIntensity3173_g58668 = _GlobalWindIntensity;
				float WIND_MODE2462_g58668 = _WindEnableMode;
				float lerpResult3147_g58668 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58668 ) , _WindLocalIntensity , WIND_MODE2462_g58668);
				float _WIND_STRENGHT2400_g58668 = lerpResult3147_g58668;
				float GlobalWindRandomOffset3174_g58668 = _GlobalWindRandomOffset;
				float lerpResult3149_g58668 = lerp( GlobalWindRandomOffset3174_g58668 , _WindLocalRandomOffset , WIND_MODE2462_g58668);
				float4 transform3073_g58668 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58668 = (float2(transform3073_g58668.x , transform3073_g58668.z));
				float dotResult2341_g58668 = dot( appendResult2307_g58668 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58668 = lerp( 0.8 , ( ( lerpResult3149_g58668 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58668 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58668 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58668 );
				float _WIND_TUBULENCE_RANDOM2274_g58668 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58668 = _GlobalWindPulse;
				float lerpResult3152_g58668 = lerp( GlobalWindPulse3177_g58668 , _WindLocalPulseFrequency , WIND_MODE2462_g58668);
				float _WIND_PULSE2421_g58668 = lerpResult3152_g58668;
				float FUNC_Angle2470_g58668 = ( _WIND_STRENGHT2400_g58668 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 ) - ( VERTEX_POSITION_MATRIX2352_g58668.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58668 );
				float FUNC_Angle_SinA2424_g58668 = sin( FUNC_Angle2470_g58668 );
				float FUNC_Angle_CosA2362_g58668 = cos( FUNC_Angle2470_g58668 );
				float GlobalWindDirection3175_g58668 = _GlobalWindDirection;
				float lerpResult3150_g58668 = lerp( GlobalWindDirection3175_g58668 , _WindLocalDirection , WIND_MODE2462_g58668);
				float _WindDirection2249_g58668 = lerpResult3150_g58668;
				float2 localDirectionalEquation2249_g58668 = DirectionalEquation( _WindDirection2249_g58668 );
				float2 break2469_g58668 = localDirectionalEquation2249_g58668;
				float _WIND_DIRECTION_X2418_g58668 = break2469_g58668.x;
				float lerpResult2258_g58668 = lerp( break2265_g58668.x , ( ( break2265_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2265_g58668.x * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_X2418_g58668);
				float3 break2340_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float3 break2233_g58668 = VERTEX_POSITION_MATRIX2352_g58668;
				float _WIND_DIRECTION_Y2416_g58668 = break2469_g58668.y;
				float lerpResult2275_g58668 = lerp( break2233_g58668.z , ( ( break2233_g58668.y * FUNC_Angle_SinA2424_g58668 ) + ( break2233_g58668.z * FUNC_Angle_CosA2362_g58668 ) ) , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2235_g58668 = (float3(lerpResult2258_g58668 , ( ( break2340_g58668.y * FUNC_Angle_CosA2362_g58668 ) - ( break2340_g58668.z * FUNC_Angle_SinA2424_g58668 ) ) , lerpResult2275_g58668));
				float3 VERTEX_POSITION2282_g58668 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58668 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 break2518_g58668 = VERTEX_POSITION2282_g58668;
				half FUNC_SinFunction2336_g58668 = sin( ( ( _WIND_RANDOM_OFFSET2244_g58668 * 200.0 * ( 0.2 + input.ase_color.g ) ) + ( input.ase_color.g * 10.0 ) + _WIND_TUBULENCE_RANDOM2274_g58668 + ( VERTEX_POSITION_MATRIX2352_g58668.z / 2.0 ) ) );
				float GlobalWindTurbulence3176_g58668 = _GlobalWindTurbulence;
				float lerpResult3151_g58668 = lerp( ( _WindGlobalTurbulence * GlobalWindTurbulence3176_g58668 ) , _WindLocalTurbulence , WIND_MODE2462_g58668);
				float _WIND_TUBULENCE2442_g58668 = lerpResult3151_g58668;
				float3 appendResult2480_g58668 = (float3(break2518_g58668.x , ( break2518_g58668.y + ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) ) , break2518_g58668.z));
				float3 VERTEX_LEAF2396_g58668 = appendResult2480_g58668;
				float3 m_Leaf2439_g58668 = VERTEX_LEAF2396_g58668;
				float3 VERTEX_PALM2310_g58668 = ( ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) * _WIND_TUBULENCE2442_g58668 ) + VERTEX_POSITION2282_g58668 );
				float3 m_Palm2439_g58668 = VERTEX_PALM2310_g58668;
				float3 break2486_g58668 = VERTEX_POSITION2282_g58668;
				float temp_output_2514_0_g58668 = ( FUNC_SinFunction2336_g58668 * input.ase_color.b * ( FUNC_Angle2470_g58668 + ( _WIND_STRENGHT2400_g58668 / 200.0 ) ) );
				float lerpResult2482_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_X2418_g58668);
				float lerpResult2484_g58668 = lerp( 0.0 , temp_output_2514_0_g58668 , _WIND_DIRECTION_Y2416_g58668);
				float3 appendResult2489_g58668 = (float3(( break2486_g58668.x + lerpResult2482_g58668 ) , break2486_g58668.y , ( break2486_g58668.z + lerpResult2484_g58668 )));
				float3 VERTEX_GRASS2242_g58668 = appendResult2489_g58668;
				float3 m_Grass2439_g58668 = VERTEX_GRASS2242_g58668;
				float3 m_Simple2439_g58668 = VERTEX_POSITION2282_g58668;
				float clampResult2884_g58668 = clamp( ( _WIND_STRENGHT2400_g58668 - 0.95 ) , 0.0 , 1.0 );
				float3 break2708_g58668 = VERTEX_POSITION2282_g58668;
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float3 break2718_g58668 = ase_worldPos;
				float temp_output_2690_0_g58668 = ( _WIND_RANDOM_OFFSET2244_g58668 * 25.0 );
				float clampResult2691_g58668 = clamp( 25.0 , 0.2 , 0.25 );
				float2 appendResult2694_g58668 = (float2(temp_output_2690_0_g58668 , ( temp_output_2690_0_g58668 / clampResult2691_g58668 )));
				float3 appendResult2706_g58668 = (float3(break2708_g58668.x , ( break2708_g58668.y + cos( ( ( ( break2718_g58668.x + break2718_g58668.y + break2718_g58668.z ) * 2.0 ) + appendResult2694_g58668 + FUNC_Angle2470_g58668 + _WIND_TUBULENCE2442_g58668 ) ) ).x , break2708_g58668.z));
				float3 temp_output_2613_0_g58668 = ( clampResult2884_g58668 * appendResult2706_g58668 );
				float3 VERTEX_IVY997_g58668 = ( ( ( cos( temp_output_2613_0_g58668 ) + ( -0.3193 * PI ) ) * input.ase_normal * 0.2 * ( FUNC_SinFunction2336_g58668 * input.ase_color.b ) ) + ( sin( temp_output_2613_0_g58668 ) * cross( input.ase_normal , input.ase_tangent.xyz ) * 0.2 ) );
				float3 m_Ivy2439_g58668 = VERTEX_IVY997_g58668;
				float3 localWind_Typefloat3switch2439_g58668 = Wind_Typefloat3switch2439_g58668( m_switch2439_g58668 , m_Leaf2439_g58668 , m_Palm2439_g58668 , m_Grass2439_g58668 , m_Simple2439_g58668 , m_Ivy2439_g58668 );
				float3 lerpResult3142_g58668 = lerp( float3(0,0,0) , localWind_Typefloat3switch2439_g58668 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58671 = lerpResult3142_g58668;
				
				float m_switch80_g58689 = _UVMode;
				float2 m_UV080_g58689 = input.ase_texcoord.xy;
				float2 m_UV180_g58689 = input.ase_texcoord1.xy;
				float2 m_UV280_g58689 = input.ase_texcoord2.xy;
				float2 m_UV380_g58689 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58689 = float2switchUVMode80_g58689( m_switch80_g58689 , m_UV080_g58689 , m_UV180_g58689 , m_UV280_g58689 , m_UV380_g58689 );
				float2 temp_output_1955_0_g58671 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58671 = (_MainUVs).zw;
				float2 Offset235_g58689 = temp_output_1953_0_g58671;
				float2 temp_output_41_0_g58689 = ( ( localfloat2switchUVMode80_g58689 * temp_output_1955_0_g58671 ) + Offset235_g58689 );
				float2 vertexToFrag70_g58689 = temp_output_41_0_g58689;
				output.ase_texcoord2.xy = vertexToFrag70_g58689;
				float3 temp_output_143_0_g58715 = ( ase_worldPos - _WorldSpaceCameraPos );
				float3 vertexToFrag145_g58715 = temp_output_143_0_g58715;
				output.ase_texcoord3.xyz = vertexToFrag145_g58715;
				output.ase_texcoord4.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58671;

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

			half4 frag(	PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float2 vertexToFrag70_g58689 = input.ase_texcoord2.xy;
				float2 UV213_g58671 = vertexToFrag70_g58689;
				float4 tex2DNode2048_g58671 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58671 );
				float temp_output_22_0_g58711 = tex2DNode2048_g58671.a;
				float temp_output_22_0_g58714 = temp_output_22_0_g58711;
				float3 vertexToFrag145_g58715 = input.ase_texcoord3.xyz;
				float3 temp_output_124_0_g58715 = cross( ddy( vertexToFrag145_g58715 ) , ddx( vertexToFrag145_g58715 ) );
				float3 normalizeResult125_g58715 = normalize( temp_output_124_0_g58715 );
				float3 ase_worldPos = input.ase_texcoord4.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float dotResult74_g58711 = dot( normalizeResult125_g58715 , ase_viewDirSafeWS );
				float temp_output_76_0_g58711 = ( 1.0 - abs( dotResult74_g58711 ) );
				#ifdef UNITY_PASS_SHADOWCASTER
				float staticSwitch281_g58711 = 1.0;
				#else
				float staticSwitch281_g58711 = ( 1.0 - ( temp_output_76_0_g58711 * temp_output_76_0_g58711 ) );
				#endif
				float lerpResult80_g58711 = lerp( 1.0 , staticSwitch281_g58711 , ( _GlancingClipMode + ( ( _CATEGORY_ALPHACLIPPING + _SPACE_ALPHACLIP ) * 0.0 ) ));
				float temp_output_98_0_g58711 = ( ( (0.0 + (( 1.0 - temp_output_22_0_g58714 ) - 0.0) * (( _AlphaRemapMin + ( _AlphaRemap.x * 0.0 ) ) - 0.0) / (1.0 - 0.0)) + (0.0 + (temp_output_22_0_g58714 - 0.0) * (_AlphaRemapMax - 0.0) / (1.0 - 0.0)) ) * lerpResult80_g58711 );
				

				float Alpha = temp_output_98_0_g58711;
				float AlphaClipThreshold = _AlphaCutoffBias;

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
Node;AmplifyShaderEditor.CommentaryNode;2909;-4409.182,-4285.391;Inherit;False;327.9829;621.4482;TRANSLUCENCY ASE;7;2912;2913;2914;2915;2916;2917;2918;;0,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;2932;-4704,-3584;Inherit;False;DESF Weather Wind;151;;58668;b135a554f7e4d0b41bba02c61b34ae74;5,3133,1,2371,1,2432,1,3138,0,3139,0;0;1;FLOAT3;2190
Node;AmplifyShaderEditor.StickyNoteNode;2910;-4768.354,-4280.409;Inherit;False;329.998;401.4002;;;0,0,0,1;_ASETransmissionShadow$$_ASETranslucencyStrength$_ASETranslucencyNormalDistortion$_ASETranslucencyScattering$_ASETranslucencyDirect$_ASETranslucencyAmbient$_ASETranslucencyShadow;0;0
Node;AmplifyShaderEditor.FunctionNode;2879;-4416,-3584;Inherit;False;DESF Core Lit;1;;58671;e0cdd7758f4404849b063afff4596424;21,442,2,2132,0,2213,0,2282,0,2283,0,2284,0,2172,0,2481,0,96,2,2559,0,2591,0,1368,0,2125,0,2028,0,1333,0,2126,1,1896,0,1415,0,830,0,2523,1,3576,0;1;1234;FLOAT3;0,0,0;False;17;FLOAT3;38;FLOAT3;35;FLOAT;37;FLOAT3;1922;FLOAT;33;FLOAT;34;FLOAT;46;FLOAT;814;FLOAT;1660;FLOAT3;656;FLOAT3;657;FLOAT3;655;FLOAT3;1235;FLOAT3;2760;SAMPLERSTATE;1819;SAMPLERSTATE;1824;SAMPLERSTATE;1818
Node;AmplifyShaderEditor.RangedFloatNode;2918;-4384,-3760;Inherit;False;Property;_ASETranslucencyShadow;ASE Translucency Shadow ;173;0;Create;False;1;;0;0;True;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2917;-4384,-3840;Inherit;False;Property;_ASETranslucencyAmbient;ASE Translucency Ambient;150;0;Create;False;1;;0;0;True;0;False;0.8339342;0.8339342;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2916;-4384,-3920;Inherit;False;Property;_ASETranslucencyDirect;ASE Translucency Direct ;149;0;Create;False;1;;0;0;True;0;False;0.9;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2915;-4384,-4000;Inherit;False;Property;_ASETranslucencyScattering;ASE Translucency Scattering ;148;0;Create;False;1;;0;0;True;0;False;2;2;1;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2914;-4384,-4080;Inherit;False;Property;_ASETranslucencyNormalDistortion;ASE Translucency Normal Distortion ;147;0;Create;False;1;;0;0;True;0;False;0.2735869;0.2735869;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2913;-4384,-4160;Inherit;False;Property;_ASETranslucencyStrength;ASE Translucency Strength;146;0;Create;False;1;;0;0;True;0;False;1;1;1;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2912;-4384,-4240;Inherit;False;Property;_ASETransmissionShadow;ASE Transmission Shadow;145;0;Create;False;1;;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2919;-4752,-4096;Inherit;False;Property;_CATEGORY_TRANSLUCENCYASE;CATEGORY_TRANSLUCENCY ASE;144;0;Create;True;0;0;0;True;1;DE_DrawerCategory(TRANSLUCENCY ASE,true,_ASETranslucencyStrength,0,0);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2911;-4752,-4016;Inherit;False;Property;_SPACE_TRANSLUCENCYASE;SPACE_TRANSLUCENCYASE;174;0;Create;True;0;0;0;True;1;DE_DrawerSpace(10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;2895;-3968,-3664;Inherit;False;Property;_Cull;Render Face;0;2;[HideInInspector];[Enum];Create;False;0;0;1;Front,2,Back,1,Both,0;True;0;False;0;2;False;0;1;INT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2920;-3953.374,-3584;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2922;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2923;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2924;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2925;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2926;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2927;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2928;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2929;-3953.374,-3640.745;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2921;-3968,-3584;Float;False;True;-1;2;ALP8310_ShaderGUI;0;12;ALP/Cutout Translucency Wind;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;45;Lighting Model;0;0;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Alpha Clipping;1;0;  Use Shadow Threshold;1;638176265525101667;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;1;638176265066812660;Transmission;1;638176265074118805;  Transmission Shadow;0.5,True,_ASETransmissionShadow;638176265195258737;Translucency;1;638176265085853908;  Translucency Strength;1,True,_ASETranslucencyStrength;638176265248324945;  Normal Distortion;0.5,True,_ASETranslucencyNormalDistortion;638176265293808092;  Scattering;2,True,_ASETranslucencyScattering;638176265318147341;  Direct;0.9,True,_ASETranslucencyScattering;638176265361412706;  Ambient;0.1,True,_ASETranslucencyAmbient;638176265400314693;  Shadow;0.5,True,_ASETranslucencyShadow;638176265464707402;Cast Shadows;1;0;Receive Shadows;1;0;Receive SSAO;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,True,_TessellationPhong;0;  Type;0;0;  Tess;16,True,_TessellationStrength;0;  Min;10,True,_TessellationDistanceMin;0;  Max;25,True,_TessellationDistanceMax;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;0;Clear Coat;0;0;0;11;False;True;True;True;True;True;True;False;True;True;True;False;;True;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2930;-3968,-3484;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;2879;1234;2932;2190
WireConnection;2921;0;2879;38
WireConnection;2921;1;2879;35
WireConnection;2921;3;2879;37
WireConnection;2921;4;2879;33
WireConnection;2921;5;2879;34
WireConnection;2921;6;2879;46
WireConnection;2921;7;2879;814
WireConnection;2921;16;2879;1660
WireConnection;2921;14;2879;656
WireConnection;2921;15;2879;657
WireConnection;2921;8;2879;1235
ASEEND*/
//CHKSM=8D4451C2284258E1A043630D43E7FB3E7A560D56