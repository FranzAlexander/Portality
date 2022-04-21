Shader "Portal/URPUnlitPortalCutout"{}
// Shader "Portal/URPUnlitPortalCutout"
// {
//     Properties
//     {
//         _MainTex ("Main Texture", 2D) = "white" {}
//     }
//     SubShader
//     {
//         Tags
//         {
//             "RenderPipeline"="UniversalPipeline"
//             "RenderType"="Transparent"
//             "UniversalMaterialType" = "Unlit"
//             "Queue"="Transparent"
//         }
//         Pass
//         {
//             Name "Universal Forward"

//             Cull Back

//             HLSLPROGRAM

//             // Pragmas
//             #pragma target 4.5
//             #pragma exclude_renderers gles gles3 glcore
//             #pragma multi_compile_instancing
//             #pragma multi_compile_fog
//             #pragma instancing_options renderinglayer
//             #pragma multi_compile _ DOTS_INSTANCING_ON
//             #pragma vertex vert
//             #pragma fragment frag

//             // Includes
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//             #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

//             struct Attributes
//             {
//                 float4 vertex : POSITION;

//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 uint instanceID : INSTANCEID_SEMANTIC
//                 #endif
//             };

//             struct Varyings
//             {
//                 float4 vertex : SV_POSITION;
//                 float4 screenPosition : TEXCOORD0;

//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 uint instanceID : INSTANCEID_SEMANTIC
//                 #endif

//                 #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
//                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
//                 #endif
//                 #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
//                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
//                 #endif
//                 #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
//                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
//                 #endif
//             };

//             Varyings vert(Attributes input)
//             {
//                 Varyings output;
//                 ZERO_INITIALIZE(Varyings, output);
//                 #if UNITY_ANY_INSTANCING_ENABLED
//                 output.instanceID = input.instanceID;
//                 #endif
//                 #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
//                 output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
//                 #endif
//                 #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
//                 output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
//                 #endif
//                 #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
//                 output.cullFace = input.cullFace;
//                 #endif

//                 return output;
//             }

//             float4 frag(Varyings input) 
//             {

//             }

//             ENDHLSL
//         }
//     }
// }