// Made with Amplify Shader Editor v1.9.7.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ALP/Surface Detail Wind"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector][Enum(Front,2,Back,1,Both,0)]_Cull("Render Face", Int) = 2
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
		_NormalStrength("Normal Strength", Float) = 1
		[DE_DrawerSpace(10)]_SPACE_SURFACEINPUTS("SPACE_SURFACE INPUTS", Float) = 0
		[DE_DrawerCategory(TRANSMISSION,true,_TransmissionEnable,0,0)]_CATEGORY_TRANSMISSION("CATEGORY_TRANSMISSION", Float) = 0
		[DE_DrawerSpace(10)]_SPACE_TRANSLUCENCY("SPACE_TRANSLUCENCY", Float) = 0
		[DE_DrawerCategory(DETAIL MAPPING,true,_DetailEnable,0,0)]_CATEGORY_DETAILMAPPING("CATEGORY_DETAIL MAPPING", Float) = 0
		[DE_DrawerToggleLeft]_DetailEnable("ENABLE DETAIL MAP", Float) = 0
		[HDR]_DetailColor("Detail Color", Color) = (1,1,1,0)
		_DetailBrightness("Brightness", Range( 0 , 2)) = 1
		[DE_DrawerTextureSingleLine]_DetailColorMap("Detail Map", 2D) = "white" {}
		[DE_DrawerTilingOffset]_DetailUVs("Detail UVs", Vector) = (1,1,0,0)
		[DE_DrawerFloatEnum(UV0 _UV1 _UV2 _UV3)]_DetailUVMode("Detail UV Mode", Float) = 0
		_DetailUVRotation("Detail UV Rotation", Float) = 0
		[Normal][DE_DrawerTextureSingleLine]_DetailNormalMap("Normal Map", 2D) = "bump" {}
		_DetailNormalStrength("Normal Strength", Float) = 1
		[DE_DrawerFloatEnum(Off _Red _Green _Blue _Alpha)]_DetailBlendVertexColor("Blend Vertex Color", Int) = 0
		[DE_DrawerFloatEnum(BaseColor _Detail)]_DetailBlendSource("Blend Source", Float) = 1
		_DetailBlendStrength("Blend Strength", Range( 0 , 3)) = 1
		_DetailBlendHeight("Blend Height", Range( 0.001 , 3)) = 0.5
		_DetailBlendSmooth("Blend Smooth", Range( 0.001 , 1)) = 0.5
		[DE_DrawerToggleLeft][Space(5)]_DetailBlendEnableAltitudeMask("ENABLE ALTITUDE MASK", Float) = 0
		_DetailBlendHeightMin("Blend Height Min", Float) = -0.5
		_DetailBlendHeightMax("Blend Height Max", Float) = 1
		[DE_DrawerToggleLeft][Space(10)]_DetailMaskEnable("ENABLE DETAIL MAP MASK", Float) = 0
		[DE_DrawerToggleNoKeyword]_DetailMaskIsInverted("Detail Mask Is Inverted", Float) = 0
		[DE_DrawerTextureSingleLine]_DetailMaskMap("Detail Mask", 2D) = "white" {}
		[DE_DrawerTilingOffset]_DetailMaskUVs("Detail Mask UVs", Vector) = (1,1,0,0)
		_DetailMaskUVRotation("Detail Mask Rotation", Float) = 0
		_DetailMaskBlendStrength("Detail Mask Blend Strength", Range( 0.001 , 1)) = 1
		_DetailMaskBlendHardness("Detail Mask Blend Hardness", Range( 0.001 , 5)) = 1
		_DetailMaskBlendFalloff("Detail Mask Blend Falloff", Range( 0.001 , 0.999)) = 0.999
		[DE_DrawerSpace(10)]_SPACE_DETAIL("SPACE_DETAIL", Float) = 0
		[DE_DrawerCategory(DETAIL MAPPING SECONDARY,true,_DetailSecondaryEnable,0,0)]_CATEGORY_DETAILMAPPINGSECONDARY("CATEGORY_DETAIL MAPPING SECONDARY", Float) = 0
		[DE_DrawerSpace(10)]_SPACE_DETAILSECONDARY("SPACE_DETAILSECONDARY", Float) = 0
		[DE_DrawerCategory(WIND,true,_WindEnable,0,0)]_CATEGORY_WIND("CATEGORY_WIND", Float) = 0
		[DE_DrawerToggleLeft]_WindEnable("ENABLE WIND", Float) = 0
		[DE_DrawerFloatEnum(Global _Local)]_WindEnableMode("Enable Wind Mode", Float) = 0
		[Header(WIND GLOBAL)]_WindGlobalIntensity("Wind Intensity", Float) = 1
		[Header(WIND LOCAL)]_WindLocalIntensity("Local Wind Intensity", Float) = 1
		_WindLocalPulseFrequency("Local Wind Pulse Frequency", Float) = 0.1
		_WindLocalRandomOffset("Local Random Offset", Float) = 0.2
		_WindLocalDirection("Local Wind Direction", Range( 0 , 360)) = 0
		[DE_DrawerSpace(10)]_SPACE_WIND("SPACE_WIND", Float) = 0


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
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#ifdef UNITY_COLORSPACE_GAMMA//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(2.0, 2.0, 2.0, 2.0)//ASE Color Space Def
			#else // Linear values//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(4.59479380, 4.59479380, 4.59479380, 2.0)//ASE Color Space Def
			#endif//ASE Color Space Def
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL


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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_DetailColorMap);
			TEXTURE2D(_DetailMaskMap);
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);
			TEXTURE2D(_DetailNormalMap);
			TEXTURE2D(_MainMaskMap);
			SAMPLER(sampler_MainMaskMap);


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
			
			float2 float2switchUVMode80_g58675( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58584( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58587( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float MaskVCSwitch44_g58598( float m_switch, float m_Off, float m_R, float m_G, float m_B, float m_A )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_R;
				else if(m_switch ==2)
					return m_G;
				else if(m_switch ==3)
					return m_B;
				else if(m_switch ==4)
					return m_A;
				else
				return float(0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				
				float m_switch80_g58675 = _UVMode;
				float2 m_UV080_g58675 = input.texcoord.xy;
				float2 m_UV180_g58675 = input.texcoord1.xy;
				float2 m_UV280_g58675 = input.texcoord2.xy;
				float2 m_UV380_g58675 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58675 = float2switchUVMode80_g58675( m_switch80_g58675 , m_UV080_g58675 , m_UV180_g58675 , m_UV280_g58675 , m_UV380_g58675 );
				float2 temp_output_1955_0_g58657 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58657 = (_MainUVs).zw;
				float2 Offset235_g58675 = temp_output_1953_0_g58657;
				float2 temp_output_41_0_g58675 = ( ( localfloat2switchUVMode80_g58675 * temp_output_1955_0_g58657 ) + Offset235_g58675 );
				float2 vertexToFrag70_g58675 = temp_output_41_0_g58675;
				output.ase_texcoord9.xy = vertexToFrag70_g58675;
				float temp_output_6_0_g58584 = _DetailUVRotation;
				float temp_output_200_0_g58584 = radians( temp_output_6_0_g58584 );
				float temp_output_13_0_g58584 = cos( temp_output_200_0_g58584 );
				float m_switch80_g58584 = _DetailUVMode;
				float2 m_UV080_g58584 = input.texcoord.xy;
				float2 m_UV180_g58584 = input.texcoord1.xy;
				float2 m_UV280_g58584 = input.texcoord2.xy;
				float2 m_UV380_g58584 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58584 = float2switchUVMode80_g58584( m_switch80_g58584 , m_UV080_g58584 , m_UV180_g58584 , m_UV280_g58584 , m_UV380_g58584 );
				float2 temp_output_9_0_g58584 = float2( 0.5,0.5 );
				float2 break39_g58584 = ( localfloat2switchUVMode80_g58584 - temp_output_9_0_g58584 );
				float temp_output_14_0_g58584 = sin( temp_output_200_0_g58584 );
				float2 appendResult36_g58584 = (float2(( ( temp_output_13_0_g58584 * break39_g58584.x ) + ( temp_output_14_0_g58584 * break39_g58584.y ) ) , ( ( temp_output_13_0_g58584 * break39_g58584.y ) - ( temp_output_14_0_g58584 * break39_g58584.x ) )));
				float2 Offset235_g58584 = (_DetailUVs).zw;
				float2 temp_output_41_0_g58584 = ( ( ( appendResult36_g58584 * ( (_DetailUVs).xy / 1.0 ) ) + temp_output_9_0_g58584 ) + Offset235_g58584 );
				float2 _ConstantAnchor = float2(0.5,0.5);
				float2 vertexToFrag70_g58584 = ( temp_output_41_0_g58584 - ( ( ( (_DetailUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord9.zw = vertexToFrag70_g58584;
				float temp_output_6_0_g58587 = _DetailMaskUVRotation;
				float temp_output_200_0_g58587 = radians( temp_output_6_0_g58587 );
				float temp_output_13_0_g58587 = cos( temp_output_200_0_g58587 );
				float DetailUVMode1060_g58570 = _DetailUVMode;
				float m_switch80_g58587 = DetailUVMode1060_g58570;
				float2 m_UV080_g58587 = input.texcoord.xy;
				float2 m_UV180_g58587 = input.texcoord1.xy;
				float2 m_UV280_g58587 = input.texcoord2.xy;
				float2 m_UV380_g58587 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58587 = float2switchUVMode80_g58587( m_switch80_g58587 , m_UV080_g58587 , m_UV180_g58587 , m_UV280_g58587 , m_UV380_g58587 );
				float2 temp_output_9_0_g58587 = float2( 0.5,0.5 );
				float2 break39_g58587 = ( localfloat2switchUVMode80_g58587 - temp_output_9_0_g58587 );
				float temp_output_14_0_g58587 = sin( temp_output_200_0_g58587 );
				float2 appendResult36_g58587 = (float2(( ( temp_output_13_0_g58587 * break39_g58587.x ) + ( temp_output_14_0_g58587 * break39_g58587.y ) ) , ( ( temp_output_13_0_g58587 * break39_g58587.y ) - ( temp_output_14_0_g58587 * break39_g58587.x ) )));
				float2 Offset235_g58587 = (_DetailMaskUVs).zw;
				float2 temp_output_41_0_g58587 = ( ( ( appendResult36_g58587 * ( (_DetailMaskUVs).xy / 1.0 ) ) + temp_output_9_0_g58587 ) + Offset235_g58587 );
				float2 vertexToFrag70_g58587 = ( temp_output_41_0_g58587 - ( ( ( (_DetailMaskUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord10.xy = vertexToFrag70_g58587;
				
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord10.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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
						 ) : SV_Target
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

				float3 temp_output_1923_0_g58657 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58675 = input.ase_texcoord9.xy;
				float2 UV213_g58657 = vertexToFrag70_g58675;
				float4 tex2DNode2048_g58657 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58657 );
				float3 temp_output_3_0_g58657 = ( temp_output_1923_0_g58657 * (tex2DNode2048_g58657).rgb * _Brightness );
				float3 temp_output_39_0_g58570 = temp_output_3_0_g58657;
				float localStochasticTiling159_g58575 = ( 0.0 );
				float2 vertexToFrag70_g58584 = input.ase_texcoord9.zw;
				float2 temp_output_1334_0_g58570 = vertexToFrag70_g58584;
				float2 UV159_g58575 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58575 = _DetailColorMap_TexelSize;
				float4 Offsets159_g58575 = float4( 0,0,0,0 );
				float2 Weights159_g58575 = float2( 0,0 );
				{
				UV159_g58575 = UV159_g58575 * TexelSize159_g58575.zw - 0.5;
				float2 f = frac( UV159_g58575 );
				UV159_g58575 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58575.x - 0.5, UV159_g58575.x + 1.5, UV159_g58575.y - 0.5, UV159_g58575.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58575 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58575.xyxy;
				Weights159_g58575 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58576 = Offsets159_g58575;
				float2 Input_FetchWeights143_g58576 = Weights159_g58575;
				float2 break46_g58576 = Input_FetchWeights143_g58576;
				float4 lerpResult20_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yw ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xw ) , break46_g58576.x);
				float4 lerpResult40_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yz ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xz ) , break46_g58576.x);
				float4 lerpResult22_g58576 = lerp( lerpResult20_g58576 , lerpResult40_g58576 , break46_g58576.y);
				float4 Output_Fetch2D44_g58576 = lerpResult22_g58576;
				float3 temp_output_44_0_g58570 = ( (_DetailColor).rgb * (Output_Fetch2D44_g58576).rgb * _DetailBrightness );
				float3 temp_output_1272_0_g58570 = (unity_ColorSpaceDouble).rgb;
				float3 temp_output_1190_0_g58570 = ( temp_output_44_0_g58570 * temp_output_1272_0_g58570 );
				float3 BaseColor_RGB40_g58570 = temp_output_39_0_g58570;
				float localStochasticTiling159_g58582 = ( 0.0 );
				float2 vertexToFrag70_g58587 = input.ase_texcoord10.xy;
				float2 temp_output_1339_0_g58570 = vertexToFrag70_g58587;
				float2 UV159_g58582 = temp_output_1339_0_g58570;
				float4 TexelSize159_g58582 = _DetailMaskMap_TexelSize;
				float4 Offsets159_g58582 = float4( 0,0,0,0 );
				float2 Weights159_g58582 = float2( 0,0 );
				{
				UV159_g58582 = UV159_g58582 * TexelSize159_g58582.zw - 0.5;
				float2 f = frac( UV159_g58582 );
				UV159_g58582 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58582.x - 0.5, UV159_g58582.x + 1.5, UV159_g58582.y - 0.5, UV159_g58582.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58582 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58582.xyxy;
				Weights159_g58582 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58583 = Offsets159_g58582;
				float2 Input_FetchWeights143_g58583 = Weights159_g58582;
				float2 break46_g58583 = Input_FetchWeights143_g58583;
				float4 lerpResult20_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yw ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xw ) , break46_g58583.x);
				float4 lerpResult40_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yz ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xz ) , break46_g58583.x);
				float4 lerpResult22_g58583 = lerp( lerpResult20_g58583 , lerpResult40_g58583 , break46_g58583.y);
				float4 Output_Fetch2D44_g58583 = lerpResult22_g58583;
				float4 break50_g58583 = Output_Fetch2D44_g58583;
				float lerpResult997_g58570 = lerp( ( 1.0 - break50_g58583.r ) , break50_g58583.r , _DetailMaskIsInverted);
				float temp_output_15_0_g58596 = ( 1.0 - lerpResult997_g58570 );
				float saferPower2_g58596 = abs( max( saturate( (0.0 + (temp_output_15_0_g58596 - ( 1.0 - _DetailMaskBlendStrength )) * (_DetailMaskBlendHardness - 0.0) / (1.0 - ( 1.0 - _DetailMaskBlendStrength ))) ) , 0.0 ) );
				float Blend_DetailMask986_g58570 = saturate( pow( saferPower2_g58596 , ( 1.0 - _DetailMaskBlendFalloff ) ) );
				float3 lerpResult1194_g58570 = lerp( BaseColor_RGB40_g58570 , temp_output_1190_0_g58570 , Blend_DetailMask986_g58570);
				float temp_output_1162_0_g58570 = ( 1.0 - Blend_DetailMask986_g58570 );
				float3 appendResult1161_g58570 = (float3(temp_output_1162_0_g58570 , temp_output_1162_0_g58570 , temp_output_1162_0_g58570));
				float3 lerpResult1005_g58570 = lerp( temp_output_1190_0_g58570 , ( ( lerpResult1194_g58570 * Blend_DetailMask986_g58570 ) + appendResult1161_g58570 ) , _DetailMaskEnable);
				float3 BaseColor_Detail255_g58570 = lerpResult1005_g58570;
				float BaseColor_R1273_g58570 = temp_output_39_0_g58570.x;
				float BaseColor_DetailR887_g58570 = Output_Fetch2D44_g58576.r;
				float lerpResult1105_g58570 = lerp( BaseColor_R1273_g58570 , BaseColor_DetailR887_g58570 , _DetailBlendSource);
				float m_switch44_g58598 = (float)_DetailBlendVertexColor;
				float m_Off44_g58598 = 1.0;
				float dotResult58_g58598 = dot( input.ase_color.g , input.ase_color.g );
				float dotResult61_g58598 = dot( input.ase_color.b , input.ase_color.b );
				float m_R44_g58598 = ( dotResult58_g58598 + dotResult61_g58598 );
				float dotResult57_g58598 = dot( input.ase_color.r , input.ase_color.r );
				float m_G44_g58598 = ( dotResult57_g58598 + dotResult58_g58598 );
				float m_B44_g58598 = ( dotResult57_g58598 + dotResult61_g58598 );
				float m_A44_g58598 = input.ase_color.a;
				float localMaskVCSwitch44_g58598 = MaskVCSwitch44_g58598( m_switch44_g58598 , m_Off44_g58598 , m_R44_g58598 , m_G44_g58598 , m_B44_g58598 , m_A44_g58598 );
				float clampResult54_g58598 = clamp( ( ( localMaskVCSwitch44_g58598 * _DetailBlendHeight ) / _DetailBlendSmooth ) , 0.0 , 1.0 );
				float Blend647_g58570 = saturate( ( ( ( ( lerpResult1105_g58570 - 0.5 ) * ( ( 1.0 - _DetailBlendStrength ) - 0.9 ) ) / ( 1.0 - _DetailBlendSmooth ) ) + ( 1.0 - clampResult54_g58598 ) ) );
				float temp_output_1171_0_g58570 = ( 1.0 - Blend647_g58570 );
				float3 appendResult1174_g58570 = (float3(temp_output_1171_0_g58570 , temp_output_1171_0_g58570 , temp_output_1171_0_g58570));
				float3 temp_output_1173_0_g58570 = ( ( BaseColor_Detail255_g58570 * Blend647_g58570 ) + appendResult1174_g58570 );
				float temp_output_20_0_g58599 = _DetailBlendHeightMin;
				float temp_output_21_0_g58599 = _DetailBlendHeightMax;
				float3 worldToObj1466_g58570 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 WorldPosition1436_g58570 = worldToObj1466_g58570;
				float smoothstepResult25_g58599 = smoothstep( temp_output_20_0_g58599 , temp_output_21_0_g58599 , WorldPosition1436_g58570.y);
				float DetailBlendHeight1440_g58570 = smoothstepResult25_g58599;
				float3 lerpResult1438_g58570 = lerp( temp_output_1173_0_g58570 , temp_output_39_0_g58570 , DetailBlendHeight1440_g58570);
				float3 lerpResult1457_g58570 = lerp( temp_output_1173_0_g58570 , lerpResult1438_g58570 , _DetailBlendEnableAltitudeMask);
				float3 temp_output_1180_0_g58570 = ( temp_output_39_0_g58570 * lerpResult1457_g58570 );
				float temp_output_634_0_g58570 = ( _DetailEnable + ( ( _CATEGORY_DETAILMAPPING + _SPACE_DETAIL + _CATEGORY_DETAILMAPPINGSECONDARY + _SPACE_DETAILSECONDARY ) * 0.0 ) );
				float3 lerpResult409_g58570 = lerp( temp_output_39_0_g58570 , temp_output_1180_0_g58570 , temp_output_634_0_g58570);
				
				float3 unpack1891_g58657 = UnpackNormalScale( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, UV213_g58657 ), _NormalStrength );
				unpack1891_g58657.z = lerp( 1, unpack1891_g58657.z, saturate(_NormalStrength) );
				float3 temp_output_38_0_g58570 = unpack1891_g58657;
				float localStochasticTiling159_g58581 = ( 0.0 );
				float2 UV159_g58581 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58581 = _DetailNormalMap_TexelSize;
				float4 Offsets159_g58581 = float4( 0,0,0,0 );
				float2 Weights159_g58581 = float2( 0,0 );
				{
				UV159_g58581 = UV159_g58581 * TexelSize159_g58581.zw - 0.5;
				float2 f = frac( UV159_g58581 );
				UV159_g58581 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58581.x - 0.5, UV159_g58581.x + 1.5, UV159_g58581.y - 0.5, UV159_g58581.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58581 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58581.xyxy;
				Weights159_g58581 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58580 = Offsets159_g58581;
				float2 Input_FetchWeights143_g58580 = Weights159_g58581;
				float2 break46_g58580 = Input_FetchWeights143_g58580;
				float4 lerpResult20_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yw ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xw ) , break46_g58580.x);
				float4 lerpResult40_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yz ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xz ) , break46_g58580.x);
				float4 lerpResult22_g58580 = lerp( lerpResult20_g58580 , lerpResult40_g58580 , break46_g58580.y);
				float4 Output_Fetch2D44_g58580 = lerpResult22_g58580;
				float3 unpack499_g58570 = UnpackNormalScale( Output_Fetch2D44_g58580, _DetailNormalStrength );
				unpack499_g58570.z = lerp( 1, unpack499_g58570.z, saturate(_DetailNormalStrength) );
				float3 Normal_In880_g58570 = temp_output_38_0_g58570;
				float3 lerpResult1286_g58570 = lerp( Normal_In880_g58570 , unpack499_g58570 , Blend_DetailMask986_g58570);
				float3 lerpResult1011_g58570 = lerp( unpack499_g58570 , lerpResult1286_g58570 , _DetailMaskEnable);
				float3 Normal_Detail199_g58570 = lerpResult1011_g58570;
				float layeredBlendVar1278_g58570 = Blend647_g58570;
				float3 layeredBlend1278_g58570 = ( lerp( temp_output_38_0_g58570,Normal_Detail199_g58570 , layeredBlendVar1278_g58570 ) );
				float3 break817_g58570 = layeredBlend1278_g58570;
				float3 appendResult820_g58570 = (float3(break817_g58570.x , break817_g58570.y , ( break817_g58570.z + 0.001 )));
				float3 lerpResult410_g58570 = lerp( temp_output_38_0_g58570 , appendResult820_g58570 , temp_output_634_0_g58570);
				
				float4 tex2DNode2050_g58657 = SAMPLE_TEXTURE2D( _MainMaskMap, sampler_MainMaskMap, UV213_g58657 );
				
				float lerpResult2650_g58657 = lerp( tex2DNode2050_g58657.g , ( 1.0 - tex2DNode2050_g58657.g ) , _MainMaskType);
				float temp_output_2693_0_g58657 = ( lerpResult2650_g58657 * _SmoothnessStrength );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float2 appendResult2645_g58657 = (float2(ase_viewDirSafeWS.xy));
				float3 appendResult2644_g58657 = (float3(appendResult2645_g58657 , ( ase_viewDirSafeWS.z / 1.06 )));
				float3 break2680_g58657 = unpack1891_g58657;
				float3 normalizeResult2641_g58657 = normalize( ( ( WorldTangent * break2680_g58657.x ) + ( WorldBiTangent * break2680_g58657.y ) + ( WorldNormal * break2680_g58657.z ) ) );
				float3 Normal_Per_Pixel2690_g58657 = normalizeResult2641_g58657;
				float fresnelNdotV2685_g58657 = dot( normalize( Normal_Per_Pixel2690_g58657 ), appendResult2644_g58657 );
				float fresnelNode2685_g58657 = ( 0.0 + ( 1.0 - _SmoothnessFresnelScale ) * pow( max( 1.0 - fresnelNdotV2685_g58657 , 0.0001 ), _SmoothnessFresnelPower ) );
				float lerpResult2636_g58657 = lerp( temp_output_2693_0_g58657 , ( temp_output_2693_0_g58657 - fresnelNode2685_g58657 ) , _SmoothnessFresnelEnable);
				
				float lerpResult3414_g58657 = lerp( 1.0 , min( tex2DNode2050_g58657.b , input.ase_color.a ) , _OcclusionStrengthAO);
				

				float3 BaseColor = lerpResult409_g58570;
				float3 Normal = lerpResult410_g58570;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = ( _MetallicStrength * tex2DNode2050_g58657.r );
				float Smoothness = saturate( lerpResult2636_g58657 );
				float Occlusion = saturate( lerpResult3414_g58657 );
				float Alpha = 1;
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
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
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
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			

			float3 _LightDirection;
			float3 _LightPosition;

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;
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

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

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

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
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
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				

				float Alpha = 1;
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
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#ifdef UNITY_COLORSPACE_GAMMA//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(2.0, 2.0, 2.0, 2.0)//ASE Color Space Def
			#else // Linear values//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(4.59479380, 4.59479380, 4.59479380, 2.0)//ASE Color Space Def
			#endif//ASE Color Space Def
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_DetailColorMap);
			TEXTURE2D(_DetailMaskMap);


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
			
			float2 float2switchUVMode80_g58675( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58584( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58587( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float MaskVCSwitch44_g58598( float m_switch, float m_Off, float m_R, float m_G, float m_B, float m_A )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_R;
				else if(m_switch ==2)
					return m_G;
				else if(m_switch ==3)
					return m_B;
				else if(m_switch ==4)
					return m_A;
				else
				return float(0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				
				float m_switch80_g58675 = _UVMode;
				float2 m_UV080_g58675 = input.texcoord0.xy;
				float2 m_UV180_g58675 = input.texcoord1.xy;
				float2 m_UV280_g58675 = input.texcoord2.xy;
				float2 m_UV380_g58675 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58675 = float2switchUVMode80_g58675( m_switch80_g58675 , m_UV080_g58675 , m_UV180_g58675 , m_UV280_g58675 , m_UV380_g58675 );
				float2 temp_output_1955_0_g58657 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58657 = (_MainUVs).zw;
				float2 Offset235_g58675 = temp_output_1953_0_g58657;
				float2 temp_output_41_0_g58675 = ( ( localfloat2switchUVMode80_g58675 * temp_output_1955_0_g58657 ) + Offset235_g58675 );
				float2 vertexToFrag70_g58675 = temp_output_41_0_g58675;
				output.ase_texcoord4.xy = vertexToFrag70_g58675;
				float temp_output_6_0_g58584 = _DetailUVRotation;
				float temp_output_200_0_g58584 = radians( temp_output_6_0_g58584 );
				float temp_output_13_0_g58584 = cos( temp_output_200_0_g58584 );
				float m_switch80_g58584 = _DetailUVMode;
				float2 m_UV080_g58584 = input.texcoord0.xy;
				float2 m_UV180_g58584 = input.texcoord1.xy;
				float2 m_UV280_g58584 = input.texcoord2.xy;
				float2 m_UV380_g58584 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58584 = float2switchUVMode80_g58584( m_switch80_g58584 , m_UV080_g58584 , m_UV180_g58584 , m_UV280_g58584 , m_UV380_g58584 );
				float2 temp_output_9_0_g58584 = float2( 0.5,0.5 );
				float2 break39_g58584 = ( localfloat2switchUVMode80_g58584 - temp_output_9_0_g58584 );
				float temp_output_14_0_g58584 = sin( temp_output_200_0_g58584 );
				float2 appendResult36_g58584 = (float2(( ( temp_output_13_0_g58584 * break39_g58584.x ) + ( temp_output_14_0_g58584 * break39_g58584.y ) ) , ( ( temp_output_13_0_g58584 * break39_g58584.y ) - ( temp_output_14_0_g58584 * break39_g58584.x ) )));
				float2 Offset235_g58584 = (_DetailUVs).zw;
				float2 temp_output_41_0_g58584 = ( ( ( appendResult36_g58584 * ( (_DetailUVs).xy / 1.0 ) ) + temp_output_9_0_g58584 ) + Offset235_g58584 );
				float2 _ConstantAnchor = float2(0.5,0.5);
				float2 vertexToFrag70_g58584 = ( temp_output_41_0_g58584 - ( ( ( (_DetailUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord4.zw = vertexToFrag70_g58584;
				float temp_output_6_0_g58587 = _DetailMaskUVRotation;
				float temp_output_200_0_g58587 = radians( temp_output_6_0_g58587 );
				float temp_output_13_0_g58587 = cos( temp_output_200_0_g58587 );
				float DetailUVMode1060_g58570 = _DetailUVMode;
				float m_switch80_g58587 = DetailUVMode1060_g58570;
				float2 m_UV080_g58587 = input.texcoord0.xy;
				float2 m_UV180_g58587 = input.texcoord1.xy;
				float2 m_UV280_g58587 = input.texcoord2.xy;
				float2 m_UV380_g58587 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58587 = float2switchUVMode80_g58587( m_switch80_g58587 , m_UV080_g58587 , m_UV180_g58587 , m_UV280_g58587 , m_UV380_g58587 );
				float2 temp_output_9_0_g58587 = float2( 0.5,0.5 );
				float2 break39_g58587 = ( localfloat2switchUVMode80_g58587 - temp_output_9_0_g58587 );
				float temp_output_14_0_g58587 = sin( temp_output_200_0_g58587 );
				float2 appendResult36_g58587 = (float2(( ( temp_output_13_0_g58587 * break39_g58587.x ) + ( temp_output_14_0_g58587 * break39_g58587.y ) ) , ( ( temp_output_13_0_g58587 * break39_g58587.y ) - ( temp_output_14_0_g58587 * break39_g58587.x ) )));
				float2 Offset235_g58587 = (_DetailMaskUVs).zw;
				float2 temp_output_41_0_g58587 = ( ( ( appendResult36_g58587 * ( (_DetailMaskUVs).xy / 1.0 ) ) + temp_output_9_0_g58587 ) + Offset235_g58587 );
				float2 vertexToFrag70_g58587 = ( temp_output_41_0_g58587 - ( ( ( (_DetailMaskUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord5.xy = vertexToFrag70_g58587;
				
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord5.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				float3 temp_output_1923_0_g58657 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58675 = input.ase_texcoord4.xy;
				float2 UV213_g58657 = vertexToFrag70_g58675;
				float4 tex2DNode2048_g58657 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58657 );
				float3 temp_output_3_0_g58657 = ( temp_output_1923_0_g58657 * (tex2DNode2048_g58657).rgb * _Brightness );
				float3 temp_output_39_0_g58570 = temp_output_3_0_g58657;
				float localStochasticTiling159_g58575 = ( 0.0 );
				float2 vertexToFrag70_g58584 = input.ase_texcoord4.zw;
				float2 temp_output_1334_0_g58570 = vertexToFrag70_g58584;
				float2 UV159_g58575 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58575 = _DetailColorMap_TexelSize;
				float4 Offsets159_g58575 = float4( 0,0,0,0 );
				float2 Weights159_g58575 = float2( 0,0 );
				{
				UV159_g58575 = UV159_g58575 * TexelSize159_g58575.zw - 0.5;
				float2 f = frac( UV159_g58575 );
				UV159_g58575 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58575.x - 0.5, UV159_g58575.x + 1.5, UV159_g58575.y - 0.5, UV159_g58575.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58575 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58575.xyxy;
				Weights159_g58575 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58576 = Offsets159_g58575;
				float2 Input_FetchWeights143_g58576 = Weights159_g58575;
				float2 break46_g58576 = Input_FetchWeights143_g58576;
				float4 lerpResult20_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yw ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xw ) , break46_g58576.x);
				float4 lerpResult40_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yz ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xz ) , break46_g58576.x);
				float4 lerpResult22_g58576 = lerp( lerpResult20_g58576 , lerpResult40_g58576 , break46_g58576.y);
				float4 Output_Fetch2D44_g58576 = lerpResult22_g58576;
				float3 temp_output_44_0_g58570 = ( (_DetailColor).rgb * (Output_Fetch2D44_g58576).rgb * _DetailBrightness );
				float3 temp_output_1272_0_g58570 = (unity_ColorSpaceDouble).rgb;
				float3 temp_output_1190_0_g58570 = ( temp_output_44_0_g58570 * temp_output_1272_0_g58570 );
				float3 BaseColor_RGB40_g58570 = temp_output_39_0_g58570;
				float localStochasticTiling159_g58582 = ( 0.0 );
				float2 vertexToFrag70_g58587 = input.ase_texcoord5.xy;
				float2 temp_output_1339_0_g58570 = vertexToFrag70_g58587;
				float2 UV159_g58582 = temp_output_1339_0_g58570;
				float4 TexelSize159_g58582 = _DetailMaskMap_TexelSize;
				float4 Offsets159_g58582 = float4( 0,0,0,0 );
				float2 Weights159_g58582 = float2( 0,0 );
				{
				UV159_g58582 = UV159_g58582 * TexelSize159_g58582.zw - 0.5;
				float2 f = frac( UV159_g58582 );
				UV159_g58582 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58582.x - 0.5, UV159_g58582.x + 1.5, UV159_g58582.y - 0.5, UV159_g58582.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58582 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58582.xyxy;
				Weights159_g58582 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58583 = Offsets159_g58582;
				float2 Input_FetchWeights143_g58583 = Weights159_g58582;
				float2 break46_g58583 = Input_FetchWeights143_g58583;
				float4 lerpResult20_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yw ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xw ) , break46_g58583.x);
				float4 lerpResult40_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yz ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xz ) , break46_g58583.x);
				float4 lerpResult22_g58583 = lerp( lerpResult20_g58583 , lerpResult40_g58583 , break46_g58583.y);
				float4 Output_Fetch2D44_g58583 = lerpResult22_g58583;
				float4 break50_g58583 = Output_Fetch2D44_g58583;
				float lerpResult997_g58570 = lerp( ( 1.0 - break50_g58583.r ) , break50_g58583.r , _DetailMaskIsInverted);
				float temp_output_15_0_g58596 = ( 1.0 - lerpResult997_g58570 );
				float saferPower2_g58596 = abs( max( saturate( (0.0 + (temp_output_15_0_g58596 - ( 1.0 - _DetailMaskBlendStrength )) * (_DetailMaskBlendHardness - 0.0) / (1.0 - ( 1.0 - _DetailMaskBlendStrength ))) ) , 0.0 ) );
				float Blend_DetailMask986_g58570 = saturate( pow( saferPower2_g58596 , ( 1.0 - _DetailMaskBlendFalloff ) ) );
				float3 lerpResult1194_g58570 = lerp( BaseColor_RGB40_g58570 , temp_output_1190_0_g58570 , Blend_DetailMask986_g58570);
				float temp_output_1162_0_g58570 = ( 1.0 - Blend_DetailMask986_g58570 );
				float3 appendResult1161_g58570 = (float3(temp_output_1162_0_g58570 , temp_output_1162_0_g58570 , temp_output_1162_0_g58570));
				float3 lerpResult1005_g58570 = lerp( temp_output_1190_0_g58570 , ( ( lerpResult1194_g58570 * Blend_DetailMask986_g58570 ) + appendResult1161_g58570 ) , _DetailMaskEnable);
				float3 BaseColor_Detail255_g58570 = lerpResult1005_g58570;
				float BaseColor_R1273_g58570 = temp_output_39_0_g58570.x;
				float BaseColor_DetailR887_g58570 = Output_Fetch2D44_g58576.r;
				float lerpResult1105_g58570 = lerp( BaseColor_R1273_g58570 , BaseColor_DetailR887_g58570 , _DetailBlendSource);
				float m_switch44_g58598 = (float)_DetailBlendVertexColor;
				float m_Off44_g58598 = 1.0;
				float dotResult58_g58598 = dot( input.ase_color.g , input.ase_color.g );
				float dotResult61_g58598 = dot( input.ase_color.b , input.ase_color.b );
				float m_R44_g58598 = ( dotResult58_g58598 + dotResult61_g58598 );
				float dotResult57_g58598 = dot( input.ase_color.r , input.ase_color.r );
				float m_G44_g58598 = ( dotResult57_g58598 + dotResult58_g58598 );
				float m_B44_g58598 = ( dotResult57_g58598 + dotResult61_g58598 );
				float m_A44_g58598 = input.ase_color.a;
				float localMaskVCSwitch44_g58598 = MaskVCSwitch44_g58598( m_switch44_g58598 , m_Off44_g58598 , m_R44_g58598 , m_G44_g58598 , m_B44_g58598 , m_A44_g58598 );
				float clampResult54_g58598 = clamp( ( ( localMaskVCSwitch44_g58598 * _DetailBlendHeight ) / _DetailBlendSmooth ) , 0.0 , 1.0 );
				float Blend647_g58570 = saturate( ( ( ( ( lerpResult1105_g58570 - 0.5 ) * ( ( 1.0 - _DetailBlendStrength ) - 0.9 ) ) / ( 1.0 - _DetailBlendSmooth ) ) + ( 1.0 - clampResult54_g58598 ) ) );
				float temp_output_1171_0_g58570 = ( 1.0 - Blend647_g58570 );
				float3 appendResult1174_g58570 = (float3(temp_output_1171_0_g58570 , temp_output_1171_0_g58570 , temp_output_1171_0_g58570));
				float3 temp_output_1173_0_g58570 = ( ( BaseColor_Detail255_g58570 * Blend647_g58570 ) + appendResult1174_g58570 );
				float temp_output_20_0_g58599 = _DetailBlendHeightMin;
				float temp_output_21_0_g58599 = _DetailBlendHeightMax;
				float3 worldToObj1466_g58570 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 WorldPosition1436_g58570 = worldToObj1466_g58570;
				float smoothstepResult25_g58599 = smoothstep( temp_output_20_0_g58599 , temp_output_21_0_g58599 , WorldPosition1436_g58570.y);
				float DetailBlendHeight1440_g58570 = smoothstepResult25_g58599;
				float3 lerpResult1438_g58570 = lerp( temp_output_1173_0_g58570 , temp_output_39_0_g58570 , DetailBlendHeight1440_g58570);
				float3 lerpResult1457_g58570 = lerp( temp_output_1173_0_g58570 , lerpResult1438_g58570 , _DetailBlendEnableAltitudeMask);
				float3 temp_output_1180_0_g58570 = ( temp_output_39_0_g58570 * lerpResult1457_g58570 );
				float temp_output_634_0_g58570 = ( _DetailEnable + ( ( _CATEGORY_DETAILMAPPING + _SPACE_DETAIL + _CATEGORY_DETAILMAPPINGSECONDARY + _SPACE_DETAILSECONDARY ) * 0.0 ) );
				float3 lerpResult409_g58570 = lerp( temp_output_39_0_g58570 , temp_output_1180_0_g58570 , temp_output_634_0_g58570);
				

				float3 BaseColor = lerpResult409_g58570;
				float3 Emission = 0;
				float Alpha = 1;
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

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#ifdef UNITY_COLORSPACE_GAMMA//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(2.0, 2.0, 2.0, 2.0)//ASE Color Space Def
			#else // Linear values//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(4.59479380, 4.59479380, 4.59479380, 2.0)//ASE Color Space Def
			#endif//ASE Color Space Def
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_DetailColorMap);
			TEXTURE2D(_DetailMaskMap);


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
			
			float2 float2switchUVMode80_g58675( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58584( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58587( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float MaskVCSwitch44_g58598( float m_switch, float m_Off, float m_R, float m_G, float m_B, float m_A )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_R;
				else if(m_switch ==2)
					return m_G;
				else if(m_switch ==3)
					return m_B;
				else if(m_switch ==4)
					return m_A;
				else
				return float(0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				
				float m_switch80_g58675 = _UVMode;
				float2 m_UV080_g58675 = input.ase_texcoord.xy;
				float2 m_UV180_g58675 = input.ase_texcoord1.xy;
				float2 m_UV280_g58675 = input.ase_texcoord2.xy;
				float2 m_UV380_g58675 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58675 = float2switchUVMode80_g58675( m_switch80_g58675 , m_UV080_g58675 , m_UV180_g58675 , m_UV280_g58675 , m_UV380_g58675 );
				float2 temp_output_1955_0_g58657 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58657 = (_MainUVs).zw;
				float2 Offset235_g58675 = temp_output_1953_0_g58657;
				float2 temp_output_41_0_g58675 = ( ( localfloat2switchUVMode80_g58675 * temp_output_1955_0_g58657 ) + Offset235_g58675 );
				float2 vertexToFrag70_g58675 = temp_output_41_0_g58675;
				output.ase_texcoord2.xy = vertexToFrag70_g58675;
				float temp_output_6_0_g58584 = _DetailUVRotation;
				float temp_output_200_0_g58584 = radians( temp_output_6_0_g58584 );
				float temp_output_13_0_g58584 = cos( temp_output_200_0_g58584 );
				float m_switch80_g58584 = _DetailUVMode;
				float2 m_UV080_g58584 = input.ase_texcoord.xy;
				float2 m_UV180_g58584 = input.ase_texcoord1.xy;
				float2 m_UV280_g58584 = input.ase_texcoord2.xy;
				float2 m_UV380_g58584 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58584 = float2switchUVMode80_g58584( m_switch80_g58584 , m_UV080_g58584 , m_UV180_g58584 , m_UV280_g58584 , m_UV380_g58584 );
				float2 temp_output_9_0_g58584 = float2( 0.5,0.5 );
				float2 break39_g58584 = ( localfloat2switchUVMode80_g58584 - temp_output_9_0_g58584 );
				float temp_output_14_0_g58584 = sin( temp_output_200_0_g58584 );
				float2 appendResult36_g58584 = (float2(( ( temp_output_13_0_g58584 * break39_g58584.x ) + ( temp_output_14_0_g58584 * break39_g58584.y ) ) , ( ( temp_output_13_0_g58584 * break39_g58584.y ) - ( temp_output_14_0_g58584 * break39_g58584.x ) )));
				float2 Offset235_g58584 = (_DetailUVs).zw;
				float2 temp_output_41_0_g58584 = ( ( ( appendResult36_g58584 * ( (_DetailUVs).xy / 1.0 ) ) + temp_output_9_0_g58584 ) + Offset235_g58584 );
				float2 _ConstantAnchor = float2(0.5,0.5);
				float2 vertexToFrag70_g58584 = ( temp_output_41_0_g58584 - ( ( ( (_DetailUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord2.zw = vertexToFrag70_g58584;
				float temp_output_6_0_g58587 = _DetailMaskUVRotation;
				float temp_output_200_0_g58587 = radians( temp_output_6_0_g58587 );
				float temp_output_13_0_g58587 = cos( temp_output_200_0_g58587 );
				float DetailUVMode1060_g58570 = _DetailUVMode;
				float m_switch80_g58587 = DetailUVMode1060_g58570;
				float2 m_UV080_g58587 = input.ase_texcoord.xy;
				float2 m_UV180_g58587 = input.ase_texcoord1.xy;
				float2 m_UV280_g58587 = input.ase_texcoord2.xy;
				float2 m_UV380_g58587 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58587 = float2switchUVMode80_g58587( m_switch80_g58587 , m_UV080_g58587 , m_UV180_g58587 , m_UV280_g58587 , m_UV380_g58587 );
				float2 temp_output_9_0_g58587 = float2( 0.5,0.5 );
				float2 break39_g58587 = ( localfloat2switchUVMode80_g58587 - temp_output_9_0_g58587 );
				float temp_output_14_0_g58587 = sin( temp_output_200_0_g58587 );
				float2 appendResult36_g58587 = (float2(( ( temp_output_13_0_g58587 * break39_g58587.x ) + ( temp_output_14_0_g58587 * break39_g58587.y ) ) , ( ( temp_output_13_0_g58587 * break39_g58587.y ) - ( temp_output_14_0_g58587 * break39_g58587.x ) )));
				float2 Offset235_g58587 = (_DetailMaskUVs).zw;
				float2 temp_output_41_0_g58587 = ( ( ( appendResult36_g58587 * ( (_DetailMaskUVs).xy / 1.0 ) ) + temp_output_9_0_g58587 ) + Offset235_g58587 );
				float2 vertexToFrag70_g58587 = ( temp_output_41_0_g58587 - ( ( ( (_DetailMaskUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord3.xy = vertexToFrag70_g58587;
				
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				float3 temp_output_1923_0_g58657 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58675 = input.ase_texcoord2.xy;
				float2 UV213_g58657 = vertexToFrag70_g58675;
				float4 tex2DNode2048_g58657 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58657 );
				float3 temp_output_3_0_g58657 = ( temp_output_1923_0_g58657 * (tex2DNode2048_g58657).rgb * _Brightness );
				float3 temp_output_39_0_g58570 = temp_output_3_0_g58657;
				float localStochasticTiling159_g58575 = ( 0.0 );
				float2 vertexToFrag70_g58584 = input.ase_texcoord2.zw;
				float2 temp_output_1334_0_g58570 = vertexToFrag70_g58584;
				float2 UV159_g58575 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58575 = _DetailColorMap_TexelSize;
				float4 Offsets159_g58575 = float4( 0,0,0,0 );
				float2 Weights159_g58575 = float2( 0,0 );
				{
				UV159_g58575 = UV159_g58575 * TexelSize159_g58575.zw - 0.5;
				float2 f = frac( UV159_g58575 );
				UV159_g58575 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58575.x - 0.5, UV159_g58575.x + 1.5, UV159_g58575.y - 0.5, UV159_g58575.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58575 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58575.xyxy;
				Weights159_g58575 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58576 = Offsets159_g58575;
				float2 Input_FetchWeights143_g58576 = Weights159_g58575;
				float2 break46_g58576 = Input_FetchWeights143_g58576;
				float4 lerpResult20_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yw ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xw ) , break46_g58576.x);
				float4 lerpResult40_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yz ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xz ) , break46_g58576.x);
				float4 lerpResult22_g58576 = lerp( lerpResult20_g58576 , lerpResult40_g58576 , break46_g58576.y);
				float4 Output_Fetch2D44_g58576 = lerpResult22_g58576;
				float3 temp_output_44_0_g58570 = ( (_DetailColor).rgb * (Output_Fetch2D44_g58576).rgb * _DetailBrightness );
				float3 temp_output_1272_0_g58570 = (unity_ColorSpaceDouble).rgb;
				float3 temp_output_1190_0_g58570 = ( temp_output_44_0_g58570 * temp_output_1272_0_g58570 );
				float3 BaseColor_RGB40_g58570 = temp_output_39_0_g58570;
				float localStochasticTiling159_g58582 = ( 0.0 );
				float2 vertexToFrag70_g58587 = input.ase_texcoord3.xy;
				float2 temp_output_1339_0_g58570 = vertexToFrag70_g58587;
				float2 UV159_g58582 = temp_output_1339_0_g58570;
				float4 TexelSize159_g58582 = _DetailMaskMap_TexelSize;
				float4 Offsets159_g58582 = float4( 0,0,0,0 );
				float2 Weights159_g58582 = float2( 0,0 );
				{
				UV159_g58582 = UV159_g58582 * TexelSize159_g58582.zw - 0.5;
				float2 f = frac( UV159_g58582 );
				UV159_g58582 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58582.x - 0.5, UV159_g58582.x + 1.5, UV159_g58582.y - 0.5, UV159_g58582.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58582 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58582.xyxy;
				Weights159_g58582 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58583 = Offsets159_g58582;
				float2 Input_FetchWeights143_g58583 = Weights159_g58582;
				float2 break46_g58583 = Input_FetchWeights143_g58583;
				float4 lerpResult20_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yw ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xw ) , break46_g58583.x);
				float4 lerpResult40_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yz ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xz ) , break46_g58583.x);
				float4 lerpResult22_g58583 = lerp( lerpResult20_g58583 , lerpResult40_g58583 , break46_g58583.y);
				float4 Output_Fetch2D44_g58583 = lerpResult22_g58583;
				float4 break50_g58583 = Output_Fetch2D44_g58583;
				float lerpResult997_g58570 = lerp( ( 1.0 - break50_g58583.r ) , break50_g58583.r , _DetailMaskIsInverted);
				float temp_output_15_0_g58596 = ( 1.0 - lerpResult997_g58570 );
				float saferPower2_g58596 = abs( max( saturate( (0.0 + (temp_output_15_0_g58596 - ( 1.0 - _DetailMaskBlendStrength )) * (_DetailMaskBlendHardness - 0.0) / (1.0 - ( 1.0 - _DetailMaskBlendStrength ))) ) , 0.0 ) );
				float Blend_DetailMask986_g58570 = saturate( pow( saferPower2_g58596 , ( 1.0 - _DetailMaskBlendFalloff ) ) );
				float3 lerpResult1194_g58570 = lerp( BaseColor_RGB40_g58570 , temp_output_1190_0_g58570 , Blend_DetailMask986_g58570);
				float temp_output_1162_0_g58570 = ( 1.0 - Blend_DetailMask986_g58570 );
				float3 appendResult1161_g58570 = (float3(temp_output_1162_0_g58570 , temp_output_1162_0_g58570 , temp_output_1162_0_g58570));
				float3 lerpResult1005_g58570 = lerp( temp_output_1190_0_g58570 , ( ( lerpResult1194_g58570 * Blend_DetailMask986_g58570 ) + appendResult1161_g58570 ) , _DetailMaskEnable);
				float3 BaseColor_Detail255_g58570 = lerpResult1005_g58570;
				float BaseColor_R1273_g58570 = temp_output_39_0_g58570.x;
				float BaseColor_DetailR887_g58570 = Output_Fetch2D44_g58576.r;
				float lerpResult1105_g58570 = lerp( BaseColor_R1273_g58570 , BaseColor_DetailR887_g58570 , _DetailBlendSource);
				float m_switch44_g58598 = (float)_DetailBlendVertexColor;
				float m_Off44_g58598 = 1.0;
				float dotResult58_g58598 = dot( input.ase_color.g , input.ase_color.g );
				float dotResult61_g58598 = dot( input.ase_color.b , input.ase_color.b );
				float m_R44_g58598 = ( dotResult58_g58598 + dotResult61_g58598 );
				float dotResult57_g58598 = dot( input.ase_color.r , input.ase_color.r );
				float m_G44_g58598 = ( dotResult57_g58598 + dotResult58_g58598 );
				float m_B44_g58598 = ( dotResult57_g58598 + dotResult61_g58598 );
				float m_A44_g58598 = input.ase_color.a;
				float localMaskVCSwitch44_g58598 = MaskVCSwitch44_g58598( m_switch44_g58598 , m_Off44_g58598 , m_R44_g58598 , m_G44_g58598 , m_B44_g58598 , m_A44_g58598 );
				float clampResult54_g58598 = clamp( ( ( localMaskVCSwitch44_g58598 * _DetailBlendHeight ) / _DetailBlendSmooth ) , 0.0 , 1.0 );
				float Blend647_g58570 = saturate( ( ( ( ( lerpResult1105_g58570 - 0.5 ) * ( ( 1.0 - _DetailBlendStrength ) - 0.9 ) ) / ( 1.0 - _DetailBlendSmooth ) ) + ( 1.0 - clampResult54_g58598 ) ) );
				float temp_output_1171_0_g58570 = ( 1.0 - Blend647_g58570 );
				float3 appendResult1174_g58570 = (float3(temp_output_1171_0_g58570 , temp_output_1171_0_g58570 , temp_output_1171_0_g58570));
				float3 temp_output_1173_0_g58570 = ( ( BaseColor_Detail255_g58570 * Blend647_g58570 ) + appendResult1174_g58570 );
				float temp_output_20_0_g58599 = _DetailBlendHeightMin;
				float temp_output_21_0_g58599 = _DetailBlendHeightMax;
				float3 worldToObj1466_g58570 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 WorldPosition1436_g58570 = worldToObj1466_g58570;
				float smoothstepResult25_g58599 = smoothstep( temp_output_20_0_g58599 , temp_output_21_0_g58599 , WorldPosition1436_g58570.y);
				float DetailBlendHeight1440_g58570 = smoothstepResult25_g58599;
				float3 lerpResult1438_g58570 = lerp( temp_output_1173_0_g58570 , temp_output_39_0_g58570 , DetailBlendHeight1440_g58570);
				float3 lerpResult1457_g58570 = lerp( temp_output_1173_0_g58570 , lerpResult1438_g58570 , _DetailBlendEnableAltitudeMask);
				float3 temp_output_1180_0_g58570 = ( temp_output_39_0_g58570 * lerpResult1457_g58570 );
				float temp_output_634_0_g58570 = ( _DetailEnable + ( ( _CATEGORY_DETAILMAPPING + _SPACE_DETAIL + _CATEGORY_DETAILMAPPINGSECONDARY + _SPACE_DETAILSECONDARY ) * 0.0 ) );
				float3 lerpResult409_g58570 = lerp( temp_output_39_0_g58570 , temp_output_1180_0_g58570 , temp_output_634_0_g58570);
				

				float3 BaseColor = lerpResult409_g58570;
				float Alpha = 1;
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
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
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
			#define ASE_NEEDS_FRAG_COLOR


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
				float4 ase_color : COLOR;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_DetailColorMap);
			TEXTURE2D(_DetailNormalMap);
			TEXTURE2D(_DetailMaskMap);


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
			
			float2 float2switchUVMode80_g58675( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58584( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float MaskVCSwitch44_g58598( float m_switch, float m_Off, float m_R, float m_G, float m_B, float m_A )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_R;
				else if(m_switch ==2)
					return m_G;
				else if(m_switch ==3)
					return m_B;
				else if(m_switch ==4)
					return m_A;
				else
				return float(0);
			}
			
			float2 float2switchUVMode80_g58587( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				
				float m_switch80_g58675 = _UVMode;
				float2 m_UV080_g58675 = input.ase_texcoord.xy;
				float2 m_UV180_g58675 = input.ase_texcoord1.xy;
				float2 m_UV280_g58675 = input.ase_texcoord2.xy;
				float2 m_UV380_g58675 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58675 = float2switchUVMode80_g58675( m_switch80_g58675 , m_UV080_g58675 , m_UV180_g58675 , m_UV280_g58675 , m_UV380_g58675 );
				float2 temp_output_1955_0_g58657 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58657 = (_MainUVs).zw;
				float2 Offset235_g58675 = temp_output_1953_0_g58657;
				float2 temp_output_41_0_g58675 = ( ( localfloat2switchUVMode80_g58675 * temp_output_1955_0_g58657 ) + Offset235_g58675 );
				float2 vertexToFrag70_g58675 = temp_output_41_0_g58675;
				output.ase_texcoord5.xy = vertexToFrag70_g58675;
				float temp_output_6_0_g58584 = _DetailUVRotation;
				float temp_output_200_0_g58584 = radians( temp_output_6_0_g58584 );
				float temp_output_13_0_g58584 = cos( temp_output_200_0_g58584 );
				float m_switch80_g58584 = _DetailUVMode;
				float2 m_UV080_g58584 = input.ase_texcoord.xy;
				float2 m_UV180_g58584 = input.ase_texcoord1.xy;
				float2 m_UV280_g58584 = input.ase_texcoord2.xy;
				float2 m_UV380_g58584 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58584 = float2switchUVMode80_g58584( m_switch80_g58584 , m_UV080_g58584 , m_UV180_g58584 , m_UV280_g58584 , m_UV380_g58584 );
				float2 temp_output_9_0_g58584 = float2( 0.5,0.5 );
				float2 break39_g58584 = ( localfloat2switchUVMode80_g58584 - temp_output_9_0_g58584 );
				float temp_output_14_0_g58584 = sin( temp_output_200_0_g58584 );
				float2 appendResult36_g58584 = (float2(( ( temp_output_13_0_g58584 * break39_g58584.x ) + ( temp_output_14_0_g58584 * break39_g58584.y ) ) , ( ( temp_output_13_0_g58584 * break39_g58584.y ) - ( temp_output_14_0_g58584 * break39_g58584.x ) )));
				float2 Offset235_g58584 = (_DetailUVs).zw;
				float2 temp_output_41_0_g58584 = ( ( ( appendResult36_g58584 * ( (_DetailUVs).xy / 1.0 ) ) + temp_output_9_0_g58584 ) + Offset235_g58584 );
				float2 _ConstantAnchor = float2(0.5,0.5);
				float2 vertexToFrag70_g58584 = ( temp_output_41_0_g58584 - ( ( ( (_DetailUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord5.zw = vertexToFrag70_g58584;
				float temp_output_6_0_g58587 = _DetailMaskUVRotation;
				float temp_output_200_0_g58587 = radians( temp_output_6_0_g58587 );
				float temp_output_13_0_g58587 = cos( temp_output_200_0_g58587 );
				float DetailUVMode1060_g58570 = _DetailUVMode;
				float m_switch80_g58587 = DetailUVMode1060_g58570;
				float2 m_UV080_g58587 = input.ase_texcoord.xy;
				float2 m_UV180_g58587 = input.ase_texcoord1.xy;
				float2 m_UV280_g58587 = input.ase_texcoord2.xy;
				float2 m_UV380_g58587 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58587 = float2switchUVMode80_g58587( m_switch80_g58587 , m_UV080_g58587 , m_UV180_g58587 , m_UV280_g58587 , m_UV380_g58587 );
				float2 temp_output_9_0_g58587 = float2( 0.5,0.5 );
				float2 break39_g58587 = ( localfloat2switchUVMode80_g58587 - temp_output_9_0_g58587 );
				float temp_output_14_0_g58587 = sin( temp_output_200_0_g58587 );
				float2 appendResult36_g58587 = (float2(( ( temp_output_13_0_g58587 * break39_g58587.x ) + ( temp_output_14_0_g58587 * break39_g58587.y ) ) , ( ( temp_output_13_0_g58587 * break39_g58587.y ) - ( temp_output_14_0_g58587 * break39_g58587.x ) )));
				float2 Offset235_g58587 = (_DetailMaskUVs).zw;
				float2 temp_output_41_0_g58587 = ( ( ( appendResult36_g58587 * ( (_DetailMaskUVs).xy / 1.0 ) ) + temp_output_9_0_g58587 ) + Offset235_g58587 );
				float2 vertexToFrag70_g58587 = ( temp_output_41_0_g58587 - ( ( ( (_DetailMaskUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord6.xy = vertexToFrag70_g58587;
				
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord6.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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
						 )
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

				float2 vertexToFrag70_g58675 = input.ase_texcoord5.xy;
				float2 UV213_g58657 = vertexToFrag70_g58675;
				float3 unpack1891_g58657 = UnpackNormalScale( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, UV213_g58657 ), _NormalStrength );
				unpack1891_g58657.z = lerp( 1, unpack1891_g58657.z, saturate(_NormalStrength) );
				float3 temp_output_38_0_g58570 = unpack1891_g58657;
				float3 temp_output_1923_0_g58657 = (_BaseColor).rgb;
				float4 tex2DNode2048_g58657 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58657 );
				float3 temp_output_3_0_g58657 = ( temp_output_1923_0_g58657 * (tex2DNode2048_g58657).rgb * _Brightness );
				float3 temp_output_39_0_g58570 = temp_output_3_0_g58657;
				float BaseColor_R1273_g58570 = temp_output_39_0_g58570.x;
				float localStochasticTiling159_g58575 = ( 0.0 );
				float2 vertexToFrag70_g58584 = input.ase_texcoord5.zw;
				float2 temp_output_1334_0_g58570 = vertexToFrag70_g58584;
				float2 UV159_g58575 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58575 = _DetailColorMap_TexelSize;
				float4 Offsets159_g58575 = float4( 0,0,0,0 );
				float2 Weights159_g58575 = float2( 0,0 );
				{
				UV159_g58575 = UV159_g58575 * TexelSize159_g58575.zw - 0.5;
				float2 f = frac( UV159_g58575 );
				UV159_g58575 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58575.x - 0.5, UV159_g58575.x + 1.5, UV159_g58575.y - 0.5, UV159_g58575.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58575 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58575.xyxy;
				Weights159_g58575 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58576 = Offsets159_g58575;
				float2 Input_FetchWeights143_g58576 = Weights159_g58575;
				float2 break46_g58576 = Input_FetchWeights143_g58576;
				float4 lerpResult20_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yw ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xw ) , break46_g58576.x);
				float4 lerpResult40_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yz ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xz ) , break46_g58576.x);
				float4 lerpResult22_g58576 = lerp( lerpResult20_g58576 , lerpResult40_g58576 , break46_g58576.y);
				float4 Output_Fetch2D44_g58576 = lerpResult22_g58576;
				float BaseColor_DetailR887_g58570 = Output_Fetch2D44_g58576.r;
				float lerpResult1105_g58570 = lerp( BaseColor_R1273_g58570 , BaseColor_DetailR887_g58570 , _DetailBlendSource);
				float m_switch44_g58598 = (float)_DetailBlendVertexColor;
				float m_Off44_g58598 = 1.0;
				float dotResult58_g58598 = dot( input.ase_color.g , input.ase_color.g );
				float dotResult61_g58598 = dot( input.ase_color.b , input.ase_color.b );
				float m_R44_g58598 = ( dotResult58_g58598 + dotResult61_g58598 );
				float dotResult57_g58598 = dot( input.ase_color.r , input.ase_color.r );
				float m_G44_g58598 = ( dotResult57_g58598 + dotResult58_g58598 );
				float m_B44_g58598 = ( dotResult57_g58598 + dotResult61_g58598 );
				float m_A44_g58598 = input.ase_color.a;
				float localMaskVCSwitch44_g58598 = MaskVCSwitch44_g58598( m_switch44_g58598 , m_Off44_g58598 , m_R44_g58598 , m_G44_g58598 , m_B44_g58598 , m_A44_g58598 );
				float clampResult54_g58598 = clamp( ( ( localMaskVCSwitch44_g58598 * _DetailBlendHeight ) / _DetailBlendSmooth ) , 0.0 , 1.0 );
				float Blend647_g58570 = saturate( ( ( ( ( lerpResult1105_g58570 - 0.5 ) * ( ( 1.0 - _DetailBlendStrength ) - 0.9 ) ) / ( 1.0 - _DetailBlendSmooth ) ) + ( 1.0 - clampResult54_g58598 ) ) );
				float localStochasticTiling159_g58581 = ( 0.0 );
				float2 UV159_g58581 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58581 = _DetailNormalMap_TexelSize;
				float4 Offsets159_g58581 = float4( 0,0,0,0 );
				float2 Weights159_g58581 = float2( 0,0 );
				{
				UV159_g58581 = UV159_g58581 * TexelSize159_g58581.zw - 0.5;
				float2 f = frac( UV159_g58581 );
				UV159_g58581 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58581.x - 0.5, UV159_g58581.x + 1.5, UV159_g58581.y - 0.5, UV159_g58581.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58581 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58581.xyxy;
				Weights159_g58581 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58580 = Offsets159_g58581;
				float2 Input_FetchWeights143_g58580 = Weights159_g58581;
				float2 break46_g58580 = Input_FetchWeights143_g58580;
				float4 lerpResult20_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yw ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xw ) , break46_g58580.x);
				float4 lerpResult40_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yz ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xz ) , break46_g58580.x);
				float4 lerpResult22_g58580 = lerp( lerpResult20_g58580 , lerpResult40_g58580 , break46_g58580.y);
				float4 Output_Fetch2D44_g58580 = lerpResult22_g58580;
				float3 unpack499_g58570 = UnpackNormalScale( Output_Fetch2D44_g58580, _DetailNormalStrength );
				unpack499_g58570.z = lerp( 1, unpack499_g58570.z, saturate(_DetailNormalStrength) );
				float3 Normal_In880_g58570 = temp_output_38_0_g58570;
				float localStochasticTiling159_g58582 = ( 0.0 );
				float2 vertexToFrag70_g58587 = input.ase_texcoord6.xy;
				float2 temp_output_1339_0_g58570 = vertexToFrag70_g58587;
				float2 UV159_g58582 = temp_output_1339_0_g58570;
				float4 TexelSize159_g58582 = _DetailMaskMap_TexelSize;
				float4 Offsets159_g58582 = float4( 0,0,0,0 );
				float2 Weights159_g58582 = float2( 0,0 );
				{
				UV159_g58582 = UV159_g58582 * TexelSize159_g58582.zw - 0.5;
				float2 f = frac( UV159_g58582 );
				UV159_g58582 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58582.x - 0.5, UV159_g58582.x + 1.5, UV159_g58582.y - 0.5, UV159_g58582.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58582 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58582.xyxy;
				Weights159_g58582 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58583 = Offsets159_g58582;
				float2 Input_FetchWeights143_g58583 = Weights159_g58582;
				float2 break46_g58583 = Input_FetchWeights143_g58583;
				float4 lerpResult20_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yw ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xw ) , break46_g58583.x);
				float4 lerpResult40_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yz ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xz ) , break46_g58583.x);
				float4 lerpResult22_g58583 = lerp( lerpResult20_g58583 , lerpResult40_g58583 , break46_g58583.y);
				float4 Output_Fetch2D44_g58583 = lerpResult22_g58583;
				float4 break50_g58583 = Output_Fetch2D44_g58583;
				float lerpResult997_g58570 = lerp( ( 1.0 - break50_g58583.r ) , break50_g58583.r , _DetailMaskIsInverted);
				float temp_output_15_0_g58596 = ( 1.0 - lerpResult997_g58570 );
				float saferPower2_g58596 = abs( max( saturate( (0.0 + (temp_output_15_0_g58596 - ( 1.0 - _DetailMaskBlendStrength )) * (_DetailMaskBlendHardness - 0.0) / (1.0 - ( 1.0 - _DetailMaskBlendStrength ))) ) , 0.0 ) );
				float Blend_DetailMask986_g58570 = saturate( pow( saferPower2_g58596 , ( 1.0 - _DetailMaskBlendFalloff ) ) );
				float3 lerpResult1286_g58570 = lerp( Normal_In880_g58570 , unpack499_g58570 , Blend_DetailMask986_g58570);
				float3 lerpResult1011_g58570 = lerp( unpack499_g58570 , lerpResult1286_g58570 , _DetailMaskEnable);
				float3 Normal_Detail199_g58570 = lerpResult1011_g58570;
				float layeredBlendVar1278_g58570 = Blend647_g58570;
				float3 layeredBlend1278_g58570 = ( lerp( temp_output_38_0_g58570,Normal_Detail199_g58570 , layeredBlendVar1278_g58570 ) );
				float3 break817_g58570 = layeredBlend1278_g58570;
				float3 appendResult820_g58570 = (float3(break817_g58570.x , break817_g58570.y , ( break817_g58570.z + 0.001 )));
				float temp_output_634_0_g58570 = ( _DetailEnable + ( ( _CATEGORY_DETAILMAPPING + _SPACE_DETAIL + _CATEGORY_DETAILMAPPINGSECONDARY + _SPACE_DETAILSECONDARY ) * 0.0 ) );
				float3 lerpResult410_g58570 = lerp( temp_output_38_0_g58570 , appendResult820_g58570 , temp_output_634_0_g58570);
				

				float3 Normal = lerpResult410_g58570;
				float Alpha = 1;
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

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_VERSION 19702
			#define ASE_SRP_VERSION 170003
			#ifdef UNITY_COLORSPACE_GAMMA//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(2.0, 2.0, 2.0, 2.0)//ASE Color Space Def
			#else // Linear values//ASE Color Space Def
			#define unity_ColorSpaceDouble half4(4.59479380, 4.59479380, 4.59479380, 2.0)//ASE Color Space Def
			#endif//ASE Color Space Def
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL


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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_DetailColorMap);
			TEXTURE2D(_DetailMaskMap);
			TEXTURE2D(_BumpMap);
			SAMPLER(sampler_BumpMap);
			TEXTURE2D(_DetailNormalMap);
			TEXTURE2D(_MainMaskMap);
			SAMPLER(sampler_MainMaskMap);


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

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
			
			float2 float2switchUVMode80_g58675( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58584( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float2 float2switchUVMode80_g58587( float m_switch, float2 m_UV0, float2 m_UV1, float2 m_UV2, float2 m_UV3 )
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
			
			float MaskVCSwitch44_g58598( float m_switch, float m_Off, float m_R, float m_G, float m_B, float m_A )
			{
				if(m_switch ==0)
					return m_Off;
				else if(m_switch ==1)
					return m_R;
				else if(m_switch ==2)
					return m_G;
				else if(m_switch ==3)
					return m_B;
				else if(m_switch ==4)
					return m_A;
				else
				return float(0);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				
				float m_switch80_g58675 = _UVMode;
				float2 m_UV080_g58675 = input.texcoord.xy;
				float2 m_UV180_g58675 = input.texcoord1.xy;
				float2 m_UV280_g58675 = input.texcoord2.xy;
				float2 m_UV380_g58675 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58675 = float2switchUVMode80_g58675( m_switch80_g58675 , m_UV080_g58675 , m_UV180_g58675 , m_UV280_g58675 , m_UV380_g58675 );
				float2 temp_output_1955_0_g58657 = (_MainUVs).xy;
				float2 temp_output_1953_0_g58657 = (_MainUVs).zw;
				float2 Offset235_g58675 = temp_output_1953_0_g58657;
				float2 temp_output_41_0_g58675 = ( ( localfloat2switchUVMode80_g58675 * temp_output_1955_0_g58657 ) + Offset235_g58675 );
				float2 vertexToFrag70_g58675 = temp_output_41_0_g58675;
				output.ase_texcoord9.xy = vertexToFrag70_g58675;
				float temp_output_6_0_g58584 = _DetailUVRotation;
				float temp_output_200_0_g58584 = radians( temp_output_6_0_g58584 );
				float temp_output_13_0_g58584 = cos( temp_output_200_0_g58584 );
				float m_switch80_g58584 = _DetailUVMode;
				float2 m_UV080_g58584 = input.texcoord.xy;
				float2 m_UV180_g58584 = input.texcoord1.xy;
				float2 m_UV280_g58584 = input.texcoord2.xy;
				float2 m_UV380_g58584 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58584 = float2switchUVMode80_g58584( m_switch80_g58584 , m_UV080_g58584 , m_UV180_g58584 , m_UV280_g58584 , m_UV380_g58584 );
				float2 temp_output_9_0_g58584 = float2( 0.5,0.5 );
				float2 break39_g58584 = ( localfloat2switchUVMode80_g58584 - temp_output_9_0_g58584 );
				float temp_output_14_0_g58584 = sin( temp_output_200_0_g58584 );
				float2 appendResult36_g58584 = (float2(( ( temp_output_13_0_g58584 * break39_g58584.x ) + ( temp_output_14_0_g58584 * break39_g58584.y ) ) , ( ( temp_output_13_0_g58584 * break39_g58584.y ) - ( temp_output_14_0_g58584 * break39_g58584.x ) )));
				float2 Offset235_g58584 = (_DetailUVs).zw;
				float2 temp_output_41_0_g58584 = ( ( ( appendResult36_g58584 * ( (_DetailUVs).xy / 1.0 ) ) + temp_output_9_0_g58584 ) + Offset235_g58584 );
				float2 _ConstantAnchor = float2(0.5,0.5);
				float2 vertexToFrag70_g58584 = ( temp_output_41_0_g58584 - ( ( ( (_DetailUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord9.zw = vertexToFrag70_g58584;
				float temp_output_6_0_g58587 = _DetailMaskUVRotation;
				float temp_output_200_0_g58587 = radians( temp_output_6_0_g58587 );
				float temp_output_13_0_g58587 = cos( temp_output_200_0_g58587 );
				float DetailUVMode1060_g58570 = _DetailUVMode;
				float m_switch80_g58587 = DetailUVMode1060_g58570;
				float2 m_UV080_g58587 = input.texcoord.xy;
				float2 m_UV180_g58587 = input.texcoord1.xy;
				float2 m_UV280_g58587 = input.texcoord2.xy;
				float2 m_UV380_g58587 = input.ase_texcoord3.xy;
				float2 localfloat2switchUVMode80_g58587 = float2switchUVMode80_g58587( m_switch80_g58587 , m_UV080_g58587 , m_UV180_g58587 , m_UV280_g58587 , m_UV380_g58587 );
				float2 temp_output_9_0_g58587 = float2( 0.5,0.5 );
				float2 break39_g58587 = ( localfloat2switchUVMode80_g58587 - temp_output_9_0_g58587 );
				float temp_output_14_0_g58587 = sin( temp_output_200_0_g58587 );
				float2 appendResult36_g58587 = (float2(( ( temp_output_13_0_g58587 * break39_g58587.x ) + ( temp_output_14_0_g58587 * break39_g58587.y ) ) , ( ( temp_output_13_0_g58587 * break39_g58587.y ) - ( temp_output_14_0_g58587 * break39_g58587.x ) )));
				float2 Offset235_g58587 = (_DetailMaskUVs).zw;
				float2 temp_output_41_0_g58587 = ( ( ( appendResult36_g58587 * ( (_DetailMaskUVs).xy / 1.0 ) ) + temp_output_9_0_g58587 ) + Offset235_g58587 );
				float2 vertexToFrag70_g58587 = ( temp_output_41_0_g58587 - ( ( ( (_DetailMaskUVs).xy / 1.0 ) * _ConstantAnchor ) - _ConstantAnchor ) );
				output.ase_texcoord10.xy = vertexToFrag70_g58587;
				
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord10.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

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

			FragmentOutput frag ( PackedVaryings input
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
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

				float3 temp_output_1923_0_g58657 = (_BaseColor).rgb;
				float2 vertexToFrag70_g58675 = input.ase_texcoord9.xy;
				float2 UV213_g58657 = vertexToFrag70_g58675;
				float4 tex2DNode2048_g58657 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, UV213_g58657 );
				float3 temp_output_3_0_g58657 = ( temp_output_1923_0_g58657 * (tex2DNode2048_g58657).rgb * _Brightness );
				float3 temp_output_39_0_g58570 = temp_output_3_0_g58657;
				float localStochasticTiling159_g58575 = ( 0.0 );
				float2 vertexToFrag70_g58584 = input.ase_texcoord9.zw;
				float2 temp_output_1334_0_g58570 = vertexToFrag70_g58584;
				float2 UV159_g58575 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58575 = _DetailColorMap_TexelSize;
				float4 Offsets159_g58575 = float4( 0,0,0,0 );
				float2 Weights159_g58575 = float2( 0,0 );
				{
				UV159_g58575 = UV159_g58575 * TexelSize159_g58575.zw - 0.5;
				float2 f = frac( UV159_g58575 );
				UV159_g58575 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58575.x - 0.5, UV159_g58575.x + 1.5, UV159_g58575.y - 0.5, UV159_g58575.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58575 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58575.xyxy;
				Weights159_g58575 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58576 = Offsets159_g58575;
				float2 Input_FetchWeights143_g58576 = Weights159_g58575;
				float2 break46_g58576 = Input_FetchWeights143_g58576;
				float4 lerpResult20_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yw ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xw ) , break46_g58576.x);
				float4 lerpResult40_g58576 = lerp( SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).yz ) , SAMPLE_TEXTURE2D( _DetailColorMap, sampler_MainTex, (Input_FetchOffsets70_g58576).xz ) , break46_g58576.x);
				float4 lerpResult22_g58576 = lerp( lerpResult20_g58576 , lerpResult40_g58576 , break46_g58576.y);
				float4 Output_Fetch2D44_g58576 = lerpResult22_g58576;
				float3 temp_output_44_0_g58570 = ( (_DetailColor).rgb * (Output_Fetch2D44_g58576).rgb * _DetailBrightness );
				float3 temp_output_1272_0_g58570 = (unity_ColorSpaceDouble).rgb;
				float3 temp_output_1190_0_g58570 = ( temp_output_44_0_g58570 * temp_output_1272_0_g58570 );
				float3 BaseColor_RGB40_g58570 = temp_output_39_0_g58570;
				float localStochasticTiling159_g58582 = ( 0.0 );
				float2 vertexToFrag70_g58587 = input.ase_texcoord10.xy;
				float2 temp_output_1339_0_g58570 = vertexToFrag70_g58587;
				float2 UV159_g58582 = temp_output_1339_0_g58570;
				float4 TexelSize159_g58582 = _DetailMaskMap_TexelSize;
				float4 Offsets159_g58582 = float4( 0,0,0,0 );
				float2 Weights159_g58582 = float2( 0,0 );
				{
				UV159_g58582 = UV159_g58582 * TexelSize159_g58582.zw - 0.5;
				float2 f = frac( UV159_g58582 );
				UV159_g58582 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58582.x - 0.5, UV159_g58582.x + 1.5, UV159_g58582.y - 0.5, UV159_g58582.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58582 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58582.xyxy;
				Weights159_g58582 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58583 = Offsets159_g58582;
				float2 Input_FetchWeights143_g58583 = Weights159_g58582;
				float2 break46_g58583 = Input_FetchWeights143_g58583;
				float4 lerpResult20_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yw ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xw ) , break46_g58583.x);
				float4 lerpResult40_g58583 = lerp( SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).yz ) , SAMPLE_TEXTURE2D( _DetailMaskMap, sampler_MainTex, (Input_FetchOffsets70_g58583).xz ) , break46_g58583.x);
				float4 lerpResult22_g58583 = lerp( lerpResult20_g58583 , lerpResult40_g58583 , break46_g58583.y);
				float4 Output_Fetch2D44_g58583 = lerpResult22_g58583;
				float4 break50_g58583 = Output_Fetch2D44_g58583;
				float lerpResult997_g58570 = lerp( ( 1.0 - break50_g58583.r ) , break50_g58583.r , _DetailMaskIsInverted);
				float temp_output_15_0_g58596 = ( 1.0 - lerpResult997_g58570 );
				float saferPower2_g58596 = abs( max( saturate( (0.0 + (temp_output_15_0_g58596 - ( 1.0 - _DetailMaskBlendStrength )) * (_DetailMaskBlendHardness - 0.0) / (1.0 - ( 1.0 - _DetailMaskBlendStrength ))) ) , 0.0 ) );
				float Blend_DetailMask986_g58570 = saturate( pow( saferPower2_g58596 , ( 1.0 - _DetailMaskBlendFalloff ) ) );
				float3 lerpResult1194_g58570 = lerp( BaseColor_RGB40_g58570 , temp_output_1190_0_g58570 , Blend_DetailMask986_g58570);
				float temp_output_1162_0_g58570 = ( 1.0 - Blend_DetailMask986_g58570 );
				float3 appendResult1161_g58570 = (float3(temp_output_1162_0_g58570 , temp_output_1162_0_g58570 , temp_output_1162_0_g58570));
				float3 lerpResult1005_g58570 = lerp( temp_output_1190_0_g58570 , ( ( lerpResult1194_g58570 * Blend_DetailMask986_g58570 ) + appendResult1161_g58570 ) , _DetailMaskEnable);
				float3 BaseColor_Detail255_g58570 = lerpResult1005_g58570;
				float BaseColor_R1273_g58570 = temp_output_39_0_g58570.x;
				float BaseColor_DetailR887_g58570 = Output_Fetch2D44_g58576.r;
				float lerpResult1105_g58570 = lerp( BaseColor_R1273_g58570 , BaseColor_DetailR887_g58570 , _DetailBlendSource);
				float m_switch44_g58598 = (float)_DetailBlendVertexColor;
				float m_Off44_g58598 = 1.0;
				float dotResult58_g58598 = dot( input.ase_color.g , input.ase_color.g );
				float dotResult61_g58598 = dot( input.ase_color.b , input.ase_color.b );
				float m_R44_g58598 = ( dotResult58_g58598 + dotResult61_g58598 );
				float dotResult57_g58598 = dot( input.ase_color.r , input.ase_color.r );
				float m_G44_g58598 = ( dotResult57_g58598 + dotResult58_g58598 );
				float m_B44_g58598 = ( dotResult57_g58598 + dotResult61_g58598 );
				float m_A44_g58598 = input.ase_color.a;
				float localMaskVCSwitch44_g58598 = MaskVCSwitch44_g58598( m_switch44_g58598 , m_Off44_g58598 , m_R44_g58598 , m_G44_g58598 , m_B44_g58598 , m_A44_g58598 );
				float clampResult54_g58598 = clamp( ( ( localMaskVCSwitch44_g58598 * _DetailBlendHeight ) / _DetailBlendSmooth ) , 0.0 , 1.0 );
				float Blend647_g58570 = saturate( ( ( ( ( lerpResult1105_g58570 - 0.5 ) * ( ( 1.0 - _DetailBlendStrength ) - 0.9 ) ) / ( 1.0 - _DetailBlendSmooth ) ) + ( 1.0 - clampResult54_g58598 ) ) );
				float temp_output_1171_0_g58570 = ( 1.0 - Blend647_g58570 );
				float3 appendResult1174_g58570 = (float3(temp_output_1171_0_g58570 , temp_output_1171_0_g58570 , temp_output_1171_0_g58570));
				float3 temp_output_1173_0_g58570 = ( ( BaseColor_Detail255_g58570 * Blend647_g58570 ) + appendResult1174_g58570 );
				float temp_output_20_0_g58599 = _DetailBlendHeightMin;
				float temp_output_21_0_g58599 = _DetailBlendHeightMax;
				float3 worldToObj1466_g58570 = mul( GetWorldToObjectMatrix(), float4( WorldPosition, 1 ) ).xyz;
				float3 WorldPosition1436_g58570 = worldToObj1466_g58570;
				float smoothstepResult25_g58599 = smoothstep( temp_output_20_0_g58599 , temp_output_21_0_g58599 , WorldPosition1436_g58570.y);
				float DetailBlendHeight1440_g58570 = smoothstepResult25_g58599;
				float3 lerpResult1438_g58570 = lerp( temp_output_1173_0_g58570 , temp_output_39_0_g58570 , DetailBlendHeight1440_g58570);
				float3 lerpResult1457_g58570 = lerp( temp_output_1173_0_g58570 , lerpResult1438_g58570 , _DetailBlendEnableAltitudeMask);
				float3 temp_output_1180_0_g58570 = ( temp_output_39_0_g58570 * lerpResult1457_g58570 );
				float temp_output_634_0_g58570 = ( _DetailEnable + ( ( _CATEGORY_DETAILMAPPING + _SPACE_DETAIL + _CATEGORY_DETAILMAPPINGSECONDARY + _SPACE_DETAILSECONDARY ) * 0.0 ) );
				float3 lerpResult409_g58570 = lerp( temp_output_39_0_g58570 , temp_output_1180_0_g58570 , temp_output_634_0_g58570);
				
				float3 unpack1891_g58657 = UnpackNormalScale( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, UV213_g58657 ), _NormalStrength );
				unpack1891_g58657.z = lerp( 1, unpack1891_g58657.z, saturate(_NormalStrength) );
				float3 temp_output_38_0_g58570 = unpack1891_g58657;
				float localStochasticTiling159_g58581 = ( 0.0 );
				float2 UV159_g58581 = temp_output_1334_0_g58570;
				float4 TexelSize159_g58581 = _DetailNormalMap_TexelSize;
				float4 Offsets159_g58581 = float4( 0,0,0,0 );
				float2 Weights159_g58581 = float2( 0,0 );
				{
				UV159_g58581 = UV159_g58581 * TexelSize159_g58581.zw - 0.5;
				float2 f = frac( UV159_g58581 );
				UV159_g58581 -= f;
				float4 xn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.xxxx;
				float4 yn = float4( 1.0, 2.0, 3.0, 4.0 ) - f.yyyy;
				float4 xs = xn * xn * xn;
				float4 ys = yn * yn * yn;
				float3 xv = float3( xs.x, xs.y - 4.0 * xs.x, xs.z - 4.0 * xs.y + 6.0 * xs.x );
				float3 yv = float3( ys.x, ys.y - 4.0 * ys.x, ys.z - 4.0 * ys.y + 6.0 * ys.x );
				float4 xc = float4( xv.xyz, 6.0 - xv.x - xv.y - xv.z );
				float4 yc = float4( yv.xyz, 6.0 - yv.x - yv.y - yv.z );
				float4 c = float4( UV159_g58581.x - 0.5, UV159_g58581.x + 1.5, UV159_g58581.y - 0.5, UV159_g58581.y + 1.5 );
				float4 s = float4( xc.x + xc.y, xc.z + xc.w, yc.x + yc.y, yc.z + yc.w );
				float w0 = s.x / ( s.x + s.y );
				float w1 = s.z / ( s.z + s.w );
				Offsets159_g58581 = ( c + float4( xc.y, xc.w, yc.y, yc.w ) / s ) * TexelSize159_g58581.xyxy;
				Weights159_g58581 = float2( w0, w1 );
				}
				float4 Input_FetchOffsets70_g58580 = Offsets159_g58581;
				float2 Input_FetchWeights143_g58580 = Weights159_g58581;
				float2 break46_g58580 = Input_FetchWeights143_g58580;
				float4 lerpResult20_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yw ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xw ) , break46_g58580.x);
				float4 lerpResult40_g58580 = lerp( SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).yz ) , SAMPLE_TEXTURE2D( _DetailNormalMap, sampler_BumpMap, (Input_FetchOffsets70_g58580).xz ) , break46_g58580.x);
				float4 lerpResult22_g58580 = lerp( lerpResult20_g58580 , lerpResult40_g58580 , break46_g58580.y);
				float4 Output_Fetch2D44_g58580 = lerpResult22_g58580;
				float3 unpack499_g58570 = UnpackNormalScale( Output_Fetch2D44_g58580, _DetailNormalStrength );
				unpack499_g58570.z = lerp( 1, unpack499_g58570.z, saturate(_DetailNormalStrength) );
				float3 Normal_In880_g58570 = temp_output_38_0_g58570;
				float3 lerpResult1286_g58570 = lerp( Normal_In880_g58570 , unpack499_g58570 , Blend_DetailMask986_g58570);
				float3 lerpResult1011_g58570 = lerp( unpack499_g58570 , lerpResult1286_g58570 , _DetailMaskEnable);
				float3 Normal_Detail199_g58570 = lerpResult1011_g58570;
				float layeredBlendVar1278_g58570 = Blend647_g58570;
				float3 layeredBlend1278_g58570 = ( lerp( temp_output_38_0_g58570,Normal_Detail199_g58570 , layeredBlendVar1278_g58570 ) );
				float3 break817_g58570 = layeredBlend1278_g58570;
				float3 appendResult820_g58570 = (float3(break817_g58570.x , break817_g58570.y , ( break817_g58570.z + 0.001 )));
				float3 lerpResult410_g58570 = lerp( temp_output_38_0_g58570 , appendResult820_g58570 , temp_output_634_0_g58570);
				
				float4 tex2DNode2050_g58657 = SAMPLE_TEXTURE2D( _MainMaskMap, sampler_MainMaskMap, UV213_g58657 );
				
				float lerpResult2650_g58657 = lerp( tex2DNode2050_g58657.g , ( 1.0 - tex2DNode2050_g58657.g ) , _MainMaskType);
				float temp_output_2693_0_g58657 = ( lerpResult2650_g58657 * _SmoothnessStrength );
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirSafeWS = SafeNormalize( ase_viewVectorWS );
				float2 appendResult2645_g58657 = (float2(ase_viewDirSafeWS.xy));
				float3 appendResult2644_g58657 = (float3(appendResult2645_g58657 , ( ase_viewDirSafeWS.z / 1.06 )));
				float3 break2680_g58657 = unpack1891_g58657;
				float3 normalizeResult2641_g58657 = normalize( ( ( WorldTangent * break2680_g58657.x ) + ( WorldBiTangent * break2680_g58657.y ) + ( WorldNormal * break2680_g58657.z ) ) );
				float3 Normal_Per_Pixel2690_g58657 = normalizeResult2641_g58657;
				float fresnelNdotV2685_g58657 = dot( normalize( Normal_Per_Pixel2690_g58657 ), appendResult2644_g58657 );
				float fresnelNode2685_g58657 = ( 0.0 + ( 1.0 - _SmoothnessFresnelScale ) * pow( max( 1.0 - fresnelNdotV2685_g58657 , 0.0001 ), _SmoothnessFresnelPower ) );
				float lerpResult2636_g58657 = lerp( temp_output_2693_0_g58657 , ( temp_output_2693_0_g58657 - fresnelNode2685_g58657 ) , _SmoothnessFresnelEnable);
				
				float lerpResult3414_g58657 = lerp( 1.0 , min( tex2DNode2050_g58657.b , input.ase_color.a ) , _OcclusionStrengthAO);
				

				float3 BaseColor = lerpResult409_g58570;
				float3 Normal = lerpResult410_g58570;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = ( _MetallicStrength * tex2DNode2050_g58657.r );
				float Smoothness = saturate( lerpResult2636_g58657 );
				float Occlusion = saturate( lerpResult3414_g58657 );
				float Alpha = 1;
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


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				

				surfaceDescription.Alpha = 1;
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


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				

				surfaceDescription.Alpha = 1;
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
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailMaskMap_TexelSize;
			float4 _DetailMaskUVs;
			float4 _DetailColorMap_TexelSize;
			float4 _DetailUVs;
			float4 _DetailNormalMap_TexelSize;
			half4 _DetailColor;
			float4 _MainUVs;
			half4 _BaseColor;
			half _DetailMaskEnable;
			half _DetailBlendSource;
			half _DetailBlendStrength;
			half _DetailBlendSmooth;
			int _DetailBlendVertexColor;
			half _DetailBlendHeight;
			half _DetailBlendHeightMin;
			half _DetailBlendHeightMax;
			float _DetailBlendEnableAltitudeMask;
			half _DetailEnable;
			float _CATEGORY_DETAILMAPPING;
			float _SPACE_DETAIL;
			float _CATEGORY_DETAILMAPPINGSECONDARY;
			float _SPACE_DETAILSECONDARY;
			half _NormalStrength;
			half _DetailNormalStrength;
			float _MetallicStrength;
			float _MainMaskType;
			half _SmoothnessStrength;
			half _SmoothnessFresnelScale;
			half _SmoothnessFresnelPower;
			half _DetailMaskBlendFalloff;
			half _DetailMaskBlendHardness;
			int _Cull;
			half _DetailMaskIsInverted;
			float _SPACE_TRANSLUCENCY;
			float _CATEGORY_TRANSMISSION;
			float _CATEGORY_SURFACEINPUTS;
			float _SPACE_SURFACEINPUTS;
			float _SPACE_COLOR;
			float _CATEGORY_COLOR;
			half _WindGlobalIntensity;
			half _WindLocalIntensity;
			half _WindEnableMode;
			half _WindLocalRandomOffset;
			half _DetailMaskBlendStrength;
			half _WindLocalPulseFrequency;
			half _WindEnable;
			float _CATEGORY_WIND;
			float _SPACE_WIND;
			float _UVMode;
			half _Brightness;
			half _DetailUVRotation;
			half _DetailUVMode;
			half _DetailBrightness;
			half _DetailMaskUVRotation;
			half _SmoothnessFresnelEnable;
			half _WindLocalDirection;
			half _OcclusionStrengthAO;
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
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 VERTEX_POSITION_MATRIX2352_g58567 = mul( GetObjectToWorldMatrix(), float4( input.positionOS.xyz , 0.0 ) ).xyz;
				float3 break2265_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float GlobalWindIntensity3173_g58567 = _GlobalWindIntensity;
				float WIND_MODE2462_g58567 = _WindEnableMode;
				float lerpResult3147_g58567 = lerp( ( _WindGlobalIntensity * GlobalWindIntensity3173_g58567 ) , _WindLocalIntensity , WIND_MODE2462_g58567);
				float _WIND_STRENGHT2400_g58567 = lerpResult3147_g58567;
				float GlobalWindRandomOffset3174_g58567 = _GlobalWindRandomOffset;
				float lerpResult3149_g58567 = lerp( GlobalWindRandomOffset3174_g58567 , _WindLocalRandomOffset , WIND_MODE2462_g58567);
				float4 transform3073_g58567 = mul(GetObjectToWorldMatrix(),float4( 0,0,0,1 ));
				float2 appendResult2307_g58567 = (float2(transform3073_g58567.x , transform3073_g58567.z));
				float dotResult2341_g58567 = dot( appendResult2307_g58567 , float2( 12.9898,78.233 ) );
				float lerpResult2238_g58567 = lerp( 0.8 , ( ( lerpResult3149_g58567 / 2.0 ) + 0.9 ) , frac( ( sin( dotResult2341_g58567 ) * 43758.55 ) ));
				float _WIND_RANDOM_OFFSET2244_g58567 = ( ( _TimeParameters.x * 0.05 ) * lerpResult2238_g58567 );
				float _WIND_TUBULENCE_RANDOM2274_g58567 = ( sin( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 40.0 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 15.0 ) ) ) * 0.5 );
				float GlobalWindPulse3177_g58567 = _GlobalWindPulse;
				float lerpResult3152_g58567 = lerp( GlobalWindPulse3177_g58567 , _WindLocalPulseFrequency , WIND_MODE2462_g58567);
				float _WIND_PULSE2421_g58567 = lerpResult3152_g58567;
				float FUNC_Angle2470_g58567 = ( _WIND_STRENGHT2400_g58567 * ( 1.0 + sin( ( ( ( ( _WIND_RANDOM_OFFSET2244_g58567 * 2.0 ) + _WIND_TUBULENCE_RANDOM2274_g58567 ) - ( VERTEX_POSITION_MATRIX2352_g58567.z / 50.0 ) ) - ( input.ase_color.r / 20.0 ) ) ) ) * sqrt( input.ase_color.r ) * _WIND_PULSE2421_g58567 );
				float FUNC_Angle_SinA2424_g58567 = sin( FUNC_Angle2470_g58567 );
				float FUNC_Angle_CosA2362_g58567 = cos( FUNC_Angle2470_g58567 );
				float GlobalWindDirection3175_g58567 = _GlobalWindDirection;
				float lerpResult3150_g58567 = lerp( GlobalWindDirection3175_g58567 , _WindLocalDirection , WIND_MODE2462_g58567);
				float _WindDirection2249_g58567 = lerpResult3150_g58567;
				float2 localDirectionalEquation2249_g58567 = DirectionalEquation( _WindDirection2249_g58567 );
				float2 break2469_g58567 = localDirectionalEquation2249_g58567;
				float _WIND_DIRECTION_X2418_g58567 = break2469_g58567.x;
				float lerpResult2258_g58567 = lerp( break2265_g58567.x , ( ( break2265_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2265_g58567.x * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_X2418_g58567);
				float3 break2340_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float3 break2233_g58567 = VERTEX_POSITION_MATRIX2352_g58567;
				float _WIND_DIRECTION_Y2416_g58567 = break2469_g58567.y;
				float lerpResult2275_g58567 = lerp( break2233_g58567.z , ( ( break2233_g58567.y * FUNC_Angle_SinA2424_g58567 ) + ( break2233_g58567.z * FUNC_Angle_CosA2362_g58567 ) ) , _WIND_DIRECTION_Y2416_g58567);
				float3 appendResult2235_g58567 = (float3(lerpResult2258_g58567 , ( ( break2340_g58567.y * FUNC_Angle_CosA2362_g58567 ) - ( break2340_g58567.z * FUNC_Angle_SinA2424_g58567 ) ) , lerpResult2275_g58567));
				float3 VERTEX_POSITION2282_g58567 = ( mul( GetWorldToObjectMatrix(), float4( appendResult2235_g58567 , 0.0 ) ).xyz - input.positionOS.xyz );
				float3 lerpResult3142_g58567 = lerp( float3(0,0,0) , VERTEX_POSITION2282_g58567 , ( _WindEnable + ( ( _CATEGORY_WIND + _SPACE_WIND ) * 0.0 ) ));
				float3 temp_output_1234_0_g58657 = lerpResult3142_g58567;
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = temp_output_1234_0_g58657;

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

				

				float Alpha = 1;
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
Node;AmplifyShaderEditor.FunctionNode;387;416,-640;Inherit;False;DESF Weather Wind;205;;58567;b135a554f7e4d0b41bba02c61b34ae74;5,3133,0,2371,0,2432,0,3138,0,3139,0;0;1;FLOAT3;2190
Node;AmplifyShaderEditor.FunctionNode;389;1120,-704;Inherit;False;DESF Module Detail;144;;58570;49c077198be2bdb409ab6ad879c0ca28;17,666,1,1023,1,1260,1,665,1,663,1,662,1,1006,1,1012,1,650,1,1067,0,727,0,726,0,874,0,602,0,945,1,1075,0,1316,0;4;39;FLOAT3;0,0,0;False;38;FLOAT3;0,0,1;False;456;SAMPLERSTATE;0;False;464;SAMPLERSTATE;0;False;2;FLOAT3;73;FLOAT3;72
Node;AmplifyShaderEditor.IntNode;413;1408,-720;Inherit;False;Property;_Cull;Render Face;0;2;[HideInInspector];[Enum];Create;False;0;0;1;Front,2,Back,1,Both,0;True;0;False;2;2;False;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;437;704,-640;Inherit;False;DESF Core Lit;1;;58657;e0cdd7758f4404849b063afff4596424;21,442,2,2132,0,2213,0,2282,0,2283,0,2284,0,2172,0,2481,0,96,0,2559,0,2591,0,1368,0,2125,0,2028,0,1333,0,2126,0,1896,0,1415,0,830,0,2523,1,3576,0;1;1234;FLOAT3;0,0,0;False;17;FLOAT3;38;FLOAT3;35;FLOAT;37;FLOAT3;1922;FLOAT;33;FLOAT;34;FLOAT;46;FLOAT;814;FLOAT;1660;FLOAT3;656;FLOAT3;657;FLOAT3;655;FLOAT3;1235;FLOAT3;2760;SAMPLERSTATE;1819;SAMPLERSTATE;1824;SAMPLERSTATE;1818
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;427;1404.904,-640;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;429;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;430;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;431;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;432;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;433;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;434;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;435;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;436;1404.904,-678.6909;Float;False;False;-1;2;DE_ShaderGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/DE/Utility/Fallback;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;428;1408,-640;Float;False;True;-1;2;ALP8310_ShaderGUI;0;12;ALP/Surface Detail Wind;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;45;Lighting Model;0;0;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Alpha Clipping;0;638677682655968019;  Use Shadow Threshold;0;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,True,_ASETransmissionShadow;0;Translucency;0;0;  Translucency Strength;1,True,_ASETranslucencyStrength;0;  Normal Distortion;0.5,True,_ASETranslucencyNormalDistortion;0;  Scattering;2,True,_ASETranslucencyScattering;0;  Direct;0.9,True,_ASETranslucencyScattering;0;  Ambient;0.1,True,_ASETranslucencyAmbient;0;  Shadow;0.5,True,_ASETranslucencyShadow;0;Cast Shadows;1;0;Receive Shadows;1;0;Receive SSAO;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,True,_TessellationPhong;0;  Type;0;0;  Tess;16,True,_TessellationStrength;0;  Min;10,True,_TessellationDistanceMin;0;  Max;25,True,_TessellationDistanceMax;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;0;Clear Coat;0;0;0;11;False;True;True;True;True;True;True;True;True;True;True;False;;True;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;438;1408,-540;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;389;39;437;38
WireConnection;389;38;437;35
WireConnection;389;456;437;1819
WireConnection;389;464;437;1824
WireConnection;437;1234;387;2190
WireConnection;428;0;389;73
WireConnection;428;1;389;72
WireConnection;428;3;437;37
WireConnection;428;4;437;33
WireConnection;428;5;437;34
WireConnection;428;8;437;1235
ASEEND*/
//CHKSM=4C1CBE457BBCC04A513464218B289DDA24EC43D8